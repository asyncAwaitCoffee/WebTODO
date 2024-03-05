namespace WebTODO.Models
{
    public class User
    {
        static private int idCounter = 0;
        public int Id { get; } = idCounter++;
        public required string Name { get; init; }
        public required string Password { get; init; }
    }
}
