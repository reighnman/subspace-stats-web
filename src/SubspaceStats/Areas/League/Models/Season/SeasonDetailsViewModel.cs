
namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonDetailsViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required SeasonDetails Details { get; init; }
        public required OrderedDictionary<long, string> GameTypes { get; set; }
    }
}
