using System;

namespace Performance.Monitoring
{
	public class CpuTemp : ICloneable
	{
		public double Current { get; set; }
		public double Critical { get; set; }

		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

		public CpuTemp Clone()
		{
			return (CpuTemp)MemberwiseClone();
		}
	}
}
