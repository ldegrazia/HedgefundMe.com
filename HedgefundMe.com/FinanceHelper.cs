using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HedgefundMe.com
{
    public static class FinanceHelper
    {
        public const string ImageWidth = "310";
        public const string ImageHeight = "270";
        public const string Indicators3m = "&t=3m&l=on&z=l&q=l&p=m50%2Cm200&a=v&c=";
        public const string Indicators6m = "&t=3m&l=on&z=l&q=l&p=m10%2Cm20%2Cm50&a=r14%2Cm26-12-9&c=";
        public const string Indicators5d = "&t=5d&l=on&z=l&q=l&p=m10%2Cm20%2Cm50&a=r14%2Cm26-12-9&c=";
        public const string Indicators3ms = "&t=3m&l=on&z=s&q=l&p=m50%2Cm200&a=v&c=";
        public const string Indicators23ms = "&t=3m&l=on&z=m&q=l&p=m10%2Cm20%2Cm50&a=v&c=";
        public const string Indicators6ms = "&t=6m&l=on&z=s&q=l&p=m50%2Cm200&a=v&c=";
        public const string Indicators5ds = "&t=5d&l=on&z=s&q=l&a=v&c=";
        public const string Indicators1ds = "&t=1d&l=on&z=s&q=l&a=v&c=";
        public static double PriceGain(double start, double end)
        {
            return (end - start) / start;
        }
        /// <summary>
        /// Finds the correct share amount given the total dollar amount
        /// </summary>
        /// <param name="dollarAmount"></param>
        /// <param name="atThisPrice"></param>
        /// <returns></returns>
        public static int GetShareAmount(int dollarAmount, double atThisPrice)
        {

            try
            {
                if(dollarAmount == 0 || atThisPrice == 0)
                {
                    Logger.WriteLine(MessageType.Error, "Could not get share amount for " + dollarAmount + " with price of " + atThisPrice);
                    return 0;
                }
                var shareQty = Convert.ToInt32(Math.Floor(dollarAmount / atThisPrice));  //we buy in $1000 amounts at a price of 25 = 400 shares
                if (shareQty <= 0)
                {
                    return 1;
                }
                return shareQty;
            }
            catch
            {
                Logger.WriteLine(MessageType.Error, "Could not get share amount for " + dollarAmount + " with price of " + atThisPrice);
            }
            return 0;
        }
        public static string Direction(int rankChange)
        {
            if (rankChange < 0)
            {
                return Strings.Directions.DOWN;
            }
            else if (rankChange == 0)
            {
                return Strings.Directions.EVEN;
            }
            else
            {
                return Strings.Directions.UP;
            }
        }
        public static double VolumeChange(double? avgvolume, double? volume)
        {
            if (avgvolume.HasValue && volume.HasValue)
            {
                if (avgvolume.Value == 0 || volume.Value == 0)
                {
                    return 0;
                }
                return (volume.Value / avgvolume.Value) / 100;

            }
            return 0;

        }
        public static string HeatMapColor(int rankchange)
        {            
            
            if (rankchange < 0)
            {
                return Strings.Colors.RED;
            }
            if (rankchange ==0)
            {
                return Strings.Colors.BLACK;
            }

            return Strings.Colors.GREEN;
        }
        /// <summary>
        /// Gets the trades value as shares time price plus commission
        /// </summary>
        /// <param name="price"></param>
        /// <param name="shares"></param>
        /// <param name="commision"></param>
        /// <returns></returns>
        public static double GetTradeValue(double price, int shares, double commision)
        {
            return price * shares + commision;
        }
        public static string GetColor(double? gain)
        {
            string g = "darkgray";
            if (gain < 0)
            {
                return "red";
            }
            if (gain > 0)
            {
                return "#4CBB17";//"#6CFF0A";
            }
            return g;
        }
        public static string GetColor(string details)
        {
            string g = "";
            if (details.Contains("Hit stop"))
            {
                return "red";
            }
            if (details.Contains("Hit target"))
            {
                return "#4CBB17";//"#6CFF0A";
            }
            return g;
        }
        public static string IsPriceMissing(double? price)
        {
            string g = "";//#daa520 Color Hex Goldenrod
            if (!price.HasValue)
            {
                return "#daa520";
            }
            if (price.Value==0)
            {
                return "#daa520"; 
            }
            return g;
        }
        public static string GetStopColor(double? stop, double currentPrice, bool isLong)
        {
            string g = "darkgray";
            if(isLong)
            {
                if (currentPrice <=stop)
                {
                    return "red";
                }
               
                return g;
            }
            if (currentPrice >= stop)
            {
                return "red";
            }            
            return g;
        }
        public static string GetTargetColor(double? target, double currentPrice, bool isLong)
        {
            string g = "darkgray";
            if (isLong)
            {
                if (currentPrice >= target)
                {
                    return "#4CBB17";
                }

                return g;
            }
            if (currentPrice <= target)
            {
                return "#4CBB17";
            }
            return g;
        }
        public static string GetColor(int gain)
        {
            string g = "darkgray";
            if (gain < 0)
            {
                return "red";
            }
            if (gain > 0)
            {
                return "#4CBB17";
            }
            return g;
        }
        public static string GetVolumeColor(double gain)
        {
            string g = "darkgray";
            if (gain > 0.011)
            {
                return "#4CBB17";
            }
            return g;
        }
    }
}