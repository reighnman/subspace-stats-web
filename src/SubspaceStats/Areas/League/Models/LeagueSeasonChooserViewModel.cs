using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SubspaceStats.Areas.League.Models.Season;
using System.Text.Json;

namespace SubspaceStats.Areas.League.Models
{
    public class LeagueSeasonChooserViewModel
    {
        public LeagueSeasonChooserViewModel(long selectedSeasonId, List<LeagueWithSeasons> leaguesWithSeasons, IUrlHelper urlHelper)
        {
            SelectedSeasonId = selectedSeasonId;
            LeaguesWithSeasons = leaguesWithSeasons;

            foreach (LeagueWithSeasons league in LeaguesWithSeasons)
            {
                foreach (var season in league.Seasons)
                {
                    season.Url = urlHelper.Action(null, null, new { seasonId = season.SeasonId });

                    if (season.SeasonId == SelectedSeasonId)
                    {
                        SelectedLeagueId = league.LeagueId;
                    }
                }
            }

            LeaguesWithSeasonsJson = JsonSerializer.Serialize(LeaguesWithSeasons, LeagueWithSeasonsSourceGenerationContext.Default.ListLeagueWithSeasons);
        }

        public long? SelectedLeagueId { get; }
        public long SelectedSeasonId { get; }
        public string LeaguesWithSeasonsJson { get; }

        public List<LeagueWithSeasons> LeaguesWithSeasons { get; }
    }
}
