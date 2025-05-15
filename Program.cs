using System;
using System.Drawing;
using System.IO;

namespace VerticalBlackLineCounter
{
    class Program
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
                if (!Path.GetExtension(imagePath).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Error: File must be a JPG image.");
                    return;
                }

                // Executing the line counting function
                int lineCount = CountVerticalBlackLines(imagePath);
                Console.WriteLine($"The number of vertical black lines in the image is: {lineCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static int CountVerticalBlackLines(string absoluteImagePath)
        {
            // Loading image into a bitmap object.
            using var bmp = new Bitmap(absoluteImagePath);
            int width = bmp.Width;
            int height = bmp.Height;

            // for each column, counting how many black‐pixel runs (segments) appear
            int[] segmentsPerColumn = new int[width];
            for (int x = 0; x < width; x++)
            {
                int runLength = 0; //counts the current run length of black pixels
                int countRuns = 0; //counts how many separate black runs encountered. 

                for (int y = 0; y < height; y++)
                {
                    if (IsBlack(bmp.GetPixel(x, y)))
                    {
                        runLength++;
                    }
                    else if (runLength > 0)
                    {
                        // we hit white after a run
                        countRuns++;
                        runLength = 0; //resetting for next run.
                    }
                }
                // account for a run that goes to the bottom
                if (runLength > 0)
                    countRuns++;

                segmentsPerColumn[x] = countRuns;
            }

            // Cluster adjacent columns into one line,
            int lineCount = 0;
            bool inCluster = false;
            int maxInThisCluster = 0;

            for (int x = 0; x < width; x++)
            {
                if (segmentsPerColumn[x] > 0)
                {
                    inCluster = true;
                    maxInThisCluster = Math.Max(maxInThisCluster, segmentsPerColumn[x]);
                }
                else if (inCluster)
                {
                    // cluster ended, adding cluster lines
                    lineCount += maxInThisCluster;
                    inCluster = false; //resetting for the next line
                    maxInThisCluster = 0;
                }
            }
            // if image ends, while in a line, adding its runs as well. 
            if (inCluster)
                lineCount += maxInThisCluster;

            return lineCount;
        }

        static bool IsBlack(Color c)
        {
            int avg = (c.R + c.G + c.B) / 3;
            return avg < 100;
        }

        //Switched from the below method as more line counts than exist were returned. Rsearch showed that JPEG compression from MS Paint will introduce tiny artifacts—pixels that look black to the eye but are not actually black, 
        static bool DeprecatedIsBlackMethod(Color color)
        {
            return color.ToArgb() == Color.Black.ToArgb();
        }
    }
}
