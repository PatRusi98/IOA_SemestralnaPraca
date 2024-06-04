using IOA_SemestralnaPraca.Algorithm;
using IOA_SemestralnaPraca.Essentials;
using IOA_SemestralnaPraca.Structures;
using IOA_SemestralnaPraca.Utils;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using static IOA_SemestralnaPraca.Utils.Enums;

namespace IOA_SemestralnaPraca
{
    public partial class Form1 : Form
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private ForwardStar forwardStar;
        private Dijkstra dijkstra;
        private ClarkeWright clarkeWright;
        private FileHandler fileHandler = new FileHandler();

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

            CreateForwardStar();
            DrawNetwork();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

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
                    comboBox1.SelectedItem = node.Type.ToString();
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
    }
}
