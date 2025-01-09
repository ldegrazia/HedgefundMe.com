using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Net;
using HedgefundMe.com;
namespace HedgefundMe.com.Services
{
    public static class FileService
    {
        public const string NASDAQ = "NASDAQ";
        public const string AMEX = "AMEX";
        public const string NYSE = "NYSE";
        public static List<string> Exchanges
        {
            get
            {
                return new List<string>
                {
                    NASDAQ,
                    AMEX,
                   NYSE
                };
            }
        }
        /// <summary>
        /// Gets the file Creation time for the exchange file
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static DateTime GetFileDate(string exchange)
        {
            //read the file's time stamp and return
            return File.GetCreationTime(GetFilePathFor(exchange));

        }
        public const string UrlFormat = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange={0}&render=download";
        public const string FileNameFormat = "{0}.csv";
        public const int MaximumDaysInAge = 1;
        /// <summary>
        /// Gets The url for the exchange download of tickers
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static string GetUrlFor(string exchange)
        {
            return string.Format(UrlFormat, exchange);
        }
        /// <summary>
        /// returns the full file name with path for the exchange file
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static string GetFilePathFor(string exchange)
        {
            var fileName = string.Format(FileNameFormat, exchange);
            return Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), fileName);

        }
        /// <summary>
        /// Downloads the exchange file
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string DownloadFile(string from,string to)
        {            
            using (var client = new WebClient())
            {
                 
                client.DownloadFile(from, to);
                Logger.WriteLine(MessageType.Information, "Downloaded from " + from + " to " + to);
            }
            return to;
        }
        public static List<string> GetTickers(string exchange)
        {
            //open the file
            //parse the file
            //foreach row in the file, get the first column of data
            var tickers = new List<string>();
            var reader = new StreamReader(File.OpenRead(FileService.GetFilePathFor(exchange)));
            bool firstRow = true;
            while (!reader.EndOfStream)
            {
                if(firstRow)
                {
                    firstRow = false;
                    continue;
                }
                var line = reader.ReadLine();
                var values = line.Split(',');
                tickers.Add(values[0].Replace("\"",string.Empty));                 
            }
           tickers.Sort();
           return tickers;
        }
        /// <summary>
        /// Downloads each exchange file if the file is not present or if the file is 
        /// older than 24 hours
        /// </summary>
        public static void UpdateExchangeFiles()
        {
            DateTime minimumDate = DateTime.Now.AddDays(-MaximumDaysInAge);
            foreach (var exchng in FileService.Exchanges)
            {
                //get the full path the exchange file
                var filename = GetFilePathFor(exchng);
                var url = GetUrlFor(exchng);
                if(!File.Exists(filename))
                {
                    DownloadFile(url, filename);
                    continue;
                }
                if(DeleteFileIfOlderThan(GetFilePathFor(filename), minimumDate))
                {
                    //download it new
                    DownloadFile(url, filename);
                }
            }
        }
        private const int RetriesOnError = 3;
        private const int DelayOnRetry = 1000;
        private static bool DeleteFileIfOlderThan(string path, DateTime date)
        {
            for (int i = 0; i < RetriesOnError; ++i)
            {
                try
                {
                    FileInfo file = new FileInfo(path);
                    if (file.CreationTime < date)
                    {
                        Logger.WriteLine(MessageType.Information, "Deleted old file " + path);
                        file.Delete();
                    }

                    return true;
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(DelayOnRetry);
                }
                catch (UnauthorizedAccessException)
                {
                    System.Threading.Thread.Sleep(DelayOnRetry);
                }
            }

            return false;
        }

    }
}