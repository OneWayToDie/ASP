using Microsoft.EntityFrameworkCore;

public class AcademyAgainContext(DbContextOptions<AcademyAgainContext> options) : DbContext(options)
{
    public DbSet<AcademyAgain.Models.Discipline> Disciplines
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Direction> Directions
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Group> Groups
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Student> Students
    { get; set; } = default!;
}

