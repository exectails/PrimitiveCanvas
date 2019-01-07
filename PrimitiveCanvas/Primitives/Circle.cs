using System;
using System.Drawing;
using PrimitiveCanvas.Extensions;

namespace PrimitiveCanvas.Primitives
{
	/// <summary>
	/// A circular primitive.
	/// </summary>
	public class Circle : Primitive
	{
		/// <summary>
		/// Gets or sets the primitive's center point.
		/// </summary>
		public override PointF Position { get; set; }

		/// <summary>
		/// Gets or sets the circle's radius.
		/// </summary>
		public float Radius { get; set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="radius"></param>
		public Circle(float x, float y, float radius)
			: this(new PointF(x, y), radius)
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="radius"></param>
		public Circle(PointF pos, float radius)
		{
			this.Position = pos;
			this.Radius = radius;
		}

		/// <summary>
		/// Draws primitive.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public override void Draw(Graphics g, float scale, int invertHeight)
		{
			var x = (this.Position.X - this.Radius);
			var y = (this.Position.Y - this.Radius);
			var size = (this.Radius * 2);
			var pen = this.Object.GetOutlinePen();

			if (invertHeight > 0)
			{
				y = (invertHeight - y);
				y -= size;
			}

			g.DrawEllipse(pen, x / scale, y / scale, size / scale, size / scale);
		}

		/// <summary>
		/// Returns true if given position is inside this primitive.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public override bool IsInside(PointF pos)
		{
			var center = this.Position;
			var radius = this.Radius;

			return (Math.Pow(center.X - pos.X, 2) + Math.Pow(center.Y - pos.Y, 2) <= Math.Pow(radius, 2));
		}

		/// <summary>
		/// Moves circle by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void MoveBy(float x, float y)
		{
			this.Position += new SizeF(x, y);
		}

		/// <summary>
		/// Updates circle's position around the pivot point.
		/// </summary>
		/// <param name="pivot"></param>
		/// <param name="radians"></param>
		public override void Rotate(PointF pivot, double radians)
		{
			this.Position = this.Position.RotatePoint(pivot, radians);
		}
		}
	}
}
