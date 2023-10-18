using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Options
{
	public class StatRepositoryOptions
	{
		public const string StatRepositoryOptionsKey = $"{StatOptions.StatsSectionKey}:Repository";

		/// <summary>
		/// The connection string for the Subspace Stats database.
		/// </summary>
		[Required]
		public required string ConnectionString { get; set; }
	}
}
