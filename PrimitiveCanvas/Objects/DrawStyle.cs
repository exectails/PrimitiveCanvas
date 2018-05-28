using System.Drawing;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// Specifies how primitives are drawn.
	/// </summary>
	public class DrawStyle
	{
		/// <summary>
		/// Gets or sets the color used for foreground colors, such as
		/// text.
		/// </summary>
		public Color ForeColor { get; set; } = Color.Black;

		/// <summary>
		/// Gets or sets the color used for foreground colors, such as
		/// text, if the object is selected.
		/// </summary>
		public Color SelectedForeColor { get; set; } = Color.Red;

		/// <summary>
		/// Gets or sets the color of the outline.
		/// </summary>
		public Color OutlineColor { get; set; } = Color.Black;

		/// <summary>
		/// Gets or sets the color of the outline if the object
		/// is selected.
		/// </summary>
		public Color SelectedOutlineColor { get; set; } = Color.Red;

		/// <summary>
		/// Gets or sets the width of the outline.
		/// </summary>
		public float OutlineWidth { get; set; } = 1;

		/// <summary>
		/// Gets or sets the width of the outline if the object
		/// is selected.
		/// </summary>
		public float SelectedOutlineWidth { get; set; } = 2;
	}
}
