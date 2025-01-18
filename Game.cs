using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace Foot
{
    internal class Game
    {
        public Team Red { get; set; } = new Team();

        public Team Blue { get; set; } = new Team();

        public Team TeamThatHasBall { get; set; } = new Team();

        public Team TeamHautGoal { get; set; } = new Team();

        public Point BallPosition { get; set; }

        // Initialisation du match
        public void Initialize(Mat image)
        {
            Mat hsvImage = new Mat();
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            // Ireto initialisation ireto tsy maintsy milahatra
            InitializeBall(hsvImage, image);
            InitializeAllTeams(image);
            InitializeTeamAmbonyGoal();
            InitializeTeamTokonyHananaBol();

            // PRINT Maivana
            foreach (var player in Red.PlayerList)
            {
                Console.WriteLine($"Red {player.Number} sur {player.PositionPlayer.Y} avec Diametre {player.Diametre}");
            }
            Console.WriteLine("-------------------------------------------------------------------");
            foreach (var player in Blue.PlayerList)
            {
                Console.WriteLine($"Blue {player.Number} sur {player.PositionPlayer.Y} avec Diametre {player.Diametre}");
            }
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Equipe qui a le ballon: {TeamThatHasBall.Name}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Team that has the goeal eny ambony: {TeamHautGoal.Name}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Dernier defenseur Red: {Red.DernierDefenseur(TeamHautGoal.Name == Red.Name).Number}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Dernier defenseur Blue: {Blue.DernierDefenseur(TeamHautGoal.Name == Blue.Name).Number}");
            Console.WriteLine("-------------------------------------------------------------------");
            Team Defense = TeamThatHasBall.Name == "Red" ? Blue : Red;
            List<Player> offSide = Team.GetOffSide(TeamThatHasBall, Defense, TeamHautGoal, image);

            foreach (var player in offSide)
            {
                Console.WriteLine($"Hors-jeu: {player.Number}");
            }
            Console.WriteLine("-------------------------------------------------------------------");
            List<Player> okSide = Team.GetOkSide(TeamThatHasBall, Defense, TeamHautGoal, BallPosition, image);

            foreach (var player in okSide)
            {
                Console.WriteLine($"Mety: {player.Number}");
            }
        }

        // Mijery Team tokony hanana anle bol
        public void InitializeTeamTokonyHananaBol()
        {
            double max = double.MaxValue;
            foreach (var player in Blue.PlayerList)
            {
                Point playerCentre = new Point((int)(player.PositionPlayer.X + player.Diametre / 2), (int)(player.PositionPlayer.Y + (player.Diametre / 2)));
                double temp = UtilFoot.EuclideanDistance(BallPosition, playerCentre);
                //Console.WriteLine($"Bleu: {temp} player {player.Number}");
                if (max > temp)
                {
                    max = temp;     // Get minimum dans les equipes bleus
                }
            }

            double minBlue = max;

            foreach (var player in Red.PlayerList)
            {
                Point playerCentre = new Point((int)(player.PositionPlayer.X + player.Diametre / 2), (int)(player.PositionPlayer.Y + (player.Diametre / 2)));
                double tempRed = UtilFoot.EuclideanDistance(BallPosition, playerCentre);
                //Console.WriteLine($"Red: {tempRed} player {player.Number}");
                if (max > tempRed)
                {
                    max = tempRed;
                }
            }
            TeamThatHasBall = minBlue == max ? Blue : Red;
        }

        // Mijery hoe iza no goal ambony
        public void InitializeTeamAmbonyGoal()
        {
            double max = double.MaxValue;
            foreach (var player in Blue.PlayerList)
            {
                if (max > player.PositionPlayer.Y)
                {
                    max = player.PositionPlayer.Y;     // Get minimum dans les equipes bleus
                }
            }

            double minBlue = max;

            foreach (var player in Red.PlayerList)
            {
                if (max > player.PositionPlayer.Y)
                {
                    max = player.PositionPlayer.Y;
                }
            }
            TeamHautGoal = minBlue == max ? Blue : Red;
        }


        // Initialize toutes les teams
        public void InitializeAllTeams(Mat image)
        {
            int redPlayerDebut = 1;     // Atomboka 1 numero RED
            int bluePlayerDebut = 1;    // Atomboka 1 numero Bleu
            Mat hsvImage = new Mat();
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);
            var blueMask = UtilFoot.DetectColor(hsvImage, new Hsv(90, 50, 50), new Hsv(130, 255, 255));

            var redMask1 = UtilFoot.DetectColor(hsvImage, new Hsv(0, 120, 70), new Hsv(10, 255, 255));
            var redMask2 = UtilFoot.DetectColor(hsvImage, new Hsv(160, 120, 70), new Hsv(180, 255, 255));
            Mat redMask = new Mat();
            CvInvoke.BitwiseOr(redMask1, redMask2, redMask);

            // Initialisation du rouge
            Red = Team.InitializeTeam(image, redMask, Color.Red, "Red", ref redPlayerDebut, BallPosition);

            // Et bleu
            Blue = Team.InitializeTeam(image, blueMask, Color.Blue, "Blue", ref bluePlayerDebut, BallPosition);
        }


        // Initialisation de la balle
        public void InitializeBall(Mat hsvImage, Mat image)
        {
            // Création de plusieurs masques pour différentes nuances de noir
            var masks = new List<Mat>();

            // Noir pur
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 0), new Hsv(180, 255, 30)));

            // Noir avec saturation plus élevée
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 0), new Hsv(180, 100, 50)));

            // Noir grisâtre
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 20), new Hsv(180, 50, 80)));

            Mat combinedMask = new Mat();
            if (masks.Count > 0)
            {
                combinedMask = masks[0].Clone();
                for (int i = 1; i < masks.Count; i++)
                {
                    CvInvoke.BitwiseOr(combinedMask, masks[i], combinedMask);
                }
            }

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(combinedMask, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contours.Size; i++)
            {
                VectorOfPoint contour = contours[i];
                double area = CvInvoke.ContourArea(contour);
                Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);

                if (area > 50 && area < 500 && (double)boundingBox.Width / boundingBox.Height < 1.2)
                {
                    BallPosition = new Point(boundingBox.X + boundingBox.Width / 2, boundingBox.Y + boundingBox.Height / 2);
                    CvInvoke.Circle(image, BallPosition, 10, new Bgr(Color.Yellow).MCvScalar, 2);
                    Console.WriteLine("Balle initialisée");

                    // Libération des ressources
                    foreach (var mask in masks)
                    {
                        mask.Dispose();
                    }
                    combinedMask.Dispose();
                    return;
                }
            }

            // Libération des ressources si aucune balle n'est trouvée
            foreach (var mask in masks)
            {
                mask.Dispose();
            }
            combinedMask.Dispose();
            Console.WriteLine("Aucune balle détectée");
        }

    }
}
