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
    public class BackTestViewModel
    {
            public List<Position> Positions { get; set; }
            public List<BestBet> BestBets { get; set; }
            public BackTestParameters Parameters { get; set; }
            private BackTestService service;
            public string Result { get; set; }
            public IEnumerable<SelectListItem> DaysBack
            { 
                get
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    for (int i = 5; i < 95; i+=5)
                    {
                        items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    }
                    return items;
                }
            }
            public IEnumerable<SelectListItem> BuyInDollarAmount
            {
                get
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                 
                     items.Add(new SelectListItem { Text = Strings.BackTest.BuyInShareQty, Value = "0" });
                     items.Add(new SelectListItem { Text = Strings.BackTest.BuyInDollars, Value = "1" });
                    return items;
                }
            }
            public BackTestViewModel(BackTestParameters parameters, ProjectEntities context)
            {
                Positions = new List<Position>();
                BestBets = new List<BestBet>();
                service = new BackTestService(context);
                Parameters = parameters;
            }
            public void BackTest()
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime from = DateTime.Now.AddDays(-Parameters.DaysBack);
                    Positions = service.BackTest(Parameters);
                    sb.AppendLine(string.Format("<p>Backtesting {0} from <b>{1}</b> until <b>{2}</b></p>", Parameters.Ticker, Parameters.From().ToShortDateString(), DateTime.Now.ToShortDateString()));
                    if(!Positions.Any())
                    {
                        sb.AppendLine("<p>No trade signals found in the time frame specified for " + Parameters.Ticker + "</p>");
                    }
                    Result = sb.ToString();
                }
                catch (Exception ex)
                {
                    Result = ex.Message;
                }
            }
         public void GetBestBets()
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime from = DateTime.Now.AddDays(-Parameters.DaysBack);
                    BestBets = service.BestBets(Parameters);
                    sb.AppendLine(string.Format("<p>Best bets backtesting from <b>{0}</b> until <b>{1}</b></p>", Parameters.From().ToShortDateString(), DateTime.Now.ToShortDateString()));
                    if (!BestBets.Any())
                    {
                        sb.AppendLine("<p>No trade signals found in the time frame specified.</p>");
                    }
                    Result = sb.ToString();
                }
                catch (Exception ex)
                {
                    Result = ex.Message;
                }
            }
    }
}