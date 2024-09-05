namespace SubspaceStats.Models
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts the value of the current <see cref="TimeSpan"/> to a string representation that differs based on the value.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to convert to a string.</param>
        /// <returns>The string representation.</returns>
        public static string ToDisplayString(this TimeSpan value)
        {
            if (value.Days > 0)
            {
                return value.ToString("d'.'hh':'mm':'ss");
            }
            else if (value.Hours > 0)
            {
                return value.ToString("h':'mm':'ss");
            }
            else
            {
                return value.ToString("m':'ss");
            }
        }
    }
}
