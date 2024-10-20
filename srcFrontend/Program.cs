
using srcFrontend.Repositories;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using srcFrontend.Models;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDbContextConnection") ?? throw new InvalidOperationException("Connection string 'UserDbContextConnection' not found.");
{

    builder.Services.AddControllers();




    // builder.Services.AddScoped<IEmployeeRepository>(provider =>                     
    // {
    //     var configuration = provider.GetRequiredService<IConfiguration>();
    //     var cosmosEndpointUri = configuration["CosmosEndpointUri"];
    //     var cosmosPrimaryKey = configuration["CosmosPrimaryKey"];
    //     return new EmployeeRepository(cosmosEndpointUri, cosmosPrimaryKey);
    // });                                                                            
    
    builder.Services.AddControllersWithViews();
                                                                            //addet
    builder.Services.AddDbContext<UserDbContext>(options =>
    {
        options.UseSqlite(
            builder.Configuration["ConnectionStrings:UserDbContextConnection"]);
            
    });                                                                         //...


    builder.Services.AddDefaultIdentity<IdentityUser>()                     //addet
        .AddEntityFrameworkStores<UserDbContext>();                         //..



    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    builder.Services.AddSession();

    //builder.Services.AddHostedService<FaceBackgroundService>();

    var app = builder.Build();
    {

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseSession();

        app.UseAuthentication();
        //
        app.UseAuthorization();

        app.MapDefaultControllerRoute();

        app.MapRazorPages();

        app.MapControllers();

        app.Run();
    }
}