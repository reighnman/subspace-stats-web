using SkiaSharp;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Options
{
    public class LeagueOptions
    {
        public const string LeagueSectionKey = "League";

        /// <summary>
        /// IDs of the leagues to show on the league home page.
        /// </summary>
        [Required]
        public required long[] LeagueIds { get; set; }

        /// <summary>
        /// The root file system path of league images.
        /// </summary>
        /// <remarks>
        /// Team banners are stored in:
        /// &lt;ImagePath&gt;/TeamBanners/&lt;filename&gt;
        /// </remarks>
        public string? ImagePhysicalPath { get; set; }

        /// <summary>
        /// The url path for requesting league images.
        /// </summary>
        public string? ImageUrlPath { get; set; }

        /// <summary>
        /// The image format to save uploaded images as.
        /// </summary>
        public SKEncodedImageFormat ImageUploadFormat { get; set; } = SKEncodedImageFormat.Png;

        /// <summary>
        /// A way to seed the database with an admin login.
        /// </summary>
        public LeagueSeedOptions? Seed { get; set; }
    }

    public class LeagueSeedOptions
    {
        public string? AdminUsername { get; set; }
        public string? AdminPassword {  get; set; }
    }
}
