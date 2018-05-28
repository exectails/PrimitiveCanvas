using PrimitiveCanvas.Interactions;
using PrimitiveCanvas.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PrimitiveCanvas
{
	/// <summary>
	/// Draws objects on an area in itself.
	/// </summary>
	public class Canvas : Control
	{
		private List<CanvasObject> _objects = new List<CanvasObject>();

		private bool _isDragging;
		private Point _dragStartPoint, _dragLast;
		private float _dragDegree;
		private DragMode _dragMode;

		private List<CanvasObject> _selectedObjects = new List<CanvasObject>();

		private RectangleF _canvas = new RectangleF(0, 0, 0, 0);
		private Color _canvasBackColor;
		private Brush _canvasBackBrush;

		private bool _updating;

		private float _scaleMin = 1;
		private float _scaleMax = 1000;
		private float _scale = 1;
		private float _scaleStep = 1;
		private ScaleType _scaleType = ScaleType.Static;
		private bool _invertY = false;

		/// <summary>
		/// Returns true if something is currently dragged in this control.
		/// For example, moving or rotating an object.
		/// </summary>
		[Browsable(false)]
		public bool IsDragging => _isDragging;

		/// <summary>
		/// Gets or sets whether multiple objects can be selected
		/// at the same time.
		/// </summary>
		[DefaultValue(false)]
		public bool MultiSelect { get; set; }

		/// <summary>
		/// Gets or sets the controls background color.
		/// </summary>
		[DefaultValue(typeof(Color), "169,169,169")]
		public new Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				base.BackColor = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the color of the actual draw area inside
		/// the control.
		/// </summary>
		[DefaultValue(typeof(Color), "255,255,255")]
		public Color CanvasBackColor
		{
			get { return _canvasBackColor; }
			set
			{
				_canvasBackColor = value;
				_canvasBackBrush = new SolidBrush(value);
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the minimum scale value, specifying how far
		/// you can "zoom in".
		/// </summary>
		[DefaultValue(1)]
		public float ScaleMin
		{
			get { return _scaleMin; }
			set
			{
				_scaleMin = Math.Max(1, value);
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the maximum scale value, specifying how far
		/// you can "zoom out".
		/// </summary>
		[DefaultValue(1000)]
		public float ScaleMax
		{
			get { return _scaleMax; }
			set
			{
				_scaleMax = Math.Min(1000, value);
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the current scale.
		/// </summary>
		[DefaultValue(1)]
		public float ScaleCurrent
		{
			get { return _scale; }
			set
			{
				_scale = Math.Max(this.ScaleMin, Math.Min(this.ScaleMax, value));
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets by how much the scale changes when the mouse wheel
		/// is used.
		/// </summary>
		[DefaultValue(1)]
		public float ScaleStep
		{
			get { return _scaleStep; }
			set
			{
				_scaleStep = Math.Max(1, value);
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets how the canvas is scaled.
		/// </summary>
		/// <remarks>
		/// If the Static type is used the canvas scales by exactly the
		/// number in ScaleStep every time. The dynamic type uses ScaleStep
		/// as a percentage of the current scale, to scale faster when
		/// scaled out far and slower when scaled in close.
		/// </remarks>
		[DefaultValue(ScaleType.Static)]
		public ScaleType ScaleType
		{
			get { return _scaleType; }
			set { _scaleType = value; }
		}

		/// <summary>
		/// Gets or sets whether the canvas is inverted and 0,0 is found
		/// at the bottom left instead of the top right.
		/// </summary>
		[DefaultValue(false)]
		public bool InvertY
		{
			get { return _invertY; }
			set
			{
				_invertY = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Returns the number of objects currently selected.
		/// </summary>
		[Browsable(false)]
		public int SelectedObjectsCount => _selectedObjects.Count;

		/// <summary>
		/// Gets or sets whether to draw outside of the canvas' drawing
		/// area or to clip it.
		/// </summary>
		[DefaultValue(true)]
		public bool DrawOutsideCanvasArea { get; set; } = true;

		///// <summary>
		///// Gets or sets how objects are interacted with.
		///// </summary>
		//[DefaultValue(InteractionMode.Direct)]
		//public InteractionMode InteractionMode { get; set; } = InteractionMode.Direct;

		/// <summary>
		/// Gets or sets the selected interaction tool.
		/// </summary>
		[DefaultValue(Tool.Scroll)]
		public Tool SelectedTool { get; set; } = Tool.Scroll;

		/// <summary>
		/// Gets or sets the canvas' drawing area size.
		/// </summary>
		[DefaultValue(typeof(SizeF), "0,0")]
		public SizeF CanvasAreaSize
		{
			get { return new SizeF(_canvas.Width, _canvas.Height); }
			set
			{
				_canvas.Width = value.Width;
				_canvas.Height = value.Height;
			}
		}

		/// <summary>
		/// Raised when an object was moved.
		/// </summary>
		public event EventHandler<ObjectMovedEventArgs> ObjectMoved;

		/// <summary>
		/// Raised when an object was rotated.
		/// </summary>
		public event EventHandler<ObjectRotatedEventArgs> ObjectRotated;

		/// <summary>
		/// Raised when an object was selected.
		/// </summary>
		public event EventHandler<ObjectSelectedEventArgs> ObjectSelected;

		/// <summary>
		/// Raised when the canvas' scale changed.
		/// </summary>
		public event EventHandler<ScaleChangedEventArgs> ScaleChanged;

		/// <summary>
		/// Creates new instance.
		/// </summary>
		public Canvas()
		{
			this.Size = new Size(480, 360);
			base.BackColor = Color.DarkGray;
			this.DoubleBuffered = true;

			_canvasBackColor = Color.White;
			_canvasBackBrush = new SolidBrush(_canvasBackColor);
		}

		/// <summary>
		/// Stops redraws of the control until EndUpdate is called.
		/// </summary>
		public void BeginUpdate()
		{
			_updating = true;
		}

		/// <summary>
		/// Reactivates redraws and invalidates control.
		/// </summary>
		public void EndUpdate()
		{
			_updating = false;
			this.SortObjects();
			this.Invalidate();
		}

		/// <summary>
		/// Initializes canvas area after the control was created.
		/// </summary>
		protected override void OnCreateControl()
		{
			this.ScaleToFit();
			this.CenterCanvas();

			base.OnCreateControl();
		}

		/// <summary>
		/// Sets the canvas drawing area's size.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SetCanvasArea(float width, float height)
		{
			_canvas.Width = width;
			_canvas.Height = height;

			this.ScaleToFit();
			this.CenterCanvas();
		}

		/// <summary>
		/// Scales canvas area to fit into the control's area and
		/// centers it.
		/// </summary>
		public void ScaleToFitCenter()
		{
			this.ScaleToFit();
			this.CenterCanvas();
			this.Invalidate();
		}

		/// <summary>
		/// Scales canvas area to fit into the control's area.
		/// </summary>
		private void ScaleToFit()
		{
			var size = this.Size;
			var fitX = (_canvas.Width / (size.Width - 20));
			var fitY = (_canvas.Height / (size.Height - 20));

			var scale = (fitX > fitY ? fitX : fitY);

			_scale = Math.Max(1, scale);
		}

		/// <summary>
		/// Centers canvas area on control's area.
		/// </summary>
		private void CenterCanvas()
		{
			_canvas.X = this.Width / 2 - (_canvas.Width / _scale) / 2;
			_canvas.Y = this.Height / 2 - (_canvas.Height / _scale) / 2;
		}

		/// <summary>
		/// Adds object to the canvas for it to be drawn.
		/// </summary>
		/// <param name="obj"></param>
		public void Add(CanvasObject obj)
		{
			obj.Canvas = this;
			_objects.Add(obj);

			if (!_updating)
				this.SortObjects();

			this.Invalidate();
		}

		/// <summary>
		/// Sorts objects by draw order.
		/// </summary>
		private void SortObjects()
		{
			_objects.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));
		}

		/// <summary>
		/// Returns the first object at the given world position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private CanvasObject GetFirstObjectAt(PointF position)
		{
			CanvasObject result = null;

			for (var i = 0; i < _objects.Count; ++i)
			{
				var obj = _objects[i];

				if (obj.IsInside(position))
				{
					result = obj;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns first object that matches the given predicate, or null
		/// no object was found.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public CanvasObject GetObject(Func<CanvasObject, bool> predicate)
		{
			return _objects.FirstOrDefault(predicate);
		}

		/// <summary>
		/// Removes the given object from this canvas.
		/// </summary>
		/// <param name="obj"></param>
		public void Remove(CanvasObject obj)
		{
			obj.Selected = false;
			_objects.Remove(obj);
			_selectedObjects.Remove(obj);
		}

		/// <summary>
		/// Returns the first object at the given world position.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		private List<CanvasObject> GetObjectsAt(PointF position, Func<CanvasObject, bool> predicate)
		{
			var result = new List<CanvasObject>();

			for (var i = 0; i < _objects.Count; ++i)
			{
				var obj = _objects[i];

				if (obj.Visible && obj.IsInside(position) && predicate(obj))
					result.Add(obj);
			}

			result.Sort((a, b) => a.Priority.CompareTo(b.Priority));

			return result;
		}

		/// <summary>
		/// Removes all canvas objects.
		/// </summary>
		public void ClearObjects()
		{
			_objects.Clear();
			this.ClearSelectionInternal();
			this.Invalidate();
		}

		/// <summary>
		/// Paints control, incl canvas area and all objects.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_updating)
				return;

			var invertHeight = 0;
			if (this.InvertY)
				invertHeight = (int)_canvas.Height;

			var g = e.Graphics;
			g.TranslateTransform(_canvas.X, _canvas.Y);
			g.FillRectangle(_canvasBackBrush, 0, 0, _canvas.Width / _scale, _canvas.Height / _scale);

			//e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			if (!this.DrawOutsideCanvasArea)
				g.Clip = new Region(new RectangleF(0, 0, _canvas.Width / _scale, _canvas.Height / _scale));

			for (var j = 0; j < _objects.Count; ++j)
				_objects[j].Draw(g, _scale, invertHeight);

			base.OnPaint(e);
		}

		/// <summary>
		/// Paints control's background.
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if (_updating)
				return;

			base.OnPaintBackground(pevent);
		}

		/// <summary>
		/// Returns the position in 1:1 scale, relative to the canvas
		/// area and its scale.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public PointF GetWorldPosition(PointF point)
		{
			point.X -= _canvas.X;
			point.Y -= _canvas.Y;

			point.X *= _scale;
			point.Y *= _scale;

			if (this.InvertY)
				point.Y = (_canvas.Height - point.Y);

			return point;
		}

		/// <summary>
		/// Inverse of GetWorldPosition, returns position on the canvas
		/// relative to a scaled point on it.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PointF GetCanvasPosition(PointF position)
		{
			if (this.InvertY)
				position.Y = (_canvas.Height - position.Y);

			position.X /= _scale;
			position.Y /= _scale;

			return position;
		}

		/// <summary>
		/// Returns position in canvas area, based on a point on the control.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public PointF TransformCanvasPosition(PointF point)
		{
			point.X -= _canvas.X;
			point.Y -= _canvas.Y;

			return point;
		}

		/// <summary>
		/// Marks the given object as selected.
		/// </summary>
		/// <param name="obj">Canvas object to select.</param>
		/// <param name="addToSelection">
		/// Set to true if object should be added to the currently
		/// selected objects.
		/// </param>
		public void SelectObject(CanvasObject obj, bool addToSelection)
		{
			if (obj == null)
			{
				this.ClearSelection();
				return;
			}

			if (!obj.Visible)
				return;

			if (!this.MultiSelect || !addToSelection)
				this.ClearSelectionInternal();

			_selectedObjects.Add(obj);
			obj.Selected = true;

			this.Invalidate();
		}

		/// <summary>
		/// Unselects all selected objects without invalidating.
		/// </summary>
		internal void ClearSelectionInternal()
		{
			_selectedObjects.ForEach(a => a.Selected = false);
			_selectedObjects.Clear();
		}

		/// <summary>
		/// Unselects all selected objects.
		/// </summary>
		public void ClearSelection()
		{
			this.ClearSelectionInternal();
			this.Invalidate();
		}

		/// <summary>
		/// Unselected the given object.
		/// </summary>
		/// <param name="obj"></param>
		public void UnselectObject(CanvasObject obj)
		{
			obj.Selected = false;
			_selectedObjects.Remove(obj);

			this.Invalidate();
		}

		/// <summary>
		/// Updates selected objects with the given ones.
		/// </summary>
		/// <param name="objects"></param>
		private void HandleSelection(List<CanvasObject> objects)
		{
			// If there's only one object, select it.
			if (objects.Count == 1)
			{
				this.SelectObject(objects[0], false);
				this.OnObjectSelected(new ObjectSelectedEventArgs(objects[0]));
				return;
			}

			var priority = objects[0].Priority;
			var foundSelected = false;
			CanvasObject unselectedObject = null;
			var ignorePriority = (ModifierKeys == Keys.Alt);

			// Go through all objects and cycle through the ones with the
			// lowest priority.
			for (var i = 0; i < objects.Count; ++i)
			{
				var obj = objects[i];

				// Stop once the priority changed, we only want to cycle
				// through the lowest priority ones.
				if (priority != obj.Priority && !ignorePriority)
					break;

				// If the object is selected the next unselected one is the
				// one to select.
				if (obj.Selected)
				{
					foundSelected = true;
				}
				else if (foundSelected)
				{
					unselectedObject = obj;
					break;
				}

				// If this is the last iteration or the next object has a
				// different priority, and we haven't found any objects yet,
				// use the first one to complete the cycle.
				if (i + 1 == objects.Count || (objects[i + 1].Priority != obj.Priority && !ignorePriority))
				{
					unselectedObject = objects[0];
					break;
				}
			}

			if (unselectedObject != null)
			{
				this.SelectObject(unselectedObject, false);
				this.OnObjectSelected(new ObjectSelectedEventArgs(unselectedObject));
			}
		}

		/// <summary>
		/// Returns list of all selected objects.
		/// </summary>
		/// <returns></returns>
		public List<CanvasObject> GetSelectedObjects()
		{
			return _selectedObjects.ToList();
		}

		/// <summary>
		/// Initializes drag process when the mouse is pressed down.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			var worldPos = this.GetWorldPosition(e.Location);
			var tool = this.SelectedTool;

			_dragStartPoint = _dragLast = e.Location;
			_dragDegree = 0;

			if (e.Button == MouseButtons.Left)
			{
				if (tool == Tool.Free)
				{
					if (_selectedObjects.Count > 0)
					{
						_dragMode = DragMode.Move;
					}
					else
					{
						_dragMode = DragMode.Scroll;
						this.ClearSelection();
						this.OnObjectSelected(new ObjectSelectedEventArgs(null));
					}
				}
				else
				{
					switch (tool)
					{
						case Tool.Scroll: _dragMode = DragMode.Scroll; break;
						case Tool.Move: _dragMode = DragMode.Move; break;
						case Tool.Rotate: _dragMode = DragMode.Rotate; break;
					}
				}
			}
			else if (e.Button == MouseButtons.Middle)
			{
				_dragMode = DragMode.Scroll;
			}
			else if (e.Button == MouseButtons.Right && tool == Tool.Free && _selectedObjects.Count > 0)
			{
				_dragMode = DragMode.Rotate;
			}

			base.OnMouseDown(e);
		}

		/// <summary>
		/// Ends drag process when mouse is let go off.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			var tool = this.SelectedTool;

			if (e.Button == MouseButtons.Left && !_isDragging)
			{
				var worldPos = this.GetWorldPosition(e.Location);
				var objects = this.GetObjectsAt(worldPos, a => a.Is(ObjectInteractions.Selectable));

				if (objects.Count > 0)
				{
					this.HandleSelection(objects);
					this.Invalidate();
				}
				else
				{
					this.ClearSelection();
					this.OnObjectSelected(new ObjectSelectedEventArgs(null));
				}
			}

			if (_dragMode == DragMode.Scroll)
			{
				//if (tool != Tool.Direct && e.Button == MouseButtons.Left && !_isDragging)
				//{
				//	var worldPos = this.GetWorldPosition(e.Location);
				//	var objects = this.GetObjectsAt(worldPos, a => a.Is(ObjectInteractions.Selectable));

				//	if (objects.Count > 0)
				//	{
				//		this.HandleSelection(objects);
				//		this.Invalidate();
				//	}
				//	else
				//	{
				//		this.ClearSelection();
				//	}
				//}
			}
			else if (_dragMode == DragMode.Move)
			{
				var selectedObjects = _selectedObjects;
				if (selectedObjects.Count != 0)
				{
					var startWorldPos = this.GetWorldPosition(_dragStartPoint);
					var endWorldPos = this.GetWorldPosition(e.Location);

					var deltaWorldPos = endWorldPos;
					deltaWorldPos.X -= startWorldPos.X;
					deltaWorldPos.Y -= startWorldPos.Y;

					if (startWorldPos != endWorldPos)
					{
						for (var i = 0; i < selectedObjects.Count; ++i)
							this.OnObjectMoved(new ObjectMovedEventArgs(selectedObjects[i], deltaWorldPos));
					}
				}
			}
			else if (_dragMode == DragMode.Rotate)
			{
				var selectedObjects = _selectedObjects;
				if (selectedObjects.Count != 0)
				{
					if (_dragDegree != 0)
					{
						var radians = (_dragDegree * (Math.PI / 180));

						for (var i = 0; i < selectedObjects.Count; ++i)
							this.OnObjectRotated(new ObjectRotatedEventArgs(selectedObjects[i], radians));
					}

					this.Invalidate();
				}
			}

			_dragMode = DragMode.None;
			_isDragging = false;

			base.OnMouseUp(e);
		}

		/// <summary>
		/// Handles dragging process while the mouse moves over the control.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			var delta = new PointF(e.Location.X - _dragLast.X, e.Location.Y - _dragLast.Y);

			// Dragging starts if we have a drag mode and the location
			// actually started to differ from the start start point.
			if (_dragMode != DragMode.None && e.Location != _dragStartPoint)
				_isDragging = true;

			if (_dragMode == DragMode.Move)
			{
				// Move all selected, movable objects.

				var objects = _selectedObjects;
				if (objects.Count != 0)
				{
					if (this.InvertY)
						delta.Y = -delta.Y;

					delta.X *= _scale;
					delta.Y *= _scale;

					for (var i = 0; i < objects.Count; ++i)
					{
						var obj = objects[i];
						if (obj.Is(ObjectInteractions.Movable))
							obj.MoveBy(delta.X, delta.Y);
					}

					this.Invalidate();
				}
			}
			else if (_dragMode == DragMode.Scroll)
			{
				// Move the canvas area by the delta position.

				_canvas.X += delta.X;
				_canvas.Y += delta.Y;

				this.Invalidate();
			}
			else if (_dragMode == DragMode.Rotate)
			{
				// Rotate selected objects.

				var objects = _selectedObjects;
				if (objects.Count != 0)
				{
					if (this.InvertY)
						delta.Y = -delta.Y;

					_dragDegree += delta.Y;
					for (var i = 0; i < objects.Count; ++i)
					{
						var obj = objects[i];
						if (obj.Is(ObjectInteractions.Rotatable))
							obj.Rotate(delta.Y * (Math.PI / 180));
					}
					this.Invalidate();
				}
			}

			_dragLast = e.Location;

			base.OnMouseMove(e);
		}

		/// <summary>
		/// Modifies scale, zooming in and out when the mouse wheel is used.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta != 0)
			{
				// Modify scale based on mouse wheel scroll direction.

				var scaleStep = _scaleStep;

				if (_scaleType == ScaleType.Dynamic)
					scaleStep = (float)Math.Max(1, Math.Floor(_scale / _scaleStep));

				if (ModifierKeys == Keys.Control)
					scaleStep *= 2;

				var scaleValue = (e.Delta < 0 ? scaleStep : -scaleStep);
				var prevScale = _scale;

				_scale = Math.Max(_scaleMin, Math.Min(_scaleMax, (float)Math.Round(_scale + scaleValue)));

				// Adjust the canvas area location to keep the point
				// that was below the mouse where it was.
				var transPos = this.TransformCanvasPosition(e.Location);
				_canvas.X = (e.Location.X - transPos.X * prevScale / _scale);
				_canvas.Y = (e.Location.Y - transPos.Y * prevScale / _scale);

				this.OnScaleChanged(new ScaleChangedEventArgs(prevScale, _scale));

				this.Invalidate();
			}

			base.OnMouseWheel(e);
		}

		/// <summary>
		/// Resets scale area on double middle click.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				this.ScaleToFitCenter();
			}

			base.OnMouseDoubleClick(e);
		}

		/// <summary>
		/// Called when an object was moved.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnObjectMoved(ObjectMovedEventArgs e)
		{
			this.ObjectMoved?.Invoke(this, e);
		}

		/// <summary>
		/// Called when an object was rotated.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnObjectRotated(ObjectRotatedEventArgs e)
		{
			this.ObjectRotated?.Invoke(this, e);
		}

		/// <summary>
		/// Called when an object was selected.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnObjectSelected(ObjectSelectedEventArgs e)
		{
			this.ObjectSelected?.Invoke(this, e);
		}

		/// <summary>
		/// Called when the canvas' scale changed..
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnScaleChanged(ScaleChangedEventArgs e)
		{
			this.ScaleChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Scrolls the given position to the center of the canvas.
		/// </summary>
		/// <param name="pos"></param>
		public void ScrollToWorldPosition(PointF pos)
		{
			var canvasPos = this.GetCanvasPosition(pos);

			_canvas.X = (this.Width / 2) - canvasPos.X;
			_canvas.Y = (this.Height / 2) - canvasPos.Y;

			this.Invalidate();
		}
	}
}
