using NiTiS.Bots.NiTiSMaid;
using NiTiS.IO;
using Serilog;

internal class Program
{
	private static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.File("last.log")
			.CreateLogger();

		NiTiSMaidBot bot = new(new File("token.txt").ReadAllText(), new()
		{
			Debug = true,
			OwnerIDs = new ulong[] { 508012163307143168 },
		});

		bot.Start();
	}
}