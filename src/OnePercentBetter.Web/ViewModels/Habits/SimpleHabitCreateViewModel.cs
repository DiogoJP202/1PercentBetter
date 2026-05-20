using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Habits;

public class SimpleHabitCreateViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do hábito simples.")]
    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Time)]
    public TimeSpan? ScheduledTime { get; set; }
}
