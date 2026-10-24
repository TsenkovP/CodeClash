using Microsoft.EntityFrameworkCore;
using CodeClash.Models;

namespace CodeClash.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Competition> Competitions { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<ProjectSubmission> ProjectSubmissions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Competition>().HasData(
            new Competition
            {
                Id = 1,
                Title = "Училищно състезание по роботика и IoT",
                Description = "Състезание по програмиране и управление на хардуерни системи.",
                EventDate = new DateTime(2026, 11, 25),
                MaxTeamMembers = 4,
                IsOpenForRegistration = true
            },
            new Competition
            {
                Id = 2,
                Title = "Национален хакатон за уеб приложения",
                Description = "Разработка на облачни решения и разпределени системи.",
                EventDate = new DateTime(2026, 12, 10),
                MaxTeamMembers = 3,
                IsOpenForRegistration = true
            }
        );
    }
}