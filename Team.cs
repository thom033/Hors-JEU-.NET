using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace Foot
{
    internal class Team
    {
        public string Name { get; set; }

        public List<Player> PlayerList { get; set; } = new List<Player>();

        public bool HasBall { get; set; }

        public Team()
        {
        }

        // Maka ireo mpilalao mahay (Mety)
        public static List<Player> GetOkSide(Team attaquant, Team defenseur, Team avyAnyAmbony, Point ballon, Mat image)
        {
            List<Player> ret = new List<Player>();
            if (attaquant.Name != avyAnyAmbony.Name)     // Raha avy any AMBANY ny attaquant
            {
                Player dernierDefenseur = defenseur.DernierDefenseur(true);     // Defense avy any AMBONY
                foreach (Player p in attaquant.PlayerList)
                {
                    if (dernierDefenseur.PositionPlayer.Y < p.PositionPlayer.Y && ballon.Y > p.PositionPlayer.Y)
                    {
                        ret.Add(p);
                        CvInvoke.PutText(image, $"Mety", new Point(p.PositionPlayer.X, p.PositionPlayer.Y - 8), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0));
                        // Dessiner la flèche du ballon vers le joueur
                        CvInvoke.ArrowedLine(image, ballon, p.PositionPlayer, new MCvScalar(0, 255, 0), 2);
                    }
                }
            }
            else // Raha avy any AMBONY ny attaquant
            {
                Player dernierDefenseur = defenseur.DernierDefenseur(false);     // Defense avy any AMBONY
                foreach (Player p in attaquant.PlayerList)
                {
                    // Ampiana anle Diametre eto satria avy any AMBONY ny attaquant
                    if ((dernierDefenseur.PositionPlayer.Y + dernierDefenseur.Diametre) > (p.PositionPlayer.Y + p.Diametre) && (p.PositionPlayer.Y + p.Diametre) > ballon.Y)
                    {
                        CvInvoke.PutText(image, $"Mety", new Point(p.PositionPlayer.X, p.PositionPlayer.Y + 8), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0));
                        // Dessiner la flèche du ballon vers le joueur
                        CvInvoke.ArrowedLine(image, ballon, p.PositionPlayer, new MCvScalar(0, 255, 0), 2);
                        ret.Add(p);
                    }
                }
            }
            return ret;
        }

        // Maka ireo mpilalao tsy mahay (Hors jeu)
        public static List<Player> GetOffSide(Team attaquant, Team defenseur, Team avyAnyAmbony, Mat image)
        {
            List<Player> ret = new List<Player>();
            if (attaquant.Name != avyAnyAmbony.Name)     // Raha avy any AMBANY ny attaquant
            {
                Player dernierDefenseur = defenseur.DernierDefenseur(true);     // Defense avy any AMBONY
                foreach (Player p in attaquant.PlayerList)
                {
                    if (dernierDefenseur.PositionPlayer.Y > p.PositionPlayer.Y)
                    {
                        ret.Add(p);
                        CvInvoke.PutText(image, $"Hors-Jeu", new Point(p.PositionPlayer.X, p.PositionPlayer.Y - 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0));
                    }
                }
            }
            else
            {
                Player dernierDefenseur = defenseur.DernierDefenseur(false);     // Defense avy any AMBONY
                foreach (Player p in attaquant.PlayerList)
                {
                    // Ampiana anle Diametre eto satria avy any AMBONY ny attaquant
                    if (dernierDefenseur.PositionPlayer.Y + dernierDefenseur.Diametre < p.PositionPlayer.Y + p.Diametre)
                    {
                        CvInvoke.PutText(image, $"Hors-Jeu", new Point(p.PositionPlayer.X, p.PositionPlayer.Y + 10), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0));
                        ret.Add(p);
                    }
                }
            }
            return ret;
        }

        // Dernier defenseur (Avy any ambony ny ekipa sa tsia)
        public Player DernierDefenseur(bool avyAnyAmbony)
        {
            if (avyAnyAmbony)
                return PlayerList.OrderBy(player => player.PositionPlayer.Y).Skip(1).FirstOrDefault(); // Retourne le deuxieme avy any ambony
            return PlayerList.OrderBy(player => (player.PositionPlayer.Y + player.Diametre)).Skip(Math.Max(0, PlayerList.Count - 2)).FirstOrDefault(); // 2eme avy anu amn farany
        }

        // Initialisation equioe deouis image
        public static Team InitializeTeam(Mat image, Mat mask, Color color, string nameTeam, ref int playerId, Point ballPosition)
        {
            Team result = new Team();
            result.Name = nameTeam;
            Mat hsvImage = new Mat();
            CvInvoke.CvtColor(image, hsvImage, ColorConversion.Bgr2Hsv);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(mask, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contours.Size; i++)
            {
                VectorOfPoint contour = contours[i];
                Rectangle rect = CvInvoke.BoundingRectangle(contour);
                Point positionCentre = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);    // Pour calculer le ballon avec le Player
                Point positionReel = new Point(rect.X, rect.Y);

                Player player = new Player(playerId++, rect.Width, positionReel);
                result.PlayerList.Add(player);

                CvInvoke.Rectangle(image, rect, new Bgr(color).MCvScalar, 2);
                CvInvoke.PutText(image, $"{player.Number}", new Point(positionCentre.X, positionCentre.Y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0));
            }
            return result;
        }
    }
}
