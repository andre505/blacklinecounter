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
                int lineCount = CountVerticalLines(imagePath);
                Console.WriteLine($"The number of vertical black lines in the image is: {lineCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static int CountVerticalLines(string imagePath)
        {
            using (var bitmap = new Bitmap(imagePath))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;

                // Tracking which columns contain at least one black pixel
                bool[] hasBlackPixel = new bool[width];

                for (int x = 0; x < width; x++)
                {
                    // scanning every row in each column
                    for (int y = 0; y < height; y++)
                    {
                        if (IsBlack(bitmap.GetPixel(x, y)))
                        {
                            hasBlackPixel[x] = true;
                            break;
                        }
                    }
                }

                // Count runs of adjacent black columns as single lines
                int lineCount = 0; //total black lines found
                bool inBlackLine = false;

                for (int x = 0; x < width; x++)
                {
                    if (hasBlackPixel[x])
                    {
                        if (!inBlackLine)
                        {
                            lineCount++;
                            inBlackLine = true;
                        }
                    }
                    else
                    {
                        // Black line run ended
                        inBlackLine = false;
                    }
                }

                //Return total black lines found
                return lineCount;
            }
        }

        static bool IsBlack(Color c)
        {
            return c.ToArgb() == Color.Black.ToArgb();
        }
    }
}
