using WebTODO.Models;

namespace WebTODO.Repositories
{
    public class MemUsersRepository : IUsersRepository
    {
        public List<User> Users { get; } = [new() { Name = "oni", Password = "123"  }];

        public User? GetUser(string name, string password)
        {
            return Users.Find(u => u.Name == name && u.Password == password);
        }
    }
}
