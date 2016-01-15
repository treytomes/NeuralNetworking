using MazeGenerator.Math;
using MazeGenerator.Tiles;
using System;

namespace MazeGenerator.WorldGen.BSP
{
	public class Room
	{
		#region Fields

		private Random _random;

		#endregion

		#region Constructors

		public Room(Random random, RectI bounds)
		{
			_random = random;

			HasNorthDoor = false;
			HasSouthDoor = false;
			HasEastDoor = false;
			HasWestDoor = false;
			Bounds = bounds;
		}

		#endregion

		#region Properties

		public bool HasNorthDoor { get; set; }

		public bool HasSouthDoor { get; set; }

		public bool HasEastDoor { get; set; }

		public bool HasWestDoor { get; set; }

		public RectI Bounds { get; private set; }

		/// <summary>
		/// Choose a random point within this room's bounds, excluding the walls.
		/// </summary>
		public Vector2I RandomPoint
		{
			get
			{
				return new Vector2I()
				{
					X = _random.Next(Bounds.Left + 1, Bounds.Right),
					Y = _random.Next(Bounds.Top + 1, Bounds.Bottom)
				};
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Is this given point on this room's boundary?
		/// </summary>
		/// <remarks>
		/// The room will generate walls on it's boundary,
		/// so if this returns true it may be a good place to put a door.
		/// </remarks>
		public bool IsPointOnBounds(Vector2I point)
		{
			return
				(point.X == Bounds.Left) ||
				(point.X == Bounds.Right) ||
				(point.Y == Bounds.Top) ||
				(point.Y == Bounds.Bottom);
		}

		/// <summary>
		/// Is this given point in one of the room's corners?
		/// </summary>
		/// <remarks>
		/// A corner cannot be walked to, so it is a bad place to put a door.
		/// </remarks>
		public bool IsPointInCorner(Vector2I point)
		{
			return
				(point == Bounds.TopLeft) ||
				(point == Bounds.TopRight) ||
				(point == Bounds.BottomLeft) ||
				(point == Bounds.BottomRight);
		}

		public void Render(TileMap map)
		{
			map.Fill(Bounds.Inflate(-1), TileRegistry.Floor);

			map.DrawVerticalLine(Bounds.Top, Bounds.Bottom, Bounds.Left, TileRegistry.Wall);
			map.DrawVerticalLine(Bounds.Top, Bounds.Bottom, Bounds.Right, TileRegistry.Wall);

			map.DrawHorizontalLine(Bounds.Top, Bounds.Left, Bounds.Right, TileRegistry.Wall);
			map.DrawHorizontalLine(Bounds.Bottom, Bounds.Left, Bounds.Right, TileRegistry.Wall);
		}

		#endregion
	}
}