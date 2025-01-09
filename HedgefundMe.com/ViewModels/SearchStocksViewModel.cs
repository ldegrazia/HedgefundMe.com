using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class SearchStocksViewModel
    {
        private ProjectEntities _context;
        public List<StockSearchResults> SearchResults { get; set; }
        public StockSearchParameters Parameters {get;set;}
        private StockService service;
        public IEnumerable<SelectListItem> DaysBack
        {

            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for(int i = 0; i < 50; i++)
                {
                    items.Add(new SelectListItem { Text =i.ToString(), Value = i.ToString() });
                }
                return items;
            }
        }
        public IEnumerable<SelectListItem> RankChange
        {

            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (int i = -100; i <= 100; i++)
                {
                    items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
                return items;
            }
        }
        public SearchStocksViewModel(StockSearchParameters parameters, ProjectEntities context)
        {
            _context = context;
             service = new StockService(_context);
            Parameters = parameters;
            SearchResults = new List<StockSearchResults>();     
        }
        public void Search()
        {
            try
            {
                SearchResults = service.SearchStocks(Parameters);
            }
            catch (Exception)
            {
                SearchResults = new List<StockSearchResults>();                
            }
        }
         
    }
}