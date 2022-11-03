using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NiTiS.Bots.NiTiSMaid.Attributes;

public class RequireOwnerAttribute : PreconditionAttribute
{
	public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
	{
		switch (context.Client.TokenType)
		{
			case TokenType.Bot:
				if (!services.GetService<NiTiSMaidBot.Configuration>().OwnerIDs.Contains(context.User.Id))
					return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"Command can only be run by the owner"));
				return Task.FromResult(PreconditionResult.FromSuccess());
			default:
				return Task.FromResult(PreconditionResult.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}."));
		}
	}
}
