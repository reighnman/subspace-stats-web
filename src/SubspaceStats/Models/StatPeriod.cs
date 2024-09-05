using NpgsqlTypes;

namespace SubspaceStats.Models
{
    public struct StatPeriod : IEquatable<StatPeriod>
    {
        public long StatPeriodId { get; set; }
        public GameType GameType { get; set; }
        public StatPeriodType StatPeriodType { get; set; }
        public NpgsqlRange<DateTime> PeriodRange { get; set; }

        public readonly bool Equals(StatPeriod other)
        {
            return StatPeriodId == other.StatPeriodId;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is StatPeriod period && Equals(period);
        }

        public static bool operator ==(StatPeriod left, StatPeriod right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StatPeriod left, StatPeriod right)
        {
            return !(left == right);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(StatPeriodId);
        }
    }

    public static class StatPeriodExtensions
    {
        public static string GetDisplayDescription(this StatPeriod statPeriod)
        {
            return statPeriod.StatPeriodType switch
            {
                StatPeriodType.Forever => "Lifetime",
                StatPeriodType.Monthly => statPeriod.PeriodRange.LowerBound.ToString("yyyy-MM"),
                _ => statPeriod.PeriodRange.ToString(),
            };
        }
    }
}
