using PrimitiveCanvas.Primitives;
using System.Collections.Generic;
using System.Drawing;

namespace PrimitiveCanvas.Objects
{
	/// <summary>
	/// An object on the canvas that can consist of multiple primitives.
	/// </summary>
	public class CanvasObject
	{
		/// <summary>
		/// Default priority assigned to new objects.
		/// </summary>
		public const int DefaultPriority = 1000;

		/// <summary>
		/// Gets or sets the canvas this object is part of.
		/// </summary>
		internal Canvas Canvas { get; set; }

		/// <summary>
		/// Gets or sets the object's position. Its primitives are moved
		/// relative to it.
		/// </summary>
		public PointF Position { get; internal set; }

		/// <summary>
		/// Returns a list of the object's primitives.
		/// </summary>
		public List<Primitive> Primitives { get; } = new List<Primitive>();

		/// <summary>
		/// Gets or sets a tag that associates other data with this object.
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets what interactions this object accepts.
		/// </summary>
		public ObjectInteractions Interactions { get; set; } = ObjectInteractions.All;

		/// <summary>
		/// Gets or sets the interaction priority for this object.
		/// </summary>
		/// <remarks>
		/// On selection the canvas prioritizes objects with a low priority
		/// property, cycling through the ones at the given position.
		/// </remarks>
		public int Priority { get; set; } = DefaultPriority;

		/// <summary>
		/// Gets or sets the object's draw order.
		/// </summary>
		/// <remarks>
		/// The higher the property's value, the later the object is drawn,
		/// layering on top of objects with lower draw orders.
		/// 
		/// Property currently doesn't affect draw order if it's set after
		/// the object was added to a canvas.
		/// </remarks>
		public int DrawOrder { get; set; }

		/// <summary>
		/// Returns true if this object is currently selected.
		/// </summary>
		public bool Selected { get; internal set; }

		/// <summary>
		/// Gets or sets whether this object is visible.
		/// </summary>
		/// <remarks>
		/// Objects that aren't visible can't be interacted with.
		/// </remarks>
		public bool Visible { get; set; } = true;

		/// <summary>
		/// Gets or sets the object's style, specifying how its
		/// primitives are drawn.
		/// </summary>
		public DrawStyle Style { get; set; } = new DrawStyle();

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="primitives"></param>
		public CanvasObject(float x, float y, params Primitive[] primitives)
			: this(new PointF(x, y), primitives)
		{
		}

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="primitives"></param>
		public CanvasObject(PointF position, params Primitive[] primitives)
		{
			this.Position = position;

			if (primitives != null && primitives.Length != 0)
			{
				foreach (var primitive in primitives)
					this.Add(primitive);
			}
		}

		/// <summary>
		/// Adds primitive to object.
		/// </summary>
		/// <param name="primitive"></param>
		public void Add(Primitive primitive)
		{
			primitive.Object = this;
			this.Primitives.Add(primitive);
		}

		/// <summary>
		/// Returns true if the given interaction is enabled by the
		/// object.
		/// </summary>
		/// <param name="interaction"></param>
		/// <returns></returns>
		public bool Is(ObjectInteractions interaction)
		{
			return ((this.Interactions & interaction) != 0);
		}

		/// <summary>
		/// Moves object and its primitives by the given amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void MoveBy(float x, float y)
		{
			var primitives = this.Primitives;
			for (var i = 0; i < primitives.Count; ++i)
				primitives[i].MoveBy(x, y);

			this.Position += new SizeF(x, y);
		}

		/// <summary>
		/// Moves object to given position and updates its primitive's
		/// position's relative to it.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void MoveTo(float x, float y)
		{
			var pos = this.Position;
			var delta = new PointF(x - pos.X, y - pos.Y);

			this.MoveBy(delta.X, delta.Y);
		}

		/// <summary>
		/// Rotates the object's primitives by the given amount of radians
		/// around the object's position as the pivot point.
		/// </summary>
		/// <param name="radians"></param>
		public void Rotate(double radians)
		{
			var primitives = this.Primitives;
			for (var i = 0; i < primitives.Count; ++i)
				primitives[i].Rotate(this.Position, radians);
		}

		/// <summary>
		/// Resizes object's primitives by multiplying their sizes with
		/// the given value.
		/// </summary>
		/// <param name="multiplier"></param>
		public void Resize(double multiplier)
		{
			var primitives = this.Primitives;
			for (var i = 0; i < primitives.Count; ++i)
				primitives[i].Resize(this.Position, multiplier);
		}

		/// <summary>
		/// Returns true if the given position is inside one of the
		/// object's primitives.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool IsInside(PointF position)
		{
			var primitives = this.Primitives;
			for (var i = 0; i < primitives.Count; ++i)
			{
				if (primitives[i].IsInside(position))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns a pen to draw the object primitive's outlines.
		/// </summary>
		/// <returns></returns>
		public Pen GetOutlinePen()
		{
			if (!this.Selected)
				return new Pen(this.Style.OutlineColor, this.Style.OutlineWidth);
			else
				return new Pen(this.Style.SelectedOutlineColor, this.Style.SelectedOutlineWidth);
		}

		/// <summary>
		/// Returns a brush to draw the object primitive's outlines.
		/// </summary>
		/// <returns></returns>
		public Brush GetForegroundBrush()
		{
			if (!this.Selected)
				return new SolidBrush(this.Style.ForeColor);
			else
				return new SolidBrush(this.Style.SelectedForeColor);
		}

		/// <summary>
		/// Draws object's primitives to graphics.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scale"></param>
		/// <param name="invertHeight"></param>
		public void Draw(Graphics g, float scale, int invertHeight)
		{
			if (!this.Visible)
				return;

			var primitives = this.Primitives;
			for (var i = 0; i < primitives.Count; ++i)
				primitives[i].Draw(g, scale, invertHeight);
		}

		/// <summary>
		/// Implicitly converts primitive to a canvas object.
		/// </summary>
		/// <param name="primitive"></param>
		public static implicit operator CanvasObject(Primitive primitive)
		{
			return new CanvasObject(primitive.Position, primitive);
		}
	}
}
