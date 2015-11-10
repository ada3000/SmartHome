using System;
using System.Collections.Generic;
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
	}
}