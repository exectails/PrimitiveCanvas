using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace PrimitiveCanvas.Extensions
{
	/// <summary>
	/// Extensions for Point(F).
	/// </summary>
	public static class PointExtensions
	{
		/// <summary>
		/// Rotates point around pivot.
		/// </summary>
		/// <param name="point">Point to rotate.</param>
		/// <param name="pivot">Center of the rotation.</param>
		/// <param name="radians">Angle to rotate by in radians.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PointF RotatePoint(this PointF point, PointF pivot, double radians)
		{
			var cosTheta = Math.Cos(radians);
			var sinTheta = Math.Sin(radians);

			var x = (cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X);
			var y = (sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y);

			return new PointF((float)x, (float)y);
		}
	}
}
