using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

using iTeco.Lib.Base;

namespace SH.Data
{
	public class ResultDataStorage: IResultData
	{
		private const string ResultAllProc = "ObjResult$All";

		private DataSource _source = null;

		public ResultDataStorage(DataSource source)
		{
			_source = source;
		}

		public ObjHost FindOrCreate(string name, string cluster)
		{
			var row = _source
				.ExecProc(ResultAllProc, new PropContainer(new[] { "sName", "sCluster" }, new[] { name, cluster }))
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
		
		public IEnumerable<ObjResult> All()
		{
			var rows = _source.ExecProc(ResultAllProc);

			return rows.Select(r => ReadFromProps(r));
		}

		private ObjResult ReadFromProps(PropContainer props)
		{
			ObjResult result = new ObjResult
			{
				HostId = props["idHost"].ToLong(),
				SourceId = props["idSource"].ToLong(),
				Content = props["sContent"].ToString(),
				Create = props["dtCreate"].ToDateTime(DateTime.UtcNow),
				Update = props["dtUpdate"].ToDateTime(DateTime.UtcNow),
			};

			return result;
		}
	}
}
