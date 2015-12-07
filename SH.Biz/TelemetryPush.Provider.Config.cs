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
	public class TelemetryPushProviderConfig
	{
		[XmlElement("pushUrl")]
		public string PushUrl { get; set; }
		[XmlElement("serverName")]
		public string ServerName { get; set; }
		[XmlElement("clusterName")]
		public string ClusterName { get; set; }
		[XmlElement("selfUrl")]
		public string SelfUrl { get; set; }
		/// <summary>
		/// интервал отправки данных в минутах
		/// </summary>
		[XmlElement("delayMin")]
		public int DelayMin { get; set; }
		/// <summary>
		/// Параметры сенсоров
		/// </summary>
		[XmlElement("sensorDataSource")]
		public TypeParams[] SensorDataSource { get; set; }
	}
}
