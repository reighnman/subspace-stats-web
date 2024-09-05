using NpgsqlTypes;
using SubspaceStats.Models.GameDetails.TeamVersus;

namespace SubspaceStats.Models.Player
{
    public enum GameResult
    {
        Loss = -1,
        Draw = 0,
        Win = 1,
    }

    public class TeamVersusGameStats
    {
        public required long GameId { get; init; }
        public required NpgsqlRange<DateTime> TimePlayed { get; init; }
        public required int[] Score { get; init; }
        public required GameResult Result { get; init; }
        public required TimeSpan PlayDuration { get; init; }
        public required ShipMask ShipMask { get; init; }
        public required short LagOuts { get; init; }
        public required short Kills { get; init; }
        public required short Deaths { get; init; }
        public required short Knockouts { get; init; }
        public required short TeamKills { get; init; }
        public required short SoloKills { get; init; }
        public required short Assists { get; init; }
        public required short ForcedReps { get; init; }
        public required int GunDamageDealt { get; init; }
        public required int BombDamageDealt { get; init; }
        public required int TeamDamageDealt { get; init; }
        public required int GunDamageTaken { get; init; }
        public required int BombDamageTaken { get; init; }
        public required int TeamDamageTaken { get; init; }
        public required int SelfDamage { get; init; }
        public required int KillDamage { get; init; }
        public required int TeamKillDamage { get; init; }
        public required int ForcedRepDamage { get; init; }
        public required int BulletFireCount { get; init; }
        public required int BombFireCount { get; init; }
        public required int MineFireCount { get; init; }
        public required int BulletHitCount { get; init; }
        public required int BombHitCount { get; init; }
        public required int MineHitCount { get; init; }
        public required FirstOut FirstOut { get; init; }
        public required int WastedEnergy { get; init; }
        public required short WastedRepel { get; init; }
        public required short WastedRocket { get; init; }
        public required short WastedThor { get; init; }
        public required short WastedBurst { get; init; }
        public required short WastedDecoy { get; init; }
        public required short WastedPortal { get; init; }
        public required short WastedBrick { get; init; }
        public required int RatingChange { get; init; }
    }
}
