using Microsoft.EntityFrameworkCore;

public class AcademyAgainContext(DbContextOptions<AcademyAgainContext> options) : DbContext(options)
{
    public DbSet<AcademyAgain.Models.Discipline> Disciplines
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Teacher> Teachers
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Student> Students
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Direction> Directions
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Group> Groups
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Holiday> Holidays
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Salary> Salary
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Schedule> Schedule
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Exam> Exams
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Grade> Grades
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Attendance> Attendance
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.CompleteDiscipline> CompleteDisciplines
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.DependentDiscipline> DependentDisciplines
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.RequiredDiscipline> RequiredDisciplines
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.DisciplinesDirectionsRelation> DisciplinesDirectionsRelation
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.TeachersDisciplinesRelation> TeachersDisciplinesRelation
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.DaysOFF> DaysOFF
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.User> Users
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Role> Roles
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Project> Projects
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.CandidateRequest> CandidateRequests
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.News> News
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Semester> Semesters
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.SessionRule> SessionRules
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.AdmissionRequest> AdmissionRequests
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.Vacancy> Vacancies
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.VacancyApplication> VacancyApplications
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.EmailVerificationCode> EmailVerificationCodes
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.SupportChat> SupportChats
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.SupportMessage> SupportMessages
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.SupportBan> SupportBans
    { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademyAgain.Models.User>()
            .HasIndex(u => u.email)
            .IsUnique()
            .HasFilter("[email] IS NOT NULL");

        modelBuilder.Entity<AcademyAgain.Models.EmailVerificationCode>()
            .HasIndex(c => new { c.email, c.purpose, c.consumed });

        modelBuilder.Entity<AcademyAgain.Models.Attendance>()
            .HasKey(a => new { a.student, a.lesson });
        modelBuilder.Entity<AcademyAgain.Models.CompleteDiscipline>()
            .HasKey(c => new { c.group, c.discipline });
        modelBuilder.Entity<AcademyAgain.Models.DependentDiscipline>()
            .HasKey(d => new { d.discipline, d.dependent_discipline });
        modelBuilder.Entity<AcademyAgain.Models.DisciplinesDirectionsRelation>()
            .HasKey(d => new { d.direction, d.discipline });
        modelBuilder.Entity<AcademyAgain.Models.Exam>()
            .HasKey(e => new { e.student, e.discipline });
        modelBuilder.Entity<AcademyAgain.Models.Grade>()
            .HasKey(g => new { g.student, g.lesson });
        modelBuilder.Entity<AcademyAgain.Models.RequiredDiscipline>()
            .HasKey(r => new { r.discipline, r.required_discipline });
        modelBuilder.Entity<AcademyAgain.Models.TeachersDisciplinesRelation>()
            .HasKey(t => new { t.teacher, t.discipline });

        modelBuilder.Entity<AcademyAgain.Models.SupportChat>()
            .HasIndex(c => c.user_id);
        modelBuilder.Entity<AcademyAgain.Models.SupportMessage>()
            .HasIndex(m => m.chat_id);
    }
}