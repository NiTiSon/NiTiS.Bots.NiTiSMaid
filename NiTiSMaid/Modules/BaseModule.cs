using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace NiTiS.Bots.NiTiSMaid.Modules;

public class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("help", "Show help menu")]
	public async Task CommandHelp()
	{
		await RespondAsync($"Not implement yet");
	}
	[RequireBotPermission(ChannelPermission.SendMessages)]
	[SlashCommand("create-embed", "Create a embed message at specific channel")]
	public async Task CommandEmbed([ChannelTypes(ChannelType.Text)]IGuildChannel channel = null, string message = "", string title = "", bool showAuthor = false, bool showDate = false)
	{
		EmbedBuilder embed = new()
		{
			Title = title,
			Description = message,
		};

		if (showAuthor)
			embed.WithAuthor(Context.User);

		if (showDate)
			embed.WithCurrentTimestamp();
		if (channel is IMessageChannel msg)
		{
			await msg.SendMessageAsync(embed: embed.Build());
		}
		await RespondAsync($"Successfully created at <#{channel.Id}>");
	}
}