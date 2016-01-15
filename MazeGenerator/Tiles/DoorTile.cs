using System.Windows;
using System.Windows.Media;

namespace MazeGenerator.Tiles
{
	public class DoorTile : Tile
	{
		#region Fields

		private Pen _outlinePen;

		#endregion

		#region Constructors

		public DoorTile(int id)
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
				return "Door";
			}
		}

		#endregion

		#region Methods

		public override void Render(DrawingContext dc, Rect bounds)
		{
			dc.DrawRoundedRectangle(Brushes.Brown, _outlinePen, bounds, bounds.Width / 4, bounds.Height / 4);
		}

		#endregion
	}
}
