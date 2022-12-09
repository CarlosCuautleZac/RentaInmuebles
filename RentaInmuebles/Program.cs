using Microsoft.EntityFrameworkCore;
using RentaInmuebles.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Home/IniciarSesion";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    }
    
);

builder.Services.AddDbContext<sistem21_inmueblesContext>(optionsBuilder=>
    optionsBuilder.UseMySql("server=sistemas19.com;database=sistem21_inmbuebles;user=sistem21_carlos;password=sistemas19_", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.17-mariadb")) 

    );

builder.Services.AddMvc();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(x =>
{
    x.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
    x.MapDefaultControllerRoute();
}
);

app.Run();
