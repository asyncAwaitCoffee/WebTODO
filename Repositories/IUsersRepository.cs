using WebTODO.Models;

namespace WebTODO.Repositories
{
    public interface IUsersRepository
    {
        public List<User> Users { get; }
        public User? GetUser(string name, string password);
    }
}
