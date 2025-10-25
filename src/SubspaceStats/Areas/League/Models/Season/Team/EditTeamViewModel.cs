﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.Season;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Team
{
    public class EditTeamModel
    {
        [Display(Name = "Team ID")]
        [BindNever]
        [ValidateNever]
        public required long TeamId { get; set; }

        [Display(Name = "Team Name")]
        [StringLength(20, MinimumLength = 1)]
        [Required]
        public required string TeamName { get; set; }

        [Display(Name = "Banner (Small)")]
        [BindNever]
        [ValidateNever]
        public string? BannerSmallPath { get; set; }

        [Display(Name = "Set Banner")]
        public bool SetBannerSmall {  get; set; }

        [Display(Name = "Banner (Small)")]
        public IFormFile? BannerSmall { get; set; }

        [Display(Name = "Set Banner")]
        public bool SetBannerLarge { get; set; }

        [Display(Name = "Banner (Large)")]
        [BindNever]
        [ValidateNever]
        public string? BannerLargePath { get; set; }

        [Display(Name = "Banner (Large)")]
        public IFormFile? BannerLarge { get; set; }

        [Display(Name = "Franchise")]
        public long? FranchiseId { get; set; }
    }

    public class EditTeamViewModel
    {
        public required EditTeamModel Model { get; set; }
        public required SeasonDetails Season { get; set; }
        public required bool ImageUploadsEnabled { get; set; }
        public required OrderedDictionary<long, FranchiseModel> Franchises { get; set; }
    }
}
