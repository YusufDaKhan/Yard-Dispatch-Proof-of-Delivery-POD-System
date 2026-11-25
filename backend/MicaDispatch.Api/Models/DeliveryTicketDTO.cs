using System.ComponentModel.DataAnnotations;


namespace MicaDispatch.Api.Models
{
    public class DeliveryTicketDTO  
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = string.Empty; // Address where the delivery is to be made

        [MaxLength(100)]
        public string CustomerInfo { get; set; } = string.Empty; // Customer information related to the delivery ticket

        public int? VehicleId { get; set; } // Foreign key to the Vehicle entity}

        public TicketStatus Status { get; set; } = TicketStatus.Created; // Status of the delivery ticket

    }
}
