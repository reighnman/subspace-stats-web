using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using SubspaceStats.Filters;
using SubspaceStats.Options;
using SubspaceStats.Services;

namespace SubspaceStats
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<StatOptions>(builder.Configuration.GetSection(StatOptions.StatsSectionKey));
            builder.Services.Configure<LeagueOptions>(builder.Configuration.GetSection(LeagueOptions.LeagueSectionKey));

            // The connection string the services use is named: "SubspaceStats"
            // In Development use user-secrets.
            // In Production use Environment Variables or an online secrets store, depending on your host.

            builder.Services.AddHybridCache();

            builder.Services.AddSingleton<ILeagueRepository, LeagueRepository>();
            builder.Services.AddSingleton<IStatsRepository, StatsRepository>();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<OperationCanceledExceptionFilter>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.MapStaticAssets();

            IOptions<LeagueOptions> leagueOptions = app.Services.GetRequiredService<IOptions<LeagueOptions>>();

            if(!string.IsNullOrWhiteSpace(leagueOptions.Value.ImagePhysicalPath) && !string.IsNullOrWhiteSpace(leagueOptions.Value.ImageUrlPath))
            app.UseStaticFiles(
                new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(leagueOptions.Value.ImagePhysicalPath),
                    RequestPath = leagueOptions.Value.ImageUrlPath
                });

            app.UseRouting();

            app.UseAuthorization();

            app.MapAreaControllerRoute(
                name: "LeagueSeasonPlayer",
                areaName: "League",
                pattern: "League/Season/{seasonId:long}/Players/{action}",
                defaults: new { controller = "SeasonPlayer" });

            app.MapAreaControllerRoute(
                name: "LeagueSeasonTeamCreate",
                areaName: "League",
                pattern: "League/Season/{seasonId:long}/Teams/Create",
                defaults: new { controller = "Team", action = "Create" });

            app.MapAreaControllerRoute(
                name: "LeagueSeasonRound",
                areaName: "League",
                pattern: "League/Season/{seasonId:long}/Rounds/{roundNumber:int}/{action=Detail}",
                defaults: new { controller = "SeasonRound" });

            app.MapAreaControllerRoute(
                name: "LeagueSeasonRoundCreate",
                areaName: "League",
                pattern: "League/Season/{seasonId:long}/Rounds/Create",
                defaults: new { controller = "SeasonRound", action = "Create" });

            app.MapAreaControllerRoute(
                name: "LeagueSeason",
                areaName: "League",
                pattern: "League/Season/{seasonId:long}/{action=Index}",
                defaults: new { controller = "Season" });

            app.MapAreaControllerRoute(
                name: "LeagueSeasonCreate",
                areaName: "League",
                pattern: "League/Season/Create",
                defaults: new { controller = "Season", action = "Create" });

            app.MapAreaControllerRoute(
                name: "LeagueManage",
                areaName: "League",
                pattern: "League/Manage",
                defaults: new { controller = "League", action = "Index" });

            app.MapAreaControllerRoute(
                name: "LeagueNav",
                areaName: "League",
                pattern: "League/Nav",
                defaults: new { controller = "Home", action = "Nav" });

            app.MapAreaControllerRoute(
                name: "LeagueCreate",
                areaName: "League",
                pattern: "League/Create",
                defaults: new { controller = "League", action = "Create" });

            app.MapAreaControllerRoute(
                name: "LeagueFranchise",
                areaName: "League",
                pattern: "League/Franchise",
                defaults: new { controller = "Franchise", action = "Index" });

            app.MapAreaControllerRoute(
                name: "LeagueFranchise",
                areaName: "League",
                pattern: "League/Franchise/{franchiseId:long}/{action=Details}",
                defaults: new { controller = "Franchise" });

            app.MapAreaControllerRoute(
                name: "LeagueTeam",
                areaName: "League",
                pattern: "League/Team/{teamId:long}/{action=Index}",
                defaults: new { controller = "Team" });

            app.MapAreaControllerRoute(
                name: "LeagueLeague",
                areaName: "League",
                pattern: "League/{leagueId:long}/{action=Details}",
                defaults: new { controller = "League" });

            app.MapAreaControllerRoute(
                name: "LeagueAreaDefault",
                areaName: "League",
                pattern: "League/{controller=Home}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "AdminArea",
                areaName: "Admin",
                pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Game",
                pattern: "Game/{id:long}",
                defaults: new { controller = "Game", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}