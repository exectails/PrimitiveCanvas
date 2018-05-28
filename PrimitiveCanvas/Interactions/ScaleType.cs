using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveCanvas.Interactions
{
	/// <summary>
	/// Defines how a canvas is scaled/zoomed.
	/// </summary>
	public enum ScaleType
	{
		/// <summary>
		/// Every scale action modifies the scale by a specific amount.
		/// </summary>
		Static,

		/// <summary>
		/// The scale amount is dynamically chosen based on the current
		/// scale. The farther out the scale is, the faster the
		/// scaling is.
		/// </summary>
		Dynamic,
	}
}
