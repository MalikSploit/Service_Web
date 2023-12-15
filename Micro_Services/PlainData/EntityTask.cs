namespace Entities;

public class EntityTask
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public bool IsDone { get; set; }

}
public class TaskCreate
{
    public required string Text { get; set; }
    public bool IsDone { get; set; }
}
