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
            builder.Services.Configure<StatOptions>(
                builder.Configuration.GetSection(StatOptions.StatsSectionKey));

            // StatRepositoryOptions contains the connection string.
            // In Development use user-secrets.
            // In Production use Environment Variables or an online secrets store, depending on your host.
            builder.Services.AddOptions<StatRepositoryOptions>()
                .Bind(builder.Configuration.GetSection(StatRepositoryOptions.StatRepositoryOptionsKey))
                .ValidateDataAnnotations();

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

            app.MapControllerRoute(
                name: "Game",
                pattern: "Game/{id}",
                defaults: new { controller = "Game", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}