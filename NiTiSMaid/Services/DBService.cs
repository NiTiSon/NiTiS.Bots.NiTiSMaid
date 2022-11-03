using NiTiS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiTiS.Bots.NiTiSMaid.Services;

public class DBService : IDisposable
{
	private readonly LiteDB.LiteDatabase litedb;
	public DBService(File dbFile) : this(dbFile.Path) { }
	public DBService(string path)
	{
		litedb = new LiteDB.LiteDatabase(path);
	}
	public void Dispose()
	{
		litedb?.Dispose();
	}
}
