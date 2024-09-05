namespace SubspaceStats.Models.Leaderboard
{
    public record TopRatingRecord(int Rank, string PlayerName, int Rating);

    public record TopAvgRatingRecord(int Rank, string PlayerName, float AvgRating);

    public record TopKillsPerMinuteRecord(int Rank, string PlayerName, float KillsPerMinute);
}
