using Microsoft.EntityFrameworkCore;

namespace MicaDispatch.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.DeliveryTicket> DeliveryTickets { get; set; } = null!;
        public DbSet<Models.TicketItem> TicketItems { get; set; } = null!;
        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Models.PodRecord> PodRecords { get; set; } = null!;
    }
}
