using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using iTeco.Lib.Base;

namespace Profile.Service
{
	/// <summary>
	/// Параметры конфигурации
	/// </summary>
	public static class Config
	{
		// Название полей
		private const string FN_ProcessesFilePath = "ConfigFilePath";
		private const string FN_CheckConfigTimeoutSec = "CheckConfigTimeoutSec";
		
		/// <summary>
		/// Путь к файлу с параметрами запуска процессов
		/// </summary>
		public static string ProcessesFilePath 
		{
			get
			{
				return ConfigurationManager.AppSettings[FN_ProcessesFilePath];
			}
		}
		/// <summary>
		/// Интервал проверки параметров сервиса
		/// </summary>
		public static int CheckConfigTimeoutSec
		{
			get
			{
				return ConfigurationManager.AppSettings[FN_CheckConfigTimeoutSec].ToInt(60);
			}
		}
	}
}
