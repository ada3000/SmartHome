using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.TelemetrySource
{
	public class MemUsage
	{
		private PerformanceCounter _avaibleCounter;
		private PerformanceCounter _usedCounter;

		public MemUsage()
		{
			_avaibleCounter = new PerformanceCounter("Memory", "Available MBytes");
			_usedCounter = new PerformanceCounter("Memory", "Available MBytes");
		}
		public float TotalMB
		{
			get
			{
				var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
				return ci.TotalPhysicalMemory / 1024.0f / 1024.0f;
			}
		}
		public float AvaibleMB
		{
			get
			{
				//return _avaibleCounter.NextValue();
				var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
				return ci.AvailablePhysicalMemory / 1024.0f / 1024.0f;
			}
		}

	}
}
