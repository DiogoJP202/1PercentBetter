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
            _ => value.ToString()
        };
    }

    public static string ToDisplayName(this ItemStatus status)
    {
        return status switch
        {
            ItemStatus.Active => "Ativo",
            ItemStatus.Paused => "Pausado",
            ItemStatus.Completed => "Concluido",
            ItemStatus.Canceled => "Cancelado",
            _ => status.ToString()
        };
    }

    public static string ToDisplayName(this GoalPriority priority)
    {
        return priority switch
        {
            GoalPriority.Low => "Baixa",
            GoalPriority.Medium => "Media",
            GoalPriority.High => "Alta",
            _ => priority.ToString()
        };
    }

    public static string ToDisplayName(this HabitDifficulty difficulty)
    {
        return difficulty switch
        {
            HabitDifficulty.VeryEasy => "Muito facil",
            HabitDifficulty.Easy => "Facil",
            HabitDifficulty.Medium => "Media",
            HabitDifficulty.Hard => "Dificil",
            HabitDifficulty.VeryHard => "Muito dificil",
            _ => difficulty.ToString()
        };
    }

    public static string ToDisplayName(this HabitFrequencyType frequencyType)
    {
        return frequencyType switch
        {
            HabitFrequencyType.Daily => "Diario",
            HabitFrequencyType.SpecificDays => "Dias especificos",
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
            HabitLogStatus.Completed => "Concluido",
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
            NoteType.DailyReflection => "Reflexao diaria",
            NoteType.Idea => "Ideia",
            NoteType.Learning => "Aprendizado",
            NoteType.Difficulty => "Dificuldade",
            NoteType.Victory => "Vitoria",
            NoteType.SystemAdjustment => "Ajuste de sistema",
            NoteType.EmotionalRecord => "Registro emocional",
            NoteType.WeeklyReview => "Revisao semanal",
            NoteType.MonthlyReview => "Revisao mensal",
            _ => noteType.ToString()
        };
    }

    public static IEnumerable<SelectListItem> ToSelectList<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value => new SelectListItem
            {
                Value = Convert.ToInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                Text = ((Enum)(object)value).ToDisplayName()
            });
    }
}
