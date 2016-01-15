using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MazeGenerator.WorldGen;
using MazeGenerator.WorldGen.Dugout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MazeGenerator.ViewModel
{
	public class DugoutViewModel : ViewModelBase
	{
		#region Fields

		private BindableStringWriter _logWriter;
		private int _rows;
		private int _columns;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public DugoutViewModel()
		{
			if (IsInDesignMode)
			{
			}
			else
			{
			}

			_logWriter = new BindableStringWriter();
			_logWriter.Flushed += (sender, e) => RaisePropertyChanged(() => Log);

			_rows = 32;
			_columns = 32;

			Regenerate();

			RegenerateCommand = new RelayCommand(Regenerate);
		}

		#endregion

		#region Properties

		public string Name
		{
			get
			{
				return "Dugout Generator";
			}
		}

		public string Log
		{
			get
			{
				return _logWriter.ToString();
			}
		}

		public TileMap TileMap { get; private set; }

		public int Rows
		{
			get
			{
				return _rows;
			}
			set
			{
				if (_rows != value)
				{
					_rows = value;
					RaisePropertyChanged(() => Rows);
				}
			}
		}

		public int Columns
		{
			get
			{
				return _columns;
			}
			set
			{
				if (_columns != value)
				{
					_columns = value;
					RaisePropertyChanged(() => Columns);
				}
			}
		}

		public ICommand RegenerateCommand { get; private set; }

		#endregion

		#region Methods

		private void Regenerate()
		{
			TileMap = new DugoutDungeonTileMapGenerator(Rows, Columns).Generate(_logWriter);
			RaisePropertyChanged(() => TileMap);
		}

		#endregion
	}
}
