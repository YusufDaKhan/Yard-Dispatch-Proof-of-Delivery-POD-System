using MicaDispatch.Api.Data;
using MicaDispatch.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TicketItemsController : ControllerBase
{
    private readonly AppDbContext _context;
    public TicketItemsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/TicketItem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketItemDTO>>> GetTicketItem()
    {
        return await _context.TicketItems.Select(ticketItem => TicketItemToDTO(ticketItem)).ToListAsync();
    }

    // GET: api/TicketItem/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketItemDTO>> GetTicketItem(int id)
    {
        var ticketitem = await _context.TicketItems.FindAsync(id);

        if (ticketitem == null)
        {
            return NotFound();
        }

        return TicketItemToDTO(ticketitem);
    }

    // PUT: api/TicketItem/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTicketItem(int id, TicketItemDTO ticketitemDTO)
    {
        if (id != ticketitemDTO.Id)
        {
            return BadRequest();
        }

        var ticketitem = await _context.TicketItems.FindAsync(id);
        if (ticketitem == null)
        {
            return NotFound();
        }

        // Update the properties of the ticket item with the values from the DTO
        ticketitem.ItemName = ticketitemDTO.ItemName;
        ticketitem.Quantity = ticketitemDTO.Quantity;
        ticketitem.WeightKg = ticketitemDTO.WeightKg;
        ticketitem.DeliveryTicketId = ticketitemDTO.DeliveryTicketId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TicketItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/TicketItem
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TicketItemDTO>> PostTicketItem(TicketItemDTO ticketitemDTO)
    {
        var ticketitem = new TicketItem
        {
            ItemName = ticketitemDTO.ItemName,
            Quantity = ticketitemDTO.Quantity,
            WeightKg = ticketitemDTO.WeightKg,
            DeliveryTicketId = ticketitemDTO.DeliveryTicketId
        };
        _context.TicketItems.Add(ticketitem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicketItem), new { id = ticketitem.Id }, TicketItemToDTO(ticketitem));
    }

    // DELETE: api/TicketItem/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicketItem(int id)
    {
        var ticketitem = await _context.TicketItems.FindAsync(id);
        if (ticketitem == null)
        {
            return NotFound();
        }

        _context.TicketItems.Remove(ticketitem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TicketItemExists(int id)
    {
        return _context.TicketItems.Any(e => e.Id == id);
    }

    private static TicketItemDTO TicketItemToDTO(TicketItem ticketItem) => // Converts a TicketItem entity to a TicketItemDTO object for returning in API responses
        new TicketItemDTO
        {
            Id = ticketItem.Id,
            ItemName = ticketItem.ItemName,
            Quantity = ticketItem.Quantity,
            WeightKg = ticketItem.WeightKg,
            DeliveryTicketId = ticketItem.DeliveryTicketId
        };
}
