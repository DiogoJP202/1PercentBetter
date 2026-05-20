namespace OnePercentBetter.Web.ViewModels.Tasks;

public sealed record TaskColorOption(string Name, string Value);

public sealed record TaskIconOption(string Name, string Value);

public static class TaskVisualOptions
{
    public const string DefaultColor = "#a78bfa";

    public const string DefaultIcon = "list-checks";

    public static IReadOnlyList<TaskColorOption> Colors { get; } =
    [
        new("Roxo", "#a78bfa"),
        new("Ciano", "#38bdf8"),
        new("Verde", "#22c55e"),
        new("Amarelo", "#f59e0b"),
        new("Vermelho", "#f43f5e")
    ];

    public static IReadOnlyList<TaskIconOption> Icons { get; } =
    [
        new("Checklist", "list-checks"),
        new("Alvo", "target"),
        new("Codigo", "code-2"),
        new("Calendario", "calendar-days"),
        new("Relogio", "clock-3"),
        new("Documento", "file-text"),
        new("Estudo", "book-open"),
        new("Casa", "home"),
        new("Trabalho", "briefcase-business"),
        new("Saude", "heart-pulse")
    ];

    public static bool IsAllowedIcon(string? icon)
    {
        return !string.IsNullOrWhiteSpace(icon)
            && Icons.Any(option => string.Equals(option.Value, icon, StringComparison.OrdinalIgnoreCase));
    }
}
