using bachelorOppgave13.Interfaces;
using bachelorOppgave13.Repositories;
using bachelorOppgave13.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using bachelorOppgave13.Services.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using bachelorOppgave13.Models;


var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("UserDbContextConnection") ?? throw new InvalidOperationException("Connection string 'UserDbContextConnection' not found.");
{

    builder.Services.AddControllers();
    

    // builder.Services.AddScoped<IEmployeeRepository>(provider =>                     
    // {
    //     var configuration = provider.GetRequiredService<IConfiguration>();
    //     var cosmosEndpointUri = configuration["CosmosEndpointUri"];
    //     var cosmosPrimaryKey = configuration["CosmosPrimaryKey"];
    //     return new EmployeeRepository(cosmosEndpointUri, cosmosPrimaryKey);
    // });                                                                            
                                                                              

    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();
    builder.Services.AddScoped<IComputerVisionService, ComputerVisionService>();
    builder.Services.AddControllersWithViews();
                                                                            
    /*builder.Services.AddDbContext<UserDbContext>(options =>
    {
        options.UseSqlite(
            builder.Configuration["ConnectionStrings:UserDbContextConnection"]);
    }); */                                                                        


    /*builder.Services.AddDefaultIdentity<IdentityUser>()                    
        .AddEntityFrameworkStores<UserDbContext>();  */                       



    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    builder.Services.AddSession();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors(options => {
        options.AddPolicy(name: "AllowedCorsOrigins",
            builder => {
                builder
                    .WithOrigins("https://localhost:7164","https://bachelor13-fwa.azurewebsites.net")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    //builder.Services.AddHostedService<FaceBackgroundService>();
    
    var app = builder.Build();
    {

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", " v1"); });

        app.UseHttpsRedirection();
        
        app.UseCors("AllowedCorsOrigins");

        app.UseStaticFiles();

        app.UseSession();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapDefaultControllerRoute();

        app.MapRazorPages();

        app.MapControllers();

        app.Run();
    }
}


