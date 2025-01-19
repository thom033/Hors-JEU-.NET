using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Foot;
public class ImageUploaderForm : Form
{
    private Button importButton;
    private Button importButton2;
    private Button verifyButton;
    private Button verifyButton2;
    private PictureBox pictureBox;
    private PictureBox pictureBox2;
    private string imagePath;
    private string imagePath2;
    private Game game1;
    private Game game2;

    public string ImagePath => imagePath;
    public string ImagePath2 => imagePath2;

    public ImageUploaderForm()
    {
        // Initialisation des boutons et PictureBox
        importButton = new Button { Text = "Importer Image", Dock = DockStyle.Top };
        importButton.Click += ImportButton_Click;

        importButton2 = new Button { Text = "Importer Image 2", Dock = DockStyle.Top };
        importButton2.Click += ImportButton_Click2;

        verifyButton = new Button { Text = "Vérifier", Dock = DockStyle.Top };
        verifyButton.Click += VerifyButton_Click;

        verifyButton2 = new Button { Text = "Vérifier 2", Dock = DockStyle.Top };
        verifyButton2.Click += VerifyButton_Click2;

        pictureBox = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage };
        pictureBox2 = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage };

        // Configuration du TableLayoutPanel
        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Ajout des contrôles au TableLayoutPanel
        tableLayoutPanel.Controls.Add(importButton, 0, 0);
        tableLayoutPanel.Controls.Add(importButton2, 1, 0);
        tableLayoutPanel.Controls.Add(verifyButton, 0, 1);
        tableLayoutPanel.Controls.Add(verifyButton2, 1, 1);
        tableLayoutPanel.Controls.Add(pictureBox, 0, 2);
        tableLayoutPanel.Controls.Add(pictureBox2, 1, 2);

        // Ajout du TableLayoutPanel au formulaire
        Controls.Add(tableLayoutPanel);

        // Configuration du formulaire
        this.WindowState = FormWindowState.Maximized;
    }

    private void ImportButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName;
                pictureBox.Image = System.Drawing.Image.FromFile(imagePath);
                MessageBox.Show("Image importée avec succès: " + imagePath);
            }
            else
            {
                MessageBox.Show("Aucune image sélectionnée.");
            }
        }
    }

    private void ImportButton_Click2(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath2 = openFileDialog.FileName;
                pictureBox2.Image = System.Drawing.Image.FromFile(imagePath2);
                MessageBox.Show("Image importée avec succès: " + imagePath2);
            }
            else
            {
                MessageBox.Show("Aucune image sélectionnée.");
            }
        }
    }

    private void VerifyButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            MessageBox.Show("Veuillez importer une image d'abord.");
            return;
        }

        Mat img = CvInvoke.Imread(imagePath);

        if (img.IsEmpty)
        {
            MessageBox.Show("Impossible de charger l'image.");
            return;
        }

        game1 = new Game();
        game1.Initialize(img);
        pictureBox.Image = img.ToBitmap();
    }

    private void VerifyButton_Click2(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(imagePath2))
        {
            MessageBox.Show("Veuillez importer une image d'abord.");
            return;
        }

        Mat img = CvInvoke.Imread(imagePath2);

        if (img.IsEmpty)
        {
            MessageBox.Show("Impossible de charger l'image.");
            return;
        }

        game2 = new Game();
        game2.Initialize(img);
        game2.ValidateGoal(game1);
        pictureBox2.Image = img.ToBitmap();
    }

    // private Image ResizeImage(Image image, int width, int height)
    // {
    //     var destRect = new Rectangle(0, 0, width, height);
    //     var destImage = new Bitmap(width, height);

    //     destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

    //     using (var graphics = Graphics.FromImage(destImage))
    //     {
    //         graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
    //         graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
    //         graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
    //         graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
    //         graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

    //         using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
    //         {
    //             wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
    //             graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
    //         }
    //     }

    //     return destImage;
    // }
}