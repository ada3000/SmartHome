using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.Utils
{
	public class HttpProcessorEventArgs : EventArgs
	{
		public HttpProcessor Processor { get; private set; }
		public StreamReader InputStream { get; private set; }
		public HttpProcessorEventArgs(HttpProcessor processor, StreamReader inputStream = null)
		{
			Processor = processor;
			InputStream = inputStream;
		}
	}
}
