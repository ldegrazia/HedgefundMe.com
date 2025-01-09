using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using HedgefundMe.com.Models;
using HedgefundMe.com.Services;
using System.Configuration;
using System.IO;
using System.Text;
namespace HedgefundMe.com.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        RebootRoleProvider _roleProvider;
        UserService _userService;
        /// <summary>
        /// Displays the users information
        /// </summary>
        /// <returns></returns>       
        public ViewResult MyAccount()
        {
            //get the current logged in user
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var usr = _userService.Get(User.Identity.Name);
            //see if the user is admin
            _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            ViewBag.IsAdmin = (_roleProvider.IsUserInRole(usr.UserName, Constants.AdministratorsRole));
            //get the users's role
            _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            ViewBag.Roles = _roleProvider.GetRolesForUser(usr.UserId);
            return View(usr);
        }
        /// <summary>
        /// Edits the users information
        /// </summary>
        /// <param name="changedUser"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult MyAccount(User changedUser, FormCollection form)
        {
            //get the current logged in user
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            ViewBag.Roles = _roleProvider.GetRolesForUser(changedUser.UserId);
            ViewBag.IsAdmin = (_roleProvider.IsUserInRole(changedUser.UserId, Constants.AdministratorsRole));
            try
            {
                var usr = _userService.Update(changedUser, Request.Files[Constants.Photo]);
                ViewBag.Result = Constants.ChangesSaved;
                return View(usr);
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;
            }
            //something happend return the original state
            var currentUser = _userService.Get(User.Identity.Name);
            return View(currentUser);

        }
        /// <summary>
        /// Cancels a users account
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CancelAccount()
        {
            //get this user and make sure they want to cancel
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var userCurrent = _userService.Get(User.Identity.Name);

            if (userCurrent.UserName == Constants.Admin)
            {
                ViewBag.ErrorMessage = ErrorConstants.CannotDeleteAdministrator;
                return View(); 
            }

            return View(userCurrent);
        }
        /// <summary>
        /// Cancels a users account
        /// </summary>
        /// <param name="canceledUser"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelAccount(User canceledUser)
        {
            //get this user and make sure they want to cancel
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            //double check admin
            var userCurrent = _userService.Get(canceledUser.UserId);
            if (userCurrent.IsReadOnly)
            {
                ViewBag.ErrorMessage = ErrorConstants.CannotChangeReadonlyUsers;
                return View();
            }
            var thisUser = _userService.Get(User.Identity.Name);
            if (thisUser.UserId != thisUser.UserId)//is the current user the same user?
            {

                ViewBag.ErrorMessage = ErrorConstants.CannotCancelOtherUsers;
                return View();
            }
            var sb = EmailService.GetUserDetails(thisUser); 
            Logger.WriteLine(MessageType.Warning, "User cancelled account:" +sb.ToString());
            EmailService.SendEmail("User Cancelled Account!", sb.ToString());
            _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            _userService.LogOff(canceledUser.UserId);
            _roleProvider.RemoveUserFromAllRoles(canceledUser.UserId);
            _userService.Delete(canceledUser.UserId);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the users photo, or nothing if there is no username
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActionResult UserPhoto(string name, string userName)
        {
            var ups = new UserPhotoService(userName);
            try
            {

                return new FileStreamResult(new FileStream(ups.GetPhoto(name), FileMode.Open), ups.GetContentType(name));
            }
            catch (Exception)
            {

                return null;
            }

        }
        [Authorize]
        public ActionResult ProfileImage()
        {
            try
            {
                _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                var userCurrent = _userService.Get(User.Identity.Name);
                var ups = new UserPhotoService(userCurrent.UserName);
                return new FileStreamResult(new FileStream(ups.GetPhoto(userCurrent.Photo), FileMode.Open), ups.GetContentType(userCurrent.Photo));
            }
            catch (Exception)
            {

                return null;
            }
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Logon(string returnUrl)
        {
            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logon(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());

                if (_userService.LogOn(model.UserName, model.Password))
                {
                    if (UserService.IsUserNameAnEmail(model.UserName))
                    {
                        //get the real username
                        model.UserName = _userService.GetUserName(model.UserName);
                    }
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorConstants.LogonFailure);
            }
            return View(model);   // If we got this far, something failed, redisplay form


        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            try
            {
                _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                _userService.LogOff(User.Identity.Name);
                FormsAuthentication.SignOut();

            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, ex.Message);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

       // [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccountRequest()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AccountRequestConfirmed(AccountRequestModel model)
        {
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountRequest(AccountRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //build the details of the message
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("A user is requesting an account for HedgeFundMe.com");
                    sb.AppendLine(string.Format("Requested User Name: {0}", model.UserName));
                    sb.AppendLine(string.Format("Name: {0} {1} {2}",model.FirstName,model.MiddleName, model.LastName));
                    sb.AppendLine("Address:");
                    sb.AppendLine(model.StreetAddress1);
                    sb.AppendLine(model.StreetAddress2);
                    sb.AppendLine(string.Format("{0},{1} {2}",model.City,model.State,model.Zip));
                    sb.AppendLine(model.Email);
                    var details = sb.ToString();
                    Logger.WriteLine(MessageType.Information, details);
                    EmailService.SendEmail("New account request created!", details);
                    return RedirectToAction("AccountRequestConfirmed", "Account",model);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Account request email failed:" + ex.Message);
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }
        //
        // POST: /Account/Register

        //[AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    //does this user exist already?
                    _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                    if(_userService.EmailExists(model.Email))
                    {
                        //this user exists so they already have an account
                        throw new Exception(ErrorConstants.EmailExists);
                    }
                    model.UserName = model.Email.Split('@')[0];
                    
                    //save this user in the session
                    Session["registeredUser"] = model;
                    var registerdUser = _userService.Register(model, null);
                    _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                    _roleProvider.AddToUsersRole(registerdUser.UserId);
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("MyAccount");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);  // If we got this far, something failed, redisplay form
        }
        [AllowAnonymous]
        public ActionResult ConfirmAndPay()
        {
            if (Session["registeredUser"] == null)
            {
                //there was a problem...
                //we need to redirect them to the failed page
                return RedirectToAction("Register", "Account"); //if there is no account in the session somebody made stuff up
            }
            var model = Session["registeredUser"] as RegisterModel;
            //does this user exist already? double check
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            if (_userService.EmailExists(model.Email))
            {
                //this user exists so they already have an account
                throw new Exception(ErrorConstants.EmailExists);
            }
            //generate the jwt details needed here....
            var googleWallet = new GoogleWalletService();
            var jwt = googleWallet.CreateSubscriptionJwt(model.Email);
            ViewBag.Jwt = jwt; //store in viewbag
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost] 
        public ActionResult Google(FormCollection form)
        {
            // Get Post Params Here
            if (form["jwt"] == null)
            {
                //we have an issue
                //return the failure....
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.ContentType = "text/plain";
                return Content(string.Empty);
            }
            string jwt = form["jwt"];
            var googleWallet = new GoogleWalletService();
            try
            {
                //verify the JWT
                var payload = googleWallet.ParseJWT(jwt);
                //we have data
                //get the user email, then order id and save it
                Logger.WriteLine(MessageType.Information, "Ok for " + payload[0] + ",orderid:" + payload[1]);
                //save this stuff
                //update the database that they paid
                //send them an email
                //send me an email
                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.ContentType = "text/plain";
                return Content(payload[1]);
            }
            catch(Exception)
            {
                //return the failure....
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.ContentType = "text/plain";
                return Content(string.Empty); 
            }
            
        }
        [AllowAnonymous]
        public ActionResult Success()
        {
            //referer should be paypal here.
            //the account should be valid
            if(Session["registeredUser"]==null)
            {
                //there was a problem...
                //we need to redirect them to the failed page
                return RedirectToAction("PaymentError", "Account"); //if there is no account in the session somebody made stuff up
            }
            //register them here, log them in. We are good.
            try
            {
                var model = Session["registeredUser"] as RegisterModel;
                _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                var registerdUser = _userService.Register(model, null);
                _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                _roleProvider.AddToUsersRole(registerdUser.UserId);
                FormsAuthentication.SetAuthCookie(model.UserName, true /* createPersistentCookie */);
                var sb = EmailService.GetUserDetails(registerdUser);
                Logger.WriteLine(MessageType.Information, "User created an account:" + sb.ToString());
                EmailService.SendEmail("User created an account.", sb.ToString());
                //Session["registeredUser"] = null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Registration Success threw and error:" + ex.Message);
                return RedirectToAction("PaymentError");
            }
            return View();
        }
        /// <summary>
        /// If the payment is invalid
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PaymentError()
        {
            return View();
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            //only the Admin can change the admin password
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                    changePasswordSucceeded = _userService.ChangePassword(User.Identity.Name, model.OldPassword,
                                                                 model.Password);

                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, ErrorConstants.PasswordChangeFailure + ex.Message);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", ErrorConstants.PasswordChangeFailure);
            }
            return View(model);// If we got this far, something failed, redisplay form

        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        public ViewResult ForgotPassword()
        {
            var m = new ResetPasswordModel();
            return View(m);
        }
        /// <summary>
        /// Takes the user to the reset password form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //use the user service to get the user
                try
                {
                    _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                    User u = _userService.GetByEmail(model.Email);
                    _userService.RequestPasswordReset(u.UserId);

                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, ErrorConstants.ErrorSendingPasswordResetLink + ex.Message);
                }
            }
            return RedirectToAction("PasswordLinkSent", "Account");

        }
        /// <summary>
        /// Informs the user their password has been reset if email is valid
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ViewResult PasswordLinkSent()
        {

            return View();
        }
        /// <summary>
        /// If the request is invalid
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ViewResult PasswordResetInvalid()
        {

            return View();
        }

        /// <summary>
        /// Resets the password
        /// </summary>
        /// <param name="reset"></param>
        /// <param name="username"></param>
        /// <param name="hash"> </param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string reset, string username, string hash)
        {
            if ((reset != null) && (username != null) && (hash != null))
            {
                try
                {
                    //use the default user service to get the userid for this request
                    //if it is invalid, throw an error

                    int resetReqId = Convert.ToInt32(reset);

                    _userService = new UserService(ConfigurationManager.AppSettings[Constants.SessionAppNameKey]);
                    var currentUser = _userService.GetRequestUser(resetReqId, hash);
                    //is this a validResetRequest?
                    if (_userService.IsValidPasswordResetRequest(currentUser.UserId, resetReqId, hash))
                    {
                        //the request is valid, get the old password for the user and let them change the password
                        var newChange = new PasswordResetModel { RequestId = resetReqId, UserName = currentUser.UserName, UserId = currentUser.UserId };
                        HttpContext.Session[Constants.ApplicationNameKey] = currentUser.AppName;//set the conext of the application for the user
                        return View(newChange);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, ErrorConstants.CouldNotResetPassword + ex.Message);
                }
            }
            return RedirectToAction("PasswordResetInvalid");
        }
        /// <summary>
        /// Tries to reset the password to the new credentials
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(PasswordResetModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
                    User u = _userService.Get(model.UserId);//try to get the userid 
                    _userService.ResetPassword(u.UserId, model.Password); //update the password for the user
                    _userService.RemovePasswordResetRequests(u.UserId);
                    return RedirectToAction("ChangePasswordSuccess"); //redirect to Succes Page
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, ErrorConstants.CouldNotResetPassword + ex.Message);
                }
            }
            return View(model);  // If we got this far, something failed, redisplay form
        }
    }
}
