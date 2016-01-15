using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MazeGenerator.WorldGen;
using MazeGenerator.WorldGen.BSP;
using System.Windows.Input;

namespace MazeGenerator.ViewModel
{
    public class BSPViewModel : ViewModelBase
	{
		#region Fields

		private BindableStringWriter _logWriter;
		private int _rows;
		private int _columns;
		private RoomGenerator _roomGenerator;

		#endregion

		#region Constructors

		/// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
		public BSPViewModel()
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
			_roomGenerator = new RoomGenerator(5, 6, 12, 6, 12);
			
			Regenerate();

			RegenerateCommand = new RelayCommand(Regenerate);
		}

		#endregion

		#region Properties

		public string Name
		{
			get
			{
				return "BSP Generator";
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

		public int MaxRooms
		{
			get
			{
				return _roomGenerator.MaxRooms;
			}
			set
			{
				if (_roomGenerator.MaxRooms != value)
				{
					_roomGenerator.MaxRooms = value;
					RaisePropertyChanged(() => _roomGenerator.MaxRooms);
				}
			}
		}

		public ICommand RegenerateCommand { get; private set; }

		#endregion

		#region Methods

		private void Regenerate()
		{
			TileMap = new BSPDungeonTileMapGenerator(Rows, Columns, _roomGenerator).Generate(_logWriter);
			RaisePropertyChanged(() => TileMap);
		}

		#endregion
	}
}