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
	public class TelemetryPushServerProcess : ProcessBase<TelemetryProviderConfig>
    {
		private HttpServer _webServer = null;

		public TelemetryPushServerProcess() : base("TelemetryPushServer") { }

		protected override void OnConfigChanged()
		{
			if (_webServer != null)
				_webServer.Stop();

			_webServer = new HttpServer(Config.WebServerPort);

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
					Logger.Debug(Cfg.Name + ": push success! ");
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
			throw new NotImplementedException();
		}

		protected override void DisposeManaged()
		{
			base.DisposeManaged();
			_webServer.Stop();
		}
    }
}
