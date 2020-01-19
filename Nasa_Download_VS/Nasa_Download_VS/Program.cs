using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nasa_Download_VS
{
	public static class Program
	{
		static string[] defaultDates = { "02/27/17", "June 2, 2018", "Jul-13-2016", "April 30, 2018" };
		public static void Main(string[] args)
		{
			List<DateTime> dates = ReadDateTimesFromFile();

			if (dates.Count == 0)
			{
				WriteToErrorLog("No readable dates found in dates.txt.", true);
				return;
			}
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
			foreach (string imageURL in imageURLs)
			{
				index++;
				if (!DownloadURLImage(imageURL))
				{ failedDownloads++; }
				Console.WriteLine("Downloading Image: " + index + "/" + imageURLs.Count + "  \t\t" + (index - failedDownloads) + "/" + index + "\tSuccessful.");
			}
		}
		private static List<string> FindURLs(DateTime date, string rover)
		{
			List<string> urls = new List<string>();
			string dataForDate;
			string url = "https://api.nasa.gov/mars-photos/api/v1/rovers/" + rover + "/photos?earth_date="
				+ date.ToString("yyyy-MM-dd") + "&api_key=DEMO_KEY";//andre's personal api key: a9BuCHb2NSF0EmhsU7rIK8qRsgGYXXazLscJ9GSK
																	//Demo Key: DEMO_KEY
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
