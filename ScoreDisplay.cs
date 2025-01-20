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
        private DataGridView dataGridView;
        private Button resetButton;
        private Button playerButton;
        private Button teamButton;

        public ScoreDisplay(string action)
        {
            InitializeComponent();
            LoadData(action);
        }

        private void InitializeComponent()
        {
            this.dataGridView = new DataGridView();
            this.resetButton = new Button();
            this.playerButton = new Button();
            this.teamButton = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = DockStyle.Top;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(800, 400);
            this.dataGridView.TabIndex = 0;

            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(250, 410);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(100, 30);
            this.resetButton.TabIndex = 1;
            this.resetButton.Text = "Reset Scores";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);

            // 
            // playerButton
            // 
            this.playerButton.Location = new System.Drawing.Point(370, 410);
            this.playerButton.Name = "playerButton";
            this.playerButton.Size = new System.Drawing.Size(100, 30);
            this.playerButton.TabIndex = 2;
            this.playerButton.Text = "Player Scores";
            this.playerButton.UseVisualStyleBackColor = true;
            this.playerButton.Click += new System.EventHandler(this.PlayerButton_Click);

            // 
            // teamButton
            // 
            this.teamButton.Location = new System.Drawing.Point(490, 410);
            this.teamButton.Name = "teamButton";
            this.teamButton.Size = new System.Drawing.Size(100, 30);
            this.teamButton.TabIndex = 3;
            this.teamButton.Text = "Team Scores";
            this.teamButton.UseVisualStyleBackColor = true;
            this.teamButton.Click += new System.EventHandler(this.TeamButton_Click);

            // 
            // ScoreDisplay
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.playerButton);
            this.Controls.Add(this.teamButton);
            this.Controls.Add(this.dataGridView);
            this.Name = "ScoreDisplay";
            this.Text = "Team Scores";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadData(string action)
        {
            DBConnection dbConnection = new DBConnection();
            string query = "";
            if (action.Equals("equipe"))
            {
                query = "SELECT * FROM equipe_points";
            }
            else
            {
                query = "SELECT * FROM player_points";
            }

            using (NpgsqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView.DataSource = dataTable;
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

        private void TeamButton_Click(object sender, EventArgs e)
        {
            LoadData("equipe");
        }

        public new void Show()
        {
            this.ShowDialog();
        }
    }
}