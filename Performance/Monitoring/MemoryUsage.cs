using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance.Monitoring
{
	public class MemoryUsage : ICloneable
	{
		public long WorkingSet { get; set; }
		public long PrivateMemorySize { get; set; }
		public long GcTotalMemory { get; set; }

		object ICloneable.Clone() =>
			MemberwiseClone();

		public MemoryUsage Clone() => 
			(MemoryUsage)MemberwiseClone();
	}
}
