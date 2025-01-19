using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Npgsql;

namespace Foot
{
    internal class Game
    {
        public Team Red { get; set; } = new Team();

        public Team Blue { get; set; } = new Team();

        public Team TeamThatHasBall { get; set; } = new Team();

        public Player PlayerThatHasBall { get; set; }

        public Team TeamHautGoal { get; set; } = new Team();

        public Point BallPosition { get; set; }

        public Rectangle TopGoal { get; private set; }
        public Rectangle BottomGoal { get; private set; }

        public Boolean goal { get; set; } = false;

        // Attributs pour stocker les positions des cages
        private List<Rectangle> detectedGoals = new List<Rectangle>();
        public List<Rectangle> DetectedGoals
        {
            get { return detectedGoals; }
            private set { detectedGoals = value; }
        }

        // Initialisation du match
        public void Initialize(Mat image)
        {
            Mat hsvImage = new Mat();
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            // Ireto initialisation ireto tsy maintsy milahatra
            InitializeBall(hsvImage, image);
            InitializeGoals(image);
            InitializeAllTeams(image);
            InitializeTeamAmbonyGoal();
            InitializeTeamTokonyHananaBol();
            IsGoal();

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
            Console.WriteLine($"Team that has the goeal eny ambony: {TeamHautGoal.Name}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Dernier defenseur Red: {Red.DernierDefenseur(TeamHautGoal.Name == Red.Name).Number}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Dernier defenseur Blue: {Blue.DernierDefenseur(TeamHautGoal.Name == Blue.Name).Number}");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"EstBUT : {goal}");
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
            VerifyPlayerThatHasBallOffside(offSide);
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine($"Equipe qui a le ballon: {TeamThatHasBall.Name}");
            Console.WriteLine($"Player qui a le ballon: {PlayerThatHasBall.Number} est {(PlayerThatHasBall.isOffSide ? "hors-jeu" : "en jeu")}");

            // PlayerThatHasBall.Number
            // TeamThatHasBall.Name
            // points = 1

        }

        // Mijery Team tokony hanana anle bol
        public void InitializeTeamTokonyHananaBol()
        {
            double max = double.MaxValue;
            Player closestPlayer = null;
            foreach (var player in Blue.PlayerList)
            {
                Point playerCentre = new Point((int)(player.PositionPlayer.X + player.Diametre / 2), (int)(player.PositionPlayer.Y + (player.Diametre / 2)));
                double temp = UtilFoot.EuclideanDistance(BallPosition, playerCentre);
                //Console.WriteLine($"Bleu: {temp} player {player.Number}");
                if (max > temp)
                {
                    max = temp;     // Get minimum dans les equipes bleus
                    closestPlayer = player;
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
                    closestPlayer = player;
                }
            }
            TeamThatHasBall = minBlue == max ? Blue : Red;
            PlayerThatHasBall = closestPlayer;
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

        public void InitializeGoals(Mat image)
        {
            Mat hsvImage = new Mat();
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            // Création des masques pour détecter les cages de couleur #2F2F2F avec plus de flexibilité
            var masks = new List<Mat>();

            // Plage principale pour #2F2F2F avec plus de tolérance
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 30), new Hsv(180, 40, 70)));

            // Plage secondaire pour variations plus claires/foncées
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 25), new Hsv(180, 50, 80)));

            // Plage pour les lignes ultra-fines
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 35), new Hsv(180, 30, 90)));

            // Plage pour les lignes très claires
            masks.Add(UtilFoot.DetectColor(hsvImage, new Hsv(0, 0, 40), new Hsv(180, 25, 100)));

            Mat combinedMask = new Mat();
            if (masks.Count > 0)
            {
                combinedMask = masks[0].Clone();
                for (int i = 1; i < masks.Count; i++)
                {
                    CvInvoke.BitwiseOr(combinedMask, masks[i], combinedMask);
                }
            }

            // Prétraitement amélioré pour les lignes fines
            // 1. Réduction du bruit
            CvInvoke.GaussianBlur(combinedMask, combinedMask, new Size(3, 3), 0);

            // 2. Amélioration du contraste pour les lignes fines
            CvInvoke.Threshold(combinedMask, combinedMask, 127, 255, ThresholdType.Binary);

            // 3. Détection des lignes horizontales (plus sensible)
            Mat horizontalKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(21, 1), new Point(-1, -1));
            Mat horizontalMask = combinedMask.Clone();
            CvInvoke.MorphologyEx(horizontalMask, horizontalMask, MorphOp.Open, horizontalKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // 4. Détection des lignes verticales (plus sensible)
            Mat verticalKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(1, 21), new Point(-1, -1));
            Mat verticalMask = combinedMask.Clone();
            CvInvoke.MorphologyEx(verticalMask, verticalMask, MorphOp.Open, verticalKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // 5. Combiner les masques et renforcer les connexions
            CvInvoke.BitwiseOr(horizontalMask, verticalMask, combinedMask);
            Mat dilateKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(combinedMask, combinedMask, dilateKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(combinedMask, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Vider la liste des cages détectées précédemment
            DetectedGoals.Clear();

            for (int i = 0; i < contours.Size; i++)
            {
                using (VectorOfPoint contour = contours[i])
                {
                    Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);
                    double aspectRatio = (double)boundingBox.Width / boundingBox.Height;

                    // Critères ultra-flexibles
                    bool isNearTopOrBottom = boundingBox.Y < image.Height * 0.25 || boundingBox.Y > image.Height * 0.75;
                    bool hasGoodWidth = boundingBox.Width > image.Width * 0.08;
                    bool hasMinimalHeight = boundingBox.Height >= 1;
                    bool hasMaxHeight = boundingBox.Height < image.Height * 0.25;
                    bool hasGoodRatio = aspectRatio > 1.2;

                    if (isNearTopOrBottom && hasGoodWidth && hasMinimalHeight && hasMaxHeight && hasGoodRatio)
                    {
                        using (VectorOfPoint approx = new VectorOfPoint())
                        {
                            CvInvoke.ApproxPolyDP(contour, approx, 0.08 * CvInvoke.ArcLength(contour, true), true);

                            if (approx.Size >= 4)
                            {
                                DetectedGoals.Add(boundingBox);
                            }
                        }
                    }
                }
            }

            // Trier les cages par position Y
            DetectedGoals = DetectedGoals.OrderBy(r => r.Y).ToList();

            if (DetectedGoals.Count >= 2)
            {
                TopGoal = DetectedGoals[0];
                BottomGoal = DetectedGoals[DetectedGoals.Count - 1];

                // Visualisation
                CvInvoke.Rectangle(image, TopGoal, new Bgr(Color.Yellow).MCvScalar, 2);
                CvInvoke.Rectangle(image, BottomGoal, new Bgr(Color.Yellow).MCvScalar, 2);
                Console.WriteLine("Cages détectées avec succès");
            }
            else
            {
                Console.WriteLine($"Impossible de détecter les deux cages. Nombre de cages trouvées : {DetectedGoals.Count}");
            }

            // Libération des ressources
            foreach (var mask in masks)
            {
                mask.Dispose();
            }
            combinedMask.Dispose();
            horizontalMask.Dispose();
            verticalMask.Dispose();
            hsvImage.Dispose();
            horizontalKernel.Dispose();
            verticalKernel.Dispose();
            dilateKernel.Dispose();
            hierarchy.Dispose();
        }

        public void ValidateGoal(Game game1)
        {
            if (this.goal)
            {
                if (game1.PlayerThatHasBall != null && !game1.PlayerThatHasBall.isOffSide)
                {
                    Console.WriteLine($"But valide marqué par le joueur {game1.PlayerThatHasBall.Number} de l'équipe {game1.TeamThatHasBall.Name}");
                    InsertGoal(game1.PlayerThatHasBall.Number, game1.TeamThatHasBall.Name, 1);
                }
                else if (game1.PlayerThatHasBall != null && game1.PlayerThatHasBall.isOffSide)
                {
                    Console.WriteLine($"But invalide : le joueur {game1.PlayerThatHasBall.Number} de l'équipe {game1.TeamThatHasBall.Name} était hors-jeu");
                    this.goal = false;
                }
                else
                {
                    Console.WriteLine("Impossible de valider le but : joueur non identifié");
                    this.goal = false;
                }
            }
        }

        public void IsGoal()
        {
            // Vérification si la balle est dans une des cages détectées
            foreach (var goalArea in DetectedGoals)
            {
                if (goalArea.Contains(BallPosition))
                {
                    goal = true;
                    return;
                }
            }

            goal = false;
        }

        public void VerifyPlayerThatHasBallOffside(List<Player> offside)
        {
            if (PlayerThatHasBall != null)
            {
                PlayerThatHasBall.isOffSide = offside.Any(p => p.Number == PlayerThatHasBall.Number);
                Console.WriteLine($"Le joueur {PlayerThatHasBall.Number} qui a le ballon est {(PlayerThatHasBall.isOffSide ? "hors-jeu" : "en jeu")}");
            }
        }


        public void InsertGoal(int playerId, string teamName, int points)
        {
            DBConnection dbConnection = new DBConnection();
            using (NpgsqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO valiny (id_player, equipe_name, points) VALUES (@playerId, @teamName, @points)";
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@playerId", playerId);
                    command.Parameters.AddWithValue("@teamName", teamName);
                    command.Parameters.AddWithValue("@points", points);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
