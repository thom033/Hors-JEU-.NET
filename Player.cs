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

        public Player() { }

        public void InsertPlayer(int number)
        {
            DBConnection dbConnection = new DBConnection();
            using (NpgsqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Players (Number) VALUES (@Number)";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Number", number);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}