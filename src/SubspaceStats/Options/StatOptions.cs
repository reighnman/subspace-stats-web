using SubspaceStats.Models;

namespace SubspaceStats.Options
{
    public class StatOptions
    {
        public const string StatsSectionKey = "Stats";

        public NavbarOptions Navbar { get; set; } = new();

        public HomeOptions Home { get; set; } = new();

        public TopOptions Top { get; set; } = new();

        public PlayerDetailOptions PlayerDetails { get; set; } = new();

        /// <summary>
        /// How far back to show data for.
        /// </summary>
        public TimeSpan PeriodCutoff { get; set; } = TimeSpan.FromDays(365);
    }

    /// <summary>
    /// Options for the main Navbar on the layout page.
    /// </summary>
    public class NavbarOptions
    {
        public List<GameType> LeaderboardGameTypes { get; set; } = new();
    }

    /// <summary>
    /// Options for the Home page.
    /// </summary>
    public class HomeOptions
    {
        /// <summary>
        /// Which ratings to display on the home page.
        /// </summary>
        public List<RatingSettings> Ratings { get; set; } = new();
    }

    /// <summary>
    /// Settings that describe what rating information to display.
    /// </summary>
    public class RatingSettings
    {
        public GameType GameType { get; set; }
        public StatPeriodType PeriodType { get; set; }
        public int Top { get; set; }
    }

    /// <summary>
    /// Options for the Top player listings.
    /// </summary>
    public class TopOptions
    {
        /// <summary>
        /// Settings for the Top Players by Average Rating.
        /// </summary>
        public TopSettings AvgRating { get; set; } = new();

        /// <summary>
        /// Settings for the Top Players by Kills Per Minute.
        /// </summary>
        public TopSettings KillsPerMinute { get; set; } = new();
    }

    public class TopSettings
    {
        /// <summary>
        /// The minimum # of games a player must have played to be included.
        /// </summary>
        public int MinGamesPlayed { get; set; } = 3;
    }

    /// <summary>
    /// Options for the Player Details page.
    /// </summary>
    public class PlayerDetailOptions
    {
        /// <summary>
        /// The maximum # of kill stat records to display.
        /// </summary>
        public int KillStatsLimit { get; set; } = 50;
    }
}
