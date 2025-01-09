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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks; 
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
using HedgefundMe.com.Services;

namespace HedgefundMe.com.Controllers
{
    public class HomeController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        
        public ActionResult Index()
        {
           return View( );
        } 
        public ActionResult About()
        {
            return View();
        }
        //public ActionResult PrivacyPolicy()
        //{
        //    return View();
        //}
        public ViewResult SiteMap()
        {
            return View();
        }
        //public ActionResult Contact()
        //{
        //    return View();
        //} 
        //[HttpPost]
        //public ActionResult Contact(ContactModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //build the details of the message
        //            StringBuilder sb = new StringBuilder();
        //            sb.AppendLine("A user has sent a message to HedgeFundMe.com");
        //            sb.AppendLine(string.Format("Name: {0} {1}  ", model.FirstName, model.LastName));
        //            sb.AppendLine(string.Format("Email: {0}  ", model.Email));
        //            sb.AppendLine("Message:");
        //            sb.AppendLine(model.Message);  
        //            var details = sb.ToString();
        //            Logger.WriteLine(MessageType.Information, details);
        //            EmailService.SendEmail("Contact request", details);
        //            return RedirectToAction("ContactReceived", "Home");
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.WriteLine(MessageType.Error, "Contact request email failed:" + ex.Message);
        //            ModelState.AddModelError("", ex.Message);
        //        }
        //    }
        //    return View(model);
        //}
        //public ActionResult ContactReceived()
        //{
        //    return View();
        //} 
        public async Task<ActionResult> ASDFGHJ()
        {
            try
            { 
                DataManager dm = new DataManager(db);
                ViewBag.Result = await dm.Run(); 
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;
                return View();
            }
        }
         
    }

}
