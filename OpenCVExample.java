import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgcodecs.Imgcodecs;
import org.opencv.imgproc.Imgproc;
import org.opencv.highgui.HighGui;

public class OpenCVExample {
    public static void main(String[] args) {
        // Load OpenCV native library
        System.loadLibrary(Core.NATIVE_LIBRARY_NAME);

        // Read an image
        Mat image = Imgcodecs.imread("E:/S5/Hors-JEU-Java-Swing/image.jpg");

        if (image.empty()) {
            System.out.println("Image not found!");
            return;
        }

        // Convert the image to grayscale
        Mat grayImage = new Mat();
        Imgproc.cvtColor(image, grayImage, Imgproc.COLOR_BGR2GRAY);

        // Apply Gaussian blur to reduce noise
        Imgproc.GaussianBlur(grayImage, grayImage, new Size(9, 9), 2, 2);

        // Detect circles using HoughCircles
        Mat circles = new Mat();
        Imgproc.HoughCircles(grayImage, circles, Imgproc.HOUGH_GRADIENT, 1, grayImage.rows() / 8, 200, 20, 0, 0);

        // Count the number of circles
        int numberOfCircles = circles.cols();

        // Print the count
        System.out.println("Number of Circles: " + numberOfCircles);

        // Draw the circles on the original image
        for (int i = 0; i < circles.cols(); i++) {
            double[] circle = circles.get(0, i);
            Point center = new Point(circle[0], circle[1]);
            int radius = (int) Math.round(circle[2]);
            // Draw the circle center
            Imgproc.circle(image, center, 3, new Scalar(0, 255, 0), -1);
            // Draw the circle outline
            Imgproc.circle(image, center, radius, new Scalar(0, 0, 255), 3);
        }

        // Display the original image with circles
        HighGui.imshow("Image Display", image);
        HighGui.waitKey();
    }
}