using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SH.BO;

namespace SH.TelemetrySource
{
	/// <summary>
	/// Использование ЦП процессами
	/// </summary>
	public class ProcessCPUUsage : ISensorValueSource
	{
		private PerformanceCounter cpuCounter = null;

		private const int TimeMult = 10;
		private const int MaxValues = 3600/TimeMult;

		private List<float> _lastValues = new List<float>(MaxValues);

		private Thread _worker = null;

		public ProcessCPUUsage(string processName = "_Total")
		{
			cpuCounter = new PerformanceCounter("Processor", "% Processor Time", processName);

			_worker = new Thread(Worker);
			_worker.IsBackground = true;
			_worker.Start();
		}

		private void Worker()
		{
			while (true)
			{
				if (_lastValues.Count == MaxValues)
					_lastValues.RemoveAt(0);

				_lastValues.Add(cpuCounter.NextValue());

				Thread.Sleep(1000 * TimeMult);
			}
		}

		private float CalcMiddle(int seconds)
		{
			if (_lastValues.Count == 0)
				return 0f;

			int start = _lastValues.Count >= seconds / TimeMult ? _lastValues.Count - seconds / TimeMult : 0;
			int end = _lastValues.Count;

			float result = _lastValues.Skip(start).Take(end - start).Sum() / (end - start);

			return result;
		}
		public float Current
		{
			get
			{
				return _lastValues.Count > 0 ? _lastValues[_lastValues.Count - 1] : 0f;
			}
		}

		public float Average1Min
		{
			get
			{
				return CalcMiddle(60);
			}
		}
		public float Average5Min
		{
			get
			{
				return CalcMiddle(300);
			}
		}

        public float AverageHour
        {
            get
            {
                return CalcMiddle(3600);
            }
        }

        public IEnumerable<SensorValue> Collect()
		{
			//SensorValue result = new SensorValue
			//		{
			//			//Date = DateTime.UtcNow,
			//			Children = new List<SensorValue>(),
			//			Name = "CPU Info",
			//			Type = SensorValueType.CPU,
			//			SubType = "CpuCores",
			//			Value = System.Environment.ProcessorCount,
			//			ValueScale = SensorValueScale.Unknown,
			//		};
			yield return new SensorValue
			{
				//Date = DateTime.UtcNow,
				Name = "CPU Current",
				Type = SensorValueType.CPU,
				SubType = "Current",
				Value = Current,
				ValueMin = 0,
				ValueMax = 100,
				ValueScale = SensorValueScale.Persent
			};

			yield return new SensorValue
			{
				//Date = DateTime.UtcNow,
				Name = "CPU Avg 1 min",
				Type = SensorValueType.CPU,
				SubType = "Average1Min",
				Value = Average1Min,
				ValueMin = 0,
				ValueMax = 100,
				ValueScale = SensorValueScale.Persent
			};

			yield return new SensorValue
			{
				Name = "CPU Avg 5 min",
				Type = SensorValueType.CPU,
				SubType = "Average5Min",
				Value = Average5Min,
				ValueMin = 0,
				ValueMax = 100,
				WarningValueMin = 50,
				ValueScale = SensorValueScale.Persent
			};

            yield return new SensorValue
            {
                Name = "CPU Avg hour",
                Type = SensorValueType.CPU,
                SubType = "AverageHour",
                Value = AverageHour,
                ValueMin = 0,
                ValueMax = 100,
                WarningValueMin = 50,
                ValueScale = SensorValueScale.Persent
            };

            //return new[] { result };
        }

		private class ProcessInfo
		{
			public int PID { get; private set; }
			public string Name { get; private set; }
			public int MaxItems { get; private set; }

			private PerformanceCounter[] _counters;
			private List<float>[] _data;

			public ProcessInfo(int pid, string name, PerformanceCounter[] Counters, int maxItems)
			{
				_counters = Counters;
				PID = pid;
				Name = name;
				MaxItems = maxItems;
			}
		}
	}
}
