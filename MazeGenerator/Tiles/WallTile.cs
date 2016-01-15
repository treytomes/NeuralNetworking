using System.Windows;
using System.Windows.Media;

namespace MazeGenerator.Tiles
{
	public class WallTile : Tile
	{
		#region Fields

		private Pen _outlinePen;

		#endregion

		#region Constructors

		public WallTile(int id)
			: base(id)
		{
			_outlinePen = new Pen(Brushes.Black, 1.0);
		}

		#endregion

		#region Properties

		public override string Name
		{
			get
			{
				return "Wall";
			}
		}

		#endregion

		#region Methods

		public override void Render(DrawingContext dc, Rect bounds)
		{
			dc.DrawRectangle(Brushes.Blue, _outlinePen, bounds);
			bounds.Inflate(-(bounds.Width / 4), -(bounds.Height / 4));
			dc.DrawRectangle(null, _outlinePen, bounds);
		}

		#endregion
	}
}
