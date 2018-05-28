using System;
using System.Drawing;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Event argument for when an object was selected.
	/// </summary>
	public class ObjectSelectedEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the selected object.
		/// </summary>
		public CanvasObject Object { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="obj"></param>
		public ObjectSelectedEventArgs(CanvasObject obj)
		{
			this.Object = obj;
		}
	}
}
