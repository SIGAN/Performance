using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Performance.Monitoring
{
	public class CpuMonitor : IMonitor
	{
		private CpuTemp[] Min { get; set; }
		private CpuTemp[] Max { get; set; }

		public CpuMonitor()
		{
			_hasStarted = false;
		}

		private bool _hasStarted { get; set; }

		public void Start()
		{
			if (_hasStarted)
				throw new InvalidOperationException("CpuMonitor'ing has been already started!");

			_hasStarted = true;

			Min = MonitoringUtils.ReadCpuTemp().ToArray();
			Max = Min.Select(readnings=>readnings.Clone()).ToArray();
		}

		private Timer _timer { get; set; }

		public void StartTimer()
		{
			if (_timer != null)
				throw new InvalidOperationException("CpuMonitor'ing timer has been already started!");
			if (!_hasStarted)
				throw new InvalidOperationException("CpuMonitor'ing was not started!");

			_timer = new Timer(s => { Check(); }, null, 0, 100);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ForEachReading(Action<CpuTemp, CpuTemp> action)
		{
		}

		public void Check()
		{
			if (!_hasStarted)
				throw new InvalidOperationException("CpuMonitor'ing was not started!");

			var curTemps = MonitoringUtils.ReadCpuTemp().ToArray();

			for (var i = 0; i < Math.Min(Math.Min(Min.Length, Max.Length), curTemps.Length); i++)
			{
				var min = Min[i];
				var max = Max[i];
				var cur = curTemps[i];

				if (min.Current > cur.Current) min.Current = cur.Current;
				if (max.Current < cur.Current) max.Current = cur.Current;
			}
		}

		public void StopTimer()
		{
			if (_timer == null)
				throw new InvalidOperationException("CpuMonitor'ing timer was not started!");
			if (!_hasStarted)
				throw new InvalidOperationException("CpuMonitor'ing was not started!");

			_timer.Dispose();
			_timer = null;
		}

		public void Stop()
		{
			if (!_hasStarted)
				throw new InvalidOperationException("CpuMonitor'ing was not started!");

			Check();

			_hasStarted = false;
		}

		public IEnumerable<CpuTemp> GetDiff()
		{

			for (var i = 0; i < Math.Min(Min.Length, Max.Length); i++)
			{
				var min = Min[i];
				var max = Max[i];

				yield return new CpuTemp { Current = max.Current - min.Current, Critical = min.Critical };
			}
		}
	}
}
