using Npgsql;
using NpgsqlTypes;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Team;
using System.Data;
using System.Text.Json;

namespace SubspaceStats.Services
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StatsRepository> _logger;
        private readonly NpgsqlDataSource _dataSource;

        public LeagueRepository(
            IConfiguration configuration,
            ILogger<StatsRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataSource = NpgsqlDataSource.Create(
                _configuration.GetConnectionString("SubspaceStats") ?? throw new Exception("Missing 'SubspaceStats' connection string."));
        }

        public async Task<List<SeasonStandings>?> GetLatestSeasonsStandingsAsync(long[] leagueIds, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.get_latest_seasons_with_standings($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.AddWithValue(leagueIds);
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                throw new Exception("Expected a row.");

                            if (await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false))
                                return [];

                            Stream stream = await reader.GetStreamAsync(0, cancellationToken).ConfigureAwait(false);
                            await using (stream.ConfigureAwait(false))
                            {
                                return await JsonSerializer.DeserializeAsync(stream, SeasonStandingsSourceGenerationContext.Default.ListSeasonStandings, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest seasons. (league_ids: {league_ids})", leagueIds);
                throw;
            }
        }

        public async Task<List<SeasonRoster>?> GetSeasonRostersAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.get_season_rosters($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                throw new Exception("Expected a row.");

                            if (await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false))
                                return [];

                            Stream stream = await reader.GetStreamAsync(0, cancellationToken).ConfigureAwait(false);
                            await using (stream.ConfigureAwait(false))
                            {
                                return await JsonSerializer.DeserializeAsync(stream, SeasonRosterSourceGenerationContext.Default.ListSeasonRoster, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rosters for season. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<ScheduledGame>> GetScheduledGamesAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_scheduled_games($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_leagueId = reader.GetOrdinal("league_id");
                            int column_leagueName = reader.GetOrdinal("league_name");
                            int column_seasonId = reader.GetOrdinal("season_id");
                            int column_seasonName = reader.GetOrdinal("season_name");
                            int column_seasonGameId = reader.GetOrdinal("season_game_id");
                            int column_roundNumber = reader.GetOrdinal("round_number");
                            int column_roundName = reader.GetOrdinal("round_name");
                            int column_scheduledTimestamp = reader.GetOrdinal("scheduled_timestamp");
                            int column_teams = reader.GetOrdinal("teams");
                            int column_gameStatusId = reader.GetOrdinal("game_status_id");

                            List<ScheduledGame> gameList = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                gameList.Add(
                                    new ScheduledGame()
                                    {
                                        LeagueId = reader.GetInt64(column_leagueId),
                                        LeagueName = reader.GetString(column_leagueName),
                                        SeasonId = reader.GetInt64(column_seasonId),
                                        SeasonName = reader.GetString(column_seasonName),
                                        SeasonGameId = reader.GetInt64(column_seasonGameId),
                                        RoundNumber = reader.IsDBNull(column_roundNumber) ? null : reader.GetInt32(column_roundNumber),
                                        RoundName = reader.IsDBNull(column_roundName) ? null : reader.GetString(column_roundName),
                                        ScheduledTimestamp = reader.IsDBNull(column_scheduledTimestamp) ? null : reader.GetDateTime(column_scheduledTimestamp),
                                        Teams = reader.GetString(column_teams),
                                        Status = (GameStatus)reader.GetInt64(column_gameStatusId),
                                    });
                            }

                            return gameList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled games for season. (season_id: {season_id})", seasonId);
                throw;
            }
        }


        public async Task<List<TeamStanding>> GetSeasonStandingsAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_standings($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_teamId = reader.GetOrdinal("team_id");
                            int column_teamName = reader.GetOrdinal("team_name");
                            int column_wins = reader.GetOrdinal("wins");
                            int column_losses = reader.GetOrdinal("losses");
                            int column_draws = reader.GetOrdinal("draws");

                            List<TeamStanding> standingList = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                standingList.Add(
                                    new TeamStanding()
                                    {
                                        TeamId = reader.GetInt64(column_teamId),
                                        TeamName = reader.GetString(column_teamName),
                                        Wins = reader.GetInt32(column_wins),
                                        Losses = reader.GetInt32(column_losses),
                                        Draws = reader.GetInt32(column_draws),
                                    });
                            }

                            return standingList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting standings for season. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<LeagueWithSeasons>> GetLeaguesWithSeasonsAsync(CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.get_leagues_with_seasons()", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess,cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                throw new Exception("Expected a row.");

                            if (await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false))
                                return [];

                            Stream stream = await reader.GetStreamAsync(0, cancellationToken).ConfigureAwait(false);
                            await using (stream.ConfigureAwait(false))
                            {
                                return await JsonSerializer.DeserializeAsync(stream, LeagueWithSeasonsSourceGenerationContext.Default.ListLeagueWithSeasons, cancellationToken).ConfigureAwait(false) ?? [];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leagues with seasons.");
                throw;
            }
        }

        public async Task<List<FranchiseListItem>> GetFranchiseListAsync(CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_franchises_with_teams();", connection);
                    await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                    await using (command.ConfigureAwait(false))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_franchiseId = reader.GetOrdinal("franchise_id");
                            int column_franchiseName = reader.GetOrdinal("franchise_name");
                            int column_teams = reader.GetOrdinal("teams");

                            List<FranchiseListItem> itemList = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                itemList.Add(new FranchiseListItem()
                                {
                                    Id = reader.GetInt64(column_franchiseId),
                                    Name = reader.GetString(column_franchiseName),
                                    Teams = reader.IsDBNull(column_teams) ? null : reader.GetString(column_teams),
                                });
                            }

                            return itemList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting franchise list.");
                throw;
            }
        }

        public async Task<Franchise?> GetFranchiseAsync(long franchiseId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_franchise($1);", connection);
                    command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = franchiseId });
                    await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                    await using (command.ConfigureAwait(false))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                return null;

                            return new Franchise
                            {
                                Id = franchiseId,
                                Name = reader.GetString("franchise_name"),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting franchise. (franchise_id: {franchise_id})", franchiseId);
                throw;
            }
        }

        public async Task<List<TeamAndSeason>> GetFranchiseTeamsAsync(long franchiseId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_franchise_teams($1);", connection);
                    command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = franchiseId });
                    await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                    await using (command.ConfigureAwait(false))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_teamId = reader.GetOrdinal("team_id");
                            int column_teamName = reader.GetOrdinal("team_name");
                            int column_seasonId = reader.GetOrdinal("season_id");
                            int column_seasonName = reader.GetOrdinal("season_name");
                            int column_leagueId = reader.GetOrdinal("league_id");
                            int column_leagueName = reader.GetOrdinal("league_name");

                            List<TeamAndSeason> itemList = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                itemList.Add(new TeamAndSeason()
                                {
                                    TeamId = reader.GetInt64(column_teamId),
                                    TeamName = reader.GetString(column_teamName),
                                    SeasonId = reader.GetInt64(column_seasonId),
                                    SeasonName = reader.GetString(column_seasonName),
                                    LeagueId = reader.GetInt64(column_leagueId),
                                    LeagueName = reader.GetString(column_leagueName),
                                });
                            }

                            return itemList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting franchise teams. (franchise_id: {franchise_id})", franchiseId);
                throw;
            }
        }

        public async Task<long> InsertFranchiseAsync(string franchiseName, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.insert_franchise($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.AddWithValue(franchiseName);
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        object? franchiseIdObj = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        if (franchiseIdObj is null || franchiseIdObj == DBNull.Value)
                            throw new Exception("Expected an value.");

                        return (long)franchiseIdObj;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting franchise. (franchise_name:{franchise_name})", franchiseName);
                throw;
            }
        }

        public async Task UpdateFranchiseAsync(Franchise franchise, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.update_franchise($1,$2)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = franchise.Id });
                        command.Parameters.AddWithValue(franchise.Name);
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating franchise. (franchise_id: {franchise_id}, franchise_name: {franchise_name})", franchise.Id, franchise.Name);
                throw;
            }
        }

        public async Task DeleteFranchiseAsync(long franchiseId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.delete_franchise($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = franchiseId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting league. (franchise_id: {franchise_id})", franchiseId);
                throw;
            }
        }

        public async Task<List<LeagueModel>> GetLeagueListAsync(CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_leagues();", connection);
                    await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                    await using (command.ConfigureAwait(false))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_leagueId = reader.GetOrdinal("league_id");
                            int column_leagueName = reader.GetOrdinal("league_name");
                            int column_gameTypeId = reader.GetOrdinal("game_type_id");

                            List<LeagueModel> leagueList = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                leagueList.Add(new LeagueModel()
                                {
                                    Id = reader.GetInt64(column_leagueId),
                                    Name = reader.GetString(column_leagueName),
                                    GameTypeId = reader.GetInt64(column_gameTypeId),
                                });
                            }

                            return leagueList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting league list.");
                throw;
            }
        }

        public async Task<LeagueModel?> GetLeagueAsync(long leagueId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_league($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = leagueId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_leagueName = reader.GetOrdinal("league_name");
                            int column_gameTypeId = reader.GetOrdinal("game_type_id");

                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                return null;

                            return new LeagueModel()
                            {
                                Id = leagueId,
                                Name = reader.GetString(column_leagueName),
                                GameTypeId = reader.GetInt64(column_gameTypeId),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting league info. (league_id: {league_id})", leagueId);
                throw;
            }
        }

        public async Task<long> InsertLeagueAsync(string name, long gameTypeId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.insert_league($1,$2)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.AddWithValue(name);
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = gameTypeId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        object? leagueIdObj = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        if (leagueIdObj is null || leagueIdObj == DBNull.Value)
                            throw new Exception("Expected an value.");

                        return (long)leagueIdObj;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting league. (league_name:{league_name}, game_type_id:{game_type_id})", name, gameTypeId);
                throw;
            }
        }

        public async Task UpdateLeagueAsync(LeagueModel league, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.update_league($1,$2,$3)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = league.Id });
                        command.Parameters.AddWithValue(league.Name);
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = league.GameTypeId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating league. (league_id: {league_id}, league_name: {league_name}, game_type_id:{game_type_id})", league.Id, league.Name, league.GameTypeId);
                throw;
            }
        }

        public async Task DeleteLeagueAsync(long leagueId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.delete_league($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = leagueId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting league. (league_id: {league_id})", leagueId);
                throw;
            }
        }

        public async Task<List<Areas.League.Models.League.SeasonListItem>> GetSeasonsAsync(long leagueId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_seasons($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = leagueId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_seasonId = reader.GetOrdinal("season_id");
                            int column_seasonName = reader.GetOrdinal("season_name");
                            int column_createdTimestamp = reader.GetOrdinal("created_timestamp");

                            List<Areas.League.Models.League.SeasonListItem> seasonList = new(32);
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                seasonList.Add(
                                    new Areas.League.Models.League.SeasonListItem()
                                    {
                                        Id = reader.GetInt64(column_seasonId),
                                        Name = reader.GetString(column_seasonName),
                                        CreatedTimestamp = reader.GetDateTime(column_createdTimestamp),
                                    });
                            }

                            return seasonList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seasons for league. (league_id: {league_id})", leagueId);
                throw;
            }
        }

        public async Task<SeasonDetails?> GetSeasonDetailsAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_details($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_seasonName = reader.GetOrdinal("season_name");
                            int column_createdTimestamp = reader.GetOrdinal("created_timestamp");
                            int column_startDate = reader.GetOrdinal("start_date");
                            int column_endDate = reader.GetOrdinal("end_date");
                            int column_statPeriodId = reader.GetOrdinal("stat_period_id");
                            int column_statPeriodRange = reader.GetOrdinal("stat_period_range");
                            int column_statGameTypeId = reader.GetOrdinal("stat_game_type_id");
                            int column_leagueGameTypeId = reader.GetOrdinal("league_game_type_id");

                            
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                return null;

                            return new SeasonDetails
                            {
                                SeasonName = reader.GetString(column_seasonName),
                                CreatedTimestamp = reader.GetDateTime(column_createdTimestamp),
                                StartDate = reader.IsDBNull(column_startDate) ? null : reader.GetFieldValue<DateOnly>(column_startDate),
                                EndDate = reader.IsDBNull(column_endDate) ? null : reader.GetFieldValue<DateOnly>(column_endDate),
                                StatPeriodId = reader.IsDBNull(column_statPeriodId) ? null : reader.GetInt64(column_statPeriodId),
                                StatPeriodRange = reader.IsDBNull(column_statPeriodRange) ? null : reader.GetFieldValue<NpgsqlRange<DateTime>>(column_statPeriodRange),
                                StatGameTypeId = reader.IsDBNull(column_statGameTypeId) ? null : reader.GetInt64(column_statGameTypeId),
                                LeagueGameTypeId = reader.IsDBNull(column_leagueGameTypeId) ? null : reader.GetInt64(column_leagueGameTypeId),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season details. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task StartSeasonAsync(long seasonId, DateTime? startDate, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.start_season($1,$2);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });

                        if (startDate is not null)
                            command.Parameters.Add(new NpgsqlParameter<DateTime> { TypedValue = startDate.Value });
                        else
                            command.Parameters.AddWithValue(DBNull.Value);

                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting season. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<long> CopySeasonAsync(long seasonId, string seasonName, bool includePlayers, bool includeTeams, bool includeGames, bool includeRounds, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.copy_season($1,$2,$3,$4,$5,$6)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        command.Parameters.AddWithValue(seasonName);
                        command.Parameters.Add(new NpgsqlParameter<bool> { TypedValue = includePlayers });
                        command.Parameters.Add(new NpgsqlParameter<bool> { TypedValue = includeTeams });
                        command.Parameters.Add(new NpgsqlParameter<bool> { TypedValue = includeGames });
                        command.Parameters.Add(new NpgsqlParameter<bool> { TypedValue = includeRounds });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                throw new Exception("Expected a result");

                            return reader.GetInt64(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying season. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<TeamModel>> GetSeasonTeamsAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_teams($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_teamId = reader.GetOrdinal("team_id");
                            int column_teamName = reader.GetOrdinal("team_name");
                            int column_bannerSmall = reader.GetOrdinal("banner_small");
                            int column_bannerLarge = reader.GetOrdinal("banner_large");
                            int column_isEnabled = reader.GetOrdinal("is_enabled");
                            int column_franchiseId = reader.GetOrdinal("franchise_id");

                            List<TeamModel> teams = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                teams.Add(new TeamModel()
                                {
                                    Id = reader.GetInt64(column_teamId),
                                    Name = reader.GetString(column_teamName),
                                    BannerSmall = reader.IsDBNull(column_bannerSmall) ? null : reader.GetString(column_bannerSmall),
                                    BannerLarge = reader.IsDBNull(column_bannerLarge) ? null : reader.GetString(column_bannerLarge),
                                    IsEnabled = reader.GetBoolean(column_isEnabled),
                                    FranchiseId = reader.IsDBNull(column_franchiseId) ? null : reader.GetInt64(column_franchiseId),
                                });
                            }

                            return teams;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season teams. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<PlayerListItem>> GetSeasonPlayersAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_players($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_playerId = reader.GetOrdinal("player_id");
                            int column_playerName = reader.GetOrdinal("player_name");
                            int column_signupTimestamp = reader.GetOrdinal("signup_timestamp");
                            int column_teamId = reader.GetOrdinal("team_id");
                            int column_enrollTimestamp = reader.GetOrdinal("enroll_timestamp");
                            int column_isCaptain = reader.GetOrdinal("is_captain");
                            int column_isSuspended = reader.GetOrdinal("is_suspended");

                            List<PlayerListItem> teams = [];

                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                teams.Add(new PlayerListItem()
                                {
                                    Id = reader.GetInt64(column_playerId),
                                    Name = reader.GetString(column_playerName),
                                    SignupTimestamp = reader.IsDBNull(column_signupTimestamp) ? null : reader.GetDateTime(column_signupTimestamp),
                                    TeamId = reader.IsDBNull(column_teamId) ? null : reader.GetInt64(column_teamId),
                                    EnrollTimestamp = reader.IsDBNull(column_enrollTimestamp) ? null : reader.GetDateTime(column_enrollTimestamp),
                                    IsCaptain = reader.GetBoolean(column_isCaptain),
                                    IsSuspended = reader.GetBoolean(column_isSuspended),
                                });
                            }

                            return teams;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season players. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<TeamGameRecord>> GetTeamGames(long teamId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_team_games($1)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = teamId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_seasonGameId = reader.GetOrdinal("season_game_id");
                            int column_roundNumber = reader.GetOrdinal("round_number");
                            int column_roundName = reader.GetOrdinal("round_name");
                            int column_gameTimestamp = reader.GetOrdinal("game_timestamp");
                            int column_gameId = reader.GetOrdinal("game_id");
                            int column_teams = reader.GetOrdinal("teams");
                            int column_winLoseDraw = reader.GetOrdinal("win_lose_draw");
                            int column_scores = reader.GetOrdinal("scores");

                            List<TeamGameRecord> records = new(32);
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                long seasonGameId = reader.GetInt64(column_seasonGameId);
                                int? roundNumber = reader.IsDBNull(column_roundNumber) ? null : reader.GetInt32(column_roundNumber);
                                string? roundName = reader.IsDBNull(column_roundName) ? null : reader.GetString(column_roundName);
                                DateTime? gameTimestamp = reader.IsDBNull(column_gameTimestamp) ? null : reader.GetDateTime(column_gameTimestamp);
                                long? gameId = reader.IsDBNull(column_gameId) ? null : reader.GetInt64(column_gameId);
                                string teams = reader.GetString(column_teams);
                                char? winLoseDraw = reader.IsDBNull(column_winLoseDraw) ? null : reader.GetChar(column_winLoseDraw);
                                GameResult? result = winLoseDraw is null
                                    ? null
                                    : winLoseDraw.Value switch
                                    {
                                        'W' => GameResult.Win,
                                        'L' => GameResult.Loss,
                                        'D' => GameResult.Draw,
                                        _ => null
                                    };

                                string? scores = reader.IsDBNull(column_scores) ? null : reader.GetString(column_scores);

                                records.Add(
                                    new TeamGameRecord()
                                    {
                                        SeasonGameId = seasonGameId,
                                        RoundNumber = roundNumber,
                                        RoundName = roundName,
                                        GameTimestamp = gameTimestamp,
                                        GameId = gameId,
                                        Teams = teams,
                                        Result = result,
                                        Scores = scores,
                                    });
                            }

                            return records;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting games for team. (team_id: {team_id})", teamId);
                throw;
            }
        }

        public async Task<TeamWithSeasonInfo?> GetTeamsWithSeasonInfosync(long teamId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_team_with_season_info($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = teamId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_teamName = reader.GetOrdinal("team_name");
                            int column_bannerSmall = reader.GetOrdinal("banner_small");
                            int column_bannerLarge = reader.GetOrdinal("banner_large");
                            int column_isEnabled = reader.GetOrdinal("is_enabled");
                            int column_franchiseId = reader.GetOrdinal("franchise_id");
                            int column_franchiseName = reader.GetOrdinal("franchise_name");
                            int column_leagueId = reader.GetOrdinal("league_id");
                            int column_leagueName = reader.GetOrdinal("league_name");
                            int column_seasonId = reader.GetOrdinal("season_id");
                            int column_seasonName = reader.GetOrdinal("season_name");
                            int column_wins = reader.GetOrdinal("wins");
                            int column_losses = reader.GetOrdinal("losses");
                            int column_draws = reader.GetOrdinal("draws");

                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                return null;

                            return new TeamWithSeasonInfo
                            {
                                Id = teamId,
                                Name = reader.GetString("team_name"),
                                BannerSmall = reader.IsDBNull(column_bannerSmall) ? null : reader.GetString(column_bannerSmall),
                                BannerLarge = reader.IsDBNull(column_bannerLarge) ? null : reader.GetString(column_bannerLarge),
                                IsEnabled = reader.GetBoolean(column_isEnabled),
                                FranchiseId = reader.IsDBNull(column_franchiseId) ? null : reader.GetInt64(column_franchiseId),
                                FranchiseName = reader.IsDBNull(column_franchiseName) ? null : reader.GetString(column_franchiseName),
                                LeagueId = reader.GetInt64(column_leagueId),
                                LeagueName = reader.GetString(column_leagueName),
                                SeasonId = reader.GetInt64(column_seasonId),
                                SeasonName = reader.GetString(column_seasonName),
                                Wins = reader.IsDBNull(column_wins) ? null : reader.GetInt32(column_wins),
                                Losses = reader.IsDBNull(column_losses) ? null : reader.GetInt32(column_losses),
                                Draws = reader.IsDBNull(column_draws) ? null : reader.GetInt32(column_draws),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team. (team_id: {team_id})", teamId);
                throw;
            }
        }

        public async Task<TeamModel?> GetTeamAsync(long teamId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_team($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = teamId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_teamName = reader.GetOrdinal("team_name");
                            int column_bannerSmall = reader.GetOrdinal("banner_small");
                            int column_bannerLarge = reader.GetOrdinal("banner_large");
                            int column_isEnabled = reader.GetOrdinal("is_enabled");
                            int column_franchiseId = reader.GetOrdinal("franchise_id");

                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                return null;

                            return new TeamModel
                            {
                                Id = teamId,
                                Name = reader.GetString("team_name"),
                                BannerSmall = reader.IsDBNull(column_bannerSmall) ? null : reader.GetString(column_bannerSmall),
                                BannerLarge = reader.IsDBNull(column_bannerLarge) ? null : reader.GetString(column_bannerLarge),
                                IsEnabled = reader.GetBoolean(column_isEnabled),
                                FranchiseId = reader.IsDBNull(column_franchiseId) ? null : reader.GetInt64(column_franchiseId),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team. (team_id: {team_id})", teamId);
                throw;
            }
        }

        public async Task<List<GameListItem>> GetSeasonGamesAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_games($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_seasonGameId = reader.GetOrdinal("season_game_id");
                            int column_roundNumber = reader.GetOrdinal("round_number");
                            int column_scheduledTimestamp = reader.GetOrdinal("scheduled_timestamp");
                            int column_gameId = reader.GetOrdinal("game_id");
                            int column_gameStatusId = reader.GetOrdinal("game_status_id");
                            int column_teamIds = reader.GetOrdinal("team_ids");

                            List<GameListItem> list = [];
                            while(await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                list.Add(new GameListItem
                                {
                                    SeasonGameId = reader.GetInt64(column_seasonGameId),
                                    RoundNumber = reader.IsDBNull(column_roundNumber) ? null : reader.GetInt32(column_roundNumber),
                                    ScheduledTimestamp = reader.IsDBNull(column_scheduledTimestamp) ? null : reader.GetDateTime(column_scheduledTimestamp),
                                    GameId = reader.IsDBNull(column_gameId) ? null : reader.GetInt64(column_gameId),
                                    GameStatusId = reader.GetInt64(column_gameStatusId),
                                    TeamIds = (long[])reader.GetValue(column_teamIds),
                                });
                            }

                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season games. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<List<SeasonRound>> GetSeasonRoundsAsync(long seasonId, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_rounds($1);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_roundNumber = reader.GetOrdinal("round_number");
                            int column_roundName = reader.GetOrdinal("round_name");
                            int column_roundDescription = reader.GetOrdinal("round_description");

                            List<SeasonRound> list = [];
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                list.Add(new SeasonRound
                                {
                                    SeasonId = seasonId,
                                    RoundNumber = reader.GetInt32(column_roundNumber),
                                    RoundName = reader.GetString(column_roundName),
                                    RoundDescription = reader.IsDBNull(column_roundDescription) ? null : reader.GetString(column_roundDescription),
                                });
                            }

                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season rounds. (season_id: {season_id})", seasonId);
                throw;
            }
        }

        public async Task<SeasonRound?> GetSeasonRoundAsync(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select * from league.get_season_round($1,$2);", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        command.Parameters.Add(new NpgsqlParameter<int> { TypedValue = roundNumber });
                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        await using (reader.ConfigureAwait(false))
                        {
                            int column_roundName = reader.GetOrdinal("round_name");
                            int column_roundDescription = reader.GetOrdinal("round_description");

                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                return null;
                            }

                            return new SeasonRound
                            {
                                SeasonId = seasonId,
                                RoundNumber = roundNumber,
                                RoundName = reader.GetString(column_roundName),
                                RoundDescription = reader.IsDBNull(column_roundDescription) ? null : reader.GetString(column_roundDescription),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting season round. (season_id: {season_id}, round_number: {round_number})", seasonId, roundNumber);
                throw;
            }
        }

        public async Task InsertSeasonRoundAsync(SeasonRound seasonRound, CancellationToken cancellationToken)
        {
            if (seasonRound.RoundNumber is null)
                throw new ArgumentNullException(nameof(seasonRound), "Round number cannot be null.");

            if (seasonRound.RoundName is null)
                throw new ArgumentNullException(nameof(seasonRound), "Round name cannot be null.");

            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.insert_season_round($1,$2,$3,$4)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonRound.SeasonId });
                        command.Parameters.Add(new NpgsqlParameter<int> { TypedValue = seasonRound.RoundNumber.Value });
                        command.Parameters.AddWithValue(seasonRound.RoundName);

                        if (string.IsNullOrWhiteSpace(seasonRound.RoundDescription))
                            command.Parameters.AddWithValue(DBNull.Value);
                        else
                            command.Parameters.AddWithValue(seasonRound.RoundDescription);

                        await command.PrepareAsync(cancellationToken).ConfigureAwait(false);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inserting season round. (season_id: {season_id}, round_number: {round_number}, round_name: {round_name})",
                    seasonRound.SeasonId,
                    seasonRound.RoundNumber.Value,
                    seasonRound.RoundName);

                throw;
            }
        }

        public async Task UpdateSeasonRoundAsync(SeasonRound seasonRound, CancellationToken cancellationToken)
        {
            if (seasonRound.RoundNumber is null)
                throw new ArgumentNullException(nameof(seasonRound), "Round number cannot be null.");

            if (seasonRound.RoundName is null)
                throw new ArgumentNullException(nameof(seasonRound), "Round name cannot be null.");

            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.update_season_round($1,$2,$3,$4)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonRound.SeasonId });
                        command.Parameters.Add(new NpgsqlParameter<int> { TypedValue = seasonRound.RoundNumber.Value });
                        command.Parameters.AddWithValue(seasonRound.RoundName);

                        if (string.IsNullOrWhiteSpace(seasonRound.RoundDescription))
                            command.Parameters.AddWithValue(DBNull.Value);
                        else
                            command.Parameters.AddWithValue(seasonRound.RoundDescription);

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating season round. (season_id: {season_id}, round_number: {round_number}, round_name: {round_name})",
                    seasonRound.SeasonId,
                    seasonRound.RoundNumber.Value,
                    seasonRound.RoundName);

                throw;
            }
        }

        public async Task DeleteSeasonRoundAsync(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                await using (connection.ConfigureAwait(false))
                {
                    NpgsqlCommand command = new("select league.delete_season_round($1,$2)", connection);
                    await using (command.ConfigureAwait(false))
                    {
                        command.Parameters.Add(new NpgsqlParameter<long> { TypedValue = seasonId });
                        command.Parameters.Add(new NpgsqlParameter<int> { TypedValue = roundNumber });
                        
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting season round. (season_id: {season_id}, round_number: {round_number}",
                    seasonId,
                    roundNumber);

                throw;
            }
        }
    }
}
