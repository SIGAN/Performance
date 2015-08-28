using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance.Monitoring
{
	public class CpuLoad
	{
		public static readonly CpuLoad Zero = new CpuLoad { PrivilegedTime = TimeSpan.Zero, UserTime = TimeSpan.Zero, TotalTime = TimeSpan.Zero };

		public TimeSpan PrivilegedTime { get; set; }
		public TimeSpan UserTime { get; set; }
		public TimeSpan TotalTime { get; set; }
	}
}
