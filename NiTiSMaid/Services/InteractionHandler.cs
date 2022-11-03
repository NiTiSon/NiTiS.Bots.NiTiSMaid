using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NiTiS.Bots.NiTiSMaid.Services;
public class InteractionHandler
{
	private readonly DiscordSocketClient client;
	private readonly InteractionService handler;
	private readonly IServiceProvider provider;
	private readonly NiTiSMaidBot.Configuration conf;
	public InteractionHandler(IServiceProvider provider)
	{
		client = provider.GetRequiredService<DiscordSocketClient>();
		handler = provider.GetRequiredService<InteractionService>();
		conf = provider.GetRequiredService<NiTiSMaidBot.Configuration>();
		this.provider = provider;
	}

	public async Task InitializeAsync()
	{
		client.InteractionCreated += HandleInteraction;
		handler.SlashCommandExecuted += SlashCommandExecuted;
		client.Ready += ReadyAsync;
		handler.Log += LogAsync;

		await handler.AddModulesAsync(typeof(InteractionHandler).Assembly, provider);
		//await handler.AddModulesAsync(typeof(Module.SocialModule).Assembly, provider);
	}
	private static async Task LogAsync(LogMessage message)
	{
		LogEventLevel severity = message.Severity switch
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
	private async Task ReadyAsync()
	{
		if (conf.Debug)
			await handler.RegisterCommandsToGuildAsync(917478231924428872, true);
		else
			await handler.RegisterCommandsGloballyAsync(true);
	}
	private async Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
	{
		if (!arg3.IsSuccess)
		{
			switch (arg3.Error)
			{
				case InteractionCommandError.UnmetPrecondition:
					await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
					break;
				case InteractionCommandError.UnknownCommand:
					await arg2.Interaction.RespondAsync("Unknown command");
					break;
				case InteractionCommandError.BadArgs:
					await arg2.Interaction.RespondAsync("Invalid number or arguments");
					break;
				case InteractionCommandError.Exception:
					await arg2.Interaction.RespondAsync($"Command exception: {arg3.ErrorReason}");
					break;
				case InteractionCommandError.Unsuccessful:
					await arg2.Interaction.RespondAsync("Command could not be executed");
					break;
				default:
					break;
			}
		}
	}
	private async Task HandleInteraction(SocketInteraction interaction)
	{
		try
		{
			SocketInteractionContext? context = new(client, interaction);

			IResult? result = await handler.ExecuteCommandAsync(context, provider);

			if (!result.IsSuccess)
				switch (result.Error)
				{
					case InteractionCommandError.UnmetPrecondition:
						break;
					default:
						break;
				}
		}
		catch
		{
			if (interaction.Type is InteractionType.ApplicationCommand)
				await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
		}
	}
}