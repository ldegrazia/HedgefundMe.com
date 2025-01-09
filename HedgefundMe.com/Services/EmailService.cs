//================================================================================
// Reboot, Inc. Entity Framework Membership for .NET
//================================================================================
// NO unauthorized distribution of any copy of this code (including any related 
// documentation) is allowed.
// 
// The Reboot. Inc. name, trademarks and/or logo(s) of Reboot, Inc. shall not be used to 
// name (even as a part of another name), endorse and/or promote products derived 
// from this code without prior written permission from Reboot, Inc.
// 
// The use, copy, and/or distribution of this code is subject to the terms of the 
// Reboot, Inc. License Agreement. This code shall not be used, copied, 
// and/or distributed under any other license agreement.
// 
//                                         
// THIS CODE IS PROVIDED BY REBOOT, INC. 
// (“Reboot”) “AS IS” WITHOUT ANY WARRANTY OF ANY KIND. REBBOT, INC. HEREBY DISCLAIMS 
// ALL EXPRESS, IMPLIED, OR STATUTORY CONDITIONS, REPRESENTATIONS AND WARRANTIES 
// WITH RESPECT TO THIS CODE (OR ANY PART THEREOF), INCLUDING, BUT NOT LIMITED TO, 
// IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE OR 
// NON-INFRINGEMENT. REBOOT, INC. AND ITS SUPPLIERS SHALL NOT BE LIABLE FOR ANY DAMAGE 
// SUFFERED AS A RESULT OF USING THIS CODE. IN NO EVENT SHALL REBOOT, INC AND ITS 
// SUPPLIERS BE LIABLE FOR ANY DIRECT, INDIRECT, CONSEQUENTIAL, ECONOMIC, 
// INCIDENTAL, OR SPECIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, ANY LOST 
// REVENUES OR PROFITS).
// 
//                                         
// Copyright © 2012 Reboot, Inc. All rights reserved.
//================================================================================
using System;
using System.Configuration;
using System.Net.Mail;
using System.Linq;
using HedgefundMe.com.Models;
using System.Text;
using System.Collections.Generic;
namespace HedgefundMe.com.Services
{
    public partial class EmailService
    {
        public const string ReloadDataURL = "http://www.hedgefundme.com/Home/ASDFGHJ";
        public static List<string> TradeSignalRecipients = new List<string>
        {
             "comtechy@live.com" 
            //,"joseph.lobue@gmail.com"
        };

        /// <summary>
        /// Gets the user's details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static StringBuilder GetUserDetails(User model)
        {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("User details:");
             sb.AppendLine(string.Format("UserId: {0}", model.UserId));
             sb.AppendLine(string.Format("User Name: {0}", model.UserName));
             sb.AppendLine(string.Format("Name: {0} {1} {2}",model.FirstName, model.LastName));
             sb.AppendLine(string.Format("Email: {0}", model.Email));
             return sb;      
        }
        /// <summary>
        /// Sends the welcome email using the settings in the webconfig
        /// </summary>
        /// <param name="user"></param>
        public static void SendWelcomeEmail(User user)
        { 
            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.WelcomeEmailSenderKey]) };

            email.To.Add(new MailAddress(user.Email));
            string subject = ConfigurationManager.AppSettings[Constants.WelcomeEmailSubjectKey];
            email.Subject = subject.Replace(ConfigurationManager.AppSettings[Constants.DomainUrlMarkerKey], ConfigurationManager.AppSettings[Constants.DomainUrlKey]);
            email.IsBodyHtml = true;

            email.Body = FormulateWelcomeMessage(user.UserName, user.Email);
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));
            Logger.WriteLine(MessageType.Information, email.Subject + Constants.LogginSentTo + user.Email);
            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.SendWelcomeEmailKey]))
            {
                try
                {
                    smtpClient.Send(email);
                    Logger.WriteLine(MessageType.Information, user.Email + Constants.EmailWelcomeSent);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, user.Email + ErrorConstants.CouldNotSendWelcomeEmail + ex.Message);
                }
            }
        }

        /// <summary>
        /// Sends the password reset link to the user using the settings in the webconfig
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resetId"></param>
        /// <param name="hash"> </param>
        public static void SendResetEmail(User user, int resetId, string hash)
        {

            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.ResetPasswordSenderKey]) };

            email.To.Add(new MailAddress(user.Email));

            email.Subject = ConfigurationManager.AppSettings[Constants.ResetPasswordSubjectKey];
            email.IsBodyHtml = true;
            string link = ConfigurationManager.AppSettings[Constants.ResetPasswordLinkKey] + user.UserName + Constants.ResetQueryParam + resetId
                + Constants.HashResetParam + hash;
            email.Body = FormulateMessage(user.UserName, link);
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));

            Logger.WriteLine(MessageType.Information, link + Constants.LogginSentTo + user.Email);
            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.SendResetPasswordEmailKey]))
            {
                try
                {
                    smtpClient.Send(email);
                    Logger.WriteLine(MessageType.Information, user.Email + Constants.EmailResetSent + link);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, user.Email + ErrorConstants.ErrorSendingPasswordResetLink + ex.Message);
                }
            }
        }
        /// <summary>
        /// Sends and email to me@loudegrazia.com
        /// </summary>
        public static void SendEmail(string subject, string message)
        {
            //replace red here with constants
            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.WelcomeEmailSenderKey]) };

            email.To.Add(new MailAddress("me@loudegrazia.com"));
             
            email.Subject = subject;
            email.IsBodyHtml = true;

            email.Body = message + " http://www.hedgefundme.com";
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));
            Logger.WriteLine(MessageType.Information, email.Subject + Constants.LogginSentTo + "me@loudegrazia.com");

            try
            {
                smtpClient.Send(email);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Send email failed:" + ex.Message);
            }

        }
        /// <summary>
        /// Sends and email to me@loudegrazia.com
        /// </summary>
        public static void SendTradeEmail(List<TradeSignal> trades)
        {
            //replace red here with constants
            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.WelcomeEmailSenderKey]) };

            //email.To.Add(new MailAddress("me@loudegrazia.com"));
            //TradeSignalRecipients.ForEach(t => email.Bcc.Add(t));
            TradeSignalRecipients.ForEach(t => email.To.Add(t));
            email.Subject = string.Format(@"LONG TRADE SIGNALS FOR {0}", DateTime.Now.ToShortDateString());;
            email.IsBodyHtml = true; 
            email.Body = FormulateTradeMessage(trades);
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));
            Logger.WriteLine(MessageType.Information, email.Subject + Constants.LogginSentTo + "me@loudegrazia.com");

            try
            {
                smtpClient.Send(email);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Send email failed:" + ex.Message);
            }

        }
        public static void SendShortTradeEmail(List<TradeSignal> trades)
        {
            //replace red here with constants
            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.WelcomeEmailSenderKey]) };
            TradeSignalRecipients.ForEach(t => email.To.Add(t));
            email.Subject = string.Format(@"SHORT TRADE SIGNALS FOR {0}", DateTime.Now.ToShortDateString()); ;
            email.IsBodyHtml = true;
            email.Body = FormulateShortTradeMessage(trades);
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));
            Logger.WriteLine(MessageType.Information, email.Subject + Constants.LogginSentTo + "me@loudegrazia.com");

            try
            {
                smtpClient.Send(email);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Send email failed:" + ex.Message);
            }
        }
        /// <summary>
        /// Sends an email that the pnl is done
        /// </summary>
        public static void SendEndOfDayEmail()
        {
            //replace red here with constants
            var email = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings[Constants.WelcomeEmailSenderKey]) };

            email.To.Add(new MailAddress("me@loudegrazia.com"));
            email.Bcc.Add(new MailAddress("joseph.lobue@gmail.com"));
            TradeSignalRecipients.ForEach(t => email.Bcc.Add(t));
            email.Subject = string.Format(@"ALL TRADES CLOSED FOR {0}", DateTime.Now.ToShortDateString()); ;
            email.IsBodyHtml = true;
            email.Body = FormulateEodMessage();
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings[Constants.SmtpServerKey], Convert.ToInt32(ConfigurationManager.AppSettings[Constants.SmtpServerPortKey]));
            Logger.WriteLine(MessageType.Information, email.Subject + Constants.LogginSentTo + "me@loudegrazia.com"); 
            try
            {
                smtpClient.Send(email);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Send email failed:" + ex.Message);
            }
        }
        /// <summary>
        /// Creates the Reset Password email message, replacing with the username and target link
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="targetLink"></param>
        /// <returns></returns>
        public static string FormulateMessage(string userName, string targetLink)
        {
            string message = ConfigurationManager.AppSettings[Constants.ResetPasswordEmailBodyKey];
            message = message.Replace(ConfigurationManager.AppSettings[Constants.UserNameMarkerKey], userName);
            message = message.Replace(ConfigurationManager.AppSettings[Constants.EmailResetLinkMarkerKey], targetLink);
            message = message.Replace(ConfigurationManager.AppSettings[Constants.DomainUrlMarkerKey], ConfigurationManager.AppSettings[Constants.DomainUrlKey]);
            return message;
        }
        /// <summary>
        /// Creates a welcome email message for the new user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string FormulateWelcomeMessage(string userName, string email)
        {
            string message = ConfigurationManager.AppSettings[Constants.WelcomeEmailBodyKey];
            message = message.Replace(ConfigurationManager.AppSettings[Constants.UserNameMarkerKey], userName);
            message = message.Replace(ConfigurationManager.AppSettings[Constants.UserEmailMarkerKey], email);
            message = message.Replace(ConfigurationManager.AppSettings[Constants.DomainUrlMarkerKey], ConfigurationManager.AppSettings[Constants.DomainUrlKey]);
            return message;
        }
        public static string banner = @"<html>
<head>
<title></title>
 <link href=""http://www.hedgefundme.com/Content/emails.css"" rel=""stylesheet"" type=""text/css"" /> 
</head>
<body>
<div class=""main"">
<!-- Put the body of your page below this line -->
<div class=""banner"">
<a href=""http://www.hedgefundme.com"" alt=""Stock Picking For The Masses"" ><img src=""http://www.hedgefundme.com/images/logo.png"" title=""Stock picking for the masses."" alt=""HedgeFundMe.com - Stock picking for the masses""/></a>
</div> 
<div class=""heading"">
<strong>{0}</strong><br/>";


        public static string footer = @"<div class=""closing""> 
                             Careful trading and thanks for reading. 
                            <div class=""fineprint"">
                            Always perform due diligence before making any trades.
                            Data is delayed by at least 15 minutes. No information is deemed accurate. 
                            </div>
                             </div> 
                            <div class=""footer""> 
                            You are receiving this email because you have requested daily market alerts.<br/>  <br/>  
                            <div class=""copyright""> © 2014 HedgeFundMe.com <br/> All Rights Reserved. </div> 
                            </div>
                            </div>
                            </body>
                            </html>";

        /// <summary>
        /// Creates a trade email message for the  user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string FormulateTradeMessage(List<TradeSignal> trades)
        {             
            StringBuilder sb = new StringBuilder();
            string body = string.Format(banner, "LONG TRADES");
            sb.Append(body);
            string onDate = string.Format(@"<span class=""date"">SIGNALS FOR {0}</span></div>", DateTime.Now.ToLongDateString());
            sb.Append(onDate);
            int i = 0;
            if (!trades.Any())
            {
                sb.Append("<div class='pick_container'>No New signals received.</div>");
                sb.Append(footer);
                return sb.ToString();
            }
            foreach(var t in trades)
            {
                string alt = string.Empty;
                if(i %2 == 0)
                {
                    alt = "alt";
                }
                string trade =string.Format(@"<!-- begin 1 -->
                                            <div class=""pick_container {4}"">
                                            <div class=""pick_action""><span class=""{0}"">{0}&nbsp;</span></div>
                                            <div class=""pick_ticker""> {1} at {2}</div>
                                            <div class=""clear""><div>
                                            <div class=""pick_details"">{3} </div>
                                            </div> 
                                            </div>
                                            </div>
                                            <!-- end one --> ", t.Action, t.Ticker, t.Price.ToString("C2"), t.Details, alt);
                sb.Append(trade);
                i++;
            } 
            sb.Append(footer);
            return sb.ToString();
        }
        /// <summary>
        /// Creates a trade email message for the  user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string FormulateShortTradeMessage(List<TradeSignal> trades)
        {             
            StringBuilder sb = new StringBuilder();
            string body = string.Format(banner, "SHORT TRADES");
            sb.Append(body);
            string onDate = string.Format(@"<span class=""date""> SIGNALS FOR {0}</span></div>", DateTime.Now.ToLongDateString());
            sb.Append(onDate);
            int i = 0;
            if(!trades.Any())
            {
                sb.Append("<div class='pick_container'>No New signals received.</div>");
                sb.Append(footer);
                return sb.ToString();
            }
            foreach(var t in trades)
            {
                string alt = string.Empty;
                if(i %2 == 0)
                {
                    alt = "alt";
                }
                string trade =string.Format(@"<!-- begin 1 -->
                                            <div class=""pick_container {4}"">
                                            <div class=""pick_action""><span class=""{0}"">{0}&nbsp;</span></div>
                                            <div class=""pick_ticker"">{1}  at {2}</div>
                                            <div class=""clear""><div>
                                            <div class=""pick_details"">{3} </div>
                                            </div> 
                                            </div>
                                            </div>
                                            <!-- end one --> ", t.Action, t.Ticker, t.Price.ToString("C2"), t.Details, alt);
                sb.Append(trade);
                i++;
            } 
            sb.Append(footer);
            return sb.ToString();
        }
        /// <summary>
        /// Creates a trade email message for the  user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string FormulateEodMessage()
        {
            StringBuilder sb = new StringBuilder();
            string body = string.Format(banner, "Closed Positions");
            sb.Append(body);
            string onDate = string.Format(@"<span class=""date""> Closed trades on {0}</span></div>", DateTime.Now.ToLongDateString());
            sb.Append(onDate); 
            sb.Append(@"<div class='pick_container'>Final trades closed. <br/>
            See the <a href=""http://www.hedgefundme.com/Blotter/Closed"" alt=""Closed Trades"" >Closed Trades</a> for today's PnL.</div>");
            sb.Append(footer); 
            return sb.ToString();
        }
    }
}
