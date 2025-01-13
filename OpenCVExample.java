import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.imgcodecs.Imgcodecs;
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

        // Display the image
        HighGui.imshow("Image Display", image);
        HighGui.waitKey();
    }
}
