namespace SubspaceStats.Areas.League.Models.League
{
    public interface ILeagueViewModel
    {
        public LeagueModel? League { get; }
        public LeagueSection Section { get; }
    }

    public enum LeagueSection
    {
        Details,
        Roles,
        Edit,
        Manage,
    }
}
