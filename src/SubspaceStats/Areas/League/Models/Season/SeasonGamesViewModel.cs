using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonGamesViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<GameListItem> Games { get; set; }
        public required List<TeamModel> Teams { get; set; }
    }
}
