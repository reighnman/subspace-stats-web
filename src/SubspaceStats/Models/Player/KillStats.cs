namespace SubspaceStats.Models.Player
{
	public class KillStats
	{
		public required string PlayerName { get; init; }
		public required long Kills { get; init; }
		public required long Deaths { get; init; }
	}
}
