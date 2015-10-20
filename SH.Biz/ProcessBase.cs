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
	/// <summary>
	/// Обертка для процесса:
	/// OnConfigChanged
	/// OnAction
	/// SleepMSec
	/// Config
	/// Logger
	/// </summary>
	/// <typeparam name="TConfig"></typeparam>
	public abstract class ProcessBase<TConfig> : ServiceProcessBase where TConfig: new()
    {
		protected ILog Logger {get; private set;}
		protected TConfig ProcessConfig {get; private set;}

		protected int SleepMSec = 1000;
		protected int SleepErrorMSec = 1000;

		public ProcessBase(string loggerName)
		{
			Logger = LogManager.GetLogger(loggerName);
		}

		private void InitConfig()
		{
			ProcessConfig = Cfg.Xml.ToXmlReader().Deserialize<TConfig>();
		}

		protected virtual void OnConfigChanged() { }
		protected virtual void OnAction() { }
		
        protected override void DoWork(object p)
        {
			Logger.Debug("Start process: " + Cfg.Name);
			Event.CfgChanged.Set();

            while(Event.Stopping.IsReset())
            {
                try
                {
					if (Event.CfgChanged.IsSet())
					{
						InitConfig();
						OnConfigChanged();
						Event.CfgChanged.Reset();
					}

					OnAction();

					Thread.Sleep(SleepMSec);
                }
                catch(Exception ex)
                {
					Logger.Error(Cfg.Name+": " + ex);
					Thread.Sleep(SleepErrorMSec);
                }
            }

			Logger.Debug("Stop process: " + Cfg.Name);
        }

		protected override void DoError(Exception err, object p)
		{
			base.DoError(err, p);
			Logger.Error(Cfg.Name + " unexpected exception! " + err);
		}
    }
}
