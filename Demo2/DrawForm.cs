﻿using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace Demo2
{
	public partial class DrawForm : Form
	{
		public List<Node> nodes;
		public List<Edge> edges;
		Node startNode, endNode;
		static int NODE_RADIUS = 15;

		public DrawForm()
		{
			InitializeComponent();
			nodes = new List<Node>();
			edges = new List<Edge>();
		}

		private Node GetNodeAt(Point location)
		{
			//Tìm đỉnh ở vị trí xác định trên màn hình
			foreach (Node node in nodes)
			{
				int nodeSize = 2 * NODE_RADIUS;
				Rectangle nodeRect = new Rectangle(node.X - NODE_RADIUS, node.Y - NODE_RADIUS, nodeSize, nodeSize);

				if (nodeRect.Contains(location))
					return node;
			}

			return null;
		}

		private Edge GetEdgeOf(Node startNode, Node endNode)
		{
			foreach (Edge edge in edges)
			{
				if ((edge.StartNode == startNode && edge.EndNode == endNode) || (edge.StartNode == endNode && edge.EndNode == startNode))
				{
					return edge;
				}
			}

			return null;
		}
		

		private void myPanel_Paint(object sender, PaintEventArgs e)
		{
			Font font = new Font(Font.Name, NODE_RADIUS, FontStyle.Bold);
			foreach (Node node in nodes)
			{
				e.Graphics.FillEllipse(Brushes.RoyalBlue, node.X - NODE_RADIUS, node.Y - NODE_RADIUS, NODE_RADIUS * 2, NODE_RADIUS * 2);
				e.Graphics.DrawString(node.Label, font, Brushes.Black, node.X, node.Y);
			}

			foreach (Edge edge in edges)
			{
				e.Graphics.DrawLine(Pens.Black, edge.StartNode.X, edge.StartNode.Y, edge.EndNode.X, edge.EndNode.Y);
			
			}
		}
		

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Graph g = new Graph(nodes.Count);
			//thêm cung
			foreach (Edge edge in edges)
			{
				int startNode = Convert.ToInt16(edge.StartNode.Label);
				int endNode = Convert.ToInt16(edge.EndNode.Label);

				g.addEdge(startNode, endNode);
			}

			MainForm frm = (MainForm)this.ParentForm;
			frm.graph = g;

			MessageBox.Show("Cập nhật đồ thị thành công \nHãy tiếp tục thực hiện các chức năng khác.");
		}

		private void updateLabel(Node node)
		{
			int nodeIndex = Convert.ToInt16(node.Label);

			for (int i = 0; i < nodes.Count; i++)
			{
				int nodeLabel = Convert.ToInt16(nodes[i].Label);

				if (nodeLabel > nodeIndex)
				{
					nodeLabel--;
					nodes[i].Label = nodeLabel.ToString();
				}
			}

		}



		private void myPanel_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//Nếu chưa có node tại vị trí click thì tạo nút và thêm vào list
				if (GetNodeAt(e.Location) == null)
				{
					Node node = new Node() { X = e.X, Y = e.Y, Label = (nodes.Count + 1).ToString() };
					//Thêm node vào danh sách đỉnh
					nodes.Add(node);
				}
				else
				{
					if (startNode == null)
						startNode = GetNodeAt(e.Location);
					else
					{
						endNode = GetNodeAt(e.Location);
						if (GetNodeAt(e.Location) != startNode && nodes.IndexOf(startNode) >= 0)
						{
							Edge clickedEdge = GetEdgeOf(startNode, endNode);
							if (clickedEdge == null)
							{
								Edge edge = new Edge()
								{
									StartNode = startNode,
									EndNode = endNode,

								};


								edges.Add(edge);
							}
							else
							{


							}
						}
						startNode = null;
					}

				}
			}

			if (e.Button == MouseButtons.Right)
			{
				Node clickedNode = GetNodeAt(e.Location);
				if (clickedNode != null)
				{
					List<Edge> deleEdge = new List<Edge>();
					foreach (Edge edge in edges)
					{
						if (edge.StartNode == clickedNode || edge.EndNode == clickedNode)
							deleEdge.Add(edge);
					}
					foreach (Edge edge in deleEdge)
					{
						edges.Remove(edge);
					}
					updateLabel(clickedNode);
					nodes.Remove(clickedNode);
				}
			}

			Refresh();
		}

		private Node draggingNode = null;

		private void myPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				draggingNode = GetNodeAt(e.Location);
			}
		}

		private void myPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (draggingNode != null)
			{
				draggingNode.X = e.X;
				draggingNode.Y = e.Y;
				Refresh();
			}
		}

		private void myPanel_MouseUp(object sender, MouseEventArgs e)
		{
			draggingNode = null;
		}
	

		private void DrawForm_Load(object sender, EventArgs e)
		{
			

		}
	}
}