using MazeGenerator.WorldGen;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MazeGenerator
{
	public class TileMapCanvas : Canvas
	{
		#region Constants

		private const int TILE_SCALE = 16;

		#endregion

		#region Constructors

		public TileMapCanvas()
		{
		}

		#endregion

		#region Properties

		public static readonly DependencyProperty TileMapProperty =
			DependencyProperty.Register("TileMap", typeof(TileMap), typeof(TileMapCanvas), new PropertyMetadata(OnTileMapChanged));

		public TileMap TileMap
		{
			get
			{
				return (TileMap)GetValue(TileMapProperty);
			}
			set
			{
				SetValue(TileMapProperty, value);
			}
		}

		#endregion

		#region Methods

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

			if (TileMap != null)
			{
				for (var row = 0; row < TileMap.Rows; row++)
				{
					for (var column = 0; column < TileMap.Columns; column++)
					{
						var tile = TileMap[row, column];
						if (tile != null)
						{
							var tileBounds = new Rect(column * TILE_SCALE, row * TILE_SCALE, TILE_SCALE, TILE_SCALE);
							tile.Render(dc, tileBounds);
						}
					}
				}
			}
		}

		private static void OnTileMapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var canvas = sender as TileMapCanvas;
			canvas.InvalidateVisual();

			canvas.Width = canvas.TileMap.Columns * TILE_SCALE;
			canvas.Height = canvas.TileMap.Rows * TILE_SCALE;
		}

		#endregion
	}
}
