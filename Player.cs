using System.Drawing;

namespace Foot
{
    internal class Player
    {
        public int Number { get; set; }

        public Double Diametre { get; set; }

        public Point PositionPlayer { get; set; }

        public bool isOffSide { get; set; } = false;

        public Player(int number, Double diametre, Point positionPlayer)
        {
            Number = number;
            Diametre = diametre;
            PositionPlayer = positionPlayer;
        }

        public Player()
        {
        }
    }
}
