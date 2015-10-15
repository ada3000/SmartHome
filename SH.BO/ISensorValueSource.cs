using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public interface ISensorValueSource
	{
		/// <summary>
		/// Сбор данных с сенсоров
		/// </summary>
		/// <returns></returns>
		IEnumerable<SensorValue> Collect();
	}
}
