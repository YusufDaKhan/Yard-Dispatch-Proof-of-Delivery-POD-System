using MicaDispatch.Api.Data;
using MicaDispatch.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MicaDispatch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _context;
    public VehiclesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Vehicle
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicle()
    {
        return await _context.Vehicles.ToListAsync();
    }

    // GET: api/Vehicle/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    // GET: api/Vehicle/Manifest/5
    [HttpGet("{id}/manifest")]
    public async Task<ActionResult<VehicleManifestDTO>> GetVehicleManifest(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var activeTickets = await _context.DeliveryTickets.Where(dt => dt.VehicleId == id && dt.Status != TicketStatus.Delivered && dt.Status != TicketStatus.Cancelled && dt.Status != TicketStatus.Returned).OrderByDescending(dt => dt.Id).ToListAsync();
        var activeTicketIds = activeTickets.Select(dt => dt.Id).ToList();
        var activeAssignedWeightKg = await _context.TicketItems.Where(item => activeTicketIds.Contains(item.DeliveryTicketId)).SumAsync(item => (decimal)(item.Quantity * item.WeightKg));

        var activeTicketDTOs = activeTickets.Select(dt => new DeliveryTicketDTO
        {
            Id = dt.Id,
            VehicleId = dt.VehicleId,
            Status = dt.Status,
            DeliveryAddress = dt.DeliveryAddress,
            CustomerInfo = dt.CustomerInfo,
            DispatchOrderNumber = dt.DispatchOrderNumber
        });

        return Ok(new VehicleManifestDTO
        {
            VehicleId = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            DriverName = vehicle.DriverName,
            MaxLoadCapacityKg = vehicle.MaxLoadCapacityKg,
            ActiveAssignedWeightKg = activeAssignedWeightKg,
            RemainingCapacityKg = vehicle.MaxLoadCapacityKg - activeAssignedWeightKg,
            ActiveTicketCount = activeTickets.Count,
            ActiveTickets = activeTicketDTOs.ToList()
        });
    }

    // PUT: api/Vehicle/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutVehicle(int? id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest();
        }

        _context.Entry(vehicle).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Vehicle
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, vehicle);
    }

    // DELETE: api/Vehicle/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int? id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool VehicleExists(int? id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }
}
