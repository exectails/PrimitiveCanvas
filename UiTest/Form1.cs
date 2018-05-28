using PrimitiveCanvas.Interactions;
using PrimitiveCanvas.Objects;
using PrimitiveCanvas.Primitives;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiTest
{
	public partial class Form1 : Form
	{
		CanvasObject obj1;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//Canvas.InteractionMode = InteractionMode.Tools;
			Canvas.SetCanvasArea(70000, 80000);
			Canvas.ScaleType = ScaleType.Dynamic;

			Canvas.MouseClick += this.Canvas1_MouseClick;
			Canvas.KeyDown += this.Canvas1_KeyDown;
			Canvas.InvertY = checkBox1.Checked;

			//CanvasObject pic = new Picture(new Bitmap(@"..."), new RectangleF(0, 0, 300, 300), new RectangleF(0, 0, 3000, 3000));
			//pic.Interactions = ObjectInteractions.None;
			//Canvas.Add(pic);

			var flat = new FlatRect(new RectangleF(0, 0, 150, 150), Color.AliceBlue);
			Canvas.Add(new CanvasObject(0, 0, flat)
			{
				Interactions = ObjectInteractions.None,
				//DrawOrder = 1,
				Visible = true
			});

			CanvasObject o1 = new Rect(140, 140, 43, 43);
			Canvas.Add(o1);
			CanvasObject o2 = new Rect(160, 160, 43, 43);
			Canvas.Add(o2);
			CanvasObject o3 = new Circle(150, 150, 30);
			o3.Priority = 0;
			Canvas.Add(o3);



			obj1 = new CanvasObject(30, 20);
			obj1.Add(new Rect(20, 10, 10, 10));
			obj1.Add(new Rect(40, 10, 10, 10));
			obj1.Add(new Rect(30, 30, 10, 10));
			obj1.Add(new Polygon(new PointF(20, 10), new PointF(40, 10), new PointF(30, 30)));
			obj1.Interactions = ObjectInteractions.Selectable | ObjectInteractions.Movable;
			Canvas.Add(obj1);

			var obj = new CanvasObject(130, 120);
			obj.Add(new Rect(120, 110, 10, 10));
			obj.Add(new Rect(140, 110, 10, 10));
			obj.Add(new Rect(130, 130, 10, 10));
			obj.Add(new Polygon(new PointF(120, 110), new PointF(140, 110), new PointF(130, 130)));
			Canvas.Add(obj);

			Canvas.Add(new TextString(new PointF(10, 10), "Hello, World!") { StringFormat = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near } });

			//canvas1.Add(new Rect(180, 180, 10, 10));

		}

		private void Canvas1_KeyDown(object sender, KeyEventArgs e)
		{
		}

		private void Canvas1_MouseClick(object sender, MouseEventArgs e)
		{
		}

		private void Canvas1_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = Canvas.GetWorldPosition(e.Location);
			var scale = Canvas.ScaleCurrent;

			label1.Text = string.Format("{0:0.00} x {1:0.00} @ {2:0.##}", pos.X, pos.Y, scale);
		}

		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			Canvas.InvertY = !Canvas.InvertY;
			Canvas.Invalidate();
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A)
			{
				var pos = Canvas.GetWorldPosition(Canvas.PointToClient(Cursor.Position));
				//canvasContainer1.Add(new Rect(pos, 20, 20));
				Canvas.Add(new Rect(pos, 43, 43));
				//canvas1.Add(new Polygon(new[] { new Point(pos.X - 20, pos.Y - 20), new Point(pos.X + 20, pos.Y - 20), new Point(pos.X, pos.Y + 30) }));
				//canvas1.Add(new Circle(pos, 30));
			}
			else if (e.KeyCode == Keys.D)
			{
				obj1.MoveTo(150, 150);
				Canvas.Invalidate();
			}
			else if (e.KeyCode == Keys.S)
			{
				var objects = Canvas.GetSelectedObjects();
				objects.ForEach(a => a.Visible = !a.Visible);
				Canvas.ClearSelection();
			}
			else if (e.KeyCode == Keys.E)
			{
				Canvas.ClearObjects();
			}
			else if (e.KeyCode == Keys.Delete)
			{
				var objects = Canvas.GetSelectedObjects();
				objects.ForEach(a => Canvas.Remove(a));
				Canvas.Invalidate();
			}
			else if (e.KeyCode == Keys.Y)
			{
				Canvas.BeginUpdate();
			}
			else if (e.KeyCode == Keys.X)
			{
				Canvas.EndUpdate();
			}
			else if (e.KeyCode == Keys.D1)
			{
				Canvas.SelectedTool = Tool.Scroll;
			}
			else if (e.KeyCode == Keys.D2)
			{
				Canvas.SelectedTool = Tool.Move;
			}
			else if (e.KeyCode == Keys.D3)
			{
				Canvas.SelectedTool = Tool.Rotate;
			}
			else if (e.KeyCode == Keys.D0)
			{
				Canvas.SelectedTool = Tool.Free;
			}
			else if (e.KeyCode == Keys.F)
			{
				var objects = Canvas.GetSelectedObjects();
				Canvas.ScrollToWorldPosition(objects[0].Position);
			}

			label2.Text = Canvas.SelectedTool.ToString();
		}
	}
}
