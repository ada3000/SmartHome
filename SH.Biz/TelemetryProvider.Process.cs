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
	public class TelemetryProviderProcess : ServiceProcessBase
    {
		private ILog Logger = LogManager.GetLogger("TelemetryProvider");

		private HttpServer _webServer = null;

		static CPUUsage _cpu = new CPUUsage();
		static MemUsage _mem = new MemUsage();
		static MachineInfo _machineInfo = new MachineInfo();
		static DiskUsage _disk = new DiskUsage();

		private void Init()
		{
			TelemetryProviderConfig config = Cfg.Xml.ToXmlReader().Deserialize<TelemetryProviderConfig>();
			_webServer = new HttpServer(config.WebServerPort);

			_webServer.OnError += WebServer_OnError;
			_webServer.OnGetRequest += WebServer_OnGetRequest;
			_webServer.OnPostRequest += WebServer_OnPostRequest;
			_webServer.Start();
		}

		void WebServer_OnPostRequest(object sender, HttpProcessorEventArgs e)
		{
			throw new NotImplementedException();
		}

		void WebServer_OnGetRequest(object sender, HttpProcessorEventArgs e)
		{
			switch(e.Processor.HttpUrl)
			{
				case "/":
				case "":
					e.Processor.WriteSuccess("<h1>It works!</h1>");
					break;
				case"/all":
					e.Processor.WriteSuccess(GetJsonStats(), "application/json");					
					break;
				default:
					e.Processor.WriteFailure("<h1>Not found!</h1>");
					break;
			}
		}

		private string GetJsonStats()
		{
			List<SensorValue> values = new List<SensorValue>();

			TelemetryResult res = new TelemetryResult
			{
				Create = DateTime.UtcNow,
				Values = values
			};

			values.AddRange(_cpu.Collect());
			values.AddRange(_mem.Collect());
			values.AddRange(_disk.Collect());
			values.AddRange(_machineInfo.Collect());

			JsonSerializerSettings set = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			return JsonConvert.SerializeObject(res, set);
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

        protected override void DoWork(object p)
        {
			Init();

            while(Event.Stopping.IsReset())
            {
                try
                {
					//do nothing    , wait stop
                }
                catch(Exception ex)
                {
					Logger.Error("TelemetryProvider: " + ex);
                }

                Thread.Sleep(1000);
            }
        }

		protected override void DoError(Exception err, object p)
		{
			base.DoError(err, p);
			Logger.Error("TelemetryProvider unexpected exception: " + err);
		}
    }
}
