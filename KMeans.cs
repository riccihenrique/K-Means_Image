using System;
using System.Drawing;
using System.Windows.Forms;

namespace K_Means
{
    public partial class KMeans : Form
    {
        private Image image;
        private Bitmap imageBitmap;

        public KMeans()
        {
            InitializeComponent();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.gif;*.bmp;*.png;*.jpeg)|*.jpg;*.gif;*.bmp;*.png;*.jpeg";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog.FileName);
                pbOriginal.Image = image;
                pbOriginal.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            pbOriginal.Image = null;
            pbSegmentada.Image = null;
        }

        private void btnSegmentar_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image.Height, image.Width);
            imageBitmap = (Bitmap)image;
            K_Means kmeans = new K_Means();
            kmeans.Compute(imageBitmap, out imgDest);
            pbSegmentada.Image = imgDest;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
