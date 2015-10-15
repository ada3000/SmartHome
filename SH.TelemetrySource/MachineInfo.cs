using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

using SH.BO;

namespace SH.TelemetrySource
{
	public class MachineInfo: ISensorValueSource
	{
		//private static PerformanceCounter _uptime = new PerformanceCounter("System", "System Up Time");
		public MachineInfo()
		{
			SelectQuery query = new SelectQuery(@"Select * from Win32_ComputerSystem");

			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
			{
				//execute the query
				foreach (System.Management.ManagementObject process in searcher.Get())
				{
					//print system info
					process.Get();
					SystemManufacturer = process["Manufacturer"].ToString();
					SystemModel = process["Model"].ToString();
					DNSHostName = process["DNSHostName"].ToString();
					Caption = process["Caption"].ToString();
					PrimaryOwnerName = process["PrimaryOwnerName"].ToString();
					TotalPhysicalMemory = long.Parse(process["TotalPhysicalMemory"].ToString());
					CurrentTimeZone = int.Parse(process["CurrentTimeZone"].ToString());

					foreach (var item in process.Properties)
						Console.WriteLine(item.Name + " " + item.Value);

					break;
				}
			}
		}
		/// <summary>
		/// Time zone in minutes
		/// </summary>
		public int CurrentTimeZone { get; private set; }		
		public long TotalPhysicalMemory { get; private set; }		
		public string PrimaryOwnerName { get; private set; }
		public string DNSHostName { get; private set; }
		public string Caption { get; private set; }
		public string SystemManufacturer { get; private set; }
		public string SystemModel { get; private set; }
		public long UpTimeSec
		{
			get
			{
				var ticks = Stopwatch.GetTimestamp();
				var uptime = ((double)ticks) / Stopwatch.Frequency;
				return (long)uptime;
				//return (long)_uptime.NextValue();
			}
		}

		public IEnumerable<SensorValue> Collect()
		{
			List<SensorValue> result = new List<SensorValue>();

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "CurrentTimeZone",
				SubType = "CurrentTimeZone",
				ValueScale = SensorValueScale.Minute,
				Value = CurrentTimeZone
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "TotalPhysicalMemory",
				SubType = "TotalPhysicalMemory",
				ValueScale = SensorValueScale.Byte,
				Value = TotalPhysicalMemory
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "PrimaryOwnerName",
				SubType = "PrimaryOwnerName",
				ValueStr = PrimaryOwnerName
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "DNSHostName",
				SubType = "DNSHostName",
				ValueStr = DNSHostName
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "Caption",
				SubType = "Caption",
				ValueStr = Caption
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "SystemManufacturer",
				SubType = "SystemManufacturer",
				ValueStr = SystemManufacturer
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "SystemModel",
				SubType = "SystemModel",
				ValueStr = SystemModel
			});

			result.Add(new SensorValue
			{
				//Date = DateTime.UtcNow,
				Type = SensorValueType.Info,
				Name = "UpTime",
				SubType = "UpTime",
				ValueScale = SensorValueScale.Second,
				Value = UpTimeSec
			});

			return result;
		}
	}
}
