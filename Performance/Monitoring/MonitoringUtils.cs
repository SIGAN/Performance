using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using Algorithms.Core;

namespace Performance.Monitoring
{
	public static class MonitoringUtils
	{
		/// <summary>
		/// Reads CPU temperatures in Celsius via WMI
		/// </summary>
		public static IEnumerable<CpuTemp> ReadCpuTemp()
		{
			ManagementObjectSearcher searcher =
				new ManagementObjectSearcher("root\\WMI",
				"SELECT * FROM MSAcpi_ThermalZoneTemperature");

			foreach (ManagementObject queryObj in searcher.Get())
				yield return
					new CpuTemp
					{
						Current = (Convert.ToDouble(queryObj["CurrentTemperature"].ToString()) / 10 - 273.15),
						Critical = (Convert.ToDouble(queryObj["CriticalTripPoint"].ToString()) / 10 - 273.15),
					};
					
			// kelvin = raw / 10;
			// celsius = (raw / 10) - 273.15;
			// farenheight = ((raw / 10) - 273.15) * 9 / 5 + 32;					
		}

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		/// <summary>
		/// Reads CPU load
		/// </summary>
		public static CpuLoad ReadCpuLoad()
		{
			var tid = GetCurrentThreadId();
			using (var process = Process.GetCurrentProcess())
				foreach (ProcessThread thread in process.Threads)
					using (thread)
						if (thread.Id == tid)
							return new CpuLoad
							{
								PrivilegedTime = thread.PrivilegedProcessorTime,
								UserTime = thread.UserProcessorTime,
								TotalTime = thread.TotalProcessorTime,
							};
			return CpuLoad.Zero;
		}

		/// <summary>
		/// Reads memory usage details
		/// </summary>
		public static MemoryUsage ReadProcessMemory()
		{
			var process = Process.GetCurrentProcess();
			return new MemoryUsage
			{
				WorkingSet = process.WorkingSet64,
				PrivateMemorySize = process.PrivateMemorySize64,
				GcTotalMemory = GC.GetTotalMemory(false),
			};
		}

		/// <summary>
		/// Starts new monitoring session of a <typeparamref name="TMonitor"/> type calling <paramref name="callback"/> at the end.
		/// </summary>
		public static IDisposable NewSession<TMonitor>(Action<TMonitor> callback) where TMonitor : class, IMonitor, new()
		{
			var m = new TMonitor();
			return new ManagedDelegateDisposable(
				() =>
				{
					m.Start();
					m.StartTimer();
				},
				() =>
				{
					m.StopTimer();
					m.Stop();

					callback(m);
				});
		}
	}
}
