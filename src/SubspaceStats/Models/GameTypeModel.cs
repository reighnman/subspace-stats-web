namespace SubspaceStats.Models
{
    // TODO: Use this instead of the GameType enum. Read it from the database and cache it?
    public class GameTypeModel
    {
        public required long Id;
        public required string Name { get; set; }
    }
}
