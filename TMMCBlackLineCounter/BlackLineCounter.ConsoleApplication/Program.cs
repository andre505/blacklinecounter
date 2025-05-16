using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace VerticalBlackLineCounter
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Checking if the correct number of arguments was provided
                if (args.Length != 1)
                {
                    Console.WriteLine("Error: This problem requires exactly one argument - the absolute path of the test image.");
                    Console.WriteLine("Example: counter.exe C:\\path\\to\\img_1.jpg");
                    return;
                }

                string imagePath = args[0];

                //Checking if the file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"Error: The file '{imagePath}' does not exist.");
                    return;
                }

                // Validating file extension
                if (!Path.GetExtension(imagePath).Equals(".jpg", StringComparison.OrdinalIgnoreCase) || !Path.GetExtension(imagePath).Equals(".jpeg"))
                {
                    Console.WriteLine("Error: File must be a JPG image.");
                    return;
                }

                // Executing the line counting function
                Console.WriteLine(CountVerticalBlackLines(imagePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static int CountVerticalBlackLines(string absoluteImagePath) // Takes the image path and returns line count
        {
            // Loading image from disk into bitmap obj.
            using var bmp = new Bitmap(absoluteImagePath);

            int width = bmp.Width;   // width in pixels
            int height = bmp.Height; // height in pixels

            // Defining the area of the bitmap that will be locked (entire image)
            var rect = new Rectangle(0, 0, width, height); // Rectangle covering full image

            // Lock the bitmap into system memory for fast, direct byte access
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // Defining stride: stride is the number of bytes in a single scan row
            int stride = bmpData.Stride;

            // Total byte size: absolute stride times number of rows
            int bufSize = Math.Abs(stride) * height;

            // Allocating byte array to hold all pixel data
            byte[] buffer = new byte[bufSize];

            // Copy raw pixel data from locked bitmap memory into our buffer
            Marshal.Copy(bmpData.Scan0, buffer, 0, bufSize);

            // Unlocking bitmap since bytes loaded into memory.
            bmp.UnlockBits(bmpData);

            // Array to count how many separate black-pixel runs appear in each column
            int[] segmentsPerColumn = new int[width];

            // Threshold to decide whether a pixel is "black enough" (0–255)
            const int BLACK_THRESHOLD = 100; //choosing 100

            // Allowing multiple threads to process different columns concurrently.
            Parallel.For(0, width, x =>
            {
                // runLength tracks the length of the current consecutive black-pixel streak
                int runLength = 0;

                // countRuns records how many distinct vertical runs (segments) in this column
                int countRuns = 0;

                // Iterating over each row in column x
                for (int y = 0; y < height; y++)
                {
                    // Calculating index into buffer for pixel (x,y) by doing row offset + column offset (3 bytes per pixel: B,G,R)
                    int idx = y * stride + x * 3;

                    // Extract Blue, Green, Red channels directly from the buffer
                    int blue = buffer[idx + 0]; // Blue byte
                    int green = buffer[idx + 1]; // Green byte
                    int red = buffer[idx + 2]; // Red byte

                    // Calculating average intensity of current pixel
                    int avg = (red + green + blue) / 3;

                    // If darker than threshold, black run is still ongoing.
                    if (avg < BLACK_THRESHOLD)
                    {
                        runLength++; // increment current run
                    }
                    else if (runLength > 0)
                    {
                        countRuns++;
                        runLength = 0; // reset for next segment
                    }
                }

                // Incrementing segment if image ends on a black run.
                if (runLength > 0)
                    countRuns++;

                segmentsPerColumn[x] = countRuns;
            });

            // Implementing another loop for combining adjacent columns into final line count.
            int lineCount = 0;       // total number of vertical lines
            bool inCluster = false;  // tracks if we're inside a span of columns with runs
            int maxInCluster = 0;    // highest number of runs found in current cluster

            // Scanning columns left to right
            for (int x = 0; x < width; x++)
            {
                if (segmentsPerColumn[x] > 0) //if yes, cluster of columns contains runs

                {
                    inCluster = true;
                    // Tracking max runs in this cluster
                    maxInCluster = Math.Max(maxInCluster, segmentsPerColumn[x]);
                }
                else if (inCluster)
                {
                    // Cluster ended: add its maximum-run count to the total, then reset
                    lineCount += maxInCluster;
                    inCluster = false;
                    maxInCluster = 0;
                }
            }

            // If the last column ended inside a cluster, adding that cluster too
            if (inCluster)
                lineCount += maxInCluster;

            return lineCount;
        }

        //Deprecating this as refactored multithreaded solution does not require .GetPixel() method as it is not thread-safe.
        static bool IsBlack(Color c)
        {
            int avg = (c.R + c.G + c.B) / 3;
            return avg < 100;
        }

        //Switched from this method to the above isBlack(...) method as more line counts than actually exist were returned. Research showed that JPEG compression from MS Paint will introduce tiny artifacts—pixels that look black to the eye but are not actually black.
        static bool DeprecatedIsBlackMethod(Color color)
        {
            return color.ToArgb() == Color.Black.ToArgb();
        }
    }
}
