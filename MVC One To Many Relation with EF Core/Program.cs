using Microsoft.EntityFrameworkCore;
using MVC_One_To_Many_Relation_with_EF_Core.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt => {
    opt.UseSqlServer("Server=MSI;Database=MVCB-B206-Sliders;Trusted_Connection=True");

});


var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=home}/{Action=index}");

app.UseStaticFiles();

app.Run();
