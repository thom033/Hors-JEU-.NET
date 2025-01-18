using System;
using System.Windows.Forms;
using System.IO;

namespace Foot
{
    public class ImageUploader : Form
    {
        private Button importButton1;
        private Button importButton2;
        private Button verifyButton;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Panel imagePanel;

        public ImageUploader()
        {
            // Initialisation du bouton "Importer Image 1"
            importButton1 = new Button
            {
                Text = "Importer Image 1",
                Dock = DockStyle.Top
            };
            importButton1.Click += ImportButton1_Click;

            // Initialisation du bouton "Importer Image 2"
            importButton2 = new Button
            {
                Text = "Importer Image 2",
                Dock = DockStyle.Top
            };
            importButton2.Click += ImportButton2_Click;

            // Initialisation du PictureBox pour afficher la première image importée
            pictureBox1 = new PictureBox
            {
                Dock = DockStyle.Left,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = this.ClientSize.Width / 2
            };

            // Initialisation du PictureBox pour afficher la deuxième image importée
            pictureBox2 = new PictureBox
            {
                Dock = DockStyle.Right,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = this.ClientSize.Width / 2
            };

            // Initialisation du panel pour contenir les images
            imagePanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            imagePanel.Controls.Add(pictureBox1);
            imagePanel.Controls.Add(pictureBox2);

            // Initialisation du bouton "Vérifier"
            verifyButton = new Button
            {
                Text = "Vérifier",
                Dock = DockStyle.Bottom
            };
            verifyButton.Click += VerifyButton_Click;

            // Ajout des contrôles au formulaire
            Controls.Add(verifyButton);
            Controls.Add(imagePanel);
            Controls.Add(importButton2);
            Controls.Add(importButton1);

            // Configuration du formulaire pour qu'il couvre tout l'écran
            this.WindowState = FormWindowState.Maximized;
        }

        private void ImportButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    pictureBox1.Image = System.Drawing.Image.FromFile(filePath);
                    MessageBox.Show("Image 1 importée avec succès: " + filePath);
                }
                else
                {
                    MessageBox.Show("Aucune image sélectionnée pour Image 1.");
                }
            }
        }

        private void ImportButton2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    pictureBox2.Image = System.Drawing.Image.FromFile(filePath);
                    MessageBox.Show("Image 2 importée avec succès: " + filePath);
                }
                else
                {
                    MessageBox.Show("Aucune image sélectionnée pour Image 2.");
                }
            }
        }

        private void VerifyButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && pictureBox2.Image != null)
            {
                MessageBox.Show("Les deux images ont été importées avec succès.");
            }
            else
            {
                MessageBox.Show("Veuillez importer les deux images avant de vérifier.");
            }
        }

        // [STAThread]
        // static void Main()
        // {
        //     Application.EnableVisualStyles();
        //     Application.SetCompatibleTextRenderingDefault(false);
        //     Application.Run(new ImageUploader());
        // }
    }
}