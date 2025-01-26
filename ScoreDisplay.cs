using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Data;
using Npgsql;

namespace Foot
{
    public class ScoreDisplay : Form
    {
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button resetButton;
        private Button playerButton;

        public ScoreDisplay()
        {
            InitializeComponent();
            LoadData("equipe");
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();
            this.resetButton = new Button();
            this.playerButton = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(800, 200);
            this.dataGridView1.TabIndex = 0;

            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = DockStyle.Top;
            this.dataGridView2.Location = new System.Drawing.Point(0, 210);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(800, 200);
            this.dataGridView2.TabIndex = 1;

            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(350, 420);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(100, 30);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset Scores";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);

            // 
            // playerButton
            // 
            this.playerButton.Location = new System.Drawing.Point(470, 420);
            this.playerButton.Name = "playerButton";
            this.playerButton.Size = new System.Drawing.Size(100, 30);
            this.playerButton.TabIndex = 3;
            this.playerButton.Text = "Player Scores";
            this.playerButton.UseVisualStyleBackColor = true;
            this.playerButton.Click += new System.EventHandler(this.PlayerButton_Click);

            // 
            // ScoreDisplay
            // 
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.playerButton);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ScoreDisplay";
            this.Text = "Team Scores";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadData(string action)
        {
            DBConnection dbConnection = new DBConnection();
            string query1 = "";
            string query2 = "SELECT * FROM equipe_arret";

            if (action.Equals("equipe"))
            {
                query1 = "SELECT * FROM equipe_points";
            }
            else
            {
                query1 = "SELECT * FROM player_points";
            }

            using (NpgsqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                using (NpgsqlDataAdapter adapter1 = new NpgsqlDataAdapter(query1, conn))
                {
                    DataTable dataTable1 = new DataTable();
                    adapter1.Fill(dataTable1);
                    dataGridView1.DataSource = dataTable1;
                }

                using (NpgsqlDataAdapter adapter2 = new NpgsqlDataAdapter(query2, conn))
                {
                    DataTable dataTable2 = new DataTable();
                    adapter2.Fill(dataTable2);
                    dataGridView2.DataSource = dataTable2;
                }
            }
        }

        private void Reset()
        {
            DBConnection dbConnection = new DBConnection();
            string query = "DELETE FROM valiny";

            using (NpgsqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }

            LoadData("equipe");
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void PlayerButton_Click(object sender, EventArgs e)
        {
            LoadData("player");
        }

        public new void Show()
        {
            this.ShowDialog();
        }
    }
}