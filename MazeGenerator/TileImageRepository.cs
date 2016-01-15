using MazeGenerator.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
	public class TileImageRepository
	{
		public TileImageRepository(Bitmap sourceImage, int tileWidth, int tileHeight)
		{
			var bmp = Resources._8x_dungeon_tiles;
		}

		public TileImageRepository()
			: this(Resources._8x_dungeon_tiles, 8, 8)
		{
		}
	}
}
