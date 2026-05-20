namespace OnePercentBetter.Web.Models.Entities;

public class TaskItemTag
{
    public int TaskItemId { get; set; }

    public TaskItem? TaskItem { get; set; }

    public int TaskTagId { get; set; }

    public TaskTag? TaskTag { get; set; }
}
