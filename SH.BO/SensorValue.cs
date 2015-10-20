using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	/*
	 item:
{
	type:"cpu|mem|hdd|info",
	subtype:null, //for cpu: now, middle1min, middle5min
	name: null, //string value
	valueMin:0,
	valueMax: 100,
	value: 12.2,
	date: "2015-04-05T18:06:45Z",
	warningMinValue: 90,
}
	 */
	/// <summary>
	/// Обобщенная страктура значения сенсора
	/// </summary>
	public class SensorValue
	{
		public SensorValueType Type;
		public string SubType;

		public string Name;
		public string Description;
		/// <summary>
		/// Минимальное значение для сигнализации
		/// </summary>
		public double? WarningValueMin;

		public double? ValueMin;
		public double? ValueMax;
		public double? Value;
		public string ValueStr;
		/// <summary>
		/// Шкала для значения: градусы, мегабайты/быйты
		/// </summary>
		public SensorValueScale ValueScale;

		public List<SensorValue> Children;

		public override string ToString()
		{
			return string.Format("{0}-{1} {2}, Value={3}{7}, Min={4}, Max={5}, Scale={8}, Warning={6}", 
				Type, SubType, Name, Value, ValueMin, ValueMax, WarningValueMin, ValueStr,ValueScale);
		}
	}
}
