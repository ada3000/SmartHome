using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using SH.Data;
using SH.BO;
using SH.Utils;

namespace SH.Biz
{
	[XmlRoot("process")]
	public class TelemetryProviderConfig
    {
		[XmlElement("webServerPort")]
		public int WebServerPort {get;set;}
    }
}
