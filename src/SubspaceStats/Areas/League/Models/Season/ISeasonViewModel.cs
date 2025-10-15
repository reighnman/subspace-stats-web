namespace SubspaceStats.Areas.League.Models.Season
{
    public interface ISeasonViewModel
    {
        public SeasonDetails SeasonDetails { get; }
        public SeasonPage Page { get; }
        public LeagueNavItem League { get; }
        public SeasonNavItem Season { get; }
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
