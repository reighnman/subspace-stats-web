namespace SubspaceStats.Models
{
    // TODO: Use this instead of the GameType enum. Read it from the database and cache it?
    public class GameType
    {
        public required long Id;
        public required string Name { get; init; }

        public required GameMode GameMode { get; init; }
    }

    public enum GameMode
    {
        Undefined = 0,

        OneVersusOne = 1,
        TeamVersus = 2,
        Powerball = 3,
    }
}
