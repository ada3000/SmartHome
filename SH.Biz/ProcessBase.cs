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
		
		protected void Debug(string msg)
		{
			Logger.Debug(Cfg.Name+": "+ msg);
		}
		protected void DebugFormat(string msg, params object[] arguments)
		{
			Logger.DebugFormat(Cfg.Name + ": " + msg, arguments);
		}
		protected void Error(string msg)
		{
			Logger.Error(Cfg.Name + ": " + msg);
		}
        protected override void DoWork(object p)
        {
			Debug("Start process");
			Event.CfgChanged.Set();

            while(Event.Stopping.IsReset())
            {
                try
                {
					if (Event.CfgChanged.IsSet())
					{
						Debug("CfgChanged ...");
						InitConfig();
						OnConfigChanged();
						Event.CfgChanged.Reset();
						Debug("CfgChanged end");
					}

					Debug("OnAction ...");
					OnAction();
					Debug("OnAction end");

					Event.Wait(SleepMSec);
                }
                catch(Exception ex)
                {
					Logger.Error(Cfg.Name+": " + ex);
					Event.Wait(SleepErrorMSec);
                }
            }

			Debug("Stop process");
        }

		protected override void DoError(Exception err, object p)
		{
			base.DoError(err, p);
			Error(" unexpected exception! " + err);
		}
    }
}
