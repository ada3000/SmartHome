using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	/// <summary>
	/// Обобщенная страктура значения сенсора
	/// </summary>
	public enum SensorValueScale
	{
		Unknown = 0,
		Persent = 10,
		
		Byte = 20,
		KByte = 21,
		MByte = 22,
		GByte = 23,
		TByte = 24,
		PByte = 25,

		Degree = 40,

		Second = 50,
		Minute = 51,
		Hour = 52,
	}
}
