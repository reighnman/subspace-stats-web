namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<ScheduledGame> ScheduledGames { get; init; }
        public required List<TeamStanding> Standings { get; init; }
    }
}
