<p align = "center">
    <img src = "https://www.nasa.gov/sites/default/files/thumbnails/image/nasa-logo-web-rgb.png" alt = "NASA">
</p>

# NASA Images Download Project for Motorola Coding Exercise
This project is a simple console application. Using user desired dates (from a text file), the program searches for all publically available images from NASA's MARS Rovers for those specified dates. These images are then downloaded locally to a folder, aptly titled "Images." The application is built using C# and the .NET Core framework, using Visual Studio 19 as an IDE, and has been verified to properly work on a PC operating Windows 10.


## Inputs
### dates.txt
If the program's directory contains the file "dates.txt," the file is read and the dates within are used to download images from the NASA APIs.

## Outputs
### Auto-Generated_Error_Log.txt
If the program encounters an error (Unrecognized date, unable to reach url, etc.) an error log is created or appended to. This error log details the nature of the error(s) that occurred.

### "Images" Folder
If the program runs properly and is able to download images from nasa for the required dates, these images are saved to a local folder generated in the executable's directory.

## Running the Program
It's recommended that you clone the repository using SourceTree or another type of GitHub Repository management software to your local device.

The current published version, which has been tested and verified to work, can be found in the repository by following the path: Nasa_Download_VS\Nasa_Download_VS\bin\Release\netcoreapp3.1\publish\Nasa_Download_VS.exe.

Otherwise, the program can be run by opening the visual studio project and running a debug version of the app by selecting it from the menu bar or using a shortcut (typically: Ctrl + F5).


##### Sole Software Developer: Andre Bigos
