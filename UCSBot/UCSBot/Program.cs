using MediatR;
using UCSBot.Application.Handlers;
using UCSBot.Domain.Contracts;
using UCSBot.HostedServices;
using UCSBot.Infrastructure;
using UCSBot.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var discordConfig = builder.Configuration.GetSection("Discord").Get<DiscordConfiguration>();

builder.Services.AddSingleton<IBotClient, DiscordBotClient>()
    .AddHostedService<BotHostedService>()
    .AddMediatR(typeof(RegisterChannelToFeedHandler))
    .AddSingleton<IChannelRepository, InMemoryChannelRepository>()
    .Configure<DiscordConfiguration>(o => { o.BotToken = discordConfig.BotToken; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();


app.Run();