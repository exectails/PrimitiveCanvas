using System.Drawing;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// A string to be drawn on a canvas.
	/// </summary>
	public class TextString : Primitive
	{
		private SizeF _size;

		/// <summary>
		/// Gets or sets the text's position.
		/// </summary>
		public override PointF Position { get; set; }

		/// <summary>
		/// Gets or sets the font the text is drawn with.
		/// </summary>
		public Font Font { get; set; }

		/// <summary>
		/// Gets or sets the string's value.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the text's alignment.
		/// </summary>
		public StringFormat StringFormat { get; set; } = new StringFormat();

		/// <summary>
		/// Creates new instance with Arial as its font.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="position"></param>
		public TextString(PointF position, string text)
			: this(position, new Font("Arial", 10), text)
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="font"></param>
		/// <param name="text"></param>
		public TextString(PointF position, Font font, string text)
		{
			this.Position = position;
			this.Font = font;
			this.Text = text;
		}

		/// <summary>
		/// Draws string to graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public override void Draw(Graphics g, float scale, int invertHeight)
		{
			var pos = this.Position;
			var brush = this.Object.GetForegroundBrush();

			_size = g.MeasureString(this.Text, this.Font);

			if (invertHeight > 0)
			{
				pos.Y = (invertHeight - pos.Y);

				switch (this.StringFormat.LineAlignment)
				{
					case StringAlignment.Near: pos.Y -= _size.Height; break;
					case StringAlignment.Center: break;
					case StringAlignment.Far: pos.Y += _size.Height; break;
				}
			}

			var container = g.BeginContainer();
			g.ScaleTransform(1 / scale, 1 / scale);
			g.DrawString(this.Text, this.Font, brush, pos, this.StringFormat);
			g.EndContainer(container);
		}

		/// <summary>
		/// Returns true if the given position is within the text's bounds.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public override bool IsInside(PointF position)
		{
			var pos = this.Position;

			if (this.StringFormat.Alignment == StringAlignment.Center)
				pos.X -= (_size.Width / 2);
			else if (this.StringFormat.Alignment == StringAlignment.Far)
				pos.X -= _size.Width;

			if (this.StringFormat.LineAlignment == StringAlignment.Center)
				pos.Y -= (_size.Height / 2);
			else if (this.StringFormat.LineAlignment == StringAlignment.Far)
				pos.Y -= _size.Height;

			return !(position.X < pos.X || position.Y < pos.Y || position.X > pos.X + _size.Width || position.Y > pos.Y + _size.Height);
		}

		/// <summary>
		/// Moves the text by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void MoveBy(float x, float y)
		{
			this.Position += new SizeF(x, y);
		}

		/// <summary>
		/// Does nothing for a string.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public override void Rotate(PointF pivot, double radians)
		{
		}
	}
}
