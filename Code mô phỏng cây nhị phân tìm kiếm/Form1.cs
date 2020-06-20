using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Y2Collections;
using System.Drawing.Drawing2D;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

            this.Text = Application.ProductName + " " + Application.ProductVersion;

            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            lblMessage.Text = String.Empty;
            lblNodeCount.Text = String.Empty;

            btnGenerateTree_Click(null, null);
       }

        private void btnGenerateTree_Click(object sender, EventArgs e)
        {
            bsTreePanel1.GenerateTree((int)numSize.Value,
                (int)numMin.Value,(int)numMax.Value);
            
                UpdateInfo();
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = new Bitmap(bsTreePanel1.Image);

                Graphics g = Graphics.FromImage(img);                                

                System.Drawing.Imaging.ImageFormat format;
                switch (saveFileDialog1.FilterIndex)
                {                        
                    case 1:
                        format=System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case 2:
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        g.Clear(Color.White);
                        break;  
                    case 3:
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                        g.Clear(Color.White);
                        break;
                    default:
                        format=System.Drawing.Imaging.ImageFormat.Png; break;
                }
                                                

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawImage(bsTreePanel1.Image, 0, 0);

                    Brush b=new SolidBrush(Color.FromArgb(100,Color.Cyan));
                    g.FillRectangle(b,0,img.Height-32,300,32);
                    
                
                
                img.Save(saveFileDialog1.FileName, format);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            if (!bsTreePanel1.SearchNode((int)numericUpDown1.Value))
                lblMessage.Text = "Tree does not contain value " + numericUpDown1.Value;
            else
                UpdateInfo();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            if(!bsTreePanel1.DeleteNode((int)numericUpDown1.Value))
                lblMessage.Text = "Tree does not contain value " + numericUpDown1.Value;
            else
                UpdateInfo();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            if (!bsTreePanel1.AddNode((int)numericUpDown1.Value))
                lblMessage.Text = "Tree already contain an node with that value";
            else
                UpdateInfo();
        }

        void UpdateInfo()
        {
            lblNodeCount.Text = "Node Count: " + bsTreePanel1.NodeCount;
            lblTreeHeight.Text = "Tree Height: " + bsTreePanel1.TreeHeight;
        }

        private void btnInOrderTraverse_Click(object sender, EventArgs e)
        {
            bsTreePanel1.InOrderTraverse();            
        }

        private void btnPreOrderTraverse_Click(object sender, EventArgs e)
        {
            bsTreePanel1.PreOrderTraverse();  
        }

        private void btnPostOrderTraverse_Click(object sender, EventArgs e)
        {
            bsTreePanel1.PostOrderTraverse();  
        }

       

    }
}
