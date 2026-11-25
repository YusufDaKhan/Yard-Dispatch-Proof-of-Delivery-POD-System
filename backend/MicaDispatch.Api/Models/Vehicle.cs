using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MicaDispatch.Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty; // License plate number of the vehicle

        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty; // Name of the driver assigned to the vehicle

        [MaxLength(500)]
        public string? Notes { get; set; } = string.Empty; // Additional notes or comments about the vehicle if needed

        [Precision(18, 2)]
        public decimal MaxLoadCapacity { get; set; } = decimal.Zero; // Maximum load capacity of the vehicle in kilograms

        public ICollection<DeliveryTicket> DeliveryTickets { get; set; } = new List<DeliveryTicket>(); // Navigation property to the associated DeliveryTicket entities
    }
}
