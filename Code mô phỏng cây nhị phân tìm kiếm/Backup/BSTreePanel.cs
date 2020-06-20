using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Y2Collections;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication2
{
    partial class BSTreePanel : UserControl
    {
        #region Properties

        const int RADIUS = 20;
        const int DIAMETER = RADIUS * 2;
        const int HOR_DISTANCE = 70;
        const int VER_DISTANCE = 100;

        Pen _penNormal;
        Pen _penHighLight;
        Brush _brush;
        Font _font;

        Y2BinaryTree<int> _Tree;

        float _leftRoot;
        float _minLeft;
        float _maxLeft;

        float _ratio;

        public Image Image
        {
            get { return pictureBox1.Image; }
        }

        Graphics _g;

        Queue<int> _queue;

        public int NodeCount
        {
            get { return _Tree.Count; }
        }
        public int TreeHeight
        {
            get { return _Tree.GetHeight(); }
        }
        #endregion

        public BSTreePanel()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);

            _brush = new LinearGradientBrush(new Rectangle(0, VER_DISTANCE / 2, 100, VER_DISTANCE),
        Color.LightSkyBlue, Color.White, LinearGradientMode.Vertical);
            _penNormal = new Pen(Color.DarkBlue, 2);
            _penHighLight = new Pen(Color.Red, 3);
            _font = new Font("Arial", 18);
            _Tree = new Y2BinaryTree<int>(50);


        }
        private void BSTreePanel_Load(object sender, EventArgs e)
        {

        }
        public void GenerateTree(int size, int min, int max)
        {
            if (min >= max || (max - min < size))
            {
                MessageBox.Show("Invalid parameters!", "OMG");
                return;
            }

            _queue = null;

            _Tree.Clear();

            Random rnd = new Random();

            int[] arr = new int[size];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = rnd.Next(min, max);
            _Tree.Add(arr);

            

            BeginDraw(true);
        }        
        private void BeginDraw()
        {
            BeginDraw(false);
        }
        private void BeginDraw(bool resetAll)
        {
            _ratio = (_Tree.Count * 200) / this.Width;// +RADIUS / this.Width;
                            
            if (resetAll)
            {
                if (pictureBox1.Image != null)
                    pictureBox1.Image.Dispose();

                _leftRoot = pictureBox1.Width / 2;
                _minLeft = _leftRoot;
                _maxLeft = _leftRoot;

                using (Graphics g = pictureBox1.CreateGraphics())
                {
                    CalculateSize(g, _leftRoot, _Tree.Root);
                }



                _leftRoot -= _minLeft;
                if (_Tree.Count < 4)
                    _leftRoot += 100;
                _maxLeft -= _minLeft;
                _minLeft = 0;
                _leftRoot += 100;
                _maxLeft += 250;
            }
            Image img = new Bitmap((int)_maxLeft, (_Tree.GetHeight() + 1) * VER_DISTANCE + 100);

            if (_g != null)
            {
                _g.Dispose();
                pictureBox1.Image.Dispose();
            }

            pictureBox1.Image = img;// new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _g = Graphics.FromImage(pictureBox1.Image);
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawTreeNode(_g, new PointF(_leftRoot, DIAMETER * 2), _Tree.Root, true);

            

            //if (_Tree.Count > 20)
            //    ZoomIn();
            //else
            //    ZoomOut();
        }

        private void CalculateSize(Graphics g, float left, Y2BinaryTreeNode<int> node)
        {
            if (node != null)
            {
                string text = node.Value.ToString();
                SizeF size = g.MeasureString(text, pictureBox1.Font);

                float x = left - (RADIUS + size.Width) / 2;

                if (node.HasLeftChild)
                {
                    float p2 = x - Math.Abs(node.Value - node.LeftChild.Value) * _ratio;
                    if (p2 < _minLeft)
                        _minLeft = p2;
                    if (p2 > _maxLeft)
                        _maxLeft = p2;

                    CalculateSize(g, p2, node.LeftChild);
                }
                if (node.HasRightChild)
                {
                    float p2 = x + Math.Abs(node.RightChild.Value - node.Value) * _ratio;

                    if (p2 < _minLeft)
                        _minLeft = p2;
                    if (p2 > _maxLeft)
                        _maxLeft = p2;

                    CalculateSize(g, p2, node.RightChild);
                }
                
                    
            }
        }

        private void DrawTreeNode(Graphics g, PointF p, Y2BinaryTreeNode<int> node, bool highlight)
        {

            if (node != null)
            {
                string text = node.Value.ToString();
                SizeF size = g.MeasureString(text, _font);

                float ellipseWidth = RADIUS + size.Width;
                float ellipseHeight = RADIUS + size.Height;

                float left = p.X - ellipseWidth / 2;
                float top = p.Y - ellipseHeight / 2;

                Pen pen = _penNormal;


                if (node.HasLeftChild)
                {
                    PointF p1 = p;
                    PointF p2 = p;

                    p1.X = left + ellipseWidth / 2;


                    p2.X -= (node.Value - node.LeftChild.Value) * _ratio;

                    p2.Y += VER_DISTANCE;

                    bool hlight = false;

                    if (_queue != null && _queue.Count > 0)
                    {
                        if (_queue.Peek() == node.LeftChild.Value)
                        {
                            _queue.Dequeue();
                            pen = _penHighLight;
                            hlight = true;
                        }
                    }
                    g.DrawLine(pen, p1, p2);

                    DrawTreeNode(g, p2, node.LeftChild, hlight);

                    if (p2.X < _minLeft)
                        _minLeft = p2.X;
                    if (p2.X > _maxLeft)
                        _maxLeft = p2.X;
                }
                if (node.HasRightChild)
                {
                    PointF p1 = p;
                    PointF p2 = p;
                    p1.X = left + ellipseWidth / 2;

                    p2.X += (node.RightChild.Value - node.Value) * _ratio;

                    p2.Y += VER_DISTANCE;

                    pen = _penNormal;
                    bool hlight = false;
                    if (_queue != null && _queue.Count > 0)
                    {
                        if (_queue.Peek() == node.RightChild.Value)
                        {
                            _queue.Dequeue();
                            pen = _penHighLight;
                            hlight = true;
                        }
                    }
                    g.DrawLine(pen, p1, p2);

                    DrawTreeNode(g, p2, node.RightChild, hlight);

                    if (p2.X < _minLeft)
                        _minLeft = p2.X;
                    if (p2.X > _maxLeft)
                        _maxLeft = p2.X;
                }

                pen = highlight ? _penHighLight : _penNormal;

                g.FillEllipse(_brush, left, top, ellipseWidth, ellipseHeight);
                g.DrawEllipse(pen, left, top, ellipseWidth, ellipseHeight);

                g.DrawString(text, _font, Brushes.Black, left + RADIUS / 2, top + RADIUS / 2);

            }
        }

        public bool AddNode(int value)
        {
            if (!_Tree.Add(value))
            {
                txtOutput.Text = "Tree already contain an node with value " + value;
                txtOutput.SelectAll();
                return false;
            }
            txtOutput.Clear();            
            BeginDraw(true);
            return true;
        }

        public bool SearchNode(int value)
        {
            _queue = _Tree.FindPath(value);
            if (_queue == null)
            {
                txtOutput.Text = "Tree does not contain value " + value;
                txtOutput.SelectAll();
                return false;
            }
            txtOutput.Clear();
            BeginDraw();
            return true;
        }

        public bool DeleteNode(int value)
        {
            if (!_Tree.Remove(value))
            {
                txtOutput.Text = "Tree does not contain value " + value;
                txtOutput.SelectAll();
                return false;
            }
            txtOutput.Clear();
            BeginDraw();
            return true;
        }

        public void InOrderTraverse()
        {
            txtOutput.Clear();
            StringBuilder str = new StringBuilder("In-Order Traversal: ");
            List<int> list = _Tree.InOrderTraverse();
            list.ForEach(i => str.Append(i).Append(", "));

            txtOutput.Text = str.ToString().Remove(str.Length - 2);
            list.Clear();
            list = null;
        }
        public void PreOrderTraverse()
        {
            txtOutput.Clear();
            StringBuilder str = new StringBuilder("Pre-Order Traversal: ");
            List<int> list = _Tree.PreOrderTraverse();
            list.ForEach(i => str.Append(i).Append(", "));

            txtOutput.Text = str.ToString().Remove(str.Length - 2);
            list.Clear();
            list = null;
        }
        public void PostOrderTraverse()
        {
            txtOutput.Clear();
            StringBuilder str = new StringBuilder("Post-Order Traversal: ");
            List<int> list = _Tree.PostOrderTraverse();
            list.ForEach(i => str.Append(i).Append(", "));

            txtOutput.Text = str.ToString().Remove(str.Length - 2);
            list.Clear();
            list = null;
        }

        private void txtOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter)
                return;
            try
            {
                string str = txtOutput.Text;
                str = Regex.Replace(str, @"\s+", " ");
                str = Regex.Replace(str, @",\s", " ");
                string[] s = str.Split(' ', ',');
                switch (s[0].ToLower())
                {
                    case "random":
                        GenerateTree(int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                        break;
                    case "clear":
                        _Tree.Clear();
                        txtOutput.Clear();
                        BeginDraw();
                        break;
                    case "add":
                        for (int i = 1; i < s.Length; i++)
                            AddNode(int.Parse(s[i]));
                        break;
                    case "del":
                        DeleteNode(int.Parse(s[1]));
                        break;
                    case "delete":
                        DeleteNode(int.Parse(s[1]));
                        break;
                    case "find":
                        SearchNode(int.Parse(s[1]));
                        break;
                    case "search":
                        SearchNode(int.Parse(s[1]));
                        break;
                    case "inorder":
                        InOrderTraverse();
                        break;
                    case "preorder":
                        PreOrderTraverse();
                        break;
                    case "Postorder":
                        PostOrderTraverse();
                        break;
                    case "exit":
                        this.ParentForm.Close();
                        break;
                    default:
                        txtOutput.Text = "Invalid command";
                        break;
                }

            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.Message;
                txtOutput.SelectAll();
            }
            finally
            {
                txtOutput.SelectAll();
            }

        }


        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Dock == DockStyle.Fill)            
                pictureBox1.Dock = DockStyle.None;

            pictureBox1.Width = (int)(pictureBox1.Width * 1.5);
            pictureBox1.Height = (int)(pictureBox1.Height * 1.5);
            
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Dock == DockStyle.Fill)
                pictureBox1.Dock = DockStyle.None;

            pictureBox1.Width = (int)(pictureBox1.Width /1.5);
            pictureBox1.Height = (int)(pictureBox1.Height/1.5);

        }

        private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Size = pictureBox1.Parent.Size;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            showAllToolStripMenuItem.Checked = pictureBox1.Dock == DockStyle.Fill;
        }


    }
}

