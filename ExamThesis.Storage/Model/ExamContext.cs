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

    public virtual DbSet<ExamResult> ExamResults { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }

    public virtual DbSet<QuestionPackage> QuestionPackages { get; set; }

    public virtual DbSet<QuestionsInPackage> QuestionsInPackages { get; set; }

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
            entity.ToTable("ExamCategory");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamCategories)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamCategory_Exam");

            entity.HasOne(d => d.QuestionCategory).WithMany(p => p.ExamCategories)
                .HasForeignKey(d => d.QuestionCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamCategory_QuestionCategories");
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.Property(e => e.StudentId)
                .HasMaxLength(20)
                .IsFixedLength();

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamResults_ExamId");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.QuestionText).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.Package).WithMany(p => p.Questions)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_Question_Package");
        });

        modelBuilder.Entity<QuestionPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Question__322035CC548505C7");

            entity.Property(e => e.PackageName).HasMaxLength(255);

            entity.HasOne(d => d.QuestionCategory).WithMany(p => p.QuestionPackages)
                .HasForeignKey(d => d.QuestionCategoryId)
                .HasConstraintName("FK__QuestionP__Quest__0880433F");
        });

        modelBuilder.Entity<QuestionsInPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07BB7DED0B");

            entity.ToTable("QuestionsInPackage");

            entity.HasOne(d => d.Package).WithMany(p => p.QuestionsInPackages)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Questions__Packa__0B5CAFEA");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionsInPackages)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Questions__Quest__0A688BB1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
