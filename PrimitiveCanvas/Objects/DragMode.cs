namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Specifies what kind of drag process is in progress.
	/// </summary>
	public enum DragMode
	{
		/// <summary>
		/// No dragging here.
		/// </summary>
		None,

		/// <summary>
		/// Moving an object.
		/// </summary>
		Move,

		/// <summary>
		/// Rotate an object.
		/// </summary>
		Rotate,

		/// <summary>
		/// Moving the canvas area.
		/// </summary>
		Scroll,
	}
}
