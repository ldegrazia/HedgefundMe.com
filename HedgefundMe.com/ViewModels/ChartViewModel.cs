using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class ChartViewModel
    {
        public IEnumerable<SelectListItem> Exchange
        { 
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                foreach(var exchange in FileService.Exchanges)
                {
                    items.Add(new SelectListItem { Text = exchange, Value = exchange });
                }
                return items;
            }
        }
        public IEnumerable<SelectListItem> StartsWith
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    //do something with letter 
                    items.Add(new SelectListItem { Text = c.ToString(), Value = c.ToString() });
                }
                return items;
            }
        }
        public  IPagedList<string> Tickers { get; set; }
        public string LastNasdaqFile { get; set; }
        public string LastAmexFile { get; set; }
        public string LastNYSEFile { get; set; }
        public ChartFetchParameters Parameters { get; set; }
        
        public ChartViewModel(ChartFetchParameters parameters)
        {
            
            //lets get all the files for each exchange
            try
            {
                //lets get the dates for each
                LastNasdaqFile = DateHelper.GetTimeInWords(FileService.GetFileDate(FileService.NASDAQ));
                LastNYSEFile = DateHelper.GetTimeInWords(FileService.GetFileDate(FileService.NYSE));
                LastAmexFile = DateHelper.GetTimeInWords(FileService.GetFileDate(FileService.AMEX)); 
                if(string.IsNullOrEmpty(parameters.Exchange))
                {    
                    FileService.UpdateExchangeFiles();
                    Tickers = null;
                    Parameters = parameters; 
                    return;
                }
                
                //we have a parameter set so lets find them
                var stockList = FileService.GetTickers(parameters.Exchange).Where(t => t.StartsWith(parameters.StartsWith));
                //now only return the stocks that start with the letter and then page them
                Tickers = stockList.ToPagedList(parameters.PageNumber,parameters.PageSize);//).Skip(parameters.CurrentPage * parameters.ItemsPerPage).Take(parameters.ItemsPerPage).ToList();
                
                Parameters = parameters;
             

            }
            catch(Exception ex)
            {
                LastNasdaqFile ="Unknown";
                LastNYSEFile = "Unknown";
                LastAmexFile = "Unknown";
                Tickers = null;
                Logger.WriteLine(MessageType.Error, ex.Message);
            }
        }

    }
}