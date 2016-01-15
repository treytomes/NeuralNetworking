using System.Windows;
using System.Windows.Media;

namespace MazeGenerator.Tiles
{
	public class FloorTile : Tile
	{
		#region Fields

		private Pen _outlinePen;

		#endregion

		#region Constructors

		public FloorTile(int id)
			: base(id)
		{
			_outlinePen = new Pen(Brushes.Goldenrod, 1.0);
		}

		#endregion

		#region Properties

		public override string Name
		{
			get
			{
				return "Floor";
			}
		}

		#endregion

		#region Methods

		public override void Render(DrawingContext dc, Rect bounds)
		{
			dc.DrawRectangle(Brushes.Goldenrod, _outlinePen, bounds);
		}

		#endregion
	}
}
