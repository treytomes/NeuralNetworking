namespace MazeGenerator.Tiles
{
	public static class TileRegistry
	{
		public static readonly Tile Floor = new FloorTile(0);
		public static readonly Tile Wall = new WallTile(1);
		public static readonly Tile Door = new DoorTile(2);
		public static readonly Tile Chest = new ChestTile(3);
	}
}
