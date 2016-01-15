using System;

namespace MazeGenerator
{
	public interface ICloneable<T> : ICloneable
	{
		new T Clone();
	}
}
