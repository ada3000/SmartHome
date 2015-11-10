using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

using iTeco.Lib.Base;

namespace SH.Data
{
	public class HostDataStorage: IHostData
	{
		private const string FindOrCreateProc = "ObjHost$FindOrCreate";
		private const string DelProc = "ObjHost$Del";
		private const string AllProc = "ObjHost$All";

		private DataSource _source = null;

		public HostDataStorage(DataSource source)
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
			_source.ExecProcNonQuery(DelProc, new PropContainer("id", id));
		}

		public IEnumerable<ObjHost> All()
		{
			return _source.ExecProc(AllProc).Select(p=>CreateObjHost(p));
		}

		private ObjHost CreateObjHost(PropContainer props)
		{
			return new ObjHost
			{
				Id = props["id"].ToLong(),				 
				Create = props["dtCreate"].ToDateTime(DateTime.UtcNow),
				Cluster = props["sCluster"].ToString(),
				Name = props["sName"].ToString()
			};
		}
	}
}
