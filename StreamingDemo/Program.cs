using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Hubs;
using System;
using System.Net.Http;

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

RedditApiConfig redditApiConfig = new RedditApiConfig(builder.Configuration);
builder.Services.AddSingleton<IRedditApiConfig>(redditApiConfig);

var red = builder.Configuration.GetSection("Reddit");

builder.Services.AddSignalR();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

builder.Services.AddTransient<RedditApiThrottler>();

builder.Services.AddHttpClient<IRedditTokenProvider, RedditTokenProvider>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd(redditApiConfig.UserAgent);
    });

builder.Services.AddTransient<RedditApiAuthenticationHandler>(provider =>
{
    IRedditTokenProvider tokenProvider = provider.GetRequiredService<IRedditTokenProvider>();
    return new RedditApiAuthenticationHandler(tokenProvider);
});


builder.Services.AddHttpClient<IRedditHttpClient, RedditHttpClient>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd(redditApiConfig.UserAgent);
    })
    .AddHttpMessageHandler<RedditApiThrottler>()
    .AddHttpMessageHandler<RedditApiAuthenticationHandler>();


builder.Services.AddSingleton<RedditApiClient>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Error");
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

//app.MapHub<RedditHub>("/hub");

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
