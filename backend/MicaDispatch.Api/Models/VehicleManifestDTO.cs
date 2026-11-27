using System.ComponentModel.DataAnnotations;

namespace MicaDispatch.Api.Models
{
    public class VehicleManifestDTO
    {
        public int VehicleId { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        public decimal MaxLoadCapacityKg { get; set; } = decimal.Zero;
        public decimal ActiveAssignedWeightKg { get; set; } = decimal.Zero;

        public decimal RemainingCapacityKg { get; set; } = decimal.Zero;

        public int ActiveTicketCount { get; set; } = 0;

        public List<DeliveryTicketDTO> ActiveTickets { get; set; } = new();

    }
}
