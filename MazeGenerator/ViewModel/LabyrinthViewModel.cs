using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MazeGenerator.WorldGen;
using MazeGenerator.WorldGen.Labyrinth;
using System.Windows.Input;

namespace MazeGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LabyrinthViewModel : ViewModelBase
	{
		#region Fields

		private BindableStringWriter _logWriter;
		private int _rows;
		private int _columns;
		private double _changeDirection;
		private double _sparseness;
		private double _deadEndRemoval;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public LabyrinthViewModel()
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
			_changeDirection = 0;
			_sparseness = 0;
			_deadEndRemoval = 0;

			Regenerate();

			RegenerateCommand = new RelayCommand(Regenerate);
		}

		#endregion

		#region Properties

		public string Name
		{
			get
			{
				return "Labyrinth Generator";
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

		public double ChangeDirection
		{
			get
			{
				return _changeDirection;
			}
			set
			{
				if (_changeDirection != value)
				{
					_changeDirection = value;
					RaisePropertyChanged(() => ChangeDirection);
				}
			}
		}

		public double Sparseness
		{
			get
			{
				return _sparseness;
			}
			set
			{
				if (_sparseness != value)
				{
					_sparseness = value;
					RaisePropertyChanged(() => Sparseness);
				}
			}
		}

		public double DeadEndRemoval
		{
			get
			{
				return _deadEndRemoval;
			}
			set
			{
				if (_deadEndRemoval != value)
				{
					_deadEndRemoval = value;
					RaisePropertyChanged(() => DeadEndRemoval);
				}
			}
		}

		public ICommand RegenerateCommand { get; private set; }

		#endregion

		#region Methods

		private void Regenerate()
		{
			// TODO: Finish implementing ChangeDirection, Sparseness, and DeadEndRemoval.
			// TODO: Why are doors sometimes places inside of rooms?
			// TODO: Why are corridors sometimes disconnected?

			TileMap = new LabyrinthTileMapGenerator(Rows, Columns).Generate(_logWriter);
			RaisePropertyChanged(() => TileMap);
		}

		#endregion
	}
}