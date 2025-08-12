using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Repositories;
using WebSIMS.Repositories.Interfaces;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;

namespace WebSIMS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        
        // Đăng ký DbContext
        builder.Services.AddDbContext<SIMSdbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Authen/Login";
                options.AccessDeniedPath = "/Authen/AccessDenied";
            });
        // Đăng ký các dịch vụ và kho lưu trữ
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
        builder.Services.AddScoped<IStudentService, StudentService>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<IFacultyService, FacultyService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
            options.AddPolicy("FacultyOnly", policy => policy.RequireRole("Faculty"));
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}