using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class PlayersViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<PlayerListItem> Players { get; set; }
        public required List<TeamModel> Teams { get; set; }
    }
}
