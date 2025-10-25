namespace SubspaceStats.Areas.League.Models.Season
{
    public interface ISeasonViewModel
    {
        public SeasonDetails SeasonDetails { get; }
        public SeasonPage Page { get; }
        public LeagueNavItem LeagueNav { get; }
        public SeasonNavItem SeasonNav { get; }
    }

    public enum SeasonPage
    {
        Overview,
        Rosters,
        Details,
        Players,
        Teams,
        Games,
        Rounds,
        Roles,
    }
}
