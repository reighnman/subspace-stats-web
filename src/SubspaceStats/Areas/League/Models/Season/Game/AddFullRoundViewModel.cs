using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Game
{
    public enum FullRoundOf
    {
        /// <summary>
        /// Create games for all combinations of teams (order of teams does not matter).
        /// </summary>
        Combinations,

        /// <summary>
        /// Create games for all permutations of teams (order of teams matters, e.g. home/away games).
        /// </summary>
        Permutations,
    }

    public class AddFullRoundViewModel
    {
        [ValidateNever]
        public required SeasonModel Season { get; set; }
        [ValidateNever]
        public required LeagueModel League { get; set; }

        [Required]
        public required FullRoundOf Mode { get; set; }
    }
}
