using DapperDemo.Models.Data;
using DapperDemo.Repository;
using Microsoft.EntityFrameworkCore;


namespace DapperDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSingleton<AuthRepository>();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddRazorPages();
            builder.Services.AddKendo();

            builder.Services.AddMvc()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
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

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); 
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
