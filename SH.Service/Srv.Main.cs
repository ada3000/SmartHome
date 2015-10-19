using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using iTeco.Lib.Base;
using iTeco.Lib.Srv;

namespace Profile.Service
{
	public partial class MainSrv : ServiceBase
	{
		/// <summary>
		/// Основлной сервис
		/// </summary>
		private ServiceMain _service = null;
		/// <summary>
		/// Поток для проверки конфигурации
		/// </summary>
		private Thread _checkConfig = null;

		public MainSrv()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_service = new ServiceMain(new ServiceConfigFileEx(Config.ProcessesFilePath));
			
			_checkConfig = new Thread(CheckConfig_Worker);
			_checkConfig.IsBackground = true;
			_checkConfig.Start();

			Thread.Sleep(500);
			//_service.Cfg.Read();
			_service.Event.Log = EventLog;
			_service.Event.Error = Service_Error;
			_service.Start();
		}
		/// <summary>
		/// Проверка файла конфигурации
		/// </summary>
		private void CheckConfig_Worker()
		{
			//Чтение реального названия сервиса
			EventLog.Source = Program.GetServiceName();

			while (true)
			{
				try
				{
					_service.Cfg.Read();
				}
				catch (Exception e)
				{
					EventLog.WriteEntry("ReadConfig error: " + e.ToString(), EventLogEntryType.Error);
				}

				_service.Event.Stopping.WaitOne(Config.CheckConfigTimeoutSec * 1000);
				
				if (_service.Event.Stopping.IsSet()
					|| _service.Event.Started.IsSet())
					break;
			}
		}
		/// <summary>
		/// Обработка сообщений с ошибками
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="e"></param>
		private void Service_Error(ServiceMain obj, EventArgs e)
		{
			if (obj.Error == null) return;
			EventLog.WriteEntry(obj.Error.ToString(), EventLogEntryType.Error);
		}

		protected override void OnStop()
		{
			try
			{
				_service.Stop();
				_checkConfig.Abort();
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("OnStop error: " + e.ToString(), EventLogEntryType.Error);
			}
		}
	}
}
