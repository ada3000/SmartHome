using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

namespace SH.TelemetrySource
{
	public class MemUsage: ISensorValueSource
	{
		public ulong Total
		{
			get
			{
				var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
				return ci.TotalPhysicalMemory;
			}
		}
		public ulong Available
		{
			get
			{
				//return _avaibleCounter.NextValue();
				var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
				return ci.AvailablePhysicalMemory;
			}
		}


		public IEnumerable<SensorValue> Collect()
		{
			SensorValue result = new SensorValue
			{
				//Date = DateTime.UtcNow,
				Children = new List<SensorValue>(),
				Name = "Memory Usage",
				Type = SensorValueType.Memory,
				Value = Total - Available,
				WarningValueMin = 0.9*Total, //warning when 90% is used,
				ValueScale = SensorValueScale.Byte,
				ValueMin = 0,
				ValueMax = Total
            };

			return new[] { result };
		}
	}
}
