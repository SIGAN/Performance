using System;
using System.Threading;

namespace Performance.Monitoring
{
	public class MemoryMonitor : IMonitor
	{
		private MemoryUsage Min { get; set; }
		private MemoryUsage Max { get; set; }

		public MemoryMonitor()
		{
			_hasStarted = false;
		}

		private bool _hasStarted { get; set; }

		public void Start()
		{
			if (_hasStarted)
				throw new InvalidOperationException("MemoryMonitor'ing has been already started!");

			_hasStarted = true;

			Min = MonitoringUtils.ReadProcessMemory();
			Max = Min.Clone();
		}

		private Timer _timer { get; set; }

		public void StartTimer()
		{
			if (_timer != null)
				throw new InvalidOperationException("MemoryMonitor'ing timer has been already started!");
			if (!_hasStarted)
				throw new InvalidOperationException("MemoryMonitor'ing was not started!");

			_timer = new Timer(s => { Check(); }, null, 0, 100);
		}

		public void Check()
		{
			if (!_hasStarted)
				throw new InvalidOperationException("MemoryMonitor'ing was not started!");

			var mem = MonitoringUtils.ReadProcessMemory();

			if (Min.WorkingSet > mem.WorkingSet) Min.WorkingSet = mem.WorkingSet;
			if (Min.PrivateMemorySize > mem.PrivateMemorySize) Min.PrivateMemorySize = mem.PrivateMemorySize;
			if (Min.GcTotalMemory > mem.GcTotalMemory) Min.GcTotalMemory = mem.GcTotalMemory;

			if (Max.WorkingSet < mem.WorkingSet) Max.WorkingSet = mem.WorkingSet;
			if (Max.PrivateMemorySize < mem.PrivateMemorySize) Max.PrivateMemorySize = mem.PrivateMemorySize;
			if (Max.GcTotalMemory < mem.GcTotalMemory) Max.GcTotalMemory = mem.GcTotalMemory;
		}

		public void StopTimer()
		{
			if (_timer == null)
				throw new InvalidOperationException("MemoryMonitor'ing timer was not started!");
			if (!_hasStarted)
				throw new InvalidOperationException("MemoryMonitor'ing was not started!");

			_timer.Dispose();
			_timer = null;
		}

		public void Stop()
		{
			if (!_hasStarted)
				throw new InvalidOperationException("MemoryMonitor'ing was not started!");

			Check();

			_hasStarted = false;
		}

		public MemoryUsage GetDiff() =>
			new MemoryUsage
			{
				WorkingSet = Max.WorkingSet - Min.WorkingSet,
				PrivateMemorySize = Max.PrivateMemorySize - Min.PrivateMemorySize,
				GcTotalMemory = Max.GcTotalMemory-Min.GcTotalMemory,
			};
	}
}
