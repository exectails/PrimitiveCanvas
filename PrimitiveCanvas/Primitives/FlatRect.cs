using System.Drawing;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// A rectangle filled with a color.
	/// </summary>
	public class FlatRect : Primitive
	{
		/// <summary>
		/// Gets or sets the rect's position.
		/// </summary>
		public override PointF Position
		{
			get { return this.Rectangle.Location; }
			set
			{
				var rectangle = this.Rectangle;
				rectangle.Location = value;
				this.Rectangle = rectangle;
			}
		}

		/// <summary>
		/// Gets or sets the rect's color.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Gets or sets the rect's area.
		/// </summary>
		public RectangleF Rectangle { get; set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public FlatRect(RectangleF rectangle, Color color)
		{
			this.Rectangle = rectangle;
			this.Color = color;
		}

		/// <summary>
		/// Draws rect to graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public override void Draw(Graphics g, float scale, int invertHeight)
		{
			var brush = new SolidBrush(this.Color);

			var rect = this.Rectangle;

			if (invertHeight > 0)
				rect.Y = (invertHeight - rect.Y - rect.Height);

			rect.X /= scale;
			rect.Y /= scale;
			rect.Width /= scale;
			rect.Height /= scale;

			g.FillRectangle(brush, rect);

			if (this.Object.Selected)
			{
				var pen = this.Object.GetOutlinePen();
				g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
		}

		/// <summary>
		/// Returns true if given position is within the rect.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public override bool IsInside(PointF pos)
		{
			var rect = this.Rectangle;
			return !(pos.X < rect.X || pos.X > rect.X + rect.Width || pos.Y < rect.Y || pos.Y > rect.Y + rect.Height);
		}

		/// <summary>
		/// Moves rect by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void MoveBy(float x, float y)
		{
			this.Position += new SizeF(x, y);
		}

		/// <summary>
		/// Does nothing for rect.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public override void Rotate(PointF pivot, double radians)
		{
		}
	}
}
