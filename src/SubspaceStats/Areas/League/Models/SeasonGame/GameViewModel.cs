using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Team;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models.SeasonGame
{
    /// <summary>
    /// league.season_game
    /// </summary>
    public class GameModel : IValidatableObject
    {
        [JsonPropertyName("season_game_id")]
        [Display(Name = "Match ID")]
        public long? SeasonGameId { get; set; }

        [JsonPropertyName("season_id")]
        [Display(Name = "Season ID")]
        public long? SeasonId { get; set; }

        [JsonPropertyName("round_number")]
        [Display(Name = "Round Number")]
        [Required]
        public required int RoundNumber { get; set; }

        [JsonPropertyName("game_timestamp")]
        [Display(Name = "Game Time")]
        [DataType(DataType.DateTime)]
        public DateTime? GameTimestamp { get; set; }

        [JsonPropertyName("game_id")]
        public long? GameId { get; set; }

        [JsonPropertyName("game_status_id")]
        [Display(Name = "Game Status")]
        [Required]
        public required GameStatus Status { get; set; }

        [JsonPropertyName("teams")]
        [Display(Name = "Teams")]
        [Required]
        public required List<GameTeamModel> Teams { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Teams.Count < 2)
            {
                yield return new ValidationResult("At least 2 teams must be included.");
            }
            else
            {
                for (int i = 1; i < Teams.Count; i++)
                {
                    GameTeamModel team = Teams[i];

                    for (int x = 0; x < i; x++)
                    {
                        GameTeamModel otherTeam = Teams[x];
                        if (otherTeam.TeamId == team.TeamId)
                        {
                            yield return new ValidationResult("Team is already used.", [$"{nameof(Teams)}[{i}].{nameof(GameTeamModel.TeamId)}"]);
                        }

                        if (otherTeam.Freq == team.Freq)
                        {
                            yield return new ValidationResult("Freq is already used.", [$"{nameof(Teams)}[{i}].{nameof(GameTeamModel.Freq)}"]);
                        }
                    }
                }
            }

            if (Status == GameStatus.Complete)
            {
                foreach (var team in Teams)
                {
                    if (team.Score is null)
                    {
                        yield return new ValidationResult("A completed game must have a score for every team.", [nameof(Teams)]);
                    }
                }

                // There doesn't need to be a winner. It could have been a draw.
                // Also, there could be more than one winner.
                // It's up to the game mode, as a generic league system, we do not enforce rules like that.
            }
        }
    }

    /// <summary>
    /// league.season_game_team
    /// </summary>
    public class GameTeamModel
    {
        [JsonPropertyName("team_id")]
        [Required]
        public long TeamId { get; set; }

        [JsonPropertyName("freq")]
        [Required]
        public short Freq { get; set; }

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("is_winner")]
        public bool IsWinner { get; set; }
    }

    [JsonSerializable(typeof(List<GameModel>), GenerationMode = JsonSourceGenerationMode.Metadata)]
    internal partial class GameModelSourceGenerationContext : JsonSerializerContext
    {
    }

    public class GameViewModel
    {
        public required GameModel Game { get; set; }

        public required SeasonModel Season { get; init; }

        public required LeagueModel League { get; init; }

        [Display(Name = "Automatically assign freqs")]
        public required bool AutoAssignFreqs { get; init; }

        public required OrderedDictionary<long, TeamModel> Teams { get; init; }

        public required OrderedDictionary<int, SeasonRound> Rounds { get; init; }
    }
}
