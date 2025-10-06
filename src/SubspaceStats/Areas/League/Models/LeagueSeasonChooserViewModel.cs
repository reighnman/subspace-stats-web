using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SubspaceStats.Areas.League.Models
{
    public class LeagueSeasonChooserViewModel
    {
        public LeagueSeasonChooserViewModel(long selectedSeasonId, List<LeagueNavItem> leaguesWithSeasons, IUrlHelper urlHelper)
        {
            LeaguesWithSeasons = leaguesWithSeasons;

            foreach (LeagueNavItem league in LeaguesWithSeasons)
            {
                foreach (var season in league.Seasons)
                {
                    season.Url = urlHelper.Action(null, null, new { seasonId = season.SeasonId });

                    if (season.SeasonId == selectedSeasonId)
                    {
                        SelectedLeague = league;
                        SelectedSeason = season;
                    }
                }
            }

            LeaguesWithSeasonsJson = JsonSerializer.Serialize(LeaguesWithSeasons, SeasonNavSourceGenerationContext.Default.ListLeagueNavItem);
        }

        public List<LeagueNavItem> LeaguesWithSeasons { get; }
        public LeagueNavItem? SelectedLeague { get; }
        public SeasonNavItem? SelectedSeason { get; }
        public string LeaguesWithSeasonsJson { get; }
    }
}
