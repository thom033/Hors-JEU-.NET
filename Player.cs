using System.Drawing;
using Npgsql;

namespace Foot
{
    internal class Player
    {
        public int Number { get; set; }
        public double Diametre { get; set; }
        public Point PositionPlayer { get; set; }

        public bool isOffSide { get; set; } = false;

        public Player(int number, double diametre, Point positionPlayer)
        {
            Number = number;
            Diametre = diametre;
            PositionPlayer = positionPlayer;
        }
    }
}