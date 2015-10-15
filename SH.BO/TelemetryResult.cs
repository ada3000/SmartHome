using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.BO
{
	public class TelemetryResult
	{
		public DateTime Create;
		public IEnumerable<SensorValue> Values;
	}
}
