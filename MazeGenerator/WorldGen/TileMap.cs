using MazeGenerator.Math;
using MazeGenerator.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.WorldGen
{
	public class TileMap
	{
		#region Fields

		private Tile[,] _data;

		#endregion

		#region Constructors

		public TileMap(int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
			_data = new Tile[rows, columns];
		}

		#endregion

		#region Properties

		public int Rows { get; private set; }

		public int Columns { get; private set; }

		public Tile this[int row, int column]
		{
			get
			{
				if (!MathHelper.IsInRange(row, 0, Rows - 1) ||
					!MathHelper.IsInRange(column, 0, Columns - 1))
				{
					return null;
				}
				else
				{
					return _data[row, column];
				}
			}
			set
			{
				_data[row, column] = value;
			}
		}

		public Tile this[Vector2I vector]
		{
			get
			{
				return this[vector.Row, vector.Column];
			}
			set
			{
				this[vector.Row, vector.Column] = value;
			}
		}

		#endregion

		#region Methods

		public void Fill(Tile tile)
		{
			Fill(new RectI(0, 0, Columns, Rows), tile);
		}

		public void Fill(RectI bounds, Tile tile)
		{
			for (var row = bounds.Top; row <= bounds.Bottom; row++)
			{
				for (var column = bounds.Left; column <= bounds.Right; column++)
				{
					this[row, column] = tile;
				}
			}
		}

		public void DrawVerticalLine(int topRow, int bottomRow, int column, Tile tile)
		{
			for (var row = topRow; row <= bottomRow; row++)
			{
				this[row, column] = tile;
			}
		}

		public void DrawHorizontalLine(int row, int leftColumn, int rightColumn, Tile tile)
		{
			for (var column = leftColumn; column <= rightColumn; column++)
			{
				this[row, column] = tile;
			}
		}

		#endregion
	}
}
