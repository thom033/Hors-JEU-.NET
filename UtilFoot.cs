using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Foot
{
    internal class UtilFoot
    {
        public static double EuclideanDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public static Mat DetectColor(Mat hsvImage, Hsv lower, Hsv upper)
        {
            Mat mask = new Mat();
            CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(lower.Hue, lower.Satuation, lower.Value)),
                             new ScalarArray(new MCvScalar(upper.Hue, upper.Satuation, upper.Value)), mask);
            return mask;
        }

    }
}
