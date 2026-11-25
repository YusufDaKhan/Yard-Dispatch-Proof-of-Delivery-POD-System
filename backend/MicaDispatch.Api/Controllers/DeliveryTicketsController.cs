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
    public async Task<ActionResult<IEnumerable<DeliveryTicketDTO>>> GetDeliveryTicket()
    {
        return await _context.DeliveryTickets.Select(x => TicketToDTO(x)).ToListAsync(); // Retrieves all delivery tickets from the database and converts them to DeliveryTicketDTO objects for the response
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
        _context.DeliveryTickets.Add(deliveryticket);
        await _context.SaveChangesAsync();

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

