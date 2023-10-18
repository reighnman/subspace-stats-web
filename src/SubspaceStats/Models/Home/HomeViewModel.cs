using SubspaceStats.Models.Leaderboard;

namespace SubspaceStats.Models.Home
{
	public class HomeViewModel
	{
		public required List<(StatPeriod Period, List<TopRatingRecord> TopRatingList)> TopRatings { get; init; }
	}
}
