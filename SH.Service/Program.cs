using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Profile.Service
{
	static class Program
	{
		//start log4net
		private static ILog _logger = LogManager.GetLogger("1");
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//start log4net
			_logger.Debug("1");
			
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new MainSrv() 
			};
			//ServicesToRun[0].ServiceName = GetServiceName();

			ServiceBase.Run(ServicesToRun);
		}
		public static String GetServiceName()
		{
			int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
			String query = "SELECT * FROM Win32_Service where ProcessId = " + processId;
			System.Management.ManagementObjectSearcher searcher =
				new System.Management.ManagementObjectSearcher(query);

			foreach (System.Management.ManagementObject queryObj in searcher.Get())
			{
				return queryObj["Name"].ToString();
			}

			throw new Exception("Can not get the ServiceName");
		}
	}
}
