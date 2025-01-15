using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections.Generic;

class FootballMatchAnalyzer
{
    class Player
    {
        public int Number { get; set; }
        public bool HasBall { get; set; }
        public Point Position { get; set; }
        public string Role { get; set; } // Gardien, Défenseur, Attaquant

        public bool IsOffside { get; set; }

        public Player(int number, bool hasBall, Point position)
        {
            Number = number;
            HasBall = hasBall;
            Position = position;
            Role = "Unknown";
        }

        public static List<Player> GetDefenseurs(List<Player> teamA, List<Player> teamB)
        {
            foreach (var player in teamA)
            {
                if (player.Role == "Défenseur")
                {
                    return teamA;
                }
            }

            return teamB;
        }

        public static List<Player> GetAttaquants(List<Player> teamA, List<Player> teamB)
        {
            foreach (var player in teamA)
            {
                if (player.Role == "Attaquant")
                {
                    return teamA;
                }
            }

            return teamB;
        }
        
        public static Player GetPlayerWithBall(List<Player> team)
        {
            foreach (var player in team)
            {
                if (player.HasBall)
                {
                    return player;
                }
            }
            return null;
        }
        public override string ToString()
        {
            return $"Player #{Number} ({Role}), Position: ({Position.X}, {Position.Y}), Has Ball: {HasBall}";
        }
    }

    class Ball
    {
        public Point Position { get; set; }

        public Ball(Point position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return $"Ball Position: ({Position.X}, {Position.Y})";
        }
    }

    static void Main(string[] args)
    {
        string imagePath = @"e:\S5\Foot_hors_jeu\img\image\terrain3.png";

        Mat image = CvInvoke.Imread(imagePath);

        if (image.IsEmpty)
        {
            Console.WriteLine("Impossible de charger l'image.");
            return;
        }

        Mat hsvImage = new Mat();
        CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

        var redMask1 = DetectColor(hsvImage, new Hsv(0, 120, 70), new Hsv(10, 255, 255));
        var redMask2 = DetectColor(hsvImage, new Hsv(160, 120, 70), new Hsv(180, 255, 255));
        Mat redMask = new Mat();
        CvInvoke.BitwiseOr(redMask1, redMask2, redMask);

        var blueMask = DetectColor(hsvImage, new Hsv(90, 50, 50), new Hsv(130, 255, 255));

        var blackMask = DetectColor(hsvImage, new Hsv(0, 0, 0), new Hsv(180, 255, 50));

        List<Player> equipe1 = new List<Player>();
        List<Player> equipe2 = new List<Player>();
        Ball ball = DetectBall(hsvImage, image, new Hsv(0, 0, 0), new Hsv(180, 255, 50));

        int redPlayerId = 1;
        int bluePlayerId = 1;
        AddPlayers(image, redMask, Color.Red, "RT", ref equipe1, ref redPlayerId, ball);
        AddPlayers(image, blueMask, Color.Blue, "BT", ref equipe2, ref bluePlayerId, ball);

        // Assigner les rôles des joueurs
        AssignRoles(equipe1, image.Width, image.Height, isLeftToRight: true);
        AssignRoles(equipe2, image.Width, image.Height, isLeftToRight: false);

        Player goalkeeper1 = DetectGoalKeeper(equipe1, redMask, image, "Haut", image.Width, image.Height, Color.Red);
        Player goalkeeper2 = DetectGoalKeeper(equipe2, blueMask, image, "Bas", image.Width, image.Height, Color.Blue);

        // Player lastDefender1 = DetectLastDefender(equipe1, image, image.Height);
        // Player lastDefender2 = DetectLastDefender(equipe2, image, image.Height);

        Console.WriteLine("Equipe 1 (Rouge) :");
        foreach (var player in equipe1)
            Console.WriteLine(player.ToString());
        Console.WriteLine($"\nEquipe 2 (Bleu) :");
        foreach (var player in equipe2)
            Console.WriteLine(player.ToString());

        // Console.WriteLine("\nDernier défenseur de l'équipe 1 :");
        // Console.WriteLine(lastDefender1 != null ? lastDefender1.ToString() : "Aucun");

        // Console.WriteLine("\nDernier défenseur de l'équipe 2 :");
        // Console.WriteLine(lastDefender2 != null ? lastDefender2.ToString() : "Aucun");

        if (ball != null)
        {
            Console.WriteLine($"\nBallon détecté : {ball}");
            Console.WriteLine("\nAnalyse des attaquants :");

            // Marquer les attaquants hors-jeu pour chaque équipe
            MarkOffsideAttackers(image, equipe1, equipe2, blueMask, "Bas", image.Height);
            MarkOffsideAttackers(image, equipe2, equipe1, redMask, "Haut", image.Height);
        }
        else
        {
            Console.WriteLine("\nBallon non détecté.");
        }

        CvInvoke.Imshow("Match Analysis", image);
        CvInvoke.WaitKey(0);
    }


    static void AssignRoles(List<Player> team, int fieldWidth, int fieldHeight, bool isLeftToRight)
    {
        // Déterminer les lignes de but
        int ownGoalLine = isLeftToRight ? 0 : fieldWidth;
        int opponentGoalLine = isLeftToRight ? fieldWidth : 0;

        // Trier par proximité au but propre
        team.Sort((p1, p2) =>
            EuclideanDistance(p1.Position, new Point(ownGoalLine, fieldHeight / 2))
            .CompareTo(EuclideanDistance(p2.Position, new Point(ownGoalLine, fieldHeight / 2))));

        // Le joueur le plus proche du but est le gardien
        if (team.Count > 0) team[0].Role = "Goalkeeper";

        // Assignation des autres rôles
        bool hasBall = false;

        // Vérifier si un joueur a le ballon
        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].HasBall)
            {
                hasBall = true;
                break;
            }
        }

        // Assigner les rôles en fonction de la possession du ballon
        for (int i = 0; i < team.Count; i++)
        {
            team[i].Role = hasBall ? "Attaquant" : "Défenseur";
        }
    }

    static Mat DetectColor(Mat hsvImage, Hsv lower, Hsv upper)
    {
        Mat mask = new Mat();
        CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(lower.Hue, lower.Satuation, lower.Value)),
                         new ScalarArray(new MCvScalar(upper.Hue, upper.Satuation, upper.Value)), mask);
        return mask;
    }

    static Ball DetectBall(Mat hsvImage, Mat image, Hsv lower, Hsv upper)
    {
        Mat ballMask = new Mat();
        CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(lower.Hue, lower.Satuation, lower.Value)),
                         new ScalarArray(new MCvScalar(upper.Hue, upper.Satuation, upper.Value)), ballMask);

        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(ballMask, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        Ball ball = null;

        for (int i = 0; i < contours.Size; i++)
        {
            VectorOfPoint contour = contours[i];
            double area = CvInvoke.ContourArea(contour);
            Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);

            if (area > 50 && area < 500 && (double)boundingBox.Width / boundingBox.Height < 1.2)
            {
                Point position = new Point(boundingBox.X + boundingBox.Width / 2, boundingBox.Y + boundingBox.Height / 2);
                ball = new Ball(position);

                CvInvoke.Circle(image, position, 10, new Bgr(Color.Yellow).MCvScalar, 2);
                break;
            }
        }

        return ball;
    }

    static void AddPlayers(Mat image, Mat mask, Color color, string label, ref List<Player> team, ref int playerId, Ball ball)
    {
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(mask, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        for (int i = 0; i < contours.Size; i++)
        {
            VectorOfPoint contour = contours[i];
            Rectangle rect = CvInvoke.BoundingRectangle(contour);
            Point position = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            bool hasBall = ball != null && EuclideanDistance(position, ball.Position) < 60.0;

            Player player = new Player(playerId++, hasBall, position);
            team.Add(player);

            CvInvoke.Rectangle(image, rect, new Bgr(color).MCvScalar, 2);
        }
    }

    static Player DetectGoalKeeper(List<Player> players, Mat mask, Mat image, string teamPosition, int fieldWidth, int fieldHeight, Color color)
    {
        Point goalCenter = teamPosition == "Haut"
            ? new Point(fieldWidth / 2, 0)
            : new Point(fieldWidth / 2, fieldHeight);

        double maxDistance = double.MinValue;
        Player goalkeeper = null;
        
        foreach (var player in players)
        {
            double distance = EuclideanDistance(goalCenter, player.Position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                goalkeeper = player;
            }
        }

        if (goalkeeper != null)
        {
            goalkeeper.Role = "Goalkeeper";
            CvInvoke.Circle(image, goalkeeper.Position, 10, new Bgr(color).MCvScalar, 2);
        }

        return goalkeeper;
    }

    static string GetPositionTerrain(List<Player> team, int fieldHeight)
    {
        // Trouver le gardien de but dans la liste des joueurs
        var goalkeeper = team.FirstOrDefault(player => player.Role == "Goalkeeper");
    
        if (goalkeeper == null)
        {
            throw new Exception("Aucun gardien trouvé dans l'équipe.");
        }
    
        // Déterminer la position du gardien de but sur l'axe Y
        int goalkeeperY = goalkeeper.Position.Y;
    
        // Comparer la position du gardien avec la moitié de la hauteur du terrain
        if (goalkeeperY < fieldHeight / 2)
        {
            return "Haut";
        }
        else
        {
            return "Bas";
        }
    }

    static Player DetectLastDefender(List<Player> team, Mat image, int fieldHeight)
    {
        bool isTeamInDefense = false;
        foreach (var player in team)
        {
            if (player.Role == "Défenseur" && player.Role != "Goalkeeper")
            {
                isTeamInDefense = true;
                break;
            }
        }

        if (isTeamInDefense == false)
        {
            return null;
        }

        string teamPosition = GetPositionTerrain(team, fieldHeight);
        Console.WriteLine($"Position de l'équipe : {teamPosition}");

        // Déterminer la position du but
        bool isTeamAtBottom = teamPosition == "Bas";
        int goalY = isTeamAtBottom ? fieldHeight : 0;

        // Trier les joueurs par leur distance au but
        var closestPlayer = team
            .Where(player => player.Role == "Défenseur" && player.Role != "Goalkeeper")
            .OrderBy(player => Math.Abs(player.Position.Y - goalY))
            .FirstOrDefault();

        // Calculer la position de la ligne d'hors-jeu
        int offsideLineY = closestPlayer.Position.Y;

        // Dessiner la ligne d'hors-jeu sur l'image
        CvInvoke.Line(image,
            new Point(0, offsideLineY), // Début de la ligne (gauche de l'image)
            new Point(image.Width, offsideLineY), // Fin de la ligne (droite de l'image)
            new MCvScalar(255, 0, 0), // Couleur de la ligne (bleu)
            2); // Épaisseur de la ligne

        // Annoter visuellement le dernier défenseur
        CvInvoke.Circle(image, closestPlayer.Position, 10, new Bgr(Color.Cyan).MCvScalar, 2);
        CvInvoke.PutText(image, $"Last Def #{closestPlayer.Number}",
            new Point(closestPlayer.Position.X - 5, closestPlayer.Position.Y - 5),
            FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 255));

        return closestPlayer;
    }
    
    static Player GetGoalkeeper(List<Player> team)
    {
        if (team == null || team.Count == 0)
        {
            return null;
        }

        foreach (var player in team)
        {
            if (player.Role.Equals("Goalkeeper", StringComparison.OrdinalIgnoreCase))
            {
                return player;
            }
        }

        return null; // Return null if no goalkeeper is found
    }

    static List<Player> IsOffSide(List<Player> team, int lastDefenderY, int goalkeeperY)
    {
        List<Player> offsidePlayers = new List<Player>();

        foreach (var player in team)
        {
            if (player.Position.Y > Math.Min(goalkeeperY, lastDefenderY) && player.Position.Y < Math.Max(goalkeeperY, lastDefenderY))
            {
                player.IsOffside = true;
                offsidePlayers.Add(player);
            }
            else
            {
                player.IsOffside = false;
            }
        }

        return offsidePlayers;
    }

    static void PaintOffSide(Mat image, List<Player> team)
    {
        foreach (var player in team)
        {
            if (player.IsOffside)
            {
                Console.WriteLine($"Player #{player.Number} est hors-jeu !");
                CvInvoke.PutText(image, $"Offside #{player.Number}",
                    new Point(player.Position.X - 5, player.Position.Y - 5),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255));
            }
            else
            {
                Console.WriteLine($"Player #{player.Number} est en jeu.");
                CvInvoke.PutText(image, $"Onside #{player.Number}",
                    new Point(player.Position.X - 5, player.Position.Y - 5),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0));
            }
        }
    }

    static void MarkOffsideAttackers(Mat image, List<Player> team1, List<Player> team2, Mat mask, string opposingTeamPosition, int fieldHeight)
    {
        List<Player> teamAttack = Player.GetAttaquants(team1, team2);
        List<Player> teamDefense = Player.GetDefenseurs(team1, team2);

        Player playerWithBall = Player.GetPlayerWithBall(teamAttack);

        // Détecter le dernier défenseur de l'équipe adverse
        Player lastDefender = DetectLastDefender(teamDefense, image, fieldHeight);
        if (lastDefender == null) return;

        bool isOpponentAtBottom = opposingTeamPosition == "Bas";
        int offsideLineY = lastDefender.Position.Y;

        // Vérifier si le ballon est devant ou derrière le dernier défenseur
        // bool isBallAheadOfLastDefender = isOpponentAtBottom
        //     ? playerWithBall.Position.Y > lastDefender.Position.Y
        //     : playerWithBall.Position.Y < lastDefender.Position.Y;

        // Déterminer la moitié du terrain
        int midFieldLineY = fieldHeight / 2;

        int lastDefenderY = lastDefender.Position.Y;
        int goalkeeperY = GetGoalkeeper(teamDefense).Position.Y;

        List<Player> offsidePlayers = IsOffSide(teamAttack, lastDefenderY, goalkeeperY);

        PaintOffSide(image, offsidePlayers);
    }

    static bool IsInMask(Player player, Mat mask)
    {
        byte pixelValue = (byte)mask.ToImage<Gray, byte>()[player.Position.Y, player.Position.X].Intensity;
        return pixelValue > 0;
    }

    static double EuclideanDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}
