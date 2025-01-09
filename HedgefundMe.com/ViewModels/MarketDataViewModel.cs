using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class MarketDataViewModel
    {
        private ProjectEntities _context;
        public MarketDataService Service;
        private AnalysisService _analService;
        public List<MarketData> Longs { get; set; }
        public List<MarketData> Shorts { get; set; }
        public IPagedList<MarketData> Data { get; set; }
        public DateTime LatestDataDate { get; set; }
        public string Results { get; set; }
        //all market data sorted by date, then the side 
        public MarketDataParameters Parameters { get; set; }
        public MarketDataViewModel(MarketDataParameters parameters, ProjectEntities context)
        {
            _context = context;
            Service = new MarketDataService(_context);
            _analService = new AnalysisService(_context, Service);
            var dta = new List<MarketData>();
            LatestDataDate = Service.StartOn;
            dta = Service.GetDataOn(Service.StartOn).OrderBy(t=>t.Date).ThenBy(t=>t.Side).ThenBy(t=>t.CurrentRank).ToList();
            Longs = dta.Where(t => t.Side == Strings.Side.Long).OrderBy(t=>t.CurrentRank).ToList();
            Shorts = dta.Where(t => t.Side == Strings.Side.Short).OrderBy(t => t.CurrentRank).ToList();
            Data = dta.ToPagedList(parameters.PageNumber, parameters.PageSize);
            Parameters = parameters;
        }
        public async void SeedData()
        {
            var str = await Service.LoadDataAsync(DateTime.Now.Date);
        }
        public async Task<string> GetDataAsync()
        {
            var str = await Service.LoadDataAsync(DateTime.Now.Date); 
            return str;
        }
        public async Task<string> RefreshAsync()
        {
            var str = await Service.RefreshTodaysPortfolioData(); 
            return str;
        }
        public void Analyze()
        {
            _analService.Analyze();
        }
        
    }
}