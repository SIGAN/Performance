using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading;
using Algorithms.Core;

namespace Performance.Utils
{
	public static class PerformanceUtils
	{
		public static void SyncTicks()
		{
			var current = Environment.TickCount;
			while (current == Environment.TickCount) ;
		}

		public static int ProcessorCount = Environment.ProcessorCount;

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentThread();
		[DllImport("kernel32.dll")]
		private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

		public static int CalculateAffinityMask(int threadCount = 1, bool assignEvenly = false)
		{
			var mask = 0;

			if (assignEvenly)
			{
				var ta = 0;

				for (var i = 0; i < ProcessorCount; i++)
				{
					mask <<= 1;
					ta += threadCount;
					if (ta >= ProcessorCount)
					{
						mask |= 1;
						ta -= ProcessorCount;
					}
				}
			}
			else
			{
				mask = (1 << threadCount) - 1;
			}

			mask &= (1 << ProcessorCount) - 1;

			return mask;
		}

		public static IDisposable SetThreadAffinity(int threadCount = 1, bool assignEvenly = false)
		{
			Contract.Assert(threadCount > 0, nameof(threadCount) + " must be greater than zero!");
			Contract.EndContractBlock();

			var original = new RefValue<IntPtr>();

			return
				new ManagedDelegateDisposable(
					() =>
					{
						var mask = CalculateAffinityMask(threadCount, assignEvenly);

						// disabling .NET thread management
						Thread.BeginThreadAffinity();
						// setting correctly affinity
						original.Value = SetThreadAffinityMask(GetCurrentThread(), new IntPtr(mask));

						if (original.Value == IntPtr.Zero)
							original.Value = Process.GetCurrentProcess().ProcessorAffinity;
					},
					() =>
					{
						// reverting to process affinity
						SetThreadAffinityMask(GetCurrentThread(), original.Value);
						// enabling .NET thread management
						Thread.EndThreadAffinity();
					});
		}

		public static IDisposable SetProcessAffinity(int threadCount = 1, bool assignEvenly = false)
		{
			Contract.Assert(threadCount > 0, nameof(threadCount) + " must be greater than zero!");
			Contract.EndContractBlock();

			var original = new RefValue<IntPtr>();

			return
				new ManagedDelegateDisposable(
					() =>
					{
						var mask = CalculateAffinityMask(threadCount, assignEvenly);
						
						// disable .NET thread management
						Thread.BeginThreadAffinity();

						// set process affinity
						using (var process = Process.GetCurrentProcess())
						{
							original.Value = process.ProcessorAffinity;
							process.ProcessorAffinity = new IntPtr(mask);
						}
					},
					() =>
					{
						// revert process affinity
						using (var process = Process.GetCurrentProcess())
						{
							original.Value = process.ProcessorAffinity;
							process.ProcessorAffinity = original;
						}

						// enable .NET thread management
						Thread.EndThreadAffinity();
					});
		}

	}
}
