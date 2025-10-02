using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Services
{
    public interface ILeagueRepository
    {
        Task<List<SeasonStandings>?> GetLatestSeasonsStandingsAsync(long[] leagueIds, CancellationToken cancellationToken);
        Task<List<SeasonRoster>?> GetSeasonRostersAsync(long seasonId, CancellationToken cancellationToken);
        Task<List<TeamStanding>> GetSeasonStandingsAsync(long seasonId, CancellationToken cancellationToken);
        Task<List<ScheduledGame>> GetScheduledGamesAsync(long seasonId, CancellationToken cancellationToken);
        Task<List<LeagueWithSeasons>> GetLeaguesWithSeasonsAsync(CancellationToken cancellationToken);
        
        #region Franchise

        Task<List<FranchiseListItem>> GetFranchiseListAsync(CancellationToken cancellationToken);
        Task<List<TeamAndSeason>> GetFranchiseTeamsAsync(long franchiseId, CancellationToken cancellationToken);
        Task<Franchise?> GetFranchiseAsync(long franchiseId, CancellationToken cancellationToken);

        Task<long> InsertFranchiseAsync(string franchiseName, CancellationToken cancellationToken);

        Task UpdateFranchiseAsync(Franchise franchise, CancellationToken cancellationToken);
        Task DeleteFranchiseAsync(long franchiseId, CancellationToken cancellationToken);

        #endregion

        #region League

        Task<List<LeagueModel>> GetLeagueListAsync(CancellationToken cancellationToken);
        Task<LeagueModel?> GetLeagueAsync(long leagueId, CancellationToken cancellationToken);
        Task<long> InsertLeagueAsync(string name, long gameType, CancellationToken cancellationToken);
        Task UpdateLeagueAsync(LeagueModel league, CancellationToken cancellationToken);
        Task DeleteLeagueAsync(long leagueId, CancellationToken cancellationToken);
        Task<List<Areas.League.Models.League.SeasonListItem>> GetSeasonsAsync(long leagueId, CancellationToken cancellationToken);

        #endregion

        #region Season

        Task<SeasonDetails?> GetSeasonDetailsAsync(long seasonId, CancellationToken cancellationToken);
        Task StartSeasonAsync(long seasonId, DateTime? startDate, CancellationToken cancellationToken);

        Task<long> CopySeasonAsync(long seasonId, string seasonName, bool includePlayers, bool includeTeams, bool includeGames, bool includeRounds, CancellationToken cancellationToken);

        //Task SetSeasonEndDateAsync(long sesaonId, DateTime? endDate, CancellationToken cancellationToken);

        // InsertSeasonAsync

        // UpdateSeasonAsync

        // DeleteSeasonAsync

        Task<List<TeamModel>> GetSeasonTeamsAsync(long seasonId, CancellationToken cancellationToken);

        #endregion

        #region Season Players

        Task<List<PlayerListItem>> GetSeasonPlayersAsync(long seasonId, CancellationToken cancellationToken);

        // BulkInsertSeasonPlayers()

        // InsertSeasonPlayer

        // UpdateSeasonPlayer

        // DeleteSeasonPlayer

        // From list of season players (/league/season/<id>/players), check off the ones to assign, click "Assign Team" button (post),
        // show a page that lists the selection (including if the player already is assigned a team, meaning the team will be switched) and the user selects the team.
        // This should make it easy to assign a single player or multiple players at once.
        // Task AssignToTeamAsync(long teamId, string[] playerNames, CancellationToken cancellationToken)

        #endregion

        #region Season Teams

        Task<List<TeamGameRecord>> GetTeamGames(long teamId, CancellationToken cancellationToken);
        Task<TeamWithSeasonInfo?> GetTeamsWithSeasonInfosync(long teamId, CancellationToken cancellationToken);
        Task<TeamModel?> GetTeamAsync(long teamId, CancellationToken cancellationToken);

        //Task<long> InsertTeamAsync(string teamName, CancellationToken cancellationToken);

        //Task UpdateTeamAsync(Team Team, CancellationToken cancellationToken);
        //Task DeleteTeamAsync(long TeamId, CancellationToken cancellationToken);

        #endregion

        

        #region Season Games

        Task<List<GameListItem>> GetSeasonGamesAsync(long seasonId, CancellationToken cancellationToken);

        // InsertSeasonGamesFor2TeamPerMatch(long seasonId, 

        // InsertSeasonGame

        // UpdateSeasonGame

        // UpdateSeasonGameResults(

        // DeleteSeasonGame

        #endregion

        #region Season Rounds

        Task<List<SeasonRound>> GetSeasonRoundsAsync(long seasonId, CancellationToken cancellationToken);
        Task<SeasonRound?> GetSeasonRoundAsync(long seasonId, int roundNumber, CancellationToken cancellationToken);
        Task InsertSeasonRoundAsync(SeasonRound seasonRound, CancellationToken cancellationToken);
        Task UpdateSeasonRoundAsync(SeasonRound seasonRound, CancellationToken cancellationToken);
        Task DeleteSeasonRoundAsync(long seasonId, int roundNumber, CancellationToken cancellationToken);

        #endregion
    }
}
