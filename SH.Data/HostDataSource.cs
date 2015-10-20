using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

using iTeco.Lib.Base;

namespace SH.Data
{
	public class HostDataSource: IHostData
	{
		private const string FindOrCreateProc = "ObjHost$FindOrCreate";
		private const string DelProc = "ObjHost$Del";

		private DataSource _source = null;

		public HostDataSource(DataSource source)
		{
			_source = source;
		}

		public ObjHost FindOrCreate(string name, string cluster)
		{
			var row = _source
				.ExecProc(FindOrCreateProc, new PropContainer(new []{"sName","sCluster"}, new []{name, cluster}))
				.First();

			ObjHost result = new ObjHost
			{
				Cluster = cluster,
				Name = name,
				Id = row["id"].ToLong(),
				Create = row["dtCreate"].ToDateTime(DateTime.UtcNow)
			};

			return result;
		}


		public void Remove(long id)
		{
			_source.ExecProcNonQuery(FindOrCreateProc, new PropContainer("id", id));
		}
	}
}
