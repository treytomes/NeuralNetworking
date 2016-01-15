using System.IO;

namespace MazeGenerator.WorldGen
{
	public interface ITileMapGenerator
	{
		TileMap Generate();
		TileMap Generate(TextWriter logStream);
		TileMap Generate(int seed, TextWriter logStream);
	}
}