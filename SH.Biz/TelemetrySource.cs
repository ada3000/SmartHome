using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;
using SH.TelemetrySource;

using Newtonsoft.Json;

namespace SH.Biz
{
	class TelemetrySource
	{
		private IEnumerable<ISensorValueSource> _sources = null;
		private string _serverName = null;
		private string _serverUrl = null;
		private string _clusterName = null;

		public TelemetrySource()
		{

		}
		public TelemetrySource(string serverName, string clusterName, string serverUrl, IEnumerable<ISensorValueSource> sources)
		{
			_serverName = serverName;
			_clusterName = clusterName;
			_serverUrl = serverUrl;
			_sources = sources;
		}
		public string GetJsonStats()
		{
			TelemetryResult res = GetStats();

			JsonSerializerSettings set = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			return JsonConvert.SerializeObject(res, set);
		}

		public TelemetryResult GetStats()
		{
			List<SensorValue> values = new List<SensorValue>();

			TelemetryResult res = new TelemetryResult
			{
				Create = DateTime.UtcNow,
				Values = values,
				 ClusterName = _clusterName,
				 ServerName = _serverName,
				 Url = _serverUrl
			};

			foreach (var source in _sources)
				values.AddRange(source.Collect());

			return res;	
		}
	}
}
