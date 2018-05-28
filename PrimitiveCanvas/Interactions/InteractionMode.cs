namespace PrimitiveCanvas.Interactions
{
	/// <summary>
	/// Specifies how objects can be interacted with.
	/// </summary>
	public enum InteractionMode
	{
		/// <summary>
		/// Objects can be directly moved, rotated, etc.
		/// </summary>
		Direct,

		/// <summary>
		/// The respective tool must be active to interact with an object.
		/// </summary>
		Tools,
	}
}
