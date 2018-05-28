using System.Drawing;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// A rectangular primitive made up of four points.
	/// </summary>
	public class Rect : Polygon
	{
		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="centerX"></param>
		/// <param name="centerY"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Rect(float centerX, float centerY, float width, float height)
			: this(new PointF(centerX, centerY), width, height)
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="center"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Rect(PointF center, float width, float height)
		{
			var minX = (int)(center.X - width / 2);
			var minY = (int)(center.Y - height / 2);
			var maxX = (int)(center.X + width / 2);
			var maxY = (int)(center.Y + height / 2);

			var points = new PointF[4];
			points[0] = new PointF(minX, minY);
			points[1] = new PointF(minX + width, minY);
			points[2] = new PointF(minX + width, minY + height);
			points[3] = new PointF(minX, minY + height);

			this.Points = points;
			this.Position = center;
			//this.BoundingBox = new RectangleF(minX, minY, width, height);
		}
	}
}
