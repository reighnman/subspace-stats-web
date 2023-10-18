namespace SubspaceStats.Models.GameDetails.TeamVersus
{
    [Flags]
    public enum FirstOut
    {
        /// <summary>
        /// The player was not the first player in the match to get knocked out first.
        /// </summary>
        No = 0x00,

        /// <summary>
        /// The player was the first player in the match to get knocked out.
        /// </summary>
        Yes = 0x01,

        /// <summary>
        /// The player was the first player in the match to get knocked out and met the criteria for being critical.
        /// The criteria for critical is: knocked out under 10 minutes and had less than 2 kills.
        /// </summary>
        YesCritical = 0x03,
    }
}
