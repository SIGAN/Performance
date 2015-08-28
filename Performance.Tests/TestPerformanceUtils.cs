using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Performance.Utils;
using Xunit;

namespace Performance.Tests
{
	public class TestPerformanceUtils
	{
		public TestPerformanceUtils()
		{
		}

		[Fact(DisplayName = "PerformanceUtils.CalculateAffinityMask")]
		public void CalculateAffinityMask()
		{
			PerformanceUtils.ProcessorCount = 4;

			Assert.Equal(0x01, PerformanceUtils.CalculateAffinityMask(1, false));
			Assert.Equal(0x03, PerformanceUtils.CalculateAffinityMask(2, false));
			Assert.Equal(0x07, PerformanceUtils.CalculateAffinityMask(3, false));
			Assert.Equal(0x0F, PerformanceUtils.CalculateAffinityMask(4, false));
			Assert.Equal(0x0F, PerformanceUtils.CalculateAffinityMask(5, false));

			PerformanceUtils.ProcessorCount = 4;

			Assert.Equal(0x01, PerformanceUtils.CalculateAffinityMask(1, true));
			Assert.Equal(0x05, PerformanceUtils.CalculateAffinityMask(2, true));
			Assert.Equal(0x07, PerformanceUtils.CalculateAffinityMask(3, true));
			Assert.Equal(0x0F, PerformanceUtils.CalculateAffinityMask(4, true));
			Assert.Equal(0x0F, PerformanceUtils.CalculateAffinityMask(5, true));

			PerformanceUtils.ProcessorCount = 8;

			Assert.Equal(0x01, PerformanceUtils.CalculateAffinityMask(1, true));
			Assert.Equal(0x11, PerformanceUtils.CalculateAffinityMask(2, true));
			Assert.Equal(0x25, PerformanceUtils.CalculateAffinityMask(3, true));
			Assert.Equal(0x55, PerformanceUtils.CalculateAffinityMask(4, true));
			Assert.Equal(0x5B, PerformanceUtils.CalculateAffinityMask(5, true));
			Assert.Equal(0xFF, PerformanceUtils.CalculateAffinityMask(8, true));
			Assert.Equal(0xFF, PerformanceUtils.CalculateAffinityMask(9, true));
		}
	}
}
