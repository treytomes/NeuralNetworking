using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Math
{
	public static class MathHelper
	{
		public static bool IsInRange(int value, int inclusiveMin, int inclusiveMax)
		{
			return (inclusiveMin <= value) && (value <= inclusiveMax);
		}

		public static int Clamp(int value, int inclusiveMin, int inclusiveMax)
		{
			if (value < inclusiveMin)
			{
				return inclusiveMin;
			}
			else if (value > inclusiveMax)
			{
				return inclusiveMax;
			}
			else
			{
				return value;
			}
		}
	}
}
