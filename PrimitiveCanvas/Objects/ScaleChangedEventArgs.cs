using System;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Event argument for when the canvas' scale changed.
	/// </summary>
	public class ScaleChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the scale before it changed.
		/// </summary>
		public float ScaleBefore { get; }

		/// <summary>
		/// Returns the current scale.
		/// </summary>
		public float Scale { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="scaleBefore"></param>
		/// <param name="scale"></param>
		public ScaleChangedEventArgs(float scaleBefore, float scale)
		{
			this.ScaleBefore = scaleBefore;
			this.Scale = scale;
		}
	}
}
