using System;
using System.Drawing;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Event argument for when an object was moved.
	/// </summary>
	public class ObjectMovedEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the moved object.
		/// </summary>
		public CanvasObject Object { get; }

		/// <summary>
		/// Returns by how much the object's position changed.
		/// </summary>
		public PointF Delta { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="deltaPos"></param>
		public ObjectMovedEventArgs(CanvasObject obj, PointF deltaPos)
		{
			this.Object = obj;
			this.Delta = deltaPos;
		}
	}
}
