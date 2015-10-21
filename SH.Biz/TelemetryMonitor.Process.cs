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
    public class TelemetryMonitorProcess: ProcessBase<TelemetryMonitorConfig>
    {
		private IHostData _hostData;
		private ISourceData _sourceData;
		private IResultData _resultData;
				
		public TelemetryMonitorProcess(): base("TelemetryMonitor")
		{

		}

		protected override void OnConfigChanged()
		{
			SleepMSec = ProcessConfig.DelayMin * 60000;

			DataSource ds = new Data.DataSource(ProcessConfig.ConnectionString);

			_hostData = new HostDataStorage(ds);
			_sourceData = new SourceDataStorage(ds);
			_resultData = new ResultDataStorage(ds);

		}

		protected override void OnAction()
		{
			ObjSource item = _sourceData.Checkout();
			if (item == null) return;

			try
			{
				string content = HttpDownloader.Get(item.Url);
				_sourceData.Checkin(item.Id, content);
				DebugFormat("Get data success! sourceId={0} url={1}", item.Id, item.Url);
			}
			catch(Exception ex)
			{
				_sourceData.CheckinError(item.Id, ex.ToString());
				throw ex;
			}
		}        
    }
}
