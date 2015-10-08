using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.TelemetrySource
{
	public class CPUUsage
	{
		private PerformanceCounter cpuCounter = null;

		private const int MaxValues = 300;
		private List<float> _lastValues = new List<float>(MaxValues);

		private Thread _worker = null;

		public CPUUsage()
		{
			cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

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

				Thread.Sleep(1000);
			}
		}

		private float CalcMiddle(int seconds)
		{
			if (_lastValues.Count == 0)
				return 0f;

			int start = _lastValues.Count >= seconds ? _lastValues.Count - seconds : 0;
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
	}
}
