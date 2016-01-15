using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
	/// <summary>
	/// Bind to the Flushed event to get updates every time the writer is updated.
	/// </summary>
	public class BindableStringWriter : StringWriter
	{
		public event EventHandler Flushed;

		public override void Flush()
		{
			base.Flush();
			if (Flushed != null)
			{
				Flushed(this, EventArgs.Empty);
			}
		}
	}
}
