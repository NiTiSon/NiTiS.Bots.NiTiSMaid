using NiTiS.Bots.NiTiSMaid;

Bot bot = new(new File("token.txt").ReadAllText(), new(true, new ulong[]
{
	508012163307143168,
}));

bot.Start();