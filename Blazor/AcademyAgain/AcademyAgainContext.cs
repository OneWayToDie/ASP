using Microsoft.EntityFrameworkCore;

public class AcademyAgainContext(DbContextOptions<AcademyAgainContext> options) : DbContext(options)
{
    public DbSet<AcademyAgain.Models.Discipline> Disciplines
    { get; set; } = default!;
}
