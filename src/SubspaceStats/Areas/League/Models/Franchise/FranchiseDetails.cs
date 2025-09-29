namespace SubspaceStats.Areas.League.Models.Franchise
{
    public class FranchiseDetails
    {
        public required Franchise Franchise { get; set; }
        public required List<TeamAndSeason> TeamsAndSeasons { get; set; }
    }
}
