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

            // StatRepositoryOptions contains the connection string.
            // In Development use user-secrets.
            // In Production use Environment Variables or an online secrets store, depending on your host.
            builder.Services.AddOptions<StatRepositoryOptions>()
                .Bind(builder.Configuration.GetSection(StatRepositoryOptions.StatRepositoryOptionsKey))
                .ValidateDataAnnotations();
            builder.Services.AddOptions<LeagueRepositoryOptions>()
                .Bind(builder.Configuration.GetSection(LeagueRepositoryOptions.SectionKey))
                .ValidateDataAnnotations();

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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "2v2leaderboard",
            //    pattern: "2v2",
            //    defaults: new { controller = "leaderboard", action = "2v2" });

            //app.MapControllerRoute(
            //    name: "3v3leaderboard",
            //    pattern: "3v3",
            //    defaults: new { controller = "leaderboard", action = "3v3" });

            //app.MapControllerRoute(
            //    name: "4v4leaderboard",
            //    pattern: "4v4",
            //    defaults: new { controller = "leaderboard", action = "4v4" });

            app.MapAreaControllerRoute(
                name: "LeagueSeason",
                areaName: "League",
                pattern: "League/Season/{id:long}/{action=Index}",
                defaults: new { controller = "Season" });

            app.MapAreaControllerRoute(
                name: "LeagueList",
                areaName: "League",
                pattern: "League/List",
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
                pattern: "League/Franchise/{id:long}/{action=Details}",
                defaults: new { controller = "Franchise" });

            app.MapAreaControllerRoute(
                name: "Leagueteam",
                areaName: "League",
                pattern: "League/Team/{id:long}/{action=Index}",
                defaults: new { controller = "Team" });

            app.MapAreaControllerRoute(
                name: "LeagueLeague",
                areaName: "League",
                pattern: "League/{id:long}/{action=Details}",
                defaults: new { controller = "League" });

            app.MapAreaControllerRoute(
                name: "LeagueAreaDefault",
                areaName: "League",
                pattern: "League/{controller=Home}/{action=Index}/{id?}");

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