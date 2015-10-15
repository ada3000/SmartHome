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
		private PerformanceCounter _avaibleCounter;
		private PerformanceCounter _usedCounter;

		public MemUsage()
		{
			_avaibleCounter = new PerformanceCounter("Memory", "Available MBytes");
			_usedCounter = new PerformanceCounter("Memory", "Available MBytes");
		}
		public ulong Total
		{
			get
			{
				var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
				return ci.TotalPhysicalMemory;
			}
		}
		public ulong Avaible
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
				Date = DateTime.UtcNow,
				Children = new List<SensorValue>(),
				Name = "Memory Usage %",
				Type = SensorValueType.Memory,
				Value = 100 - 100 * Avaible / Total,
				WarningValueMin = 90, //warning when 90% is used,
				ValueScale = SensorValueScale.Persent,
				ValueMin = 0,
				ValueMax = 100
			};

			result.Children.Add(new SensorValue
			{
				Type = SensorValueType.Memory,
				SubType = "Avaible",
				ValueScale = SensorValueScale.Byte,
				Value = Avaible
			});

			result.Children.Add(new SensorValue
			{
				Type = SensorValueType.Memory,
				SubType = "Total",
				ValueScale = SensorValueScale.Byte,
				Value = Total
			});

			return new[] { result };
		}
	}
}
