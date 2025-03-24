using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus
{
    [DebuggerDisplay("({SlotIdx},{MemberIdx}) {PlayerName}")]
    public class MemberStats
    {
        [JsonPropertyName("slot_idx")]
        public required short SlotIdx { get; init; }

        [JsonPropertyName("member_idx")]
        public required short MemberIdx { get; init; }

        [JsonPropertyName("player")]
        public required string PlayerName { get; init; }

        [JsonPropertyName("squad")]
        public required string? SquadName { get; init; }

        [JsonPropertyName("premade_group")]
        public required short? PremadeGroup { get; init; }

        [JsonPropertyName("play_duration")]
        public required TimeSpan PlayDuration { get; init; }

        [JsonPropertyName("ship_mask")]
        public required uint ShipMask { get; init; }

        [JsonPropertyName("lag_outs")]
        public required short LagOuts { get; init; }

        [JsonPropertyName("kills")]
        public required short Kills { get; init; }

        [JsonPropertyName("deaths")]
        public required short Deaths { get; init; }

        [JsonPropertyName("knockouts")]
        public required short Knockouts { get; init; }

        [JsonPropertyName("team_kills")]
        public required short TeamKills { get; init; }

        [JsonPropertyName("solo_kills")]
        public required short SoloKills { get; init; }

        [JsonPropertyName("assists")]
        public required short Assists { get; init; }

        [JsonPropertyName("forced_reps")]
        public required short ForcedReps { get; init; }

        [JsonPropertyName("gun_damage_dealt")]
        public required int GunDamageDealt { get; init; }

        [JsonPropertyName("bomb_damage_dealt")]
        public required int BombDamageDealt { get; init; }

        [JsonPropertyName("team_damage_dealt")]
        public required int TeamDamageDealt { get; init; }

        [JsonPropertyName("gun_damage_taken")]
        public required int GunDamageTaken { get; init; }

        [JsonPropertyName("bomb_damage_taken")]
        public required int BombDamageTaken { get; init; }

        [JsonPropertyName("team_damage_taken")]
        public required int TeamDamageTaken { get; init; }

        [JsonPropertyName("self_damage")]
        public required int SelfDamage { get; init; }

        [JsonPropertyName("kill_damage")]
        public required int KillDamage { get; init; }

        [JsonPropertyName("team_kill_damage")]
        public required int TeamKillDamage { get; init; }

        [JsonPropertyName("forced_rep_damage")]
        public required int ForcedRepDamage { get; init; }

        [JsonPropertyName("bullet_fire_count")]
        public required int BulletFireCount { get; init; }

        [JsonPropertyName("bomb_fire_count")]
        public required short BombFireCount { get; init; }

        [JsonPropertyName("mine_fire_count")]
        public required int MineFireCount { get; init; }

        [JsonPropertyName("bullet_hit_count")]
        public required int BulletHitCount { get; init; }

        [JsonPropertyName("bomb_hit_count")]
        public required int BombHitCount { get; init; }

        [JsonPropertyName("mine_hit_count")]
        public required int MineHitCount { get; init; }

        [JsonPropertyName("first_out")]
        public required FirstOut FirstOut { get; init; }

        [JsonPropertyName("wasted_energy")]
        public required int WastedEnergy { get; init; }

        [JsonPropertyName("wasted_repel")]
        public required short WastedRepel { get; init; }

        [JsonPropertyName("wasted_rocket")]
        public required short WastedRocket { get; init; }

        [JsonPropertyName("wasted_thor")]
        public required short WastedThor { get; init; }

        [JsonPropertyName("wasted_burst")]
        public required short WastedBurst { get; init; }

        [JsonPropertyName("wasted_decoy")]
        public required short WastedDecoy { get; init; }

        [JsonPropertyName("wasted_portal")]
        public required short WastedPortal { get; init; }

        [JsonPropertyName("wasted_brick")]
        public required short WastedBrick { get; init; }

        [JsonPropertyName("rating_change")]
        public required int RatingChange { get; init; }

        [JsonPropertyName("enemy_distance_sum")]
        public required long? EnemyDistanceSum { get; init; }

        [JsonPropertyName("enemy_distance_samples")]
        public required int? EnemyDistanceSamples { get; init; }

        [JsonPropertyName("team_distance_sum")]
        public required long? TeamDistanceSum { get; init; }

        [JsonPropertyName("team_distance_samples")]
        public required int? TeamDistanceSamples { get; init; }
    }
}
