using MazeGenerator.Math;
using MazeGenerator.Tiles;
using System;
using System.IO;

namespace MazeGenerator.WorldGen.Labyrinth
{
	public class LabyrinthTileMapGenerator : ITileMapGenerator
	{
		#region Fields

		private int _rows;
		private int _columns;

		#endregion

		#region Constructors

		public LabyrinthTileMapGenerator(int rows, int columns)
		{
			_rows = rows;
			_columns = columns;
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
			var random = new Random(seed);

			var tileMap = new TileMap(_rows, _columns);

			logStream.Write("Generating dungeon...");
			var generator = CreateDungeonGenerator(tileMap, random);
			var dungeon = generator.Generate();
			logStream.WriteLine(" done!");

			tileMap = ConvertToTileMap(tileMap, dungeon, logStream);

			return tileMap;
		}

		private TileMap ConvertToTileMap(TileMap chunk, LabyrinthDungeon dungeon, TextWriter logStream)
		{
			logStream.WriteLine("Converting dungeon into chunk...");

			logStream.Write("Generating a rocky chunk...");
			chunk = GenerateRockyChunk(chunk);
			logStream.WriteLine(" done!");

			logStream.Write("Excavating rooms...");
			chunk = ExcavateRooms(chunk, dungeon, logStream);
			logStream.WriteLine(" done!");

			logStream.Write("Excavating corridors...");
			chunk = ExcavateCorridors(chunk, dungeon);
			logStream.WriteLine(" done!");

			return chunk;
		}

		private TileMap GenerateRockyChunk(TileMap chunk)
		{
			// Initialize the tile array to rock.
			for (var row = 0; row < chunk.Rows; row++)
			{
				for (var column = 0; column < chunk.Columns; column++)
				{
					chunk[row, column] = TileRegistry.Wall;
				}
			}

			return chunk;
		}

		private TileMap ExcavateRooms(TileMap chunk, LabyrinthDungeon dungeon, TextWriter logStream)
		{
			logStream.WriteLine("Excavating room...");
			logStream.WriteLine(dungeon.Rooms.Count);
			// Fill tiles with corridor values for each room in dungeon.
			foreach (var room in dungeon.Rooms)
			{
				// Get the room min and max location in tile coordinates.
				var right = room.Bounds.X + room.Bounds.Width - 1;
				var bottom = room.Bounds.Y + room.Bounds.Height - 1;
				var minPoint = new Vector2I(room.Bounds.X * 2 + 1, room.Bounds.Y * 2 + 1);
				var maxPoint = new Vector2I(right * 2, bottom * 2);

				// Fill the room in tile space with an empty value.
				for (var row = minPoint.Y; row <= maxPoint.Y; row++)
				{
					for (var column = minPoint.X; column <= maxPoint.X; column++)
					{
						ExcavateChunkPoint(chunk, new Vector2I(column, row));
					}
				}
			}
			logStream.WriteLine("Room complete!");

			return chunk;
		}

		private TileMap ExcavateCorridors(TileMap chunk, LabyrinthDungeon dungeon)
		{
			// Loop for each corridor cell and expand it.
			foreach (var cellLocation in dungeon.CorridorCellLocations)
			{
				var tileLocation = new Vector2I(cellLocation.X * 2 + 1, cellLocation.Y * 2 + 1);
				ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y));

				if (dungeon[cellLocation].NorthSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y - 1));
				}
				else if (dungeon[cellLocation].NorthSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y - 1));
					chunk[tileLocation.Y - 1, tileLocation.X] = TileRegistry.Door;
				}

				if (dungeon[cellLocation].SouthSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y + 1));
				}
				else if (dungeon[cellLocation].SouthSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y + 1));
					chunk[tileLocation.Y + 1, tileLocation.X] = TileRegistry.Door;
				}

				if (dungeon[cellLocation].WestSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X - 1, tileLocation.Y));
				}
				else if (dungeon[cellLocation].WestSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X - 1, tileLocation.Y));
					chunk[tileLocation.Y, tileLocation.X - 1] = TileRegistry.Door;
				}

				if (dungeon[cellLocation].EastSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X + 1, tileLocation.Y));
				}
				else if (dungeon[cellLocation].EastSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X + 1, tileLocation.Y));
					chunk[tileLocation.Y, tileLocation.X + 1] = TileRegistry.Door;
				}
			}

			return chunk;
		}

		private void ExcavateChunkPoint(TileMap chunk, Vector2I chunkPoint)
		{
			var tile = chunk[chunkPoint.Y, chunkPoint.X];
			if (tile != null)
			{
				chunk[chunkPoint.Y, chunkPoint.X] = TileRegistry.Floor;
			}
		}

		private bool IsDoorAdjacent(TileMap chunk, Vector2I chunkPoint)
		{
			var north = chunk[chunkPoint.Y - 1, chunkPoint.X];
			var south = chunk[chunkPoint.Y + 1, chunkPoint.X];
			var east = chunk[chunkPoint.Y, chunkPoint.X + 1];
			var west = chunk[chunkPoint.Y, chunkPoint.X - 1];

			return
					((north != null) && (north == TileRegistry.Door)) ||
					((south != null) && (south == TileRegistry.Door)) ||
					((east != null) && (east == TileRegistry.Door)) ||
					((west != null) && (west == TileRegistry.Door));
		}

		private LabyrinthGenerator CreateDungeonGenerator(TileMap chunk, Random random)
		{
			return new LabyrinthGenerator(random, CreateRoomGenerator(random))
			{
				Rows = (chunk.Rows - 1) / 2,
				Columns = (chunk.Columns - 1) / 2,
				ChangeDirectionModifier = random.NextDouble(),
				SparsenessFactor = random.NextDouble(),
				DeadEndRemovalModifier = random.NextDouble()
			};
		}

		private RoomGenerator CreateRoomGenerator(Random random)
		{
			return new RoomGenerator(random)
			{
				NumRooms = 5,
				MinRoomRows = 2,
				MaxRoomRows = 5,
				MinRoomColumns = 2,
				MaxRoomColumns = 5
			};
		}

		#endregion
	}
}
