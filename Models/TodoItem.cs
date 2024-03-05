namespace WebTODO.Models
{
    public class TodoItem
    {
        static int idCounter = 0;
        public int Id { get; } = idCounter++;
        public required string Title { get; set; } = "";
        public required string Description { get; set; } = "";
        public required DateOnly Date { get; set; }
    }
}
