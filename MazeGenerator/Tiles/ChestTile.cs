using System.Windows;
using System.Windows.Media;

namespace MazeGenerator.Tiles
{
	public class ChestTile : Tile
	{
		#region Fields

		private Pen _outlinePen;

		#endregion

		#region Constructors

		public ChestTile(int id)
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
				return "Chest";
			}
		}

		#endregion

		#region Methods

		public override void Render(DrawingContext dc, Rect bounds)
		{
			var hingeHeight = bounds.Height / 3;

			dc.DrawRectangle(Brushes.RosyBrown, _outlinePen, new Rect(bounds.X, bounds.Y, bounds.Width, hingeHeight));
			dc.DrawRectangle(Brushes.RosyBrown, _outlinePen, new Rect(bounds.X, bounds.Y + hingeHeight, bounds.Width, bounds.Height - hingeHeight));
		}

		#endregion
	}
}
