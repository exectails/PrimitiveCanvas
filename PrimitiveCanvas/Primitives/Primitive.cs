using PrimitiveCanvas.Objects;
using System.Drawing;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// Represents a primitive shape to be drawn on the canvas.
	/// </summary>
	public abstract class Primitive
	{
		/// <summary>
		/// Gets or sets the object this primitive is part of.
		/// </summary>
		internal CanvasObject Object { get; set; }

		/// <summary>
		/// Gets or sets the primitive's center position.
		/// </summary>
		public abstract PointF Position { get; set; }

		/// <summary>
		/// Returns true if the given position is inside this primitive.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public abstract bool IsInside(PointF pos);

		/// <summary>
		/// Draws the primitive.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public abstract void Draw(Graphics g, float scale, int invertHeight);

		/// <summary>
		/// Moves the primitive by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public abstract void MoveBy(float x, float y);

		/// <summary>
		/// Rotates this primitive by the given amount of radians.
		/// </summary>
		/// <param name="radians"></param>
		public virtual void Rotate(double radians)
		{
			this.Rotate(this.Position, radians);
		}

		/// <summary>
		/// Rotates this primitive by the given amount of radians.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public abstract void Rotate(PointF pivot, double radians);
	}
}
