using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SH.BO;

namespace SH.TelemetrySource
{
	public class DiskUsage: ISensorValueSource
	{
		PerformanceCounter diskQui = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		public IEnumerable<SensorValue> Collect()
		{
			List<SensorValue> result = new List<SensorValue>();

			foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
			{
                if (di.DriveType ==  System.IO.DriveType.Removable
                    || di.DriveType== System.IO.DriveType.CDRom) continue;

					SensorValue drive = new SensorValue
					{
						//Date = DateTime.UtcNow,
						Children = new List<SensorValue>(),
						Name = di.VolumeLabel + " (" + di.Name.Replace("\\","") + ")",
						Description = "Disk persent usage",
						Type = SensorValueType.Hdd,
						Value = di.TotalSize - di.AvailableFreeSpace,
						WarningValueMin = 0.9* di.TotalSize, //warning when 90% is used,
						ValueScale = SensorValueScale.Byte,
						ValueMin = 0,
						ValueMax = di.TotalSize
                    };

                    result.Add(drive);
			}

			return result;
		}
	}
}
