using System;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Specifies which interaction's an object accepts.
	/// </summary>
	[Flags]
	public enum ObjectInteractions
	{
		/// <summary>
		/// No interactions.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// Object can be selected.
		/// </summary>
		Selectable = 0x01,

		/// <summary>
		/// Object can be moved around.
		/// </summary>
		Movable = 0x02,

		/// <summary>
		/// Object can be rotated.
		/// </summary>
		Rotatable = 0x04,

		/// <summary>
		/// Object accepts all interactions.
		/// </summary>
		All = 0xFFFFFFF,
	}
}
