using Microsoft.EntityFrameworkCore;

public class Academy2Context(DbContextOptions<Academy2Context> options) : DbContext(options)
{
    public DbSet<Academy2.Data.Disciplines> Disciplines { get; set; } = default!;
}
