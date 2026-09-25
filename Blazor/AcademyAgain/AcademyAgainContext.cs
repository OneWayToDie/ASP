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
    public DbSet<AcademyAgain.Models.Teacher> Teachers
    { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Teachers.birth_date is nullable in the DB, Students.birth_date is NOT NULL.
        // [Required] cannot be used on the shared Human.birth_date: it would make EF read
        // the column as non-nullable and throw SqlNullValueException on the NULL teacher row.
        modelBuilder.Entity<AcademyAgain.Models.Student>()
            .Property(s => s.birth_date)
            .IsRequired();

		modelBuilder.Entity<AcademyAgain.Models.TeachersDisciplinesRelations>(e =>
		{
			e.ToTable("TeachersDisciplinesRelation");
			e.HasKey(t => new { t.teacher, t.discipline });

			e.HasOne(t => t.Teacher)
				.WithMany(t => t.DisciplinesRelations)
				.HasForeignKey(t => t.teacher);

			e.HasOne(t => t.Discipline)
				.WithMany()
				.HasForeignKey(t => t.discipline);
		});
	}
}

