using System;
using System.Windows;
using System.Windows.Media;

namespace MazeGenerator.Tiles
{
	public abstract class Tile : IEquatable<Tile>
	{
		public Tile(int id)
		{
			Id = id;
		}

		#region Properties

		public int Id { get; private set; }

		public abstract string Name { get; }

		#endregion

		#region Methods

		public abstract void Render(DrawingContext dc, Rect bounds);

		public bool Equals(Tile other)
		{
			return (other != null) && (other.Id == Id);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Tile);
		}

		public override int GetHashCode()
		{
			return Id;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", Name, Id);
		}

		#endregion
	}

}
