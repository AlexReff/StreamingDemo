using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddRazorPages();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    )
);

//builder.Services.AddHttpContextAccessor();

//builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

builder.Services.AddSingleton<RedditApiClient>();
builder.Services.AddSingleton<RedditTokenProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
//app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<RedditHub>("/hub");
    //endpoints.MapControllers();
});

//app.MapControllers();

//app.MapRazorPages();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();
