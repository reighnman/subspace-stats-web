using SubspaceStats.Models;
using SubspaceStats.Models.GameDetails;
using SubspaceStats.Models.Leaderboard;
using SubspaceStats.Models.Player;

namespace SubspaceStats.Services
{
    /// <summary>
    /// Interface for a service that provides stats data.
    /// </summary>
    public interface IStatsRepository
    {
        #region Game Type

        Task<OrderedDictionary<long, GameType>> GetGameTypesAsync(CancellationToken cancellationToken);
        Task<GameType?> GetGameTypeAsync(long gameTypeId, CancellationToken cancellationToken);
        Task<long> InsertGameTypeAsync(string name, GameMode mode, CancellationToken cancellationToken);
        Task UpdateGameTypeAsync(long gameTypeId, string name, GameMode mode, CancellationToken cancellationToken);
        Task DeleteGameTypeAsync(long gameTypeId, CancellationToken cancellationToken);

        #endregion

        /// <summary>
        /// Gets the available stats periods for a specified game type and period type.
        /// </summary>
        /// <param name="gameType">The game type to get stat periods for.</param>
        /// <param name="statPeriodType">The type of stat period to get. <see langword="null"/> means get all periods (except for 'forever').</param>
        /// <param name="limit">The maximum # of stat periods to return. <see langword="null"/> means no limit.</param>
        /// <param name="offset">The offset of the stat periods to return.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The stat periods.</returns>
        Task<List<StatPeriod>> GetStatPeriods(long gameType, StatPeriodType? statPeriodType, int? limit, int offset, CancellationToken cancellationToken);

        /// <summary>
        /// Get the forever (lifetime) stat period for a specified game type.
        /// </summary>
        /// <param name="gameType">The game type to get the stat period for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The stat period, or <see langword="null"/> if not found.</returns>
        Task<StatPeriod?> GetForeverStatPeriod(long gameType, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the leaderboard for a team versus stat period.
        /// </summary>
        /// <param name="statPeriodId">Id of the stat period to get the leaderboard for. This identifies both the game type and period range.</param>
        /// <param name="limit">The maximum # of records to return (for pagination).</param>
        /// <param name="offset">The offset of the records to return (for pagination).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The leaderboard.</returns>
        Task<List<TeamVersusLeaderboardStats>> GetTeamVersusLeaderboardAsync(long statPeriodId, int limit, int offset, CancellationToken cancellationToken);

        /// <summary>
        /// Gets details about a single game.
        /// </summary>
        /// <param name="gameId">Id of the game to get data for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The game details, or <see langword="null"/> if not found.</returns>
        Task<Game?> GetGameAsync(long gameId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets information about a player and the stat periods that the player has participated in.
        /// </summary>
        /// <param name="playerName">The name of the player to get data for.</param>
        /// <param name="periodCutoff">How far back in time to look for periods.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player info and list of stat periods, or <see langword="null"/> if the player was not found.</returns>
        Task<(PlayerInfo, List<StatPeriod>)?> GetPlayerInfoAndStatPeriods(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a player's recent participation across game types.
        /// </summary>
        /// <param name="playerName">The name of the player to get data for.</param>
        /// <param name="periodCutoff">How far back in time to look for data.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player's participation records.</returns>
        Task<List<ParticipationRecord>> GetPlayerParticipationOverview(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the top players by rating for the latest stat period of a given <paramref name="gameType"/> and <paramref name="statPeriodType"/>.
        /// </summary>
        /// <param name="gameType">The game type to get the latest stat period for.</param>
        /// <param name="statPeriodType">The type of stat period to get the latest period.</param>
        /// <param name="top"><inheritdoc cref="GetTopPlayersByRating(long, int, CancellationToken)" path="/param[@name='top']"/></param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The latest stat period and the records for the top players in the period, or <see langword="null"/> if not found.</returns>
        Task<(StatPeriod, List<TopRatingRecord>)?> GetTopPlayersByRating(long gameTypeId, StatPeriodType statPeriodType, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the top players by rating for a specified stat period.
        /// </summary>
        /// <param name="statPeriodId">Id of the stat period to get data for.</param>
        /// <param name="top">
        /// The rank limit results.
        /// E.g. specify 5 to get players with rank [1 - 5].
        /// This is not the limit of the # of players to return.
        /// If multiple players share the same rank, they will all be returned.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The records of the top players.</returns>
        Task<List<TopRatingRecord>> GetTopPlayersByRating(long statPeriodId, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the top players by average rating for a specified stat period.
        /// </summary>
        /// <param name="statPeriodId">Id of the stat period to get data for.</param>
        /// <param name="top"><inheritdoc cref="GetTopPlayersByRating(long, int, CancellationToken)" path="/param[@name='top']"/></param>
        /// <param name="minGamesPlayed">The minimum # of games a player must have played to be included in the result.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The records of the top players.</returns>
        Task<List<TopAvgRatingRecord>> GetTopTeamVersusPlayersByAvgRating(long statPeriodId, int top, int minGamesPlayed, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the top players by kills per minute for a specified stat period.
        /// </summary>
        /// <param name="statPeriodId">Id of the stat period to get data for.</param>
        /// <param name="top"><inheritdoc cref="GetTopPlayersByRating(long, int, CancellationToken)" path="/param[@name='top']"/></param>
        /// <param name="minGamesPlayed">The minimum # of games a player must have played to be included in the result.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The records of the top players.</returns>
        Task<List<TopKillsPerMinuteRecord>> GetTopTeamVersusPlayersByKillsPerMinute(long statPeriodId, int top, int minGamesPlayed, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a player's team versus stats for a specified set of stat periods.
        /// </summary>
        /// <param name="playerName">The name of player to get stats for.</param>
        /// <param name="statPeriodList">The stat periods to get data for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player's period stats records.</returns>
        Task<List<TeamVersusPeriodStats>> GetTeamVersusPeriodStats(string playerName, List<StatPeriod> statPeriodList, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a player's team versus game stats for a specified stat period.
        /// </summary>
        /// <param name="playerName">The name of the player to get data for.</param>
        /// <param name="statPeriodId">The period to get data for.</param>
        /// <param name="limit">The maximum # of game records to return.</param>
        /// <param name="offset">The offset of the game records to return.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player's game stats records.</returns>
        Task<List<TeamVersusGameStats>> GetTeamVersusGameStats(string playerName, long statPeriodId, int limit, int offset, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a player's versus game ship stats for a specified stat period.
        /// </summary>
        /// <param name="playerName">The name of the player to get stats for.</param>
        /// <param name="statPeriodId">Id of the stat period to get data for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player's ship stats records.</returns>
        Task<List<TeamVersusShipStats>> GetTeamVersusShipStats(string playerName, long statPeriodId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a player's team versus kill stats for a specified stat period.
        /// </summary>
        /// <param name="playerName">The name of the player to get stats for.</param>
        /// <param name="statPeriodId">Id of the period to get stats for.</param>
        /// <param name="limit">The maximum # of records to return.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The player's kill stats records.</returns>
        Task<List<KillStats>> GetTeamVersusKillStats(string playerName, long statPeriodId, int limit, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes the player stats for a team versus stat period.
        /// </summary>
        /// <param name="statPeriodId">The stat period to refresh stats for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RefreshTeamVersusPlayerStats(long statPeriodId, CancellationToken cancellationToken);
    }
}
