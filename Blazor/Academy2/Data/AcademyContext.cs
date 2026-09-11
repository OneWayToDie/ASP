using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Academy2.Data;

public partial class AcademyContext : DbContext
{
    public AcademyContext()
    {
    }

    public AcademyContext(DbContextOptions<AcademyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendance { get; set; }

    public virtual DbSet<DaysOff> DaysOff { get; set; }

    public virtual DbSet<Directions> Directions { get; set; }

    public virtual DbSet<Disciplines> Disciplines { get; set; }

    public virtual DbSet<Exams> Exams { get; set; }

    public virtual DbSet<Grades> Grades { get; set; }

    public virtual DbSet<Groups> Groups { get; set; }

    public virtual DbSet<Holidays> Holidays { get; set; }

    public virtual DbSet<Salary> Salary { get; set; }

    public virtual DbSet<Schedule> Schedule { get; set; }

    public virtual DbSet<Students> Students { get; set; }

    public virtual DbSet<Teachers> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Academy;Trusted_Connection=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => new { e.Student, e.Lesson });

            entity.Property(e => e.Student).HasColumnName("student");
            entity.Property(e => e.Lesson).HasColumnName("lesson");
            entity.Property(e => e.Present).HasColumnName("present");

            entity.HasOne(d => d.LessonNavigation).WithMany(p => p.Attendance)
                .HasForeignKey(d => d.Lesson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Schedule");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Attendance)
                .HasForeignKey(d => d.Student)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Students");
        });

        modelBuilder.Entity<DaysOff>(entity =>
        {
            entity.HasKey(e => e.Date).HasName("PK__DaysOFF__D9DE21FCDE307EC8");

            entity.ToTable("DaysOFF");

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Holiday).HasColumnName("holiday");

            entity.HasOne(d => d.HolidayNavigation).WithMany(p => p.DaysOff)
                .HasForeignKey(d => d.Holiday)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DO_Holidays");
        });

        modelBuilder.Entity<Directions>(entity =>
        {
            entity.HasKey(e => e.DirectionId);

            entity.Property(e => e.DirectionId).HasColumnName("direction_id");
            entity.Property(e => e.DirectionName)
                .HasMaxLength(50)
                .HasColumnName("direction_name");
        });

        modelBuilder.Entity<Disciplines>(entity =>
        {
            entity.HasKey(e => e.DisciplineId);

            entity.Property(e => e.DisciplineId)
                .ValueGeneratedNever()
                .HasColumnName("discipline_id");
            entity.Property(e => e.DisciplineName)
                .HasMaxLength(150)
                .HasColumnName("discipline_name");
            entity.Property(e => e.NumberOfLessons).HasColumnName("number_of_lessons");
        });

        modelBuilder.Entity<Exams>(entity =>
        {
            entity.HasKey(e => new { e.Student, e.Discipline });

            entity.Property(e => e.Student).HasColumnName("student");
            entity.Property(e => e.Discipline).HasColumnName("discipline");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Grade).HasColumnName("grade");

            entity.HasOne(d => d.DisciplineNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.Discipline)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exams_Disciplines");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.Student)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exams_Students");
        });

        modelBuilder.Entity<Grades>(entity =>
        {
            entity.HasKey(e => new { e.Student, e.Lesson }).HasName("PK_Grades_1");

            entity.Property(e => e.Student).HasColumnName("student");
            entity.Property(e => e.Lesson).HasColumnName("lesson");
            entity.Property(e => e.Grade1).HasColumnName("grade_1");
            entity.Property(e => e.Grade2).HasColumnName("grade_2");

            entity.HasOne(d => d.LessonNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.Lesson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Schedule");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.Student)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Students");
        });

        modelBuilder.Entity<Groups>(entity =>
        {
            entity.HasKey(e => e.GroupId);

            entity.Property(e => e.GroupId)
                .ValueGeneratedNever()
                .HasColumnName("group_id");
            entity.Property(e => e.Direction).HasColumnName("direction");
            entity.Property(e => e.GroupName)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("group_name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartTime)
                .HasPrecision(0)
                .HasColumnName("start_time");
            entity.Property(e => e.Weekdays).HasColumnName("weekdays");

            entity.HasOne(d => d.DirectionNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.Direction)
                .HasConstraintName("FK_Groups_Directions");
        });

        modelBuilder.Entity<Holidays>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__Holidays__253884EA4C6CDAD2");

            entity.Property(e => e.HolidayId).HasColumnName("holiday_id");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.HolidayName)
                .HasMaxLength(150)
                .HasColumnName("holiday_name");
            entity.Property(e => e.Month).HasColumnName("month");
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Salary__ED1FC9EA75279630");

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("payment_id");
            entity.Property(e => e.Accrued)
                .HasColumnType("smallmoney")
                .HasColumnName("accrued");
            entity.Property(e => e.Received).HasColumnName("received");
            entity.Property(e => e.Teacher).HasColumnName("teacher");

            entity.HasOne(d => d.TeacherNavigation).WithMany(p => p.Salary)
                .HasForeignKey(d => d.Teacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Salary_Teachers");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.LessonId);

            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Discipline).HasColumnName("discipline");
            entity.Property(e => e.Group).HasColumnName("group");
            entity.Property(e => e.Spent).HasColumnName("spent");
            entity.Property(e => e.Teacher).HasColumnName("teacher");
            entity.Property(e => e.Time)
                .HasPrecision(0)
                .HasColumnName("time");

            entity.HasOne(d => d.DisciplineNavigation).WithMany(p => p.Schedule)
                .HasForeignKey(d => d.Discipline)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Disciplines");

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Schedule)
                .HasForeignKey(d => d.Group)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Groups");

            entity.HasOne(d => d.TeacherNavigation).WithMany(p => p.Schedule)
                .HasForeignKey(d => d.Teacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Teachers");
        });

        modelBuilder.Entity<Students>(entity =>
        {
            entity.HasKey(e => e.StudId);

            entity.Property(e => e.StudId).HasColumnName("stud_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Group).HasColumnName("group");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Photo)
                .HasColumnType("image")
                .HasColumnName("photo");

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Group)
                .HasConstraintName("FK_Students_Groups");
        });

        modelBuilder.Entity<Teachers>(entity =>
        {
            entity.HasKey(e => e.TeacherId);

            entity.Property(e => e.TeacherId)
                .ValueGeneratedNever()
                .HasColumnName("teacher_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Photo)
                .HasColumnType("image")
                .HasColumnName("photo");
            entity.Property(e => e.Rate)
                .HasColumnType("smallmoney")
                .HasColumnName("rate");
            entity.Property(e => e.WorkSince).HasColumnName("work_since");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
