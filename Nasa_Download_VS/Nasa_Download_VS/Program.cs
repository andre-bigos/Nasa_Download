using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// This simple console program accesses data from NASA's publicly availble database of records and images.
/// The program reads in dates from a text file called "dates.txt" that must be located in the same directory
/// as the executable. If no file is found, default dates are used.
/// 
/// Once the desired dates are ascertained, the program pulls all relevant data from NASA's Mars Rovers for
/// the given dates using NASA's open API.
/// 
/// The data for each date and each rover is then analyzed and each image url is pulled from the data. The
/// images are then downloaded from the urls and saved locally to a new folder called "Images" generated in
/// the same directory as the executable.
/// 
/// Should the program encounter an exception, in general, these exceptions are handled and written to an
/// auto-generated log file in the same directory as the executable.
/// 
/// 
/// 
/// This version is stable, and works properly, passed all tests (Improper dates, etc.).
/// 
/// 
/// It is a simple console app, and will be improved over time.
/// 
/// </summary>
namespace Nasa_Download_VS
{
	public static class Program
	{
		// If "dates.txt" does not exist in the exe's directory, these default dates are used. Note: "April 31, 2018"
		// has been changed to "April 30, 2018" because the date does not exist.
		static string[] defaultDates = { "02/27/17", "June 2, 2018", "Jul-13-2016", "April 30, 2018" };


		/// The main method handles the main structure of the program. The program executes a single time, performs
		/// all required actions and then terminates.
		public static void Main(string[] args)
		{
			List<DateTime> dates = ReadDateTimesFromFile();

			if (dates.Count == 0)
			{
				WriteToErrorLog("No readable dates found in dates.txt.", true);
				return;
			}

			// Here, the URLs for all rovers, for all given dates are found and added to a list of image URLs to
			// download.
			List<string> imageURLs = new List<string>();
			foreach (DateTime date in dates)
			{
				Console.WriteLine("Analyzing data for date: " + date.ToString("MM/dd/yyyy"));
				imageURLs.AddRange(FindURLs(date, "curiosity"));
				imageURLs.AddRange(FindURLs(date, "opportunity"));
				imageURLs.AddRange(FindURLs(date, "spirit"));
			}
			Console.WriteLine("Total images Expected: " + imageURLs.Count);//Debugging/Rudimentary Console UI
			int index = 0;
			int failedDownloads = 0;

			// Here, each image is downloaded from the URL pulled from NASA's Mars API.
			foreach (string imageURL in imageURLs)
			{
				index++;
				if (!DownloadURLImage(imageURL))
				{ failedDownloads++; }
				Console.WriteLine("Downloading Image: " + index + "/" + imageURLs.Count + "  \t\t"
					+ (index - failedDownloads) + "/" + index + "\tSuccessful.");//Debugging/Rudimentary Console UI
			}
		}
		/// For a given date, and a given Mars rover, this method downloads information about all available photographs
		/// from the NASA API and extracts all URLs from the data.
		/// Returns: A list of the extracted URLs as strings.
		private static List<string> FindURLs(DateTime date, string rover)
		{
			List<string> urls = new List<string>();
			string dataForDate;
			string url = "https://api.nasa.gov/mars-photos/api/v1/rovers/" + rover + "/photos?earth_date="
				+ date.ToString("yyyy-MM-dd") + "&api_key=DEMO_KEY";// Andre's personal api key: a9BuCHb2NSF0EmhsU7rIK8qRsgGYXXazLscJ9GSK
																	// Demo Key: DEMO_KEY
			try
			{
				WebClient webClient = new WebClient();
				dataForDate = webClient.DownloadString(url);
			}
			catch (Exception e)
			{
				WriteToErrorLog("Unable to Reach URL: " + url + "\nFor photos from date: " + date.ToString("yyyy-MM-dd") + "\n" + e);
				return new List<string>();
			}

			/// Awful way of doing it, but given the time constraints, it works, and it works reliably.
			/// This should be expanded to be more versatile. It's truly a waste of a well formatted data set.
			/// But to be fair, it does work. Improve later, make it easier to add further functionality.
			string[] splitData = dataForDate.Split('"');
			for (int x = 0; x < splitData.Length - 2; x++)
			{
				if (splitData[x].Equals("img_src"))
				{
					urls.Add(splitData[x + 2]);
				}
			}
			return urls;
		}
		/// This method returns a list of the desired dates received from the "dates.txt" file located in the same directory
		/// as the exe. If no "dates.txt" file is found, the default dates are used. 
		/// Returns: A list of parsed DateTime objects pulled from the text file.
		private static List<DateTime> ReadDateTimesFromFile()
		{
			List<DateTime> dates = new List<DateTime>();
			string[] readLines;
			string datesFilePath = Directory.GetCurrentDirectory() + "\\dates.txt";
			if (!File.Exists(datesFilePath))
			{
				readLines = defaultDates;
			}
			else
			{
				readLines = File.ReadAllLines(datesFilePath);
			}
			foreach (string potentialDate in readLines)
			{
				try
				{
					DateTime dt = DateTime.Parse(potentialDate);
					dates.Add(dt);
				}
				catch (Exception e)
				{
					WriteToErrorLog("Unable to parse Date: " + potentialDate + "\n" + e);
				}
			}
			return dates;
		}
		/// Downloads an image from a given URL.
		/// Returns true if download was successful, false otherwise.
		private static bool DownloadURLImage(string url)
		{
			try
			{
				WebClient webClient = new WebClient();
				string path = Directory.GetCurrentDirectory();
				path = path + "\\images";
				Directory.CreateDirectory(path);
				string filePath = Path.GetFileName(url);
				if (File.Exists(path + "\\" + filePath))
				{
					int index = 2;
					while (File.Exists(path + "\\" + Path.GetFileNameWithoutExtension(url) + "_" + index))
					{ index++; }
					filePath = Path.GetFileNameWithoutExtension(url) + "_" + index + Path.GetExtension(url);
				}
				webClient.DownloadFile(url, path + "\\" + filePath);
				webClient.Dispose();
				return true;
			}
			catch (Exception e)
			{
				WriteToErrorLog("Unable to download image from: " + url + "\n" + e.ToString());
				return false;
			}
		}
		private static void WriteToErrorLog(string errorMessage, bool exit = false)
		{
			string directory = Directory.GetCurrentDirectory();
			string errorLog = directory + "\\Auto-Generated_Error_Log.txt";
			StreamWriter w = File.AppendText(errorLog);
			w.WriteLine(errorMessage);
			w.Flush();
			w.Close();
			w.Dispose();
			if (exit)
			{
				Environment.Exit(1);
			}
		}
	}
}
