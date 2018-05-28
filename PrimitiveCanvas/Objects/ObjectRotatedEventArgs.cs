using System;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Event argument for when an object was rotated.
	/// </summary>
	public class ObjectRotatedEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the moved object.
		/// </summary>
		public CanvasObject Object { get; }

		/// <summary>
		/// Returns the number of degree the object was rotated by.
		/// </summary>
		public double Radians { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="radians"></param>
		public ObjectRotatedEventArgs(CanvasObject obj, double radians)
		{
			this.Object = obj;
			this.Radians = radians;
		}
	}
}
