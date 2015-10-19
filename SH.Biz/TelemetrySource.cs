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
		static CPUUsage _cpu = new CPUUsage();
		static MemUsage _mem = new MemUsage();
		static MachineInfo _machineInfo = new MachineInfo();
		static DiskUsage _disk = new DiskUsage();

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
				Values = values
			};

			values.AddRange(_cpu.Collect());
			values.AddRange(_mem.Collect());
			values.AddRange(_disk.Collect());
			values.AddRange(_machineInfo.Collect());

			return res;	
		}
	}
}
