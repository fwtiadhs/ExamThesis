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

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }

    public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }

    public virtual DbSet<QuestionType> QuestionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Exam;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answer");

            entity.HasIndex(e => e.QuestionId, "IX_Answer_QuestionId");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers).HasForeignKey(d => d.QuestionId);
        });

        modelBuilder.Entity<AppFile>(entity =>
        {
            entity.ToTable("AppFile");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasIndex(e => e.QuestionTypeId, "IX_Questions_QuestionTypeId");

            entity.Property(e => e.QuestionText).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.QuestionType).WithMany(p => p.Questions).HasForeignKey(d => d.QuestionTypeId);
        });

        modelBuilder.Entity<QuestionAnswer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QuestionAnswer");

            entity.HasIndex(e => e.AnswerId, "IX_QuestionAnswer_AnswerId");

            entity.HasIndex(e => e.QuestionId, "IX_QuestionAnswer_QuestionId");

            entity.HasOne(d => d.Answer).WithMany()
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Question).WithMany()
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.ToTable("QuestionType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
