using SubspaceStats.Models;
using SubspaceStats.Models.GameDetails;
using SubspaceStats.Models.Leaderboard;
using SubspaceStats.Models.Player;

namespace SubspaceStats.Services
{
    public interface IStatsRepository
    {
        Task<List<StatPeriod>> GetStatPeriods(GameType gameType, StatPeriodType statPeriodType, int limit, int offset, CancellationToken cancellationToken);

        Task<StatPeriod?> GetForeverStatPeriod(GameType gameType, CancellationToken cancellationToken);

		Task<List<TeamVersusLeaderboardStats>> GetTeamVersusLeaderboardAsync(long statPeriodId, int limit, int offset, CancellationToken cancellationToken);
        
        Task<Game?> GetGameAsync(long gameId, CancellationToken cancellationToken);

        Task<(PlayerInfo, List<StatPeriod>)?> GetPlayerInfoAndStatPeriods(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken);

		Task<List<ParticipationRecord>> GetPlayerParticipationOverview(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken);

		Task<(StatPeriod, List<TopRatingRecord>)?> GetTopPlayersByRating(GameType gameType, StatPeriodType statPeriodType, int top, CancellationToken cancellationToken);

		Task<List<TopRatingRecord>> GetTopPlayersByRating(long statPeriodId, int top, CancellationToken cancellationToken);

        Task<List<TopAvgRatingRecord>> GetTopTeamVersusPlayersByAvgRating(long statPeriodId, int top, int minGamesPlayed, CancellationToken cancellationToken);

        Task<List<TopKillsPerMinuteRecord>> GetTopTeamVersusPlayersByKillsPerMinute(long statPeriodId, int top, int minGamesPlayed,CancellationToken cancellationToken);

        Task<List<TeamVersusPeriodStats>> GetTeamVersusPeriodStats(string playerName, List<StatPeriod> statPeriodList, CancellationToken cancellationToken);

        Task<List<TeamVersusGameStats>> GetTeamVersusGameStats(string playerName, long statPeriodId, int limit, int offset, CancellationToken cancellationToken);

		Task<List<TeamVersusShipStats>> GetTeamVersusShipStats(string playerName, long statPeriodId, CancellationToken cancellationToken);

        Task<List<KillStats>> GetTeamVersusKillStats(string playerName, long statPeriodId, int limit, CancellationToken cancellationToken);
	}
}
