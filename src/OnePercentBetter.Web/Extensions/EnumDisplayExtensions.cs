using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.Extensions;

public static class EnumDisplayExtensions
{
    public static string ToDisplayName(this Enum value)
    {
        return value switch
        {
            ItemStatus itemStatus => itemStatus.ToDisplayName(),
            GoalPriority goalPriority => goalPriority.ToDisplayName(),
            HabitDifficulty habitDifficulty => habitDifficulty.ToDisplayName(),
            HabitFrequencyType habitFrequencyType => habitFrequencyType.ToDisplayName(),
            HabitLogStatus habitLogStatus => habitLogStatus.ToDisplayName(),
            MoodLevel moodLevel => moodLevel.ToDisplayName(),
            NoteType noteType => noteType.ToDisplayName(),
            TaskItemStatus taskItemStatus => taskItemStatus.ToDisplayName(),
            TaskItemPriority taskItemPriority => taskItemPriority.ToDisplayName(),
            _ => value.ToString()
        };
    }

    public static string ToDisplayName(this ItemStatus status)
    {
        return status switch
        {
            ItemStatus.Active => "Ativo",
            ItemStatus.Paused => "Pausado",
            ItemStatus.Completed => "Concluído",
            ItemStatus.Canceled => "Cancelado",
            _ => status.ToString()
        };
    }

    public static string ToDisplayName(this GoalPriority priority)
    {
        return priority switch
        {
            GoalPriority.Low => "Baixa",
            GoalPriority.Medium => "Média",
            GoalPriority.High => "Alta",
            _ => priority.ToString()
        };
    }

    public static string ToDisplayName(this HabitDifficulty difficulty)
    {
        return difficulty switch
        {
            HabitDifficulty.VeryEasy => "Muito fácil",
            HabitDifficulty.Easy => "Fácil",
            HabitDifficulty.Medium => "Média",
            HabitDifficulty.Hard => "Difícil",
            HabitDifficulty.VeryHard => "Muito difícil",
            _ => difficulty.ToString()
        };
    }

    public static string ToDisplayName(this HabitFrequencyType frequencyType)
    {
        return frequencyType switch
        {
            HabitFrequencyType.Daily => "Diário",
            HabitFrequencyType.SpecificDays => "Dias específicos",
            HabitFrequencyType.Weekly => "Semanal",
            HabitFrequencyType.Monthly => "Mensal",
            HabitFrequencyType.Custom => "Personalizado",
            _ => frequencyType.ToString()
        };
    }

    public static string ToDisplayName(this HabitLogStatus status)
    {
        return status switch
        {
            HabitLogStatus.Completed => "Concluído",
            HabitLogStatus.Skipped => "Pulado",
            HabitLogStatus.Failed => "Falhou",
            HabitLogStatus.Partial => "Parcial",
            _ => status.ToString()
        };
    }

    public static string ToDisplayName(this MoodLevel mood)
    {
        return mood switch
        {
            MoodLevel.VeryBad => "Muito ruim",
            MoodLevel.Bad => "Ruim",
            MoodLevel.Neutral => "Neutro",
            MoodLevel.Good => "Bom",
            MoodLevel.VeryGood => "Muito bom",
            _ => mood.ToString()
        };
    }

    public static string ToDisplayName(this NoteType noteType)
    {
        return noteType switch
        {
            NoteType.DailyReflection => "Reflexão diária",
            NoteType.Idea => "Ideia",
            NoteType.Learning => "Aprendizado",
            NoteType.Difficulty => "Dificuldade",
            NoteType.Victory => "Vitória",
            NoteType.SystemAdjustment => "Ajuste de sistema",
            NoteType.EmotionalRecord => "Registro emocional",
            NoteType.WeeklyReview => "Revisão semanal",
            NoteType.MonthlyReview => "Revisão mensal",
            _ => noteType.ToString()
        };
    }

    public static string ToDisplayName(this TaskItemStatus status)
    {
        return status switch
        {
            TaskItemStatus.Pending => "Pendente",
            TaskItemStatus.InProgress => "Em andamento",
            TaskItemStatus.Completed => "Concluida",
            TaskItemStatus.Cancelled => "Cancelada",
            TaskItemStatus.Postponed => "Adiada",
            _ => status.ToString()
        };
    }

    public static string ToDisplayName(this TaskItemPriority priority)
    {
        return priority switch
        {
            TaskItemPriority.Low => "Baixa",
            TaskItemPriority.Medium => "Media",
            TaskItemPriority.High => "Alta",
            TaskItemPriority.Urgent => "Urgente",
            _ => priority.ToString()
        };
    }

    public static IEnumerable<SelectListItem> ToSelectList<TEnum>()
        where TEnum : struct, Enum
    {
        return ToSelectList<TEnum>(Array.Empty<TEnum>());
    }

    public static IEnumerable<SelectListItem> ToSelectList<TEnum>(params TEnum[] excludedValues)
        where TEnum : struct, Enum
    {
        var excluded = excludedValues.ToHashSet();

        return Enum.GetValues<TEnum>()
            .Where(value => !excluded.Contains(value))
            .Select(value => new SelectListItem
            {
                Value = Convert.ToInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                Text = ((Enum)(object)value).ToDisplayName()
            });
    }
}
