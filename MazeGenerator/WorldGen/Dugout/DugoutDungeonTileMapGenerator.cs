﻿using MazeGenerator.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MazeGenerator.Tiles;

namespace MazeGenerator.WorldGen.Dugout
{
	public class DugoutDungeonTileMapGenerator : ITileMapGenerator
	{
		#region Constants

		// Misc. messages to print.
		private const string MSG_COLUMNS = "X size of dungeon: \t";
		private const string MSG_ROWS = "Y size of dungeon: \t";
		private const string MSG_MAX_OBJECTS = "max # of objects: \t";
		private const string MSG_NUM_OBJECTS = "# of objects made: \t";

		// Max size of the map.
		private const int MAX_COLUMNS = 80;
		private const int MAX_ROWS = 25;

		private const int MIN_ROOM_COLUMNS = 6;
		private const int MIN_ROOM_ROWS = 6;

		private const int MAX_ROOM_COLUMNS = 12;
		private const int MAX_ROOM_ROWS = 12;

		private const int NUM_FEATURES = 10;
		private const int NUM_SPRINKLES = 20;

		#endregion

		#region Fields

		// size of the map
		private int _columns;
		private int _rows;

		// number of "objects" to generate on the map
		int _numberOfFeatures;

		// define the %chance to generate either a room or a corridor on the map
		// BTW, rooms are 1st priority so actually it's enough to just define the chance
		// of generating a room
		const int ChanceRoom = 75;

		// our map
		private Tile[,] _dungeonMap;

		private Random _random;

		private TextWriter _logStream;

		#endregion

		#region Constructors

		public DugoutDungeonTileMapGenerator(int rows, int columns)
		{
			// Adjust the size of the map, if it's smaller or bigger than the limits.
			_rows = MathHelper.Clamp(rows, 3, MAX_ROWS);
			_columns = MathHelper.Clamp(columns, 3, MAX_COLUMNS);
		}

		#endregion

		#region Properties

		public int Corridors
		{
			get;
			private set;
		}

		#endregion

		#region Methods

		public TileMap Generate()
		{
			return Generate(Console.Out);
		}

		public TileMap Generate(TextWriter logStream)
		{
			return Generate(new Random().Next(), logStream);
		}

		public TileMap Generate(int seed, TextWriter logStream)
		{
			_random = new Random(seed);
			_logStream = logStream;

			CreateDungeon(NUM_FEATURES);

			var tileMap = new TileMap(_rows, _columns);
			for (var row = 0; row < _rows; row++)
			{
				for (var column = 0; column < _columns; column++)
				{
					// TODO: Implement these other tile types in TileMap.
					switch (_dungeonMap[row, column])
					{
						case Tile.Chest:
							tileMap[row, column] = TileRegistry.Chest;
							break;
						case Tile.Corridor:
						case Tile.DirtFloor:
						case Tile.Downstairs:
						case Tile.Upstairs:
							tileMap[row, column] = TileRegistry.Floor;
							break;
						case Tile.DirtWall:
						case Tile.StoneWall:
						case Tile.Unused:
							tileMap[row, column] = TileRegistry.Wall;
							break;
						case Tile.Door:
							tileMap[row, column] = TileRegistry.Door;
							break;
					}
				}
			}

			return tileMap;
		}

		private static bool IsWall(int x, int y, int xlen, int ylen, int xt, int yt, Direction d)
		{
			Func<int, int, int> a = GetFeatureLowerBound;

			Func<int, int, int> b = IsFeatureWallBound;

			if (d == Direction.North)
			{
				return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y - ylen + 1;
			}
			else if (d == Direction.East)
			{
				return xt == x || xt == x + xlen - 1 || yt == a(y, ylen) || yt == b(y, ylen);
			}
			else if (d == Direction.South)
			{
				return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y + ylen - 1;
			}
			else if (d == Direction.West)
			{
				return xt == x || xt == x - xlen + 1 || yt == a(y, ylen) || yt == b(y, ylen);
			}

			throw new InvalidOperationException();
		}

		private static int GetFeatureLowerBound(int c, int len)
		{
			return c - len / 2;
		}

		private static int IsFeatureWallBound(int c, int len)
		{
			return c + (len - 1) / 2;
		}

		private static int GetFeatureUpperBound(int c, int len)
		{
			return c + (len + 1) / 2;
		}

		private static IEnumerable<Vector2I> GetRoomPoints(int x, int y, int xlen, int ylen, Direction d)
		{
			// north and south share the same x strategy
			// east and west share the same y strategy
			Func<int, int, int> a = GetFeatureLowerBound;
			Func<int, int, int> b = GetFeatureUpperBound;

			if (d == Direction.North)
			{
				for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt > y - ylen; yt--) yield return new Vector2I { X = xt, Y = yt };
			}
			else if (d == Direction.East)
			{
				for (var xt = x; xt < x + xlen; xt++) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new Vector2I { X = xt, Y = yt };
			}
			else if (d == Direction.South)
			{
				for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt < y + ylen; yt++) yield return new Vector2I { X = xt, Y = yt };
			}
			else if (d == Direction.West)
			{
				for (var xt = x; xt > x - xlen; xt--) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new Vector2I { X = xt, Y = yt };
			}
		}

		private Tile GetCellType(int x, int y)
		{
			try
			{
				return _dungeonMap[y, x];
			}
			catch (IndexOutOfRangeException)
			{
				throw new ArgumentOutOfRangeException(string.Format("({0}, {1}) is out of range.", x, y), "x, y");
			}
		}

		private int GetRand(int min, int max)
		{
			return _random.Next(min, max);
		}

		private bool MakeCorridor(int x, int y, int length, Direction direction)
		{
			// define the dimensions of the corridor (er.. only the width and height..)
			int len = this.GetRand(2, length);
			const Tile Floor = Tile.Corridor;

			int xtemp;
			int ytemp = 0;

			if (direction == Direction.North)
			{
				// north
				// check if there's enough space for the corridor
				// start with checking it's not out of the boundaries
				if (x < 0 || x > this._columns) return false;
				xtemp = x;

				// same thing here, to make sure it's not out of the boundaries
				for (ytemp = y; ytemp > (y - len); ytemp--)
				{
					if (ytemp < 0 || ytemp > this._rows) return false; // oh boho, it was!
					if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
				}

				// if we're still here, let's start building
				Corridors++;
				for (ytemp = y; ytemp > (y - len); ytemp--)
				{
					this.SetCell(xtemp, ytemp, Floor);
				}
			}
			else if (direction == Direction.East)
			{
				// east
				if (y < 0 || y > this._rows) return false;
				ytemp = y;

				for (xtemp = x; xtemp < (x + len); xtemp++)
				{
					if (xtemp < 0 || xtemp > this._columns) return false;
					if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
				}

				Corridors++;
				for (xtemp = x; xtemp < (x + len); xtemp++)
				{
					this.SetCell(xtemp, ytemp, Floor);
				}
			}
			else if (direction == Direction.South)
			{
				// south
				if (x < 0 || x > this._columns) return false;
				xtemp = x;

				for (ytemp = y; ytemp < (y + len); ytemp++)
				{
					if (ytemp < 0 || ytemp > this._rows) return false;
					if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
				}

				Corridors++;
				for (ytemp = y; ytemp < (y + len); ytemp++)
				{
					this.SetCell(xtemp, ytemp, Floor);
				}
			}
			else if (direction == Direction.West)
			{
				// west
				if (ytemp < 0 || ytemp > this._rows) return false;
				ytemp = y;

				for (xtemp = x; xtemp > (x - len); xtemp--)
				{
					if (xtemp < 0 || xtemp > this._columns) return false;
					if (GetCellType(xtemp, ytemp) != Tile.Unused) return false;
				}

				Corridors++;
				for (xtemp = x; xtemp > (x - len); xtemp--)
				{
					this.SetCell(xtemp, ytemp, Floor);
				}
			}

			// woot, we're still here! let's tell the other guys we're done!!
			return true;
		}

		private IEnumerable<Tuple<Vector2I, Direction>> GetSurroundingPoints(Vector2I v)
		{
			var points = new[]
							 {
								 Tuple.Create(new Vector2I { X = v.X, Y = v.Y + 1 }, Direction.North),
								 Tuple.Create(new Vector2I { X = v.X - 1, Y = v.Y }, Direction.East),
								 Tuple.Create(new Vector2I { X = v.X , Y = v.Y-1 }, Direction.South),
								 Tuple.Create(new Vector2I { X = v.X +1, Y = v.Y  }, Direction.West),

							 };
			return points.Where(p => InBounds(p.Item1));
		}

		private IEnumerable<Tuple<Vector2I, Direction, Tile>> GetSurroundings(Vector2I v)
		{
			return
				this.GetSurroundingPoints(v)
					.Select(r => Tuple.Create(r.Item1, r.Item2, this.GetCellType(r.Item1.X, r.Item1.Y)));
		}

		private bool InBounds(int x, int y)
		{
			// TODO: Use MathHelper.IsInRange.
			return x > 0 && x < MAX_COLUMNS && y > 0 && y < MAX_ROWS;
		}

		private bool InBounds(Vector2I v)
		{
			return this.InBounds(v.X, v.Y);
		}

		private bool MakeRoom(int x, int y, int xlength, int ylength, Direction direction)
		{
			// define the dimensions of the room, it should be at least 4x4 tiles (2x2 for walking on, the rest is walls)
			int xlen = GetRand(MIN_ROOM_COLUMNS, xlength);
			int ylen = GetRand(MIN_ROOM_ROWS, ylength);

			// the tile type it's going to be filled with
			const Tile Floor = Tile.DirtFloor;

			const Tile Wall = Tile.DirtWall;
			// choose the way it's pointing at

			var points = GetRoomPoints(x, y, xlen, ylen, direction).ToArray();

			// Check if there's enough space left for it
			if (points.Any(s =>
				!MathHelper.IsInRange(s.Y, 0, _rows) ||
				!MathHelper.IsInRange(s.X, 0, _columns) ||
				GetCellType(s.X, s.Y) != Tile.Unused))
			{
				return false;
			}
			_logStream.WriteLine("Making room:int x={0}, int y={1}, int xlength={2}, int ylength={3}, int direction={4}",
				x, y, xlength, ylength, direction);

			foreach (var p in points)
			{
				SetCell(p.X, p.Y, IsWall(x, y, xlen, ylen, p.X, p.Y, direction) ? Wall : Floor);
			}

			// yay, all done
			return true;
		}

		private char GetCellTile(int x, int y)
		{
			switch (GetCellType(x, y))
			{
				case Tile.Unused:
					return ' ';
				case Tile.DirtWall:
					return '|';
				case Tile.DirtFloor:
					return '_';
				case Tile.StoneWall:
					return 'S';
				case Tile.Corridor:
					return '#';
				case Tile.Door:
					return 'D';
				case Tile.Upstairs:
					return '+';
				case Tile.Downstairs:
					return '-';
				case Tile.Chest:
					return 'C';
				default:
					throw new ArgumentOutOfRangeException("x,y");
			}
		}

		//used to print the map on the screen
		private void ShowDungeon()
		{
			for (int y = 0; y < this._rows; y++)
			{
				for (int x = 0; x < this._columns; x++)
				{
					Console.Write(GetCellTile(x, y));
				}

				if (this._columns <= MAX_COLUMNS) Console.WriteLine();
			}
		}

		private Direction RandomDirection()
		{
			int dir = this.GetRand(0, 4);
			switch (dir)
			{
				case 0:
					return Direction.North;
				case 1:
					return Direction.East;
				case 2:
					return Direction.South;
				case 3:
					return Direction.West;
				default:
					throw new InvalidOperationException();
			}
		}

		//and here's the one generating the whole map
		private bool CreateDungeon(int numberOfFeatures)
		{
			_numberOfFeatures = numberOfFeatures < 1 ? 10 : numberOfFeatures;

			_logStream.WriteLine(MSG_COLUMNS + _columns);
			_logStream.WriteLine(MSG_ROWS + _rows);
			_logStream.WriteLine(MSG_MAX_OBJECTS + _numberOfFeatures);

			// redefine the map var, so it's adjusted to our new map size
			_dungeonMap = new Tile[_rows, _columns];

			// start with making the "standard stuff" on the map
			Initialize();

			/*******************************************************************************
            And now the code of the random-map-generation-algorithm begins!
            *******************************************************************************/

			// start with making a room in the middle, which we can start building upon
			MakeRoom(_columns / 2, _rows / 2, MAX_ROOM_COLUMNS, MAX_ROOM_ROWS, RandomDirection()); // getrand saken f????r att slumpa fram riktning p?? rummet

			// keep count of the number of "objects" we've made
			int currentFeatures = 1; // +1 for the first room we just made

			// then we start the main loop
			for (int countingTries = 0; countingTries < 1000; countingTries++)
			{
				// check if we've reached our quota
				if (currentFeatures == this._numberOfFeatures)
				{
					break;
				}

				// start with a random wall
				int newx = 0;
				int xmod = 0;
				int newy = 0;
				int ymod = 0;
				Direction? validTile = null;

				// 1000 chances to find a suitable object (room or corridor)..
				for (int testing = 0; testing < 1000; testing++)
				{
					newx = this.GetRand(1, this._columns - 1);
					newy = this.GetRand(1, this._rows - 1);

					if (GetCellType(newx, newy) == Tile.DirtWall || GetCellType(newx, newy) == Tile.Corridor)
					{
						var surroundings = this.GetSurroundings(new Vector2I() { X = newx, Y = newy });

						// check if we can reach the place
						var canReach =
							surroundings.FirstOrDefault(s => s.Item3 == Tile.Corridor || s.Item3 == Tile.DirtFloor);
						if (canReach == null)
						{
							continue;
						}
						validTile = canReach.Item2;
						if (validTile.HasValue)
						{
							if (validTile.Value == Direction.North)
							{
								xmod = 0;
								ymod = -1;
							}
							else if (validTile.Value == Direction.East)
							{
								xmod = 1;
								ymod = 0;
							}
							else if (validTile.Value == Direction.South)
							{
								xmod = 0;
								ymod = 1;
							}
							else if (validTile.Value == Direction.West)
							{
								xmod = -1;
								ymod = 0;
							}
						}
						else
						{
							throw new InvalidOperationException();
						}


						// check that we haven't got another door nearby, so we won't get alot of openings besides
						// each other

						if (GetCellType(newx, newy + 1) == Tile.Door) // north
						{
							validTile = null;
						}
						else if (GetCellType(newx - 1, newy) == Tile.Door) // east
						{
							validTile = null;
						}
						else if (GetCellType(newx, newy - 1) == Tile.Door) // south
						{
							validTile = null;
						}
						else if (GetCellType(newx + 1, newy) == Tile.Door) // west
						{
							validTile = null;
						}


						// if we can, jump out of the loop and continue with the rest
						if (validTile.HasValue)
						{
							break;
						}
					}
				}

				if (validTile.HasValue)
				{
					// choose what to build now at our newly found place, and at what direction
					int feature = this.GetRand(0, 100);
					if (feature <= ChanceRoom)
					{ // a new room
						if (this.MakeRoom(newx + xmod, newy + ymod, MAX_ROOM_COLUMNS, MAX_ROOM_ROWS, validTile.Value))
						{
							currentFeatures++; // add to our quota

							// then we mark the wall opening with a door
							this.SetCell(newx, newy, Tile.Door);

							// clean up infront of the door so we can reach it
							this.SetCell(newx + xmod, newy + ymod, Tile.DirtFloor);
						}
					}
					else if (feature >= ChanceRoom)
					{ // new corridor
						if (this.MakeCorridor(newx + xmod, newy + ymod, 6, validTile.Value))
						{
							// same thing here, add to the quota and a door
							currentFeatures++;

							this.SetCell(newx, newy, Tile.Door);
						}
					}
				}
			}

			/*******************************************************************************
            All done with the building, let's finish this one off
            *******************************************************************************/
			AddSprinkles();

			// all done with the map generation, tell the user about it and finish
			Console.WriteLine(MSG_NUM_OBJECTS + currentFeatures);

			return true;
		}

		void Initialize()
		{
			for (int y = 0; y < this._rows; y++)
			{
				for (int x = 0; x < this._columns; x++)
				{
					// ie, making the borders of unwalkable walls
					if (y == 0 || y == this._rows - 1 || x == 0 || x == this._columns - 1)
					{
						this.SetCell(x, y, Tile.StoneWall);
					}
					else
					{                        // and fill the rest with dirt
						this.SetCell(x, y, Tile.Unused);
					}
				}
			}
		}

		// setting a tile's type
		private void SetCell(int x, int y, Tile celltype)
		{
			this._dungeonMap[y, x] = celltype;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void AddSprinkles()
		{
			// sprinkle out the bonusstuff (stairs, chests etc.) over the map
			int state = 0; // the state the loop is in, start with the stairs
			while (state != NUM_SPRINKLES)
			{
				for (int testing = 0; testing < 1000; testing++)
				{
					var newx = this.GetRand(1, this._columns - 1);
					int newy = this.GetRand(1, this._rows - 2);

					// Console.WriteLine("x: " + newx + "\ty: " + newy);
					int ways = 4; // from how many directions we can reach the random spot from

					// check if we can reach the spot
					if (GetCellType(newx, newy + 1) == Tile.DirtFloor || GetCellType(newx, newy + 1) == Tile.Corridor)
					{
						// north
						if (GetCellType(newx, newy + 1) != Tile.Door)
							ways--;
					}

					if (GetCellType(newx - 1, newy) == Tile.DirtFloor || GetCellType(newx - 1, newy) == Tile.Corridor)
					{
						// east
						if (GetCellType(newx - 1, newy) != Tile.Door)
							ways--;
					}

					if (GetCellType(newx, newy - 1) == Tile.DirtFloor || GetCellType(newx, newy - 1) == Tile.Corridor)
					{
						// south
						if (GetCellType(newx, newy - 1) != Tile.Door)
							ways--;
					}

					if (GetCellType(newx + 1, newy) == Tile.DirtFloor || GetCellType(newx + 1, newy) == Tile.Corridor)
					{
						// west
						if (GetCellType(newx + 1, newy) != Tile.Door)
							ways--;
					}

					if (state == 0)
					{
						if (ways == 0)
						{
							// we're in state 0, let's place a "upstairs" thing
							SetCell(newx, newy, Tile.Upstairs);
							state++;
							break;
						}
					}
					else if (state == 1)
					{
						if (ways == 0)
						{
							// state 1, place a "downstairs"
							SetCell(newx, newy, Tile.Downstairs);
							state++;
							break;
						}
					}
					else
					{
						if (ways == 0)
						{
							SetCell(newx, newy, Tile.Chest);
							state++;
							break;
						}
					}
				}
			}
		}

		#endregion
	}
}

