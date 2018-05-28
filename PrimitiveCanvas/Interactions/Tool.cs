namespace PrimitiveCanvas.Interactions
{
	/// <summary>
	/// Specifies which tool is currently selected.
	/// </summary>
	public enum Tool
	{
		/// <summary>
		/// Direct control over everything.
		/// </summary>
		Free,

		/// <summary>
		/// Scroll over map by using the mouse.
		/// </summary>
		Scroll,

		/// <summary>
		/// Move an object with the mouse.
		/// </summary>
		Move,

		/// <summary>
		/// Rotate an object with the mouse.
		/// </summary>
		Rotate,
	}
}
