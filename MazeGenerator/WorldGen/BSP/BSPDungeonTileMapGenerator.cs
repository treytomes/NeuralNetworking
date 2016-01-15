using MazeGenerator.Tiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace MazeGenerator.WorldGen.BSP
{
	public class BSPDungeonTileMapGenerator : ITileMapGenerator
	{
		#region Fields

		private int _rows;
		private int _columns;
		private RoomGenerator _roomGenerator;

		#endregion

		#region Constructors

		public BSPDungeonTileMapGenerator(int rows, int columns, RoomGenerator roomGenerator)
		{
			_rows = rows;
			_columns = columns;
			_roomGenerator = roomGenerator;
		}

		public BSPDungeonTileMapGenerator(int rows, int columns)
			: this(rows, columns, new RoomGenerator(5, 6, 12, 6, 12))
		{
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
			tileMap.Fill(TileRegistry.Wall);
			
			_roomGenerator.Reset();

			var dungeonArea = new HorizontalArea(_roomGenerator, random, 0, 0, _columns, _rows);
			RenderRooms(tileMap, dungeonArea, logStream);
			RenderCorridors(tileMap, dungeonArea, logStream);

			DecorateWithDoors(tileMap, dungeonArea, logStream);

			return tileMap;
		}

		private void RenderRooms(TileMap map, Area area, TextWriter logStream)
		{
			if (area.Room != null)
			{
				area.Room.Render(map);
			}

			if (area.SubArea1 != null)
			{
				RenderRooms(map, area.SubArea1, logStream);
			}
			if (area.SubArea2 != null)
			{
				RenderRooms(map, area.SubArea2, logStream);
			}
		}

		private void RenderCorridors(TileMap map, Area area, TextWriter logStream)
		{
			if (area.SubArea1 != null)
			{
				RenderCorridors(map, area.SubArea1, logStream);
			}
			if (area.SubArea2 != null)
			{
				RenderCorridors(map, area.SubArea2, logStream);
			}

			if ((area.SubArea1 != null) && (area.SubArea2 != null))
			{
				var room1 = area.SubArea1.GetConnectableRoom();
				var room2 = area.SubArea2.GetConnectableRoom();
				ConnectRooms(map, room1, room2, logStream);
			}
		}

		private void ConnectRooms(TileMap map, Room room1, Room room2, TextWriter logStream)
		{
			if ((room1 != null) && (room2 != null))
			{
				var start = room1.RandomPoint;
				var end = room2.RandomPoint;

				var current = start;
				while (current != end)
				{
					map[current] = TileRegistry.Floor;

					if (current.X < end.X)
					{
						current.X++;
					}
					else if (current.X > end.X)
					{
						current.X--;
					}
					else if (current.Y < end.Y)
					{
						current.Y++;
					}
					else if (current.Y > end.Y)
					{
						current.Y--;
					}
				}
			}
		}

		private void DecorateWithDoors(TileMap map, Area area, TextWriter logStream)
		{
			// TODO: This is causing rooms to be disconnected in some cases (when rooms are adjacent).

			foreach (var room in CollectRooms(area))
			{
				// Check the room bounds places to put doors, ignoring the corners.
				// This will also make sure that doors aren't placed directly next to eachother.

				for (var row = room.Bounds.Top + 1; row <= room.Bounds.Bottom - 1; row++)
				{
					if (!room.HasWestDoor)
					{
						if (map[row, room.Bounds.Left - 1] == TileRegistry.Floor)
						{
							if ((map[row - 1, room.Bounds.Left] == TileRegistry.Wall) &&
								(map[row + 1, room.Bounds.Left] == TileRegistry.Wall))
							{
								map[row, room.Bounds.Left] = TileRegistry.Door;
								room.HasWestDoor = true;
							}
						}
					}
					if (!room.HasEastDoor)
					{
						if (map[row, room.Bounds.Right + 1] == TileRegistry.Floor)
						{
							if ((map[row - 1, room.Bounds.Right] == TileRegistry.Wall) &&
								(map[row + 1, room.Bounds.Right] == TileRegistry.Wall))
							{
								map[row, room.Bounds.Right] = TileRegistry.Door;
								room.HasEastDoor = true;
							}
						}
					}
				}

				for (var column = room.Bounds.Left + 1; column <= room.Bounds.Right - 1; column++)
				{
					if (!room.HasNorthDoor)
					{
						if (map[room.Bounds.Top - 1, column] == TileRegistry.Floor)
						{
							if ((map[room.Bounds.Top, column - 1] == TileRegistry.Wall) &&
								(map[room.Bounds.Top, column + 1] == TileRegistry.Wall))
							{
								map[room.Bounds.Top, column] = TileRegistry.Door;
								room.HasNorthDoor = true;
							}
						}
					}
					if (!room.HasSouthDoor)
					{
						if (map[room.Bounds.Bottom + 1, column] == TileRegistry.Floor)
						{
							if ((map[room.Bounds.Bottom, column - 1] == TileRegistry.Wall) &&
								(map[room.Bounds.Bottom, column + 1] == TileRegistry.Wall))
							{
								map[room.Bounds.Bottom, column] = TileRegistry.Door;
								room.HasSouthDoor = true;
							}
						}
					}
				}
			}
		}

		private IEnumerable<Room> CollectRooms(Area area)
		{
			var rooms = new List<Room>();

			if (area.Room != null)
			{
				yield return area.Room;
			}

			if (area.SubArea1 != null)
			{
				foreach (var room in CollectRooms(area.SubArea1))
				{
					yield return room;
				}
			}
			if (area.SubArea2 != null)
			{
				foreach (var room in CollectRooms(area.SubArea2))
				{
					yield return room;
				}
			}

			yield break;
		}

		#endregion
	}
}