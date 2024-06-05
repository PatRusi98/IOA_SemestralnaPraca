using IOA_SemestralnaPraca.Algorithm;
using IOA_SemestralnaPraca.Essentials;
using IOA_SemestralnaPraca.Structures;
using IOA_SemestralnaPraca.Utils;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using static IOA_SemestralnaPraca.Utils.Enums;
using static IOA_SemestralnaPraca.Utils.Helpers;

namespace IOA_SemestralnaPraca
{
    public partial class Form1 : Form
    {
        private List<Node> nodes = new();
        private List<Edge> edges = new();
        private ForwardStar forwardStar;
        private Dijkstra dijkstra;
        private ClarkeWright clarkeWright;
        private FileHandler fileHandler = new();
        private int nextNodeID = 1;

        public Form1()
        {
            InitializeComponent(); // musi tu byt

            comboBox1.DisplayMember = "Description";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = Enum.GetValues(typeof(NodeType))
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .ToList();
        }

        private void CreateForwardStar()
        {
            forwardStar = new ForwardStar(nodes, edges);
            dijkstra = new Dijkstra(forwardStar, nodes);
        }

        private void DrawNetwork()
        {
            pictureBox1.MouseClick -= pictureBox1_MouseClick;
            pictureBox1.MouseClick += pictureBox1_MouseClick;

            int pictureBoxWidth = pictureBox1.Width;
            int pictureBoxHeight = pictureBox1.Height;
            int networkSize = 100;

            var bitmap = new Bitmap(pictureBoxWidth, pictureBoxHeight);
            var graphics = Graphics.FromImage(bitmap);
            var pen = new Pen(Color.Black);
            var brush = new SolidBrush(Color.Red);
            var font = new Font("Arial", 8);
            var boldFont = new Font("Arial", 8, FontStyle.Bold);
            var textBrush = new SolidBrush(Color.Black);
            var backgroundBrush = new SolidBrush(Color.White);
            var gridPen = new Pen(Color.LightGray);

            float scaleX = (float)pictureBoxWidth / networkSize;
            float scaleY = (float)pictureBoxHeight / networkSize;

            for (int i = 0; i <= networkSize; i += 10)
            {
                float x = i * scaleX;
                float y = i * scaleY;

                graphics.DrawLine(gridPen, x, 0, x, pictureBoxHeight);
                graphics.DrawLine(gridPen, 0, y, pictureBoxWidth, y);

                graphics.DrawString(i.ToString(), font, textBrush, x, 0);
                graphics.DrawString(i.ToString(), font, textBrush, 0, y);
            }

            // Draw edges
            foreach (var edge in edges)
            {
                var nodeA = edge.NodeA;
                var nodeB = edge.NodeB;
                float x1 = (float)nodeA.Coordinates.X * scaleX;
                float y1 = (float)nodeA.Coordinates.Y * scaleY;
                float x2 = (float)nodeB.Coordinates.X * scaleX;
                float y2 = (float)nodeB.Coordinates.Y * scaleY;

                graphics.DrawLine(pen, x1, y1, x2, y2);

                string edgeValue = Math.Round(edge.Distance, 2).ToString();
                float midX = (x1 + x2) / 2;
                float midY = (y1 + y2) / 2;

                var textSize = graphics.MeasureString(edgeValue, font);
                var textRect = new RectangleF(midX - textSize.Width / 2, midY - textSize.Height / 2, textSize.Width, textSize.Height);
                graphics.FillRectangle(backgroundBrush, textRect);
                graphics.DrawString(edgeValue, font, textBrush, textRect);
            }

            // Draw nodes
            foreach (var node in nodes)
            {
                float x = (float)node.Coordinates.X * scaleX;
                float y = (float)node.Coordinates.Y * scaleY;
                graphics.FillEllipse(brush, x - 5, y - 5, 10, 10);

                string nodeID = node.ID.ToString();
                var textSize = graphics.MeasureString(nodeID, boldFont);
                var textRect = new RectangleF(x + 5, y + 5, textSize.Width, textSize.Height);
                graphics.FillRectangle(backgroundBrush, textRect);
                graphics.DrawString(nodeID, boldFont, textBrush, textRect);

                if (node.Type == NodeType.PrimarySource)
                {
                    label11.Text = node.ID.ToString();
                }
            }

            pictureBox1.Image = bitmap;
        }

        private void DisplayRoutes(List<List<int>> routes)
        {
            listBox1.Items.Clear();
            foreach (var route in routes)
            {
                listBox1.Items.Add(string.Join(" -> ", route));
            }
        }

        private void SaveNetwork()
        {
            // ulozenie siete
        }

        private void LoadNetwork()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Súbor s vrcholmi";
            openFileDialog.Filter = "Text Files|*.txt";

            if (MessageBox.Show("Chceš použiť defaultne súbory?", "Načítanie siete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                nodes = fileHandler.LoadNodes("_data/_defaultNodes.txt");
                edges = fileHandler.LoadConnections("_data/_defaultEdges.txt", nodes);
            }
            else
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    nodes = fileHandler.LoadNodes(openFileDialog.FileName);
                }

                openFileDialog.Title = "Súbor s hranami";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    edges = fileHandler.LoadConnections(openFileDialog.FileName, nodes);
                }
            }

            nextNodeID = nodes.Count + 1;

            CreateForwardStar();
            UpdateNetworkConnectivityLabel();
            DrawNetwork();
        }

        private void UpdateNetworkConnectivityLabel()
        {
            if (IsNetworkConnected())
            {
                label12.Text = "súvislá";
            }
            else
            {
                label12.Text = "nesúvislá";
            }
        }

        #region DEEP FIRST SEARCH - na kontrolu suvislosti siete
        private bool IsNetworkConnected()
        {
            if (nodes.Count == 0)
                return false;

            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(nodes[0].ID);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    var neighbors = GetNeighbors(current);
                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
            }

            return visited.Count == nodes.Count;
        }

        private List<int> GetNeighbors(int nodeId)
        {
            var neighbors = new List<int>();
            int startIndex = forwardStar.NodePointers[nodeId - 1];
            int endIndex = (nodeId < forwardStar.NodePointers.Count) ? forwardStar.NodePointers[nodeId] : forwardStar.EdgesArray.Count;

            for (int i = startIndex; i < endIndex; i++)
            {
                neighbors.Add(forwardStar.EdgesArray[i].NodeB.ID);
            }

            return neighbors;
        }

        #endregion

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.ReadOnly = true;
            }
            else
            {
                textBox3.ReadOnly = false;
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(label10.Text, out int nodeId))
            {
                var node = nodes.FirstOrDefault(n => n.ID == nodeId);
                if (node != null)
                {
                    node.Coordinates.X = int.Parse(textBox1.Text);
                    node.Coordinates.Y = int.Parse(textBox6.Text);
                    node.Capacity = int.Parse(textBox2.Text);
                    node.Type = (NodeType)comboBox1.SelectedValue;

                    DrawNetwork();
                    UpdateNetworkConnectivityLabel();
                }
            }
            else if (int.TryParse(textBox4.Text, out int nodeAId) && int.TryParse(textBox5.Text, out int nodeBId))
            {
                var nodeA = nodes.FirstOrDefault(n => n.ID == nodeAId);
                var nodeB = nodes.FirstOrDefault(n => n.ID == nodeBId);

                if (nodeA != null && nodeB != null)
                {
                    var edge = edges.FirstOrDefault(e => (e.NodeA.ID == nodeAId && e.NodeB.ID == nodeBId) || (e.NodeA.ID == nodeBId && e.NodeB.ID == nodeAId));
                    if (edge == null)
                    {
                        edge = new Edge { NodeA = nodeA, NodeB = nodeB };
                        edges.Add(edge);
                    }

                    if (radioButton1.Checked)
                    {
                        edge.Distance = EuclideanDistance(nodeA.Coordinates, nodeB.Coordinates);
                    }
                    else
                    {
                        edge.Distance = double.Parse(textBox3.Text);
                    }

                    CreateForwardStar();
                    DrawNetwork();
                    UpdateNetworkConnectivityLabel();
                }
                else
                {
                    MessageBox.Show("Nie je možné pridať alebo aktualizovať hranu. Jeden z uzlov neexistuje.");
                }
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int vehicleCapacity = (int)numericUpDown1.Value;
            clarkeWright = new ClarkeWright(forwardStar, nodes, vehicleCapacity);
            var routes = clarkeWright.Execute();
            DisplayRoutes(routes);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveNetwork();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadNetwork();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float scaleX = (float)pictureBox1.Width / 100;
            float scaleY = (float)pictureBox1.Height / 100;

            if (e.Button == MouseButtons.Left)
            {
                foreach (var node in nodes)
                {
                    float x = (float)node.Coordinates.X * scaleX;
                    float y = (float)node.Coordinates.Y * scaleY;
                    var nodeRect = new RectangleF(x - 5, y - 5, 10, 10);

                    if (nodeRect.Contains(e.Location))
                    {
                        label10.Text = node.ID.ToString();
                        textBox1.Text = node.Coordinates.X.ToString();
                        textBox6.Text = node.Coordinates.Y.ToString();
                        textBox2.Text = node.Capacity.ToString();
                        comboBox1.SelectedValue = node.Type;
                        return;
                    }
                }

                foreach (var edge in edges)
                {
                    var nodeA = edge.NodeA;
                    var nodeB = edge.NodeB;
                    float x1 = (float)nodeA.Coordinates.X * scaleX;
                    float y1 = (float)nodeA.Coordinates.Y * scaleY;
                    float x2 = (float)nodeB.Coordinates.X * scaleX;
                    float y2 = (float)nodeB.Coordinates.Y * scaleY;

                    var edgePath = new GraphicsPath();
                    edgePath.AddLine(x1, y1, x2, y2);
                    var edgePen = new Pen(Color.Black, 10);
                    if (edgePath.IsOutlineVisible(e.Location, edgePen))
                    {
                        textBox4.Text = nodeA.ID.ToString();
                        textBox5.Text = nodeB.ID.ToString();
                        textBox3.Text = edge.Distance.ToString();
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (nodes.Any(n => n.Type == NodeType.PrimarySource) && (NodeType)comboBox1.SelectedValue == NodeType.PrimarySource)
                {
                    comboBox1.SelectedValue = NodeType.Customer;
                }

                float x = e.X / scaleX;
                float y = e.Y / scaleY;

                textBox1.Text = Math.Round(x, 3).ToString();
                textBox6.Text = Math.Round(y, 3).ToString();
                var newNode = new Node
                {
                    ID = nextNodeID++,
                    Coordinates = new Coordinates { X = Math.Round(x, 3), Y = Math.Round(y, 3) },
                    Capacity = 100,
                    Type = (NodeType)comboBox1.SelectedValue
                };

                nodes.Add(newNode);
                CreateForwardStar();
                DrawNetwork();
                UpdateNetworkConnectivityLabel();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nodes.Any(n => n.Type == NodeType.PrimarySource) && (NodeType)comboBox1.SelectedValue == NodeType.PrimarySource)
            {
                comboBox1.SelectedValue = NodeType.Customer;
            }

            if (int.TryParse(label10.Text, out int nodeId))
            {
                var node = nodes.FirstOrDefault(n => n.ID == nodeId);
                if (node != null)
                {
                    node.Coordinates.X = double.Parse(textBox1.Text);
                    node.Coordinates.Y = double.Parse(textBox6.Text);
                    node.Capacity = double.Parse(textBox2.Text);
                    node.Type = (NodeType)comboBox1.SelectedValue;

                    DrawNetwork();
                    UpdateNetworkConnectivityLabel();
                }
            }

            UpdateNetworkConnectivityLabel();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (int.TryParse(label10.Text, out int nodeId))
            {
                var node = nodes.FirstOrDefault(n => n.ID == nodeId);
                if (node != null && !edges.Any(e => e.NodeA.ID == nodeId || e.NodeB.ID == nodeId))
                {
                    nodes.Remove(node);
                    CreateForwardStar();
                    DrawNetwork();
                    UpdateNetworkConnectivityLabel();
                }
                else
                {
                    MessageBox.Show("Uzol nie je možné zmazať, pretože je pripojený k iným uzlom.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox4.Text, out int nodeAId) && int.TryParse(textBox5.Text, out int nodeBId))
            {
                var edge = edges.FirstOrDefault(e => (e.NodeA.ID == nodeAId && e.NodeB.ID == nodeBId) || (e.NodeA.ID == nodeBId && e.NodeB.ID == nodeAId));
                if (edge != null)
                {
                    edges.Remove(edge);
                    CreateForwardStar();
                    DrawNetwork();
                    UpdateNetworkConnectivityLabel();
                }
                else
                {
                    MessageBox.Show("Hranu nie je možné zmazať, pretože neexistuje.");
                }
            }
        }
    }
}
