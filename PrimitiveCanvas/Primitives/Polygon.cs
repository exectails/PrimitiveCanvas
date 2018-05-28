using PrimitiveCanvas.Extensions;
using System;
using System.Drawing;
using System.Linq;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// A polygon shape made up of several points.
	/// </summary>
	public class Polygon : Primitive
	{
		private PointF _position;
		private bool _initialPositionSet;

		/// <summary>
		/// Gets or sets the primitive's points that make up its shape.
		/// </summary>
		public PointF[] Points { get; set; }

		/// <summary>
		/// Gets or sets the primitive's center position.
		/// </summary>
		public override PointF Position
		{
			get { return _position; }
			set
			{
				if (!_initialPositionSet)
				{
					_position = value;
					_initialPositionSet = true;
				}
				else
				{
					var deltaX = (value.X - _position.X);
					var deltaY = (value.Y - _position.Y);

					for (var i = 0; i < this.Points.Length; ++i)
					{
						this.Points[i].X += deltaX;
						this.Points[i].Y += deltaY;
					}

					_position = value;
				}
			}
		}

		/// <summary>
		/// Creates new, uninitialized instance.
		/// </summary>
		internal Polygon()
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="points"></param>
		public Polygon(params PointF[] points)
		{
			var minX = points.Min(a => a.X);
			var minY = points.Min(a => a.Y);
			var maxX = points.Max(a => a.X);
			var maxY = points.Max(a => a.Y);
			var width = (maxX - minX);
			var height = (maxY - minY);

			this.Points = points;
			this.Position = new PointF(minX + width / 2, minY + height / 2);
			//this.BoundingBox = new RectangleF(minX, minY, width, height);
		}

		/// <summary>
		/// Draws this primitive.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public override void Draw(Graphics g, float scale, int invertHeight)
		{
			var points = new PointF[this.Points.Length];
			var pen = this.Object.GetOutlinePen();

			for (var i = 0; i < this.Points.Length; ++i)
			{
				var point = this.Points[i];

				var x = point.X;
				var y = point.Y;
				if (invertHeight > 0)
					y = (invertHeight - y);

				points[i] = new PointF(x / scale, y / scale);
			}

			g.DrawPolygon(pen, points);

			//g.FillPolygon(
			//	new System.Drawing.Drawing2D.LinearGradientBrush(
			//		new Point(0, 0),
			//		new Point(100, 0),
			//		Color.FromArgb(255, 240, 240, 240),
			//		Color.FromArgb(255, 200, 200, 200))
			//	, points);
			//g.DrawPolygon(new Pen(Color.FromArgb(40, 40, 40)), points);
		}

		/// <summary>
		/// Returns true if the given point is inside this primitive.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public override bool IsInside(PointF pos)
		{
			var result = false;
			var points = this.Points;

			for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
			{
				if (((points[i].Y > pos.Y) != (points[j].Y > pos.Y)) && (pos.X < (points[j].X - points[i].X) * (pos.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
					result = !result;
			}

			return result;
		}

		/// <summary>
		/// Moves the polygon's points by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void MoveBy(float x, float y)
		{
			for (var i = 0; i < this.Points.Length; ++i)
			{
				this.Points[i].X += x;
				this.Points[i].Y += y;
			}

			_position.X += x;
			_position.Y += y;
		}

		/// <summary>
		/// Rotates the primitive by the given amount of radian degrees.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public override void Rotate(PointF pivot, double radians)
		{
			for (var i = 0; i < this.Points.Length; ++i)
			{
				var point = this.Points[i];
				this.Points[i] = point.RotatePoint(pivot, radians);
			}
		}
	}
}
