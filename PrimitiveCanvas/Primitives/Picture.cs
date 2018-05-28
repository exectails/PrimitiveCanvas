using System.Drawing;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// An image to be drawn on a canvas.
	/// </summary>
	public class Picture : Primitive
	{
		/// <summary>
		/// Gets or sets the picture's position.
		/// </summary>
		public override PointF Position
		{
			get { return this.DestRect.Location; }
			set
			{
				var destRect = this.DestRect;
				destRect.Location = value;
				this.DestRect = destRect;
			}
		}

		/// <summary>
		/// Gets or sets the image to draw.
		/// </summary>
		public Image Image { get; set; }

		/// <summary>
		/// Gets or sets the section of the image to draw.
		/// </summary>
		public RectangleF SrcRect { get; set; }

		/// <summary>
		/// Gets or sets the images drawn dimensions.
		/// </summary>
		public RectangleF DestRect { get; set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="image"></param>
		public Picture(Image image)
			: this(image, new RectangleF(0, 0, image.Width, image.Height))
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="srcRect"></param>
		public Picture(Image image, RectangleF srcRect)
			: this(image, srcRect, srcRect)
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="srcRect"></param>
		/// <param name="destRect"></param>
		public Picture(Image image, RectangleF srcRect, RectangleF destRect)
		{
			this.Image = image;
			this.SrcRect = srcRect;
			this.DestRect = destRect;
		}

		/// <summary>
		/// Draws image to graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public override void Draw(Graphics g, float scale, int invertHeight)
		{
			var x = this.DestRect.X;
			var y = this.DestRect.Y;
			var width = this.DestRect.Width;
			var height = this.DestRect.Height;

			if (invertHeight > 0)
				y = (invertHeight - y) - height;

			x /= scale;
			y /= scale;
			width /= scale;
			height /= scale;

			var srcRect = this.SrcRect;
			var destRect = new RectangleF(x, y, width, height);

			g.DrawImage(this.Image, destRect, srcRect, GraphicsUnit.Pixel);
		}

		/// <summary>
		/// Returns true if the given position is within the image's bounds.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public override bool IsInside(PointF pos)
		{
			var x = this.Position.X;
			var y = this.Position.Y;
			var width = this.SrcRect.Width;
			var height = this.SrcRect.Height;

			return !(pos.X < x || pos.X > x + width || pos.Y < y || pos.Y > y + height);
		}

		/// <summary>
		/// Moves the image by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void MoveBy(float x, float y)
		{
			this.Position += new SizeF(x, y);
		}

		/// <summary>
		/// Does nothing for an image.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public override void Rotate(PointF pivot, double radians)
		{
		}
	}
}
