using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExamThesis.Storage.Model;

public partial class ExamContext : DbContext
{
    public ExamContext()
    {
    }

    public ExamContext(DbContextOptions<ExamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<AppFile> AppFiles { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamCategory> ExamCategories { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Exam;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answer");

            entity.HasIndex(e => e.QuestionId, "IX_Answer_QuestionId");

            entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers).HasForeignKey(d => d.QuestionId);
        });

        modelBuilder.Entity<AppFile>(entity =>
        {
            entity.ToTable("AppFile");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521C7826CB490");

            entity.ToTable("Exam");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamName).HasMaxLength(255);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExamCategory");

            entity.HasOne(d => d.Exam).WithMany()
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamCategory_Exam");

            entity.HasOne(d => d.QuestionCategory).WithMany()
                .HasForeignKey(d => d.QuestionCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamCategory_QuestionCategories");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.QuestionText).HasDefaultValueSql("(N'')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
