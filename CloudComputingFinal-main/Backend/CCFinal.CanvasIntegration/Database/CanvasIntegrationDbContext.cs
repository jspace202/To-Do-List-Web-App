using CCFinal.CanvasIntegration.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Database;

public class CanvasIntegrationDbContext : DbContext {
    public CanvasIntegrationDbContext(DbContextOptions<CanvasIntegrationDbContext> options)
        : base(options) { }

    public DbSet<UserInformation> Information { get; set; } =
        default!;
}
