using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

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
    public DbSet<AcademyAgain.Models.TeachersGroupsRelation> TeachersGroupsRelation
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
    public DbSet<AcademyAgain.Models.AuditLog> AuditLogs
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.TeacherReview> Reviews
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.PraiseTemplate> PraiseTemplates
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.LessonFeedback> LessonFeedbacks
    { get; set; } = default!;
    public DbSet<AcademyAgain.Models.LessonFeedbackTag> LessonFeedbackTags
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
        modelBuilder.Entity<AcademyAgain.Models.TeachersGroupsRelation>()
            .HasKey(t => new { t.teacher, t.group });

        modelBuilder.Entity<AcademyAgain.Models.TeacherReview>()
            .HasIndex(r => new { r.teacher_id, r.student_id })
            .IsUnique()
            .HasFilter("[is_deleted] = 0");
        modelBuilder.Entity<AcademyAgain.Models.TeacherReview>()
            .HasIndex(r => r.teacher_id);
        modelBuilder.Entity<AcademyAgain.Models.TeacherReview>()
            .HasQueryFilter(r => !r.is_deleted);

        modelBuilder.Entity<AcademyAgain.Models.LessonFeedback>()
            .HasIndex(f => new { f.lesson_id, f.student_id })
            .IsUnique();
        modelBuilder.Entity<AcademyAgain.Models.LessonFeedback>()
            .HasIndex(f => f.student_id);

        modelBuilder.Entity<AcademyAgain.Models.LessonFeedbackTag>()
            .HasKey(t => new { t.feedback_id, t.template_id });
        modelBuilder.Entity<AcademyAgain.Models.LessonFeedbackTag>()
            .HasIndex(t => t.template_id);

        modelBuilder.Entity<AcademyAgain.Models.PraiseTemplate>()
            .HasIndex(t => t.category);

        modelBuilder.Entity<AcademyAgain.Models.SupportChat>()
            .HasIndex(c => c.user_id);
        modelBuilder.Entity<AcademyAgain.Models.SupportMessage>()
            .HasIndex(m => m.chat_id);

        modelBuilder.Entity<AcademyAgain.Models.AuditLog>()
            .HasIndex(a => new { a.entity, a.created_at });
        modelBuilder.Entity<AcademyAgain.Models.AuditLog>()
            .HasIndex(a => a.user_id);

        modelBuilder.Entity<AcademyAgain.Models.Student>()
            .HasQueryFilter(s => !s.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.Teacher>()
            .HasQueryFilter(t => !t.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.Schedule>()
            .HasQueryFilter(s => !s.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.Exam>()
            .HasQueryFilter(e => !e.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.Grade>()
            .HasQueryFilter(g => !g.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.CandidateRequest>()
            .HasQueryFilter(c => !c.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.AdmissionRequest>()
            .HasQueryFilter(a => !a.is_deleted);
        modelBuilder.Entity<AcademyAgain.Models.VacancyApplication>()
            .HasQueryFilter(v => !v.is_deleted);
    }

    #region Аудит (автоперехват SaveChanges)

    private static readonly HashSet<string> AuditExcludedEntities = new()
    {
        nameof(AcademyAgain.Models.AuditLog),
        nameof(AcademyAgain.Models.SupportChat),
        nameof(AcademyAgain.Models.SupportMessage),
        nameof(AcademyAgain.Models.SupportBan),
        nameof(AcademyAgain.Models.EmailVerificationCode)
    };

    private static readonly HashSet<string> UserSensitiveProperties = new()
    {
        nameof(AcademyAgain.Models.User.role_id),
        nameof(AcademyAgain.Models.User.status),
        nameof(AcademyAgain.Models.User.password_hash)
    };

    private sealed class AuditEntry
    {
        public required string Action;
        public required string Entity;
        public string? EntityId;
        public string? Old;
        public string? New;
        public required EntityEntry Entry;
    }

    private bool _auditSuppressed;

    public override int SaveChanges()
    {
        var entries = BuildAuditEntries();
        var result = base.SaveChanges();
        WriteAuditEntries(entries, ResolveUserIdSync());
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = BuildAuditEntries();
        var result = await base.SaveChangesAsync(cancellationToken);
        try
        {
            var userId = await this.GetService<AcademyAgain.Helpers.AuditLogger>().ResolveUserIdAsync();
            await WriteAuditEntriesAsync(entries, userId, cancellationToken);
        }
        catch
        {
            // аудит не должен ломать основную операцию
        }
        return result;
    }

    private List<AuditEntry> BuildAuditEntries()
    {
        if (_auditSuppressed)
        {
            return [];
        }

        var entries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                continue;
            }

            var entityType = entry.Entity.GetType().Name;
            if (AuditExcludedEntities.Contains(entityType))
            {
                continue;
            }

            if (entityType == nameof(AcademyAgain.Models.User) && entry.State == EntityState.Modified
                && !entry.Properties.Any(p => UserSensitiveProperties.Contains(p.Metadata.Name) && p.IsModified))
            {
                continue;
            }

            var audit = new AuditEntry
            {
                Action = entry.State switch
                {
                    EntityState.Added => "create",
                    EntityState.Modified => "update",
                    _ => "delete"
                },
                Entity = entityType,
                Entry = entry
            };

            if (entry.State == EntityState.Modified)
            {
                audit.Old = Serialize(entry, useCurrent: false);
                audit.New = Serialize(entry, useCurrent: true);
            }
            else if (entry.State == EntityState.Added)
            {
                audit.New = Serialize(entry, useCurrent: true);
            }
            else
            {
                audit.Old = Serialize(entry, useCurrent: false);
            }

            entries.Add(audit);
        }
        return entries;
    }

    private void ResolveEntityIds(List<AuditEntry> entries)
    {
        foreach (var audit in entries)
        {
            var keys = audit.Entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .Select(p => Convert.ToString(p.CurrentValue, System.Globalization.CultureInfo.InvariantCulture));
            audit.EntityId = string.Join("|", keys);
        }
    }

    private void WriteAuditEntries(List<AuditEntry> entries, int? userId)
    {
        if (entries.Count == 0)
        {
            return;
        }
        ResolveEntityIds(entries);
        _auditSuppressed = true;
        try
        {
            foreach (var audit in entries)
            {
                AuditLogs.Add(new AcademyAgain.Models.AuditLog
                {
                    user_id = userId,
                    action = audit.Action,
                    entity = audit.Entity,
                    entity_id = audit.EntityId,
                    old_value = audit.Old,
                    new_value = audit.New,
                    created_at = DateTime.Now
                });
            }
            base.SaveChanges();
        }
        finally
        {
            _auditSuppressed = false;
        }
    }

    private async Task WriteAuditEntriesAsync(List<AuditEntry> entries, int? userId, CancellationToken ct)
    {
        if (entries.Count == 0)
        {
            return;
        }
        ResolveEntityIds(entries);
        _auditSuppressed = true;
        try
        {
            foreach (var audit in entries)
            {
                AuditLogs.Add(new AcademyAgain.Models.AuditLog
                {
                    user_id = userId,
                    action = audit.Action,
                    entity = audit.Entity,
                    entity_id = audit.EntityId,
                    old_value = audit.Old,
                    new_value = audit.New,
                    created_at = DateTime.Now
                });
            }
            await base.SaveChangesAsync(ct);
        }
        finally
        {
            _auditSuppressed = false;
        }
    }

    private int? ResolveUserIdSync()
    {
        try
        {
            return this.GetService<AcademyAgain.Helpers.AuditLogger>().ResolveUserIdAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return null;
        }
    }

    private static string? Serialize(EntityEntry entry, bool useCurrent)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsPrimaryKey())
            {
                continue;
            }
            var value = useCurrent ? prop.CurrentValue : prop.OriginalValue;
            dict[prop.Metadata.Name] = NormalizeValue(value);
        }
        if (dict.Count == 0)
        {
            return null;
        }
        return JsonSerializer.Serialize(dict);
    }

    private static object? NormalizeValue(object? value) => value switch
    {
        byte[] bytes => $"[bytes:{bytes.Length}]",
        _ => value
    };

    #endregion
}