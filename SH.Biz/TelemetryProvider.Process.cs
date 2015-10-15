using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SH.Data;
using SH.BO;
using SH.Utils;

using iTeco.Lib.Base;
using iTeco.Lib.Srv;

using log4net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SH.Biz
{
	public class TelemetryProviderProcess : ServiceProcessBase
    {
		private ILog Logger = LogManager.GetLogger("TelemetryProvider");

		private HttpServer _webServer = null;
        
		
        protected override void DoWork(object p)
        {
			
            while(Event.Stopping.IsReset())
            {
                try
                {
                    
                }
                catch(Exception ex)
                {
					Logger.Error("TelemetryProvider: " + ex);
                }

                Thread.Sleep(60*1000);
            }
        }

		protected override void DoError(Exception err, object p)
		{
			base.DoError(err, p);
			Logger.Error("TelemetryProvider unexpected exception: " + err);
		}
    }
}
