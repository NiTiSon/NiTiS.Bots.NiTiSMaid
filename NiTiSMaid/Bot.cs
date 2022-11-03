namespace NiTiS.Bots.NiTiSMaid;

public class Bot
{
	private readonly Configuration conf;
	public Bot(string token, Configuration config)
	{

	}

	public record class Configuration(bool Debug, ulong[] OwnerIDs);
}