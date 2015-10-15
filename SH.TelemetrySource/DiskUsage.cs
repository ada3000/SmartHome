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
		public IEnumerable<SensorValue> Collect()
		{
			List<SensorValue> result = new List<SensorValue>();

			foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
			{
					SensorValue drive = new SensorValue
					{
						Date = DateTime.UtcNow,
						Children = new List<SensorValue>(),
						Name = di.VolumeLabel + " (" + di.Name.Replace("\\","") + ")",
						Description = "Disk persent usage",
						Type = SensorValueType.Hdd,
						Value = 100 - 100 * di.AvailableFreeSpace / di.TotalSize,
						WarningValueMin = 90, //warning when 90% is used,
						ValueScale = SensorValueScale.Persent,
						ValueMin = 0,
						ValueMax = 100
					};

					drive.Children.Add(new SensorValue
						{
							Type = SensorValueType.Hdd,
							SubType = "AvailableFreeSpace",							
							ValueScale = SensorValueScale.Byte,
							Value = di.AvailableFreeSpace
						});
					drive.Children.Add(new SensorValue
					{
						Type = SensorValueType.Hdd,
						SubType = "TotalSize",						
						ValueScale = SensorValueScale.Byte,
						Value = di.TotalSize
					});

					result.Add(drive);
			}

			return result;
		}
	}
}
