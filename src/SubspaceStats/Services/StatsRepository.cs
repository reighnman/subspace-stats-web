using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using SubspaceStats.Models;
using SubspaceStats.Models.GameDetails;
using SubspaceStats.Models.GameDetails.TeamVersus;
using SubspaceStats.Models.Leaderboard;
using SubspaceStats.Models.Player;
using SubspaceStats.Options;

namespace SubspaceStats.Services
{
	public class StatsRepository : IStatsRepository
    {
        private readonly ILogger<StatsRepository> _logger;
        private readonly NpgsqlDataSource _dataSource;

        public StatsRepository(IOptions<StatRepositoryOptions> options, ILogger<StatsRepository> logger)
        {
			NpgsqlDataSourceBuilder builder = new(options.Value.ConnectionString);
			builder.EnableDynamicJson();
			//builder.ConfigureJsonOptions() // TODO: maybe we can use the source generator with this?
			_dataSource = builder.Build();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<StatPeriod>> GetStatPeriods(GameType gameType, StatPeriodType statPeriodType, int limit, int offset, CancellationToken cancellationToken)
        {
            try
            {
                using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_stat_periods($1,$2,$3,$4);");
                command.Parameters.AddWithValue(NpgsqlDbType.Bigint, (long)gameType);
                command.Parameters.AddWithValue(NpgsqlDbType.Bigint, (long)statPeriodType);
                command.Parameters.AddWithValue(NpgsqlDbType.Integer, limit);
                command.Parameters.AddWithValue(NpgsqlDbType.Integer, offset);

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                int column_statPeriodId = dataReader.GetOrdinal("stat_period_id");
                int column_periodRange = dataReader.GetOrdinal("period_range");

                List<StatPeriod> periodList = new();

                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    periodList.Add(new StatPeriod()
                    {
                        StatPeriodId = dataReader.GetInt64(column_statPeriodId),
                        GameType = gameType,
                        StatPeriodType = statPeriodType,
                        PeriodRange = await dataReader.GetFieldValueAsync<NpgsqlRange<DateTime>>(column_periodRange, cancellationToken).ConfigureAwait(false),
                    });
                }

                return periodList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stat periods (gameType:{gameType}, statPeriodType:{statPeriodType}).", gameType, statPeriodType);
                throw;
            }
        }

        public async Task<StatPeriod?> GetForeverStatPeriod(GameType gameType, CancellationToken cancellationToken)
        {
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_stat_periods($1,$2,$3,$4);");
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, (long)gameType);
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, (long)StatPeriodType.Forever);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, 1);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, 0);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_statPeriodId = dataReader.GetOrdinal("stat_period_id");
				int column_periodRange = dataReader.GetOrdinal("period_range");

				if (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
                    return new StatPeriod()
                    {
                        StatPeriodId = dataReader.GetInt64(column_statPeriodId),
                        GameType = gameType,
                        StatPeriodType = StatPeriodType.Forever,
                        PeriodRange = await dataReader.GetFieldValueAsync<NpgsqlRange<DateTime>>(column_periodRange, cancellationToken).ConfigureAwait(false),
                    };
				}

				return null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting forever stat period (gameType:{gameType}).", gameType);
				throw;
			}
		}

        public async Task<List<TeamVersusLeaderboardStats>> GetTeamVersusLeaderboardAsync(long statPeriodId, int limit, int offset, CancellationToken cancellationToken)
        {
            try
            {
                using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_team_versus_leaderboard($1,$2,$3);");
                command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
                command.Parameters.AddWithValue(NpgsqlDbType.Integer, limit);
                command.Parameters.AddWithValue(NpgsqlDbType.Integer, offset);

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                int column_ratingRank = dataReader.GetOrdinal("rating_rank");
                int column_playerName = dataReader.GetOrdinal("player_name");
                int column_squadName = dataReader.GetOrdinal("squad_name");
                int column_rating = dataReader.GetOrdinal("rating");
                int column_gamesPlayed = dataReader.GetOrdinal("games_played");
                int column_playDuration = dataReader.GetOrdinal("play_duration");
                int column_wins = dataReader.GetOrdinal("wins");
                int column_losses = dataReader.GetOrdinal("losses");
                int column_kills = dataReader.GetOrdinal("kills");
                int column_deaths = dataReader.GetOrdinal("deaths");
                int column_damageDealt = dataReader.GetOrdinal("damage_dealt");
                int column_damageTaken = dataReader.GetOrdinal("damage_taken");
                int column_killDamage = dataReader.GetOrdinal("kill_damage");
                int column_forcedReps = dataReader.GetOrdinal("forced_reps");
                int column_forcedRepDamage = dataReader.GetOrdinal("forced_rep_damage");
                int column_assists = dataReader.GetOrdinal("assists");
                int column_wastedEnergy = dataReader.GetOrdinal("wasted_energy");
                int column_firstOut = dataReader.GetOrdinal("first_out");

                List <TeamVersusLeaderboardStats> statsList = new();

                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    statsList.Add(new TeamVersusLeaderboardStats()
                    {
                        RatingRank = dataReader.GetInt64(column_ratingRank),
                        PlayerName = dataReader.GetString(column_playerName),
                        SquadName = await dataReader.IsDBNullAsync(column_squadName, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetString(column_squadName),
                        Rating = dataReader.GetInt32(column_rating),
                        GamesPlayed = dataReader.GetInt64(column_gamesPlayed),
                        PlayDuration = dataReader.GetTimeSpan(column_playDuration),
                        Wins = dataReader.GetInt64(column_wins),
                        Losses = dataReader.GetInt64(column_losses),
                        Kills = dataReader.GetInt64(column_kills),
                        Deaths = dataReader.GetInt64(column_deaths),
                        DamageDealt = dataReader.GetInt64(column_damageDealt),
                        DamageTaken = dataReader.GetInt64(column_damageTaken),
                        KillDamage = dataReader.GetInt64(column_killDamage),
                        ForcedReps = dataReader.GetInt64(column_forcedReps),
                        ForcedRepDamage = dataReader.GetInt64(column_forcedRepDamage),
                        Assists = dataReader.GetInt64(column_assists),
                        WastedEnergy = dataReader.GetInt64(column_wastedEnergy),
                        FirstOut = dataReader.GetInt64(column_firstOut),
                    });
                }

                return statsList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team versus leaderboard (statPeriodId:{statPeriodId}, limit:{limit}, offset:{offset}).", statPeriodId, limit, offset);
                throw;
            }
        }

        public async Task<Game?> GetGameAsync(long gameId, CancellationToken cancellationToken)
        {
            try
            {
                using NpgsqlCommand command = _dataSource.CreateCommand("select ss.get_game($1)");
                command.Parameters.AddWithValue(NpgsqlDbType.Bigint, gameId);

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                if (!await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    throw new Exception("Expected a row.");

                if (await dataReader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false))
                    return null;

                return await dataReader.GetFieldValueAsync<Game>(0, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game details (gameId:{gameId}).", gameId);
                throw;
            }
        }

        public async Task<(PlayerInfo, List<StatPeriod>)?> GetPlayerInfoAndStatPeriods(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken)
        {
            try
            {
                NpgsqlBatch batch = _dataSource.CreateBatch();

                batch.BatchCommands.Add(
                    new NpgsqlBatchCommand("select * from ss.get_player_info($1);")
                    {
                        Parameters =
                        {
                            new NpgsqlParameter() { Value = playerName }
                        }
                    });

                batch.BatchCommands.Add(
                    new NpgsqlBatchCommand("select * from ss.get_player_stat_periods($1, $2);")
                    {
                        Parameters =
                        {
                            new NpgsqlParameter() { Value = playerName },
                            new NpgsqlParameter<TimeSpan> { TypedValue = periodCutoff },
                        }
                    });

                using var dataReader = await batch.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                int column_squadName = dataReader.GetOrdinal("squad_name");
                int column_xRes = dataReader.GetOrdinal("x_res");
                int column_yRes = dataReader.GetOrdinal("y_res");

                if (!await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    // Player not found.
                    return null;
                }

                PlayerInfo playerInfo = new()
                {
                    SquadName = await dataReader.IsDBNullAsync(column_squadName, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetString(column_squadName),
                    XRes = await dataReader.IsDBNullAsync(column_xRes, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetInt16(column_xRes),
                    YRes = await dataReader.IsDBNullAsync(column_yRes, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetInt16(column_yRes),
                };

                if (!await dataReader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new Exception("Missing get_player_game_types result.");
                }

                int column_statPeriodId = dataReader.GetOrdinal("stat_period_id");
                int column_gameTypeId = dataReader.GetOrdinal("game_type_id");
                int column_statPeriodTypeId = dataReader.GetOrdinal("stat_period_type_id");
                int column_periodRange = dataReader.GetOrdinal("period_range");

                List<StatPeriod> periodList = new();
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    periodList.Add(
                        new StatPeriod()
                        {
                            StatPeriodId = dataReader.GetInt64(column_statPeriodId),
                            GameType = (GameType)dataReader.GetInt64(column_gameTypeId),
                            StatPeriodType = (StatPeriodType)dataReader.GetInt64(column_statPeriodTypeId),
                            PeriodRange = await dataReader.GetFieldValueAsync<NpgsqlRange<DateTime>>(column_periodRange, cancellationToken).ConfigureAwait(false),
                        });
                }

                return (playerInfo, periodList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player info and stat periods (playerName:{playerName}).", playerName);
                throw;
            }
        }

		public async Task<List<ParticipationRecord>> GetPlayerParticipationOverview(string playerName, TimeSpan periodCutoff, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_player_participation_overview($1,$2);");
				command.Parameters.AddWithValue(NpgsqlDbType.Varchar, playerName);
				command.Parameters.AddWithValue(NpgsqlDbType.Interval, periodCutoff);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_statPeriodId = dataReader.GetOrdinal("stat_period_id");
				int column_gameTypeId = dataReader.GetOrdinal("game_type_id");
				int column_statPeriodTypeId = dataReader.GetOrdinal("stat_period_type_id");
				int column_periodRange = dataReader.GetOrdinal("period_range");
				int column_rating = dataReader.GetOrdinal("rating");
				// TODO: int column_detailsJson = dataReader.GetOrdinal("details_json");

				List<ParticipationRecord> particiatpionRecordList = new();
				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					particiatpionRecordList.Add(
						new ParticipationRecord()
						{
							LastStatPeriod = new StatPeriod()
							{
								StatPeriodId = dataReader.GetInt64(column_statPeriodId),
								GameType = (GameType)dataReader.GetInt64(column_gameTypeId),
								StatPeriodType = (StatPeriodType)dataReader.GetInt64(column_statPeriodTypeId),
								PeriodRange = await dataReader.GetFieldValueAsync<NpgsqlRange<DateTime>>(column_periodRange, cancellationToken).ConfigureAwait(false),
							},
							Rating = await dataReader.IsDBNullAsync(column_rating, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetInt32(column_rating),
							// TODO: json details
						});
				}
				return particiatpionRecordList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting player participation overview (playerName:{playerName}, periodCutoff:{periodCutoff}).", playerName, periodCutoff);
				throw;
			}
		}

		public async Task<(StatPeriod, List<TopRatingRecord>)?> GetTopPlayersByRating(GameType gameType, StatPeriodType statPeriodType, int top, CancellationToken cancellationToken)
		{
            List<StatPeriod> statPeriodList = await GetStatPeriods(gameType, statPeriodType, 1, 0, cancellationToken);
            if (statPeriodList.Count == 0)
                return null;

			return (statPeriodList[0], await GetTopPlayersByRating(statPeriodList[0].StatPeriodId, top, cancellationToken));
		}

		public async Task<List<TopRatingRecord>> GetTopPlayersByRating(long statPeriodId, int top, CancellationToken cancellationToken)
        {
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_top_players_by_rating($1,$2);");
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, top);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_topRank = dataReader.GetOrdinal("top_rank");
				int column_playerName = dataReader.GetOrdinal("player_name");
                int column_rating = dataReader.GetOrdinal("rating");

				List<TopRatingRecord> topList = new(top);

				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					topList.Add(
                        new TopRatingRecord(
                            dataReader.GetInt32(column_topRank),
                            dataReader.GetString(column_playerName),
                            dataReader.GetInt32(column_rating)));
				}

				return topList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting top players by rating (statPeriodId:{statPeriodId}, top:{top}).", statPeriodId, top);
				throw;
			}
		}

		public async Task<List<TopAvgRatingRecord>> GetTopTeamVersusPlayersByAvgRating(long statPeriodId, int top, int minGamesPlayed, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_top_versus_players_by_avg_rating($1,$2,$3);");
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, top);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, minGamesPlayed);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_topRank = dataReader.GetOrdinal("top_rank");
				int column_playerName = dataReader.GetOrdinal("player_name");
				int column_avgRating = dataReader.GetOrdinal("avg_rating");

				List<TopAvgRatingRecord> topList = new(top);

				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					topList.Add(
						new TopAvgRatingRecord(
							dataReader.GetInt32(column_topRank),
							dataReader.GetString(column_playerName),
							dataReader.GetFloat(column_avgRating)));
				}

				return topList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting top versus game players by avg rating (statPeriodId:{statPeriodId}, top:{top}).", statPeriodId, top);
				throw;
			}
		}

		public async Task<List<TopKillsPerMinuteRecord>> GetTopTeamVersusPlayersByKillsPerMinute(long statPeriodId, int top, int minGamesPlayed, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_top_versus_players_by_kills_per_minute($1,$2);");
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, top);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, minGamesPlayed);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_topRank = dataReader.GetOrdinal("top_rank");
				int column_playerName = dataReader.GetOrdinal("player_name");
				int column_killsPerMinute = dataReader.GetOrdinal("kills_per_minute");

				List<TopKillsPerMinuteRecord> topList = new(top);

				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					topList.Add(
						new TopKillsPerMinuteRecord(
							dataReader.GetInt32(column_topRank),
							dataReader.GetString(column_playerName),
							dataReader.GetFloat(column_killsPerMinute)));
				}

				return topList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting top versus game players by kills per minute (statPeriodId:{statPeriodId}, top:{top}).", statPeriodId, top);
				throw;
			}
		}

		public async Task<List<TeamVersusPeriodStats>> GetTeamVersusPeriodStats(string playerName, List<StatPeriod> statPeriodList, CancellationToken cancellationToken)
		{
			ArgumentException.ThrowIfNullOrEmpty(playerName);
			ArgumentNullException.ThrowIfNull(statPeriodList);

			if (statPeriodList.Count == 0)
				throw new ArgumentException("At least one period is required.", nameof(statPeriodList));

			List<long> statPeriodIdList = new(statPeriodList.Count);
			foreach (StatPeriod statPeriod in statPeriodList)
				statPeriodIdList.Add(statPeriod.StatPeriodId);

			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_player_versus_period_stats($1,$2);");
				command.Parameters.AddWithValue(NpgsqlDbType.Varchar, playerName);
				command.Parameters.AddWithValue(NpgsqlDbType.Array | NpgsqlDbType.Bigint, statPeriodIdList);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_statPeriodId = dataReader.GetOrdinal("stat_period_id");
				int column_periodRank = dataReader.GetOrdinal("period_rank");
				int column_rating = dataReader.GetOrdinal("rating");
				int column_gamesPlayed = dataReader.GetOrdinal("games_played");
				int column_playDuration = dataReader.GetOrdinal("play_duration");
				int column_wins = dataReader.GetOrdinal("wins");
				int column_losses = dataReader.GetOrdinal("losses");
				int column_lagOuts = dataReader.GetOrdinal("lag_outs");
				int column_kills = dataReader.GetOrdinal("kills");
				int column_deaths = dataReader.GetOrdinal("deaths");
				int column_knockouts = dataReader.GetOrdinal("knockouts");
				int column_teamKills = dataReader.GetOrdinal("team_kills");
				int column_soloKills = dataReader.GetOrdinal("solo_kills");
				int column_assists = dataReader.GetOrdinal("assists");
				int column_forcedReps = dataReader.GetOrdinal("forced_reps");
				int column_gunDamageDealt = dataReader.GetOrdinal("gun_damage_dealt");
				int column_bombDamageDealt = dataReader.GetOrdinal("bomb_damage_dealt");
				int column_teamDamageDealt = dataReader.GetOrdinal("team_damage_dealt");
				int column_gunDamageTaken = dataReader.GetOrdinal("gun_damage_taken");
				int column_bombDamageTaken = dataReader.GetOrdinal("bomb_damage_taken");
				int column_teamDamageTaken = dataReader.GetOrdinal("team_damage_taken");
				int column_selfDamage = dataReader.GetOrdinal("self_damage");
				int column_killDamage = dataReader.GetOrdinal("kill_damage");
				int column_teamKillDamage = dataReader.GetOrdinal("team_kill_damage");
				int column_forcedRepDamage = dataReader.GetOrdinal("forced_rep_damage");
				int column_bulletFireCount = dataReader.GetOrdinal("bullet_fire_count");
				int column_bombFireCount = dataReader.GetOrdinal("bomb_fire_count");
				int column_mineFireCount = dataReader.GetOrdinal("mine_fire_count");
				int column_bulletHitCount = dataReader.GetOrdinal("bullet_hit_count");
				int column_bombHitCount = dataReader.GetOrdinal("bomb_hit_count");
				int column_mineHitCount = dataReader.GetOrdinal("mine_hit_count");
				int column_firstOutRegular = dataReader.GetOrdinal("first_out_regular");
				int column_firstOutCritical = dataReader.GetOrdinal("first_out_critical");
				int column_wastedEnergy = dataReader.GetOrdinal("wasted_energy");
				int column_wastedRepel = dataReader.GetOrdinal("wasted_repel");
				int column_wastedRocket = dataReader.GetOrdinal("wasted_rocket");
				int column_wastedThor = dataReader.GetOrdinal("wasted_thor");
				int column_wastedBurst = dataReader.GetOrdinal("wasted_burst");
				int column_wastedDecoy = dataReader.GetOrdinal("wasted_decoy");
				int column_wastedPortal = dataReader.GetOrdinal("wasted_portal");
				int column_wastedBrick = dataReader.GetOrdinal("wasted_brick");

				List<TeamVersusPeriodStats> periodStatsList = new();

				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					long statPeriodId = dataReader.GetInt64(column_statPeriodId);
					StatPeriod? statPeriod = null;
					foreach (StatPeriod period in statPeriodList)
					{
						if (period.StatPeriodId == statPeriodId)
						{
							statPeriod = period;
							break;
						}
					}

					if (statPeriod is null)
						continue;

					periodStatsList.Add(
						new TeamVersusPeriodStats()
						{
							StatPeriod = statPeriod.Value,
							Rank = await dataReader.IsDBNullAsync(column_periodRank, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetInt32(column_periodRank),
							Rating = await dataReader.IsDBNullAsync(column_rating, cancellationToken).ConfigureAwait(false) ? null : dataReader.GetInt32(column_rating),
							Games = dataReader.GetInt64(column_gamesPlayed),
							Wins = dataReader.GetInt64(column_wins),
							Losses = dataReader.GetInt64(column_losses),
							FirstOutRegular = dataReader.GetInt64(column_firstOutRegular),
							FirstOutCritical = dataReader.GetInt64(column_firstOutCritical),
							PlayDuration = dataReader.GetTimeSpan(column_playDuration),
							LagOuts = dataReader.GetInt64(column_lagOuts),
							Kills = dataReader.GetInt64(column_kills),
							Deaths = dataReader.GetInt64(column_deaths),
							Knockouts = dataReader.GetInt64(column_knockouts),
							TeamKills = dataReader.GetInt64(column_teamKills),
							SoloKills = dataReader.GetInt64(column_soloKills),
							Assists = dataReader.GetInt64(column_assists),
							ForcedReps = dataReader.GetInt64(column_forcedReps),
							GunDamageDealt = dataReader.GetInt64(column_gunDamageDealt),
							BombDamageDealt = dataReader.GetInt64(column_bombDamageDealt),
							TeamDamageDealt = dataReader.GetInt64(column_teamDamageDealt),
							GunDamageTaken = dataReader.GetInt64(column_gunDamageTaken),
							BombDamageTaken = dataReader.GetInt64(column_bombDamageTaken),
							TeamDamageTaken = dataReader.GetInt64(column_teamDamageTaken),
							SelfDamage = dataReader.GetInt64(column_selfDamage),
							KillDamage = dataReader.GetInt64(column_killDamage),
							TeamKillDamage = dataReader.GetInt64(column_teamKillDamage),
							ForcedRepDamage = dataReader.GetInt64(column_forcedRepDamage),
							BulletFireCount = dataReader.GetInt64(column_bulletFireCount),
							BombFireCount = dataReader.GetInt64(column_bombFireCount),
							MineFireCount = dataReader.GetInt64(column_mineFireCount),
							BulletHitCount = dataReader.GetInt64(column_bulletHitCount),
							BombHitCount = dataReader.GetInt64(column_bombHitCount),
							MineHitCount = dataReader.GetInt64(column_mineHitCount),
							WastedEnergy = dataReader.GetInt64(column_wastedEnergy),
							WastedRepel = dataReader.GetInt64(column_wastedRepel),
							WastedRocket = dataReader.GetInt64(column_wastedRocket),
							WastedThor = dataReader.GetInt64(column_wastedThor),
							WastedBurst = dataReader.GetInt64(column_wastedBurst),
							WastedDecoy = dataReader.GetInt64(column_wastedDecoy),
							WastedPortal = dataReader.GetInt64(column_wastedPortal),
							WastedBrick = dataReader.GetInt64(column_wastedBrick),
						});
				}

				return periodStatsList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting team versus period stats (playerName:{playerName}, statPeriodIds:{statPeriodIds}).", playerName, string.Join(',', statPeriodIdList));
				throw;
			}
		}

		public async Task<List<TeamVersusGameStats>> GetTeamVersusGameStats(string playerName, long statPeriodId, int limit, int offset, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_player_versus_game_stats($1,$2,$3,$4);");
				command.Parameters.AddWithValue(NpgsqlDbType.Varchar, playerName);
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, limit);
				command.Parameters.AddWithValue(NpgsqlDbType.Integer, offset);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_gameId = dataReader.GetOrdinal("game_id");
				int column_timePlayed = dataReader.GetOrdinal("time_played");
				int column_score = dataReader.GetOrdinal("score");
				int column_result = dataReader.GetOrdinal("result");
				int column_playDuration = dataReader.GetOrdinal("play_duration");
				int column_shipMask = dataReader.GetOrdinal("ship_mask");
				int column_lagOuts = dataReader.GetOrdinal("lag_outs");
				int column_kills = dataReader.GetOrdinal("kills");
				int column_deaths = dataReader.GetOrdinal("deaths");
				int column_knockouts = dataReader.GetOrdinal("knockouts");
				int column_teamKills = dataReader.GetOrdinal("team_kills");
				int column_soloKills = dataReader.GetOrdinal("solo_kills");
				int column_assists = dataReader.GetOrdinal("assists");
				int column_forcedReps = dataReader.GetOrdinal("forced_reps");
				int column_gunDamageDealt = dataReader.GetOrdinal("gun_damage_dealt");
				int column_bombDamageDealt = dataReader.GetOrdinal("bomb_damage_dealt");
				int column_teamDamageDealt = dataReader.GetOrdinal("team_damage_dealt");
				int column_gunDamageTaken = dataReader.GetOrdinal("gun_damage_taken");
				int column_bombDamageTaken = dataReader.GetOrdinal("bomb_damage_taken");
				int column_teamDamageTaken = dataReader.GetOrdinal("team_damage_taken");
				int column_selfDamage = dataReader.GetOrdinal("self_damage");
				int column_killDamage = dataReader.GetOrdinal("kill_damage");
				int column_teamKillDamage = dataReader.GetOrdinal("team_kill_damage");
				int column_forcedRepDamage = dataReader.GetOrdinal("forced_rep_damage");
				int column_bulletFireCount = dataReader.GetOrdinal("bullet_fire_count");
				int column_bombFireCount = dataReader.GetOrdinal("bomb_fire_count");
				int column_mineFireCount = dataReader.GetOrdinal("mine_fire_count");
				int column_bulletHitCount = dataReader.GetOrdinal("bullet_hit_count");
				int column_bombHitCount = dataReader.GetOrdinal("bomb_hit_count");
				int column_mineHitCount = dataReader.GetOrdinal("mine_hit_count");
				int column_firstOut = dataReader.GetOrdinal("first_out");
				int column_wastedEnergy = dataReader.GetOrdinal("wasted_energy");
				int column_wastedRepel = dataReader.GetOrdinal("wasted_repel");
				int column_wastedRocket = dataReader.GetOrdinal("wasted_rocket");
				int column_wastedThor = dataReader.GetOrdinal("wasted_thor");
				int column_wastedBurst = dataReader.GetOrdinal("wasted_burst");
				int column_wastedDecoy = dataReader.GetOrdinal("wasted_decoy");
				int column_wastedPortal = dataReader.GetOrdinal("wasted_portal");
				int column_wastedBrick = dataReader.GetOrdinal("wasted_brick");
				int column_ratingChange = dataReader.GetOrdinal("rating_change");

				List<TeamVersusGameStats> statsList = new(limit);

				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					statsList.Add(new TeamVersusGameStats()
					{
						GameId = dataReader.GetInt64(column_gameId),
						TimePlayed = await dataReader.GetFieldValueAsync<NpgsqlRange<DateTime>>(column_timePlayed, cancellationToken).ConfigureAwait(false),
						Score = await dataReader.GetFieldValueAsync<int[]>(column_score, cancellationToken).ConfigureAwait(false),
						Result = (GameResult)dataReader.GetInt32(column_result),
						PlayDuration = dataReader.GetTimeSpan(column_playDuration),
						ShipMask = (ShipMask)dataReader.GetInt16(column_shipMask),
						LagOuts = dataReader.GetInt16(column_lagOuts),
						Kills = dataReader.GetInt16(column_kills),
						Deaths = dataReader.GetInt16(column_deaths),
						Knockouts = dataReader.GetInt16(column_knockouts),
						TeamKills = dataReader.GetInt16(column_teamKills),
						SoloKills = dataReader.GetInt16(column_soloKills),
						Assists = dataReader.GetInt16(column_assists),
						ForcedReps = dataReader.GetInt16(column_forcedReps),
						GunDamageDealt = dataReader.GetInt32(column_gunDamageDealt),
						BombDamageDealt = dataReader.GetInt32(column_bombDamageDealt),
						TeamDamageDealt = dataReader.GetInt32(column_teamDamageDealt),
						GunDamageTaken = dataReader.GetInt32(column_gunDamageTaken),
						BombDamageTaken = dataReader.GetInt32(column_bombDamageTaken),
						TeamDamageTaken = dataReader.GetInt32(column_teamDamageTaken),
						SelfDamage = dataReader.GetInt32(column_selfDamage),
						KillDamage = dataReader.GetInt32(column_killDamage),
						TeamKillDamage = dataReader.GetInt32(column_teamKillDamage),
						ForcedRepDamage = dataReader.GetInt32(column_forcedRepDamage),
						BulletFireCount = dataReader.GetInt32(column_bulletFireCount),
						BombFireCount = dataReader.GetInt32(column_bombFireCount),
						MineFireCount = dataReader.GetInt32(column_mineFireCount),
						BulletHitCount = dataReader.GetInt32(column_bulletHitCount),
						BombHitCount = dataReader.GetInt32(column_bombHitCount),
						MineHitCount = dataReader.GetInt32(column_mineHitCount),
						FirstOut = (FirstOut)dataReader.GetInt16(column_firstOut),
						WastedEnergy = dataReader.GetInt32(column_wastedEnergy),
						WastedRepel = dataReader.GetInt16(column_wastedRepel),
						WastedRocket = dataReader.GetInt16(column_wastedRocket),
						WastedThor = dataReader.GetInt16(column_wastedThor),
						WastedBurst = dataReader.GetInt16(column_wastedBurst),
						WastedDecoy = dataReader.GetInt16(column_wastedDecoy),
						WastedPortal = dataReader.GetInt16(column_wastedPortal),
						WastedBrick = dataReader.GetInt16(column_wastedBrick),
						RatingChange = dataReader.GetInt32(column_ratingChange),
					});
				}

				return statsList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting team versus game stats (playerName:{playerName} statPeriodId:{statPeriodId}, limit:{limit}, offset:{offset}).", playerName, statPeriodId, limit, offset);
				throw;
			}
		}

		public async Task<List<TeamVersusShipStats>> GetTeamVersusShipStats(string playerName, long statPeriodId, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_player_versus_ship_stats($1,$2);");
				command.Parameters.AddWithValue(NpgsqlDbType.Varchar, playerName);
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_shipType = dataReader.GetOrdinal("ship_type");
				int column_gameUseCount = dataReader.GetOrdinal("game_use_count");
				int column_useDuration = dataReader.GetOrdinal("use_duration");
				int column_kills = dataReader.GetOrdinal("kills");
				int column_deaths = dataReader.GetOrdinal("deaths");
				int column_knockouts = dataReader.GetOrdinal("knockouts");
				int column_soloKills = dataReader.GetOrdinal("solo_kills");

				List<TeamVersusShipStats> shipStatsList = new();
				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					shipStatsList.Add(
						new TeamVersusShipStats()
						{
                            ShipType = (ShipType)dataReader.GetInt16(column_shipType),
                            GameUseCount = dataReader.GetInt32(column_gameUseCount),
                            UseDuration = dataReader.GetTimeSpan(column_useDuration),
                            Kills = dataReader.GetInt64(column_kills),
                            Deaths = dataReader.GetInt64(column_deaths),
                            Knockouts = dataReader.GetInt64(column_knockouts),
                            SoloKills = dataReader.GetInt64(column_soloKills),
						});
				}
				return shipStatsList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting team versus ship stats (playerName:{playerName}, statPeriodId:{statPeriodId}).", playerName, statPeriodId);
				throw;
			}
		}

		public async Task<List<KillStats>> GetTeamVersusKillStats(string playerName, long statPeriodId, int limit, CancellationToken cancellationToken)
		{
			try
			{
				using NpgsqlCommand command = _dataSource.CreateCommand("select * from ss.get_player_versus_kill_stats($1,$2,$3);");
				command.Parameters.AddWithValue(NpgsqlDbType.Varchar, playerName);
				command.Parameters.AddWithValue(NpgsqlDbType.Bigint, statPeriodId);
                command.Parameters.AddWithValue(NpgsqlDbType.Integer, limit);

				using var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

				int column_playerName = dataReader.GetOrdinal("player_name");
				int column_kills = dataReader.GetOrdinal("kills");
				int column_deaths = dataReader.GetOrdinal("deaths");

				List<KillStats> killStatsList = new();
				while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					killStatsList.Add(
						new KillStats()
						{
							PlayerName = dataReader.GetString(column_playerName),
							Kills = dataReader.GetInt64(column_kills),
							Deaths = dataReader.GetInt64(column_deaths),
						});
				}
				return killStatsList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting team versus kill stats (playerName:{playerName}, statPeriodId:{statPeriodId}).", playerName, statPeriodId);
				throw;
			}
		}
	}
}
