using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using SH.Data;
using SH.BO;
using SH.Data;
using SH.Utils;
using SH.TelemetrySource;

using iTeco.Lib.Base;
using iTeco.Lib.Srv;

using log4net;

using Newtonsoft.Json;

namespace SH.Biz
{
	public class TelemetryPushServerProcess : ProcessBase<TelemetryPushServerConfig>
    {
		private HttpServer _webServer = null;

		private IHostData _hostData;
		private ISourceData _sourceData;
		private IResultData _resultData;

		public TelemetryPushServerProcess() : base("TelemetryPushServer") { }

		protected override void OnConfigChanged()
		{
			DataSource ds = new Data.DataSource(ProcessConfig.ConnectionString);

			_hostData = new HostDataStorage(ds);
			_sourceData = new SourceDataStorage(ds);
			_resultData = new ResultDataStorage(ds);

			if (_webServer != null)
				_webServer.Stop();

			_webServer = new HttpServer(ProcessConfig.WebServerPort);

			_webServer.OnError += WebServer_OnError;
			_webServer.OnGetRequest += WebServer_OnGetRequest;
			_webServer.OnPostRequest += WebServer_OnPostRequest;
			_webServer.Start();

			SleepMSec = 60000;
		}

		void WebServer_OnPostRequest(object sender, HttpProcessorEventArgs e)
		{
			switch (e.Processor.HttpUrl)
			{
				case "/push":
					Logger.Debug(Cfg.Name + ": push ... ");

					string postData = e.InputStream.ReadToEnd();

					TelemetryResult result = JsonConvert.DeserializeObject<TelemetryResult>(postData);

					ObjHost host = _hostData.FindOrCreate(result.ServerName, result.ClusterName);
					ObjSource source = _sourceData.FindOrCreate(host.Id, result.Url, true);
					_sourceData.Checkin(source.Id, postData);

					Logger.Debug(Cfg.Name + ": push success! "+postData);
					break;
				default:
					e.Processor.WriteFailure("<h1>Not found!</h1>");
					break;
			}
		}

		void WebServer_OnGetRequest(object sender, HttpProcessorEventArgs e)
		{
			switch(e.Processor.HttpUrl)
			{
				case "/":
					e.Processor.WriteSuccess("<h1>It works!</h1>");
					break;
				default:
					e.Processor.WriteFailure("<h1>Not found!</h1>");
					break;
			}
		}

		
		void WebServer_OnError(object sender, System.IO.ErrorEventArgs e)
		{
			Logger.Error(Cfg.Name + ": WebServer_OnError " + e.GetException());
		}

		protected override void DisposeManaged()
		{
			base.DisposeManaged();
			_webServer.Stop();
		}
    }
}
