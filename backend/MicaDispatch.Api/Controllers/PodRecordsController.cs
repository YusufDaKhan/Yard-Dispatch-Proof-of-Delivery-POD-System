using MicaDispatch.Api.Data;
using MicaDispatch.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PodRecordsController : ControllerBase
{
    private readonly AppDbContext _context;
    public PodRecordsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/PodRecord
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PodRecordDTO>>> GetPodRecord()
    {
        return await _context.PodRecords.Select(podRecord => PodRecordToDTO(podRecord)).ToListAsync();
    }

    // GET: api/PodRecord/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PodRecordDTO>> GetPodRecord(int id)
    {
        var podrecord = await _context.PodRecords.FindAsync(id);

        if (podrecord == null)
        {
            return NotFound();
        }

        return PodRecordToDTO(podrecord);
    }

    // PUT: api/PodRecord/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPodRecord(int id, PodRecordDTO podrecordDTO)
    {
        if (id != podrecordDTO.Id)
        {
            return BadRequest();
        }

        var podrecord = await _context.PodRecords.FindAsync(id);
        if (podrecord == null)
        {
            return NotFound();
        }

        // Update the properties of the pod record with the values from the DTO
        podrecord.ProofOfDelivery = podrecordDTO.ProofOfDelivery;
        podrecord.ReceivedBy = podrecordDTO.ReceivedBy;
        podrecord.ReceivedDate = podrecordDTO.ReceivedDate;
        podrecord.ReceivedSignature = podrecordDTO.ReceivedSignature;
        podrecord.Notes = podrecordDTO.Notes;
        podrecord.DeliveryTicketId = podrecordDTO.DeliveryTicketId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!PodRecordExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/PodRecord
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<PodRecordDTO>> PostPodRecord(PodRecordDTO podrecordDTO)
    {


        var deliveryTicket = await _context.DeliveryTickets.FindAsync(podrecordDTO.DeliveryTicketId); // Check if the associated delivery ticket exists and is not cancelled before creating a POD record
        if (deliveryTicket == null || deliveryTicket.Status == TicketStatus.Cancelled)
        {
            return BadRequest($"Delivery ticket with ID {podrecordDTO.DeliveryTicketId} does not exist or is cancelled.");
        }

        var podExists = await _context.PodRecords.AnyAsync(p => p.DeliveryTicketId == podrecordDTO.DeliveryTicketId); // Check if a POD record already exists for the delivery ticket before creating a new one
        if (podExists)
        {
            return BadRequest($"POD record for delivery ticket with ID {podrecordDTO.DeliveryTicketId} already exists.");
        }

        var podrecord = new PodRecord
        {
            ProofOfDelivery = podrecordDTO.ProofOfDelivery,
            ReceivedBy = podrecordDTO.ReceivedBy,
            ReceivedDate = podrecordDTO.ReceivedDate,
            ReceivedSignature = podrecordDTO.ReceivedSignature,
            Notes = podrecordDTO.Notes,
            DeliveryTicketId = podrecordDTO.DeliveryTicketId
        };

        _context.PodRecords.Add(podrecord);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPodRecord), new { id = podrecord.Id }, PodRecordToDTO(podrecord));
    }

    // DELETE: api/PodRecord/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePodRecord(int id)
    {
        var podrecord = await _context.PodRecords.FindAsync(id);
        if (podrecord == null)
        {
            return NotFound();
        }

        _context.PodRecords.Remove(podrecord);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PodRecordExists(int id)
    {
        return _context.PodRecords.Any(e => e.Id == id);
    }

    private static PodRecordDTO PodRecordToDTO(PodRecord podRecord) =>
        new PodRecordDTO
        {
            Id = podRecord.Id,
            ProofOfDelivery = podRecord.ProofOfDelivery,
            ReceivedBy = podRecord.ReceivedBy,
            ReceivedDate = podRecord.ReceivedDate,
            ReceivedSignature = podRecord.ReceivedSignature,
            Notes = podRecord.Notes,
            DeliveryTicketId = podRecord.DeliveryTicketId

        };
}
