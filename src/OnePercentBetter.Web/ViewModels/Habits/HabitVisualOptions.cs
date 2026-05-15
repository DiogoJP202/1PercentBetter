namespace OnePercentBetter.Web.ViewModels.Habits;

public sealed record HabitColorOption(string Name, string Value);

public sealed record HabitIconOption(string Name, string Value);

public static class HabitVisualOptions
{
    public const string DefaultColor = "#22c55e";

    public const string DefaultIcon = "repeat-2";

    public static IReadOnlyList<HabitColorOption> Colors { get; } =
    [
        new("Azul", "#38bdf8"),
        new("Verde", "#22c55e"),
        new("Roxo", "#a78bfa"),
        new("Laranja", "#f59e0b"),
        new("Vermelho", "#f43f5e")
    ];

    public static IReadOnlyList<HabitIconOption> Icons { get; } =
    [
        new("Repetição", "repeat-2"),
        new("Livro", "book-open"),
        new("Exercício", "dumbbell"),
        new("Café", "coffee"),
        new("Mente", "brain"),
        new("Código", "code"),
        new("Idiomas", "languages"),
        new("Música", "music"),
        new("Coração", "heart"),
        new("Sono", "moon"),
        new("Manhã", "sun"),
        new("Meta", "target"),
        new("Calendário", "calendar"),
        new("Horário", "clock"),
        new("Casa", "home"),
        new("Brilho", "sparkles")
    ];

    public static bool IsAllowedIcon(string? icon)
    {
        return !string.IsNullOrWhiteSpace(icon)
            && Icons.Any(option => string.Equals(option.Value, icon, StringComparison.OrdinalIgnoreCase));
    }
}
