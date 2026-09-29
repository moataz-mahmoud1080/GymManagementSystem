using System.Threading.Tasks;
using GymManagementSystem.BLL.Profiels;
using GymManagementSystem.BLL.Service.Attstchment;
using GymManagementSystem.BLL.Service.Classes;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.DAL.Data.DbContext;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.Classes;
using GymManagementSystem.DAL.Repository.InterFases;
using GymManagementSystem.PL.Extention;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace GymManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<GymDbContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            //builder.Services.AddScoped<IPlanRepositoy,PlanRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepositoy<>));
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IMemberService,MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<ISessionService, SessionServicey>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IAttatchmentService, AttatchmentService>();
            builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
            builder.Services.AddAutoMapper(m=> m.AddProfile(new MappingProfile()));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(c =>
            {
                c.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<GymDbContext>();


            var app = builder.Build();


            await app.MigrateAndSeedAsync();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
