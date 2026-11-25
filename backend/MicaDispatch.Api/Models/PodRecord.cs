using System.ComponentModel.DataAnnotations;

namespace MicaDispatch.Api.Models
{
    public class PodRecord
    {
        public int Id { get; set; } // Unique identifier for the POD record

        [MaxLength(500)]
        public string? ProofOfDelivery { get; set; } = string.Empty; // Proof of delivery information, such as a signature or confirmation or photo evidence

        [MaxLength(100)]
        public string ReceivedBy { get; set; } = string.Empty; // Name of the person who received the delivery

        [MaxLength(500)]
        public string? ReceivedSignature { get; set; } = string.Empty; // Signature of the person who received the delivery
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow; // Date when the delivery was received

        [MaxLength(500)]
        public string? Notes { get; set; } = string.Empty; // Additional notes or comments related to the POD record
        public int DeliveryTicketId { get; set; } // Foreign key to the DeliveryTicket entity

        public DeliveryTicket DeliveryTicket { get; set; } = null!; // Navigation property to the associated DeliveryTicket entity

    }
}
