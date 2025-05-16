# TMMC VerticalBlackLineCounter

## Overview
The VerticalBlackLineCounter application counts the number of vertical black lines in a .jpg image.

## Prerequisites
- Executable: `VerticalBlackLineCounter.ConsoleApplication.exe`
- Test Images: .jpg files containing black lines
- .NET 9 SDK: Only required if building the executable from source

## Installation

### Option A: Download Executable
1. Download the executable from Google Drive:  
   https://drive.google.com/file/d/16lambOWw-PrrzBUaY-Y2sGLkF7AOjVcY/view?usp=sharing

### Option B: Build from Source
1. Clone the GitHub repository:
   ```
   git clone https://github.com/andre505/blacklinecounter.git
   ```
2. Navigate to the project folder
3. Build in Release mode:
   ```
   dotnet build -c Release
   ```
4. The executable will be located at:  
   `bin/Release/net9.0/VerticalBlackLineCounter.ConsoleApplication.exe`

## Usage
1. Open Command Prompt or PowerShell
2. Change directory to the folder containing `VerticalBlackLineCounter.ConsoleApplication.exe`  
   Example:
   ```
   cd C:\Users\YourName\Downloads
   ```
3. Run the application with the full path to your .jpg image as the only argument:
   - Command Prompt:
     ```
     VerticalBlackLineCounter.ConsoleApplication.exe C:\Users\YourName\Pictures\img_1.jpg
     ```
   - PowerShell:
     ```
     .\VerticalBlackLineCounter.ConsoleApplication.exe "C:\Users\YourName\Pictures\img_4.jpg"
     ```
4. The program will output the number of vertical black lines detected

## Example Output
```
5
```

## Notes
- Provide exactly one argument: the absolute path to an existing .jpg file
- The file must have a .jpg extension
- Errors occur if:
  - The argument count is incorrect
  - The file does not exist
  - The file is not a .jpg

## Running Tests (Optional)
1. Open the solution in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Open Test Explorer (Test → Test Explorer)
4. Click Run All to execute all xUnit tests
5. Passing tests are marked with green checkmarks
