using Emgu.CV;
using Foot;
using System.Drawing;

public class Program
{
    static void Main(string[] args)
    {
        // Chemin du fichier de configuration

        // Récupère le chemin de l'image depuis le fichier de configuration
        string path = @"E:\S5\Prog\Foot\img\exam3.jpg";


        if (string.IsNullOrEmpty(path))
        {
            Console.WriteLine("Le chemin de l'image est introuvable ou invalide dans le fichier de configuration.");
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

        Game game = new Game();
        game.Initialize(img);
        CvInvoke.Imshow("Match Analysis", img);
        CvInvoke.WaitKey(0);
    }

    private static string GetConfigValue(string key, string configFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            Console.WriteLine($"Fichier de configuration introuvable : {configFilePath}");
            return null;
        }

        foreach (var line in File.ReadAllLines(configFilePath))
        {
            if (line.StartsWith(key + "="))
            {
                return line.Substring(key.Length + 1).Trim();
            }
        }

        Console.WriteLine($"Clé introuvable dans le fichier de configuration : {key}");
        return null;
    }

}