using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class AcademyContext : DbContext
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasOne(d => d.LessonNavigation).WithMany(p => p.Attendance)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Schedule");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Attendance)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Students");
        });

        modelBuilder.Entity<DaysOff>(entity =>
        {
            entity.HasKey(e => e.Date).HasName("PK__DaysOFF__D9DE21FCA8DB614D");

            entity.HasOne(d => d.HolidayNavigation).WithMany(p => p.DaysOff)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DO_Holidays");
        });

        modelBuilder.Entity<Disciplines>(entity =>
        {
            entity.Property(e => e.DisciplineId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Exams>(entity =>
        {
            entity.HasOne(d => d.DisciplineNavigation).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exams_Disciplines");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exams_Students");
        });

        modelBuilder.Entity<Grades>(entity =>
        {
            entity.HasKey(e => new { e.Student, e.Lesson }).HasName("PK_Grades_1");

            entity.HasOne(d => d.LessonNavigation).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Schedule");

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grades_Students");
        });

        modelBuilder.Entity<Groups>(entity =>
        {
            entity.Property(e => e.GroupId).ValueGeneratedNever();
            entity.Property(e => e.GroupName).IsFixedLength();

            entity.HasOne(d => d.DirectionNavigation).WithMany(p => p.Groups).HasConstraintName("FK_Groups_Directions");
        });

        modelBuilder.Entity<Holidays>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__Holidays__253884EADD4BDE67");
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Salary__ED1FC9EA46979420");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();

            entity.HasOne(d => d.TeacherNavigation).WithMany(p => p.Salary)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Salary_Teachers");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasOne(d => d.DisciplineNavigation).WithMany(p => p.Schedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Disciplines");

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Schedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Groups");

            entity.HasOne(d => d.TeacherNavigation).WithMany(p => p.Schedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Teachers");
        });

        modelBuilder.Entity<Students>(entity =>
        {
            entity.Property(e => e.Phone).IsFixedLength();

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Students).HasConstraintName("FK_Students_Groups");
        });

        modelBuilder.Entity<Teachers>(entity =>
        {
            entity.Property(e => e.TeacherId).ValueGeneratedNever();
            entity.Property(e => e.Phone).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
