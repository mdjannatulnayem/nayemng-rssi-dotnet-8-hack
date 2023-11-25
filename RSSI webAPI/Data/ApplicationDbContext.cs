using Microsoft.EntityFrameworkCore;
using RSSI_webAPI.Models;

namespace RSSI_webAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<ReconDataModel> ReconnectionRecords { get; set; }
}
