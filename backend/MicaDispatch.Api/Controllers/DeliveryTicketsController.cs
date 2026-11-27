using MicaDispatch.Api.Data;
using MicaDispatch.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DeliveryTicketsController : ControllerBase
{
    private readonly AppDbContext _context;
    public DeliveryTicketsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/DeliveryTicket
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryTicketDTO>>> GetDeliveryTicket([FromQuery] TicketStatus? status = null, [FromQuery] int? vehicleId = null, [FromQuery] string? search = null) // Retrieves a list of delivery tickets from the database, optionally filtered by status, vehicle ID, and search term, and returns them as a list of DeliveryTicketDTO objects in descending order of their IDs
    {
        var query = _context.DeliveryTickets.AsQueryable(); // Initializes a queryable collection of delivery tickets from the database context to allow for filtering based on the provided query parameters

        if (status.HasValue)
        {
            query = query.Where(dt => dt.Status == status.Value);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(dt => dt.VehicleId == vehicleId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(dt => dt.DeliveryAddress.Contains(search) || dt.CustomerInfo.Contains(search) || dt.DispatchOrderNumber.Contains(search.Trim()));
        }

        return await query.OrderByDescending(dt => dt.Id).Select(x => TicketToDTO(x)).ToListAsync(); // Retrieves a list of delivery tickets from the database, optionally filtered by status, vehicle ID, and search term, and returns them as a list of DeliveryTicketDTO objects in descending order of their IDs
    }


    //GET: api/DeliveryTicket/5/items
    [HttpGet("{id}/items")]
    public async Task<ActionResult<IEnumerable<TicketItemDTO>>> GetTicketItemsForDeliveryTicket(int id)
    {
        var deliveryTicket = await _context.DeliveryTickets.FindAsync(id); // Retrieves the delivery ticket with the specified ID from the database to check if it exists before retrieving its associated ticket items
        if (deliveryTicket == null)
        {
            return NotFound();
        }
        var ticketItems = await _context.TicketItems // Retrieves all ticket items associated with the specified delivery ticket ID from the database and converts them to TicketItemDTO objects for the response
            .Where(ti => ti.DeliveryTicketId == id)
            .Select(ti => new TicketItemDTO
            {
                Id = ti.Id,
                ItemName = ti.ItemName,
                WeightKg = ti.WeightKg,
                Quantity = ti.Quantity,
                DeliveryTicketId = ti.DeliveryTicketId
            })
            .ToListAsync();
        return ticketItems;
    }


    //GET: api/DeliveryTicket/5/Pod
    [HttpGet("{id}/pod")]
    public async Task<ActionResult<IEnumerable<PodRecordDTO>>> GetPodRecord(int id)
    {
        var deliveryTicket = await _context.DeliveryTickets.FindAsync(id); // Retrieves the delivery ticket with the specified ID from the database to check if it exists before retrieving its associated POD records
        if (deliveryTicket == null)
        {
            return NotFound();
        }
        var podRecords = await _context.PodRecords // Retrieves all POD records associated with the specified delivery ticket ID from the database and converts them to PodRecordDTO objects for the response
            .Where(pr => pr.DeliveryTicketId == id)
            .Select(pr => new PodRecordDTO
            {
                Id = pr.Id,
                DeliveryTicketId = pr.DeliveryTicketId,
                ProofOfDelivery = pr.ProofOfDelivery,
                ReceivedSignature = pr.ReceivedSignature,
                ReceivedDate = pr.ReceivedDate
            })
            .ToListAsync();
        return podRecords;

    }

    // GET: api/DeliveryTicket/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryTicketDTO>> GetDeliveryTicket(int id)
    {
        var deliveryticket = await _context.DeliveryTickets.FindAsync(id);

        if (deliveryticket == null) // If the delivery ticket with the specified ID is not found in the database, return a 404 Not Found response
        {
            return NotFound();
        }

        return TicketToDTO(deliveryticket);
    }

    // PUT: api/DeliveryTicket/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDeliveryTicket(int id, DeliveryTicketDTO deliveryticketDTO)
    {
        if (id != deliveryticketDTO.Id)
        {
            return BadRequest();
        }

        var deliveryticket = await _context.DeliveryTickets.FindAsync(id); // Retrieves the delivery ticket with the specified ID from the database for updating its properties based on the provided DeliveryTicketDTO object
        if (deliveryticket == null)
        {
            return NotFound();
        }

        if (deliveryticket.Status == TicketStatus.Delivered || deliveryticket.Status == TicketStatus.Cancelled) // If the delivery ticket is already completed or cancelled, return a 400 Bad Request response with an error message
        {
            return BadRequest("Cannot update a completed or cancelled delivery ticket.");
        }

        if (deliveryticketDTO.Status == TicketStatus.Delivered)
        {
            var HasPod = await _context.PodRecords.FirstOrDefaultAsync(p => p.DeliveryTicketId == id); // Checks if a POD record exists for the delivery ticket before allowing it to be marked as completed
            if (HasPod == null)
            {
                return BadRequest("Cannot mark the delivery ticket as delivered without a POD record.");
            }
        }

        if (deliveryticketDTO.VehicleId != null)
        {
            var vehicle = await _context.Vehicles.FindAsync(deliveryticketDTO.VehicleId.Value); // Retrieves the vehicle with the specified VehicleId from the database to check if it exists before associating it with the delivery ticket
            if (vehicle == null)
            {
                return BadRequest($"Vehicle with ID {deliveryticketDTO.VehicleId.Value} does not exist."); // If the vehicle does not exist, return a 400 Bad Request response with an error message
            }

            var TotalWeight = await _context.TicketItems
            .Where(ti => ti.DeliveryTicketId == id)
            .SumAsync(ti => ti.WeightKg * ti.Quantity); // Calculates the total weight of all ticket items associated with the delivery ticket by multiplying the weight of each item by its quantity and summing the results

            if (TotalWeight > vehicle.MaxLoadCapacityKg)
            {
                return BadRequest($"Total weight of ticket items ({TotalWeight} kg) exceeds the maximum load capacity of the vehicle ({vehicle.MaxLoadCapacityKg} kg)."); // If the total weight exceeds the maximum load capacity of the vehicle, return a 400 Bad Request response with an error message
            }
        }


        // Update the properties of the delivery ticket with the values from the DTO
        deliveryticket.DeliveryAddress = deliveryticketDTO.DeliveryAddress;
        deliveryticket.CustomerInfo = deliveryticketDTO.CustomerInfo;
        deliveryticket.VehicleId = deliveryticketDTO.VehicleId;
        deliveryticket.Status = deliveryticketDTO.Status;


        // Mark the delivery ticket entity as modified in the DbContext to track changes for saving to the database
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!DeliveryTicketExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/DeliveryTicket
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<DeliveryTicketDTO>> PostDeliveryTicket(DeliveryTicketDTO deliveryticketDTO)
    {
        var deliveryticket = new DeliveryTicket // Creates a new DeliveryTicket entity based on the provided DeliveryTicketDTO object and adds it to the database context for saving
        {
            DeliveryAddress = deliveryticketDTO.DeliveryAddress,
            CustomerInfo = deliveryticketDTO.CustomerInfo,
            VehicleId = deliveryticketDTO.VehicleId,
            Status = deliveryticketDTO.Status
        };

        if (deliveryticket.Status == TicketStatus.Delivered || deliveryticket.Status == TicketStatus.Cancelled)
        {
            return BadRequest("Cannot create a delivery ticket with a status of 'Delivered' or 'Cancelled'."); // If the status of the new delivery ticket is set to 'Delivered' or 'Cancelled', return a 400 Bad Request response with an error message
        }

        if (deliveryticket.VehicleId != null)
        {
            var vehicle = await _context.Vehicles.FindAsync(deliveryticket.VehicleId.Value); // Retrieves the vehicle with the specified VehicleId from the database to check if it exists before associating it with the new delivery ticket
            if (vehicle == null)
            {
                return BadRequest($"Vehicle with ID {deliveryticket.VehicleId.Value} does not exist."); // If the vehicle does not exist, return a 400 Bad Request response with an error message
            }
        }

        _context.DeliveryTickets.Add(deliveryticket);
        await _context.SaveChangesAsync();



        deliveryticket.DispatchOrderNumber = $"DO-{deliveryticket.Id:D6}"; // Generates a dispatch order number for the newly created delivery ticket based on its ID and assigns it to the DispatchOrderNumber property
        await _context.SaveChangesAsync(); // Saves the changes to the database, including the generated dispatch order number

        return CreatedAtAction(nameof(GetDeliveryTicket), new { id = deliveryticket.Id }, TicketToDTO(deliveryticket)); // Returns a 201 Created response with the location of the newly created delivery ticket and its corresponding DeliveryTicketDTO object in the response body
    }

    // DELETE: api/DeliveryTicket/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeliveryTicket(int id)
    {
        var deliveryticket = await _context.DeliveryTickets.FindAsync(id);
        if (deliveryticket == null)
        {
            return NotFound();
        }

        _context.DeliveryTickets.Remove(deliveryticket);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DeliveryTicketExists(int id)
    {
        return _context.DeliveryTickets.Any(e => e.Id == id);
    }


    private static DeliveryTicketDTO TicketToDTO(DeliveryTicket deliveryTicket) => // Converts a DeliveryTicket entity to a DeliveryTicketDTO object for returning in API responses
        new DeliveryTicketDTO
        {
            Id = deliveryTicket.Id,
            DeliveryAddress = deliveryTicket.DeliveryAddress,
            CustomerInfo = deliveryTicket.CustomerInfo,
            VehicleId = deliveryTicket.VehicleId,
            Status = deliveryTicket.Status
        };
}

