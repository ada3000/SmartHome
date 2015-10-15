using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.Utils
{
	public class BadRequestException: Exception
	{
		public string Url { get; private set; }
		public string Method { get; private set; }

		public BadRequestException(string url, string method, Exception inner): base("Bad request:"+url, inner)
		{			
			Url = url;
			Method = method;
		}
	}
}
