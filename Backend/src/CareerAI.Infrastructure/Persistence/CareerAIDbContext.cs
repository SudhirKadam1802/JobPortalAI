using CareerAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = CareerAI.Domain.Entities.Application;

namespace CareerAI.Infrastructure.Persistence;

public class CareerAIDbContext : DbContext
{
    public CareerAIDbContext(
        DbContextOptions<CareerAIDbContext> options)
        : base(options)
    {
    }

    // ========================================
    // DbSets
    // ========================================
    public DbSet<RagDocumentChunk> RagDocumentChunks
    => Set<RagDocumentChunk>();
    public DbSet<User> Users =>
        Set<User>();

    public DbSet<Candidate> Candidates =>
        Set<Candidate>();

    public DbSet<Recruiter> Recruiters =>
        Set<Recruiter>();

    public DbSet<Job> Jobs =>
        Set<Job>();

    public DbSet<Skill> Skills =>
        Set<Skill>();

    public DbSet<JobSkill> JobSkills =>
        Set<JobSkill>();

    public DbSet<ApplicationEntity> Applications =>
        Set<ApplicationEntity>();

    public DbSet<Resume> Resumes =>
        Set<Resume>();

    public DbSet<Education> Educations =>
        Set<Education>();

    public DbSet<Experience> Experiences =>
        Set<Experience>();

    public DbSet<CandidateSkill> CandidateSkills =>
        Set<CandidateSkill>();

    public DbSet<Project> Projects =>
        Set<Project>();

    public DbSet<Interview> Interviews =>
        Set<Interview>();

    public DbSet<InterviewQuestion> InterviewQuestions =>
        Set<InterviewQuestion>();

    public DbSet<Notification> Notifications =>
        Set<Notification>();

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    public DbSet<ResumeAnalysis> ResumeAnalyses =>
        Set<ResumeAnalysis>();

    public DbSet<JobRecommendation> JobRecommendations =>
        Set<JobRecommendation>();

    public DbSet<AIConversation> AIConversations =>
        Set<AIConversation>();

    public DbSet<AIMessage> AIMessages =>
        Set<AIMessage>();

    public DbSet<InterviewEvaluation> InterviewEvaluations =>
        Set<InterviewEvaluation>();

    public DbSet<SavedJob> SavedJobs =>
        Set<SavedJob>();

    public DbSet<JobApplicationNote> JobApplicationNotes =>
        Set<JobApplicationNote>();

    public DbSet<JobAlert> JobAlerts =>
        Set<JobAlert>();

    public DbSet<UserProfile> UserProfiles =>
        Set<UserProfile>();


    // ========================================
    // Model Configuration
    // ========================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<RagDocumentChunk>()
    .HasOne(x => x.Resume)
    .WithMany()
    .HasForeignKey(x => x.ResumeId)
    .OnDelete(DeleteBehavior.Cascade);
        // ========================================
        // Composite Keys
        // ========================================

        modelBuilder.Entity<JobSkill>()
            .HasKey(x => new
            {
                x.JobId,
                x.SkillId
            });

        modelBuilder.Entity<CandidateSkill>()
            .HasKey(x => new
            {
                x.CandidateId,
                x.SkillId
            });


        // ========================================
        // JobSkill Relationships
        // ========================================

        modelBuilder.Entity<JobSkill>()
            .HasOne(x => x.Job)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSkill>()
            .HasOne(x => x.Skill)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // CandidateSkill Relationships
        // ========================================

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(x => x.Candidate)
            .WithMany(x => x.CandidateSkills)
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(x => x.Skill)
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // User Relationships
        // ========================================

        modelBuilder.Entity<Candidate>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Candidate>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recruiter>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Recruiter>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AIConversation>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProfile>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Job Relationships
        // ========================================

        modelBuilder.Entity<Job>()
            .HasOne(x => x.Recruiter)
            .WithMany()
            .HasForeignKey(x => x.RecruiterId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Resume Relationships
        // ========================================

        modelBuilder.Entity<Resume>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Education Relationships
        // ========================================
        // IMPORTANT:
        // WithMany(x => x.Educations) connects
        // Candidate.Educations with Education.Candidate.
        // This prevents EF Core from creating
        // the unwanted CandidateId1 shadow property.

        modelBuilder.Entity<Education>()
            .HasOne(x => x.Candidate)
            .WithMany(x => x.Educations)
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Experience Relationships
        // ========================================

        modelBuilder.Entity<Experience>()
            .HasOne(x => x.Candidate)
            .WithMany(x => x.Experiences)
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Project Relationships
        // ========================================

        modelBuilder.Entity<Project>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Application Relationships
        // ========================================

        modelBuilder.Entity<ApplicationEntity>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationEntity>()
            .HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);


        // ========================================
        // Resume Analysis Relationships
        // ========================================

        modelBuilder.Entity<ResumeAnalysis>()
            .HasOne(x => x.Resume)
            .WithMany()
            .HasForeignKey(x => x.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================================
        // Interview Relationships
        // ========================================

        modelBuilder.Entity<Interview>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewQuestion>()
            .HasOne(x => x.Interview)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewEvaluation>()
            .HasOne(x => x.InterviewQuestion)
            .WithOne(x => x.Evaluation)
            .HasForeignKey<InterviewEvaluation>(
                x => x.InterviewQuestionId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // AI Relationships
        // ========================================

        modelBuilder.Entity<AIMessage>()
            .HasOne(x => x.Conversation)
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Saved Job Relationships
        // ========================================

        modelBuilder.Entity<SavedJob>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SavedJob>()
            .HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);


        // ========================================
        // Job Application Note Relationships
        // ========================================

        modelBuilder.Entity<JobApplicationNote>()
            .HasOne(x => x.Application)
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplicationNote>()
            .HasOne(x => x.Recruiter)
            .WithMany()
            .HasForeignKey(x => x.RecruiterId)
            .OnDelete(DeleteBehavior.Restrict);


        // ========================================
        // Job Alert Relationships
        // ========================================

        modelBuilder.Entity<JobAlert>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


        // ========================================
        // Job Recommendation Relationships
        // ========================================

        modelBuilder.Entity<JobRecommendation>()
            .HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobRecommendation>()
            .HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);


        // ========================================
        // Decimal Precision
        // ========================================

        modelBuilder.Entity<Job>()
            .Property(x => x.MinimumSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Job>()
            .Property(x => x.MaximumSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Education>()
            .Property(x => x.Grade)
            .HasPrecision(5, 2);

        modelBuilder.Entity<JobAlert>()
            .Property(x => x.MinimumSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<JobRecommendation>()
            .Property(x => x.MatchScore)
            .HasPrecision(5, 2);


        // ========================================
        // Enum Conversions
        // ========================================

        modelBuilder.Entity<User>()
            .Property(x => x.Role)
            .HasConversion<int>();

        modelBuilder.Entity<ApplicationEntity>()
            .Property(x => x.Status)
            .HasConversion<int>();

        modelBuilder.Entity<AIMessage>()
            .Property(x => x.Role)
            .HasConversion<int>();
    }
}