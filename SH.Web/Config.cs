using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SH.Web
{
	public static class Config
	{
		public static string DBConStr
		{
			get
			{
				return System.Configuration.ConfigurationManager.ConnectionStrings["main"].ConnectionString;
			}
		}

		public static string Version
		{
			get
			{
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
				string version = fvi.FileVersion;

				return version;
			}
		}
	}
}