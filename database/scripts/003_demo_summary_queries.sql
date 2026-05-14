/*
    Consultas rapidas para visualizar os dados demo do MVP.
*/

DECLARE @UserId nvarchar(450) = N'demo-user-1better';

SELECT
    Id,
    Email,
    DisplayName,
    MainColor,
    ThemePreference,
    CreatedAt,
    OnboardingCompletedAt
FROM AspNetUsers
WHERE Id = @UserId;

SELECT
    i.Id,
    i.Name,
    i.IdentityStatement,
    c.Name AS CategoryName,
    i.Status,
    i.Color,
    i.Icon,
    COUNT(DISTINCT g.Id) AS GoalsCount,
    COUNT(DISTINCT h.Id) AS HabitsCount
FROM Identities i
LEFT JOIN Categories c ON c.Id = i.CategoryId
LEFT JOIN Goals g ON g.IdentityId = i.Id
LEFT JOIN Habits h ON h.IdentityId = i.Id
WHERE i.UserId = @UserId
GROUP BY
    i.Id,
    i.Name,
    i.IdentityStatement,
    c.Name,
    i.Status,
    i.Color,
    i.Icon
ORDER BY i.Id;

SELECT
    g.Id,
    g.Title,
    i.Name AS IdentityName,
    c.Name AS CategoryName,
    g.Status,
    g.Priority,
    g.StartDate,
    g.TargetDate,
    COUNT(h.Id) AS HabitsCount
FROM Goals g
LEFT JOIN Identities i ON i.Id = g.IdentityId
LEFT JOIN Categories c ON c.Id = g.CategoryId
LEFT JOIN Habits h ON h.GoalId = g.Id
WHERE g.UserId = @UserId
GROUP BY
    g.Id,
    g.Title,
    i.Name,
    c.Name,
    g.Status,
    g.Priority,
    g.StartDate,
    g.TargetDate
ORDER BY g.Id;

SELECT
    h.Id,
    h.Title,
    i.Name AS IdentityName,
    g.Title AS GoalTitle,
    c.Name AS CategoryName,
    h.FrequencyType,
    h.Difficulty,
    h.TwoMinuteVersion,
    h.[Trigger],
    h.Status
FROM Habits h
LEFT JOIN Identities i ON i.Id = h.IdentityId
LEFT JOIN Goals g ON g.Id = h.GoalId
LEFT JOIN Categories c ON c.Id = h.CategoryId
WHERE h.UserId = @UserId
ORDER BY h.Id;

SELECT
    h.Title,
    l.Date,
    l.Status,
    l.CompletedAt,
    l.Mood,
    l.EnergyLevel,
    l.DifficultyFelt,
    l.Notes
FROM HabitLogs l
INNER JOIN Habits h ON h.Id = l.HabitId
WHERE l.UserId = @UserId
ORDER BY l.Date DESC, h.Title;

SELECT
    Date,
    Mood,
    EnergyLevel,
    ProductivityLevel,
    DayScore,
    SmallWin,
    MainDifficulty,
    TomorrowAdjustment
FROM DailyCheckIns
WHERE UserId = @UserId
ORDER BY Date DESC;

SELECT
    n.Date,
    n.Title,
    n.NoteType,
    n.Tags,
    i.Name AS IdentityName,
    g.Title AS GoalTitle,
    h.Title AS HabitTitle
FROM Notes n
LEFT JOIN Identities i ON i.Id = n.IdentityId
LEFT JOIN Goals g ON g.Id = n.GoalId
LEFT JOIN Habits h ON h.Id = n.HabitId
WHERE n.UserId = @UserId
ORDER BY n.Date DESC;
