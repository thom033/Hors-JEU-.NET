using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Foot;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        ImageUploaderForm uploaderForm = new ImageUploaderForm();
        Application.Run(uploaderForm);
    }
}