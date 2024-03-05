using WebTODO.Models;

namespace WebTODO.Repositories
{
    public interface IRepository
    {
        public List<TodoItem> TodoList { get; set; }
        public TodoItem? GetItem(int id);
        public TodoItem UpdateItem(int id);
        public bool RemoveItem(TodoItem item);
        public TodoItem AddItem(TodoItem item);
    }
}
