using CodeClash.Models;

namespace CodeClash.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ако вече има състезания, базата не се инициализира повторно
        if (context.Competitions.Any())
        {
            return;
        }

        var competition1 = new Competition
        {
            Title = "Училищен хакатон 2026",
            Description = "Състезание по разработка на иновативни софтуерни решения за градска среда и образование.",
            EventDate = DateTime.UtcNow.AddDays(14),
            IsOpenForRegistration = true,
            MaxTeamMembers = 4
        };

        var competition2 = new Competition
        {
            Title = "Есенна олимпиада по ИТ",
            Description = "Турнир за уеб приложения и алгоритмично програмиране.",
            EventDate = DateTime.UtcNow.AddDays(-7),
            IsOpenForRegistration = false,
            MaxTeamMembers = 3
        };

        context.Competitions.AddRange(competition1, competition2);
        await context.SaveChangesAsync();

        var team1 = new Team
        {
            TeamName = "Binary Masters",
            MentorName = "Иван Димитров",
            ContactEmail = "binary@school.bg",
            RegisteredOn = DateTime.UtcNow.AddDays(-5),
            CompetitionId = competition1.Id
        };

        var team2 = new Team
        {
            TeamName = "CodeCrafters",
            MentorName = "Мария Георгиева",
            ContactEmail = "crafters@school.bg",
            RegisteredOn = DateTime.UtcNow.AddDays(-4),
            CompetitionId = competition1.Id
        };

        var team3 = new Team
        {
            TeamName = "AlgoRhythm",
            MentorName = "Петър Василев",
            ContactEmail = "algorhythm@school.bg",
            RegisteredOn = DateTime.UtcNow.AddDays(-10),
            CompetitionId = competition2.Id
        };

        context.Teams.AddRange(team1, team2, team3);
        await context.SaveChangesAsync();

        var submission1 = new ProjectSubmission
        {
            ProjectTitle = "SmartTraffic Burgas",
            GitHubUrl = "https://github.com/example/smart-traffic",
            Score = 95,
            SubmittedOn = DateTime.UtcNow.AddDays(-2),
            TeamId = team1.Id
        };

        var submission2 = new ProjectSubmission
        {
            ProjectTitle = "EduPlatform",
            GitHubUrl = "https://github.com/example/edu-platform",
            Score = 88,
            SubmittedOn = DateTime.UtcNow.AddDays(-1),
            TeamId = team2.Id
        };

        context.ProjectSubmissions.AddRange(submission1, submission2);
        await context.SaveChangesAsync();
    }
}