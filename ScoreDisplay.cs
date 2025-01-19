using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Foot
{
    public class ScoreDisplay : Form
    {
        private DataGridView dataGridView;

        public ScoreDisplay()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.dataGridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(800, 450);
            this.dataGridView.TabIndex = 0;
            
            // 
            // ScoreDisplay
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridView);
            this.Name = "ScoreDisplay";
            this.Text = "Team Scores";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            DBConnection dbConnection = new DBConnection();
            string query = "SELECT * FROM equipe_points";

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

        public void Show()
        {
            this.ShowDialog();
        }
    }
}