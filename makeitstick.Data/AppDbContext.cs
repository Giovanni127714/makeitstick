using makeitstick.Models;
using Microsoft.EntityFrameworkCore;

namespace makeitstick.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Klas> Classes => Set<Klas>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        public DbSet<Reference> References => Set<Reference>();
        public DbSet<QuestionSet> QuestionSets => Set<QuestionSet>();
        public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
        public DbSet<Answer> Answers => Set<Answer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(e =>
            {
                e.ToTable("teachers");
                e.Property(t => t.FirstName).HasMaxLength(100).IsRequired();
                e.Property(t => t.LastName).HasMaxLength(100).IsRequired();
                e.Property(t => t.Email).HasMaxLength(255).IsRequired();
                e.HasIndex(t => t.Email).IsUnique();
            });

            modelBuilder.Entity<Klas>(e =>
            {
                e.ToTable("classes");
                e.Property(k => k.Name).HasMaxLength(50).IsRequired();
                e.HasIndex(k => k.Name).IsUnique();
                // Een docent met klassen kan niet zomaar verwijderd worden.
                e.HasOne(k => k.Teacher)
                    .WithMany(t => t.Classes)
                    .HasForeignKey(k => k.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(e =>
            {
                e.ToTable("students");
                e.Property(s => s.StudentNumber).HasMaxLength(20).IsRequired();
                e.Property(s => s.FirstName).HasMaxLength(100).IsRequired();
                e.Property(s => s.LastName).HasMaxLength(100).IsRequired();
                e.Property(s => s.Email).HasMaxLength(255).IsRequired();
                e.HasIndex(s => s.StudentNumber).IsUnique();
                e.HasIndex(s => s.Email).IsUnique();
                e.HasOne(s => s.Klas)
                    .WithMany(k => k.Students)
                    .HasForeignKey(s => s.KlasId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Chapter>(e =>
            {
                e.ToTable("chapters");
                e.Property(c => c.Title).HasMaxLength(200).IsRequired();
                e.Property(c => c.Description).HasMaxLength(1000);
                e.HasIndex(c => c.Number).IsUnique();
            });

            modelBuilder.Entity<Reference>(e =>
            {
                e.ToTable("references");
                e.Property(r => r.Title).HasMaxLength(255).IsRequired();
                e.Property(r => r.Author).HasMaxLength(255);
                e.Property(r => r.Url).HasMaxLength(500);
                e.HasOne(r => r.Chapter)
                    .WithMany(c => c.References)
                    .HasForeignKey(r => r.ChapterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuestionSet>(e =>
            {
                e.ToTable("question_sets");
                e.Property(q => q.Question).HasMaxLength(1000).IsRequired();
                e.Property(q => q.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
                e.HasOne(q => q.Chapter)
                    .WithMany(c => c.QuestionSets)
                    .HasForeignKey(q => q.ChapterId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(q => q.Teacher)
                    .WithMany(t => t.QuestionSets)
                    .HasForeignKey(q => q.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AnswerOption>(e =>
            {
                e.ToTable("answer_options");
                e.Property(o => o.Text).HasMaxLength(500).IsRequired();
                e.HasOne(o => o.QuestionSet)
                    .WithMany(q => q.AnswerOptions)
                    .HasForeignKey(o => o.QuestionSetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Answer>(e =>
            {
                e.ToTable("answers");
                e.Property(a => a.AnsweredAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
                e.HasOne(a => a.Student)
                    .WithMany(s => s.Answers)
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.QuestionSet)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionSetId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.AnswerOption)
                    .WithMany(o => o.Answers)
                    .HasForeignKey(a => a.AnswerOptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
