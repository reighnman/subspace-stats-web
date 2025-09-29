using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class TeamsViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<TeamModel> Teams { get; set; }
    }
}
