using SubspaceStats.Models.GameDetails.TeamVersus;
using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails
{
    //[JsonPolymorphic(TypeDiscriminatorPropertyName = "game_category_id")]
    //[JsonDerivedType(typeof(SoloGame), 1)]
    //[JsonDerivedType(typeof(TeamVersusGame), 2)]
    //[JsonDerivedType(typeof(PowerballGame), 3)]
    public class Game
    {
        [JsonPropertyName("game_id")]
        public required long Id { get; init; }

        [JsonPropertyName("game_type_id")]
        public required GameType GameType { get; init; }

        [JsonPropertyName("zone_server_name")]
        public required string ZoneServerName { get; init; }

        [JsonPropertyName("arena_name")]
        public required string ArenaName { get; init; }

        [JsonPropertyName("box_number")]
        public int? BoxNumber { get; init; }

        [JsonPropertyName("start_time")]
        public required DateTime StartTime { get; init; }

        [JsonPropertyName("end_time")]
        public required DateTime EndTime { get; init; }

        [JsonPropertyName("lvl_file_name")]
        public required string LvlFileName { get; init; }

        [JsonPropertyName("lvl_checksum")]
        public required uint LvlChecksum { get; init; }

        [JsonPropertyName("events")]
        public GameEvent[]? Events { get; init; }

        //[JsonPropertyName("solo_stats")]
        //public List<SoloPlayerStats>? SoloStats { get; init; }

        [JsonPropertyName("team_stats")]
        public List<TeamStats>? TeamStats { get; init; }

        //[JsonPropertyName("pb_stats")]
        //public List<PowerballTeamStats>? PowerballStats { get; init; }
    }

    //public class SoloGame : Game
    //{
    //    [JsonPropertyName("solo_stats")]
    //    public List<SoloPlayerStats> Stats { get; init; }
    //}

    //public class TeamVersusGame : Game
    //{
    //    [JsonPropertyName("team_stats")]
    //    public List<TeamStats> Stats { get; init; }
    //}

    //public class PowerballGame : Game
    //{
    //    [JsonPropertyName("pb_stats")]
    //    public List<PowerballTeamStats> Stats { get; init; }
    //}
}
