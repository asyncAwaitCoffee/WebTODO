using WebTODO.Models;

namespace WebTODO.Repositories
{
    public class MemRepository : IRepository
    {
        public List<TodoItem> TodoList { get; set; } = [
                new() { Title = "Vacuum", Description = "Vacuum my room", Date = new DateOnly(2024,3,10)},
                new() { Title = "Dishes", Description = "Wash dishes", Date = new DateOnly(2024,3,7)},
                new() { Title = "Chores", Description = "Do chores", Date = new DateOnly(2024,3,9)},
                ];

        public TodoItem AddItem(TodoItem item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveItem(TodoItem item)
        {
            return TodoList.Remove(item);
        }

        public TodoItem? GetItem(int id)
        {
            return TodoList.Find(t => t.Id == id);
        }

        public TodoItem UpdateItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
