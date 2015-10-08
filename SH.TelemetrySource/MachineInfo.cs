using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace SH.TelemetrySource
{
	public class MachineInfo
	{
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
	}
}
