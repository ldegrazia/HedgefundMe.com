using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class StrategyViewModel
    {
        ProjectEntities _context;
        public Strategy Strategy { get; set; }
        public string ChosenStrategy { get; set; }
        public IEnumerable<SelectListItem> Strategies
        {
            get
            {
                var service = new BackTestService(_context);
                var strats = service.GetStrategies();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach( var s in strats)
                {
                    items.Add(new SelectListItem { Text = s.Name, Value = s.Name });
                }
                return items;
            }
        }
        public StrategyViewModel(ProjectEntities context)
        {
            Strategy = null; 
            _context = context; 
        }
        public void RunStrategy(string name)
        {
            ChosenStrategy = name;
            var service = new BackTestService(_context);
            Strategy = service.GetStrategy(name);
        }
    }
}