using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SH.Data;
using SH.BO;
using SH.Utils;
using SH.TelemetrySource;

using iTeco.Lib.Base;
using iTeco.Lib.Srv;

using log4net;

using Newtonsoft.Json;

namespace SH.Biz
{
	public class TelemetryPushProviderProcess : ProcessBase<TelemetryPushProviderConfig>
    {
		private TelemetrySource _telSource = null;
		public TelemetryPushProviderProcess() : base("TelemetryPushProvider") { }

		protected override void OnConfigChanged()
		{
			SleepMSec = ProcessConfig.DelayMin * 60000;
			SleepErrorMSec = 60000;

			List<ISensorValueSource> sources = new List<ISensorValueSource>();

			foreach (var type in ProcessConfig.SensorDataSource)
			{
				ISensorValueSource source = AssemblyHelper.CreateInstance(type.AssemblyName, type.TypeName) as ISensorValueSource;
				if (source == null)
					throw new ArgumentException("Type " + type + " does not implement interface ISensorValueSource");

				sources.Add(source);
			}

			if (sources.Count == 0)
				throw new ArgumentException("Telemetry sources not set! param 'sensorDataSource'");

			_telSource = new TelemetrySource(ProcessConfig.ServerName, 
				ProcessConfig.ClusterName, 
				ProcessConfig.SelfUrl,
				sources);
		}

		protected override void OnAction()
		{
			string info = Cfg.Name + ": send push url=" + ProcessConfig.PushUrl;

			Logger.Debug(info+" ...");

			string postData = _telSource.GetJsonStats();

			var result = HttpDownloader.Post(ProcessConfig.PushUrl, postData);

			Logger.Debug(info + " end");
		}
    }
}
