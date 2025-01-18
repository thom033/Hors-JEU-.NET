using System;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Chemin du fichier de configuration
        string path = string.Empty;

        // Utiliser une boîte de dialogue pour sélectionner l'image
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = @"E:\S5\Prog\Foot\img";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Sélectionnez une image à traiter";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
        }

        if (string.IsNullOrEmpty(path))
        {
            Console.WriteLine("Le chemin de l'image est introuvable ou invalide.");
            return;
        }

        Mat img = CvInvoke.Imread(path);

        if (img.IsEmpty)
        {
            Console.WriteLine("Impossible de charger l'image.");
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

        Foot.Game game = new Foot.Game();
        game.Initialize(img);
        CvInvoke.Imshow("Match Analysis", img);
        CvInvoke.WaitKey(0);
    }
}