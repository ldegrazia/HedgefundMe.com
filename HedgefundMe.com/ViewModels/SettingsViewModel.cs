using System;
using PagedList;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.ViewModels
{
    public class SettingsViewModel
    {
        public IPagedList<TradeHistoryEntry> Data { get; set; }
        public SettingsViewModel( MarketDataParameters para, ProjectEntities db)
        {
            var todaysHistory = db.TradeHistory.OrderByDescending(t=>t.OpenDate).ToList();
            Data = todaysHistory.ToPagedList(para.PageNumber, para.PageSize); 
             
        }
    }
}