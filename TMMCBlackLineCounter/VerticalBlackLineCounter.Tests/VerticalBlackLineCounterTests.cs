using System;
using System.IO;
using Xunit;
using VerticalBlackLineCounter;  // your console app namespace

namespace VerticalBlackLineCounter.Tests
{
    public class LineCounterTests
    {
        [Theory]
        [InlineData("img_1.jpg", 1)]
        [InlineData("img_2.jpg", 3)]
        [InlineData("img_3.jpg", 1)]
        [InlineData("img_4.jpg", 7)]
        [InlineData("img_5.jpg", 5)] // Added an extra case for multiple black lines in a column 
        public void CountVerticalBlackLines_ReturnsExpected(string fileName, int expected)
        {
            // Path to testimages\<fileName> in the test output folder.
            var testDir = AppContext.BaseDirectory;
            var path = Path.Combine(testDir, "testimages", fileName);

            int actual = Program.CountVerticalBlackLines(path);

            Assert.Equal(expected, actual);
        }
    }

}
