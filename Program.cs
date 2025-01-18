using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Foot;

public class ImageUploaderForm : Form
{
    private Button importButton;
    private Button verifyButton;
    private PictureBox pictureBox;
    private string imagePath;

    public string ImagePath => imagePath;

    public ImageUploaderForm()
    {
        // Initialisation du bouton "Importer Image"
        importButton = new Button
        {
            Text = "Importer Image",
            Dock = DockStyle.Top
        };
        importButton.Click += ImportButton_Click;

        // Initialisation du bouton "Vérifier"
        verifyButton = new Button
        {
            Text = "Vérifier",
            Dock = DockStyle.Top
        };
        verifyButton.Click += VerifyButton_Click;

        // Initialisation du PictureBox pour afficher l'image importée
        pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom
        };

        // Ajout des contrôles au formulaire
        Controls.Add(pictureBox);
        Controls.Add(verifyButton);
        Controls.Add(importButton);

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

        // Redimensionner l'image si elle est trop grande
        const int MAX_WIDTH = 1280;
        const int MAX_HEIGHT = 720;

        if (img.Width > MAX_WIDTH || img.Height > MAX_HEIGHT)
        {
            double scale = Math.Min((double)MAX_WIDTH / img.Width, (double)MAX_HEIGHT / img.Height);
            Size newSize = new Size((int)(img.Width * scale), (int)(img.Height * scale));
            Mat resizedImg = new Mat();
            CvInvoke.Resize(img, resizedImg, newSize);
            img = resizedImg;
        }

        // Mettre à jour le PictureBox avec l'image redimensionnée
        pictureBox.Image = img.ToBitmap();

        Game game = new Game();
        game.Initialize(img);
        CvInvoke.Imshow("Match Analysis", img);
        CvInvoke.WaitKey(0);
    }
}

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        ImageUploaderForm uploaderForm = new ImageUploaderForm();
        Application.Run(uploaderForm);
    }
}