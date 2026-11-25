using System.ComponentModel.DataAnnotations;

namespace MicaDispatch.Api.Models
{
    public class TicketItemDTO
    {
        public int Id { get; set; } // Unique identifier for the ticket item

        [MaxLength(150)]
        public string ItemName { get; set; } = string.Empty; // Name of the item associated with the ticket

        public int Quantity { get; set; } // Quantity of the item associated with the ticket

        public decimal WeightKg { get; set; } = decimal.Zero; // Weight of the item associated with the ticket

        public int DeliveryTicketId { get; set; } // Foreign key to the DeliveryTicket entity
    }
}
