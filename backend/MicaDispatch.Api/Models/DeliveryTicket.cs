using System.ComponentModel.DataAnnotations;

namespace MicaDispatch.Api.Models
{
    public class DeliveryTicket
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string DispatchOrderNumber { get; set; } = string.Empty; // Dispatch order number associated with the delivery ticket "DEL-0001"
        public int? VehicleId { get; set; } // Foreign key to the Vehicle entity

        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = string.Empty; // Address where the delivery is to be made

        [MaxLength(100)]
        public string CustomerInfo { get; set; } = string.Empty; // Customer information related to the delivery ticket

        public TicketStatus Status { get; set; } = TicketStatus.Created; // Status of the delivery ticket

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the delivery ticket was created

        public Vehicle? Vehicle { get; set; } // Navigation property to the associated Vehicle entity

        public PodRecord? PodRecord { get; set; } // Navigation property to the associated PodRecord entity

        public List<TicketItem> TicketItems { get; set; } = new List<TicketItem>(); // Navigation property to the associated TicketItem entities

    }
}

