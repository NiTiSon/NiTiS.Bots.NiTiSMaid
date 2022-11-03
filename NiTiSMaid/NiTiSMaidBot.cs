using Discord;
using Serilog.Events;
using Serilog;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using NiTiS.Bots.NiTiSMaid.Services;

namespace NiTiS.Bots.NiTiSMaid;

public class NiTiSMaidBot
{
	private readonly Configuration conf;
	private readonly IServiceProvider services; 
	private readonly DiscordSocketClient discordClient;
	public NiTiSMaidBot(string token, Configuration config)
	{

		discordClient = new(new DiscordSocketConfig()
		{
			AlwaysDownloadUsers = true,
			GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
		});
		conf = config;

		discordClient.Log += LogAsync;

		discordClient.LoginAsync(TokenType.Bot, token).Wait();

		InteractionService interactionService;

		interactionService = new(discordClient);

		services = new ServiceCollection()
			.AddSingleton(discordClient)
			.AddSingleton(this)
			.AddSingleton(config)
			.AddSingleton(interactionService)
			.AddSingleton((prov) => new InteractionHandler(prov))
			.BuildServiceProvider();
	}
	public void Start()
	{
		discordClient.StartAsync().Wait();

		services.GetRequiredService<InteractionHandler>()!.InitializeAsync().Wait();

		Task.Delay(Timeout.Infinite).Wait();
	}
	private static async Task LogAsync(LogMessage message)
	{
		var severity = message.Severity switch
		{
			LogSeverity.Critical => LogEventLevel.Fatal,
			LogSeverity.Error => LogEventLevel.Error,
			LogSeverity.Warning => LogEventLevel.Warning,
			LogSeverity.Info => LogEventLevel.Information,
			LogSeverity.Verbose => LogEventLevel.Verbose,
			LogSeverity.Debug => LogEventLevel.Debug,
			_ => LogEventLevel.Information
		};
		Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
		await Task.CompletedTask;
	}
	public record class Configuration(bool Debug, ulong[] OwnerIDs, ulong[] DiscordGuildIDs)
	{
		public Configuration() : this(false, Array.Empty<ulong>(), Array.Empty<ulong>()) { }
	}
}