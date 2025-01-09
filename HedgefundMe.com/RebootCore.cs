using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.IO;

namespace HedgefundMe.com
{
        /// <summary>
        /// Various constants used throughout the code.
        /// Change any text values you would like for your version
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// The session key
            /// </summary>
            public const string SessionAppNameKey = "_App_Name";
            //User defaults
            /// <summary>
            /// The Administrator user name 
            /// </summary>
            public const string Admin = "Admin";
            /// <summary>
            /// The administrator password default
            /// </summary>
            public const string AdminPassword = "123456";
            //Sample user details 
            public const string SampleUserName = "user";
            public const string SampleUserPassword = "Apassw0rd";
            // Role defaults
            public const string AdministratorsRole = "Administrators";
            public const string AdministratorsRoleDescription = "Administrators have full control";
            public const string UsersRole = "Users";
            public const string UsersRoleDescription = "Regular users of the site";
            public const string Level1Role = "Level 1";
            public const string Level1RoleDescription = "Level 1 users of the site";
            public const string Level2Role = "Level 2";
            public const string Level2RoleDescription = "Level 2 users of the site";
            public const string PremiumRole = "Premium Users";
            public const string PremiumRoleRoleDescription = "Premium users of the site";
            public const string XmlResponse = "text/xml";
            // forms authentication storing password routines. Consider encryption of machine keys
            public const string HashMethod = "Md5";

            //web config keys
            /// <summary>
            /// The database connection string key, also resides in the Project Entities class
            /// </summary>
            public const string ConnectionStringKey = "ApplicationServices";
            /// <summary>
            /// The default application name key in the web config
            /// </summary>
            public const string ApplicationNameKey = "ApplicationName";
            //key to lo file in wobconfig
            public const string LogFileKey = "LogFile";
            public const string ExportFileKey = "ExportDataFile";
            //EF initializations
            public const string DropRecreateDatabaseKey = "DropRecreateDatabase";
            public const string CreateSampleRolesAndUsersKey = "CreateSampleRolesAndUsers";
            public const string NumberOfSampleUsersKey = "NumberOfSampleUsers";

            public const string DomainEmailSuffixKey = "DomainEmailSuffix";
            public const string NewRoleDescription = "New Role";
            public const string ResetQueryParam = "&reset=";
            public const string HashResetParam = "&hash=";
            public const int MinimuPasswordLength = 8;

            public const string UsePasswordStrengthKey = "UsePasswordStrength";

            public const string NumberOfItemsPerPageKey = "NumberOfItemsPerPage";

            public const string DefaultUserPhotoKey = "DefaultUserPhoto";
            public const string UserImagesRootPathKey = "UserImagesRootPath";
            public const string DefaultRolePhotoKey = "DefaultRolePhoto";
            public const string RolesImagesRootPathKey = "RoleImagesRootPath";
            public const string RolePhotoServiceSessionKey = "RolePhotoService";
            public const string PasswordResetExpireInDaysKey = "PasswordResetExpireInDays";
            public const string WelcomeEmailSenderKey = "WelcomeEmailSender";
            public const string WelcomeEmailSubjectKey = "WelcomeEmailSubject";
            public const string WelcomeEmailBodyKey = "WelcomeEmailBody";
            public const string UserEmailMarkerKey = "UserEmailMarker";

            public const string DomainUrlKey = "DomainUrl";
            public const string DomainUrlMarkerKey = "DomainUrlMarker";
            public const string SmtpServerKey = "SmtpServer";
            public const string SmtpServerPortKey = "SmtpServerPort";
            public const string SendWelcomeEmailKey = "SendWelcomeEmail";
            public const string ResetPasswordSubjectKey = "ResetPasswordSubject";
            public const string ResetPasswordSenderKey = "ResetPasswordSender";
            public const string ResetPasswordLinkKey = "ResetPasswordLink";
            public const string SendResetPasswordEmailKey = "SendResetPasswordEmail";
            public const string ResetPasswordEmailBodyKey = "ResetPasswordEmailBody";
            public const string UserNameMarkerKey = "UserNameMarker";
            public const string EmailResetLinkMarkerKey = "EmailResetLinkMarker";

            public const string ThrowErrorOnDeletingPopulatedRolesKey = "ThrowErrorOnDeletingPopulatedRoles";


            public const string Photo = "photo";
            public const string File = "file";
            public const string EmailRegex =
                "^([a-zA-Z0-9_\\-\\.]+)@[a-z0-9-]+(\\.[a-z0-9-]+)*(\\.[a-zA-Z]{2,3})$";
            public const string UrlRegex = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            public const string ValidEntries = @"^[a-zA-Z0-9_@.-]*$";
            public const string AtSymbol = "@";
            public const string ChangesSaved = "Your changes have been saved.";

            public const string ApplicationVersion = "1.0.1.1";
            public const string ApplicationAuthor = "Reboot Inc.";
            public const string ApplicationMoreInfo = "http://www.RebootMyCode.com";
            public const string ApplicationDisclaimer = "© Copyright 2012, All Rights Reserved.";

            //logging strings
            public const string Adding = "Adding ";
            public const string To = " to ";
            public const string LoggedInAs = "You are logged in as ";
            public const string AddedAllUsers = "Added all users.";
            public const string RemovedAllMembers = "Removed all members.";

            public const string SqlServerCe = "SqlServerCe.4.0";
            public const string EmailResetSent = " Email reset link sent.";
            public const string EmailWelcomeSent = " Welcome email sent.";
            public const string LogginSentTo = " sent to ";

            public const string SubscriptionCost = "$99.00 Per Month";
            //root path key to the stocks file
            public const string StockDataRootPath = "StockDataRootPath";
            //default stocks.csv name
            public const string StockDataFileName = "StockDataFileName";
 

        }
        /// <summary>
        /// string ids in form submissions
        /// </summary>
        public static class FormKeys
        {
            public const string ChangeAppName = "changeappname";
            public const string NewAppName = "newappname";
            public const string DeleteInputs = "deleteInputs";
        }

        /// <summary>
        /// Class for errors that are displayed to users. There are other messages in utilities.js
        /// </summary>
        public static class ErrorConstants
        {
            public const string LogonFailed = " Logon Failed. ";
            public const string PasswordResetFailed = " Password Reset Failed. ";
            public const string NoSuchEmail = "No such email exists.";
            public const string CheckEntryAndTryAgain = "Check entries and try again.";
            public const string EmailExists = "Email exists.";
            public const string UserNameExists = "Username exists.";
            public const string RoleExists = "Role exists.";
            public const string CouldNotCreateUser = "Could not create user";
            public const string RoleIsPopulated = "Users are in the role, cannot delete.";
            public const string NotAValidEmail = "Not a valid email address.";
            public const string YouCannotDeleteyourself = "You cannot delete yourself.";
            public const string CouldNotResetPassword = "Could not reset password:";
            public const string ErrorSendingPasswordResetLink = " Error sending password rest link:";
            public const string CouldNotSendWelcomeEmail = "Could not send welcome email:";
            public const string CouldNotRegisterUser = " Could not register user:";
            public const string PasswordChangeFailure = "The current password is incorrect or the new password is invalid.";
            public const string LogonFailure = "The user name or password provided is incorrect.";
            public const string CouldNotMarkUserOffline = "Could not mark user offline.";
            public const string ChooseAtLeastOneRole = "Please choose at least one role for the user.";
            public const string CannotRenameAdministratorsRole = "You cannot rename the Administrator's role.";
            public const string CannotDeleteAdministratorsRole = "You cannot delete the Administrator's role.";
            public const string CannotDeleteAdministrator = "You cannot delete the Administrator.";
            public const string CannotChangeReadonlyUsers = "Readonly users cannot be renamed or deleted. They, themselves, can edit only certain properties.";
            public const string CannotCancelOtherUsers = "You cannot cancel other users.";
            public const string CannotChangeReadonlyRoles = "Readonly roles cannot be renamed or deleted. Readonly members cannot be removed.";
            public const string CannotRenameTheAdministrator = "You cannot rename the Administrator.";
            public const string CannotRemoveTheAdministrator = "You cannot remove the Administrator from the Administrator role.";
            public const string NothingSelected = "Nothing Selected.";
            public const string Unavailable = "Unavailable";
            public const string AppNameExists = "New Application Name Exists.";
            public const string AppNameNotValid = "New Application Name Is Not Valid.";
            public const string EntryInvalid = "Your entry has some invalid characters.";
            public const string NotaValidCsv = "Not a valid CSV file.";
            public const string NameExists = "This name already exists.";

        }
    public static class FileWriter
    {
        public static void DeleteFile(string fileName)
        {
            if(File.Exists(HttpContext.Current.Server.MapPath(fileName)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(fileName));
            }
        }
        /// <summary>
        /// Writes the string to the file path, like ~/App_Data/file.txt
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        public static void WriteLine(string fileName, string message)
        {
            try
            {
                using (var w = File.AppendText(HttpContext.Current.Server.MapPath(fileName)))
                {
                    w.WriteLine(message);
                    w.Flush();// Update the underlying file.
                    // Close the writer and underlying file.
                    w.Close();
                }
            }
            catch (Exception ex)
            {   //check here why it failed and ask user to retry if the file is in use. 
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
        public enum MessageType
        {
            Information,
            Warning,
            Error
        }
        /// <summary>
        /// Logging function which writes to te log in the location specified in the web config
        /// </summary>
        public static class Logger
        {

            /// <summary>
            /// Writes a log entry with the message
            /// </summary>
            /// <param name="message"></param>
            /// <param name="theType"></param>
            public static void WriteLine(MessageType theType, string message)
            {
                try
                {

                    using (var w = File.AppendText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.LogFileKey])))
                    {
                        Log(theType, message, w);
                        // Close the writer and underlying file.
                        w.Close();
                    }
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            /// <summary>
            /// Writes to the log
            /// </summary>
            /// <param name="type"></param>
            /// <param name="logMessage"></param>
            /// <param name="w"></param>
            public static void Log(MessageType type, string logMessage, TextWriter w)
            {
                w.WriteLine("{0},{1},{2},{3}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), type, logMessage);
                w.Flush();// Update the underlying file.
            }
            /// <summary>
            /// Returns the log
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            public static string ViewLog()
            {
                try
                {
                    using (
                        var reader =
                            File.OpenText(
                                HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.LogFileKey])))
                    {
                        // While not at the end of the file, read and write lines.
                        var sb = new StringBuilder();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            sb.AppendLine("<br/>");
                            sb.AppendLine(line);
                        }
                        reader.Close();
                        return sb.ToString();
                    }
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    return ex.Message;
                }
            }
            public static string DeleteLog()
            {
                try
                {

                    File.Delete(
                        HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.LogFileKey]));
                    return "Deleted log";
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    return ex.Message;
                }
            }
        }
        public static class Exporter
        {


            /// <summary>
            /// Writes a to the export file
            /// </summary>
            /// <param name="message"></param>
            /// <param name="theType"></param>
            public static void Export(string message)
            {
                try
                {

                    using (var w = File.AppendText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.ExportFileKey])))
                    {
                        Write(message, w);
                        // Close the writer and underlying file.
                        w.Close();
                    }
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            /// <summary>
            /// Writes to the log
            /// </summary>
            /// <param name="type"></param>
            /// <param name="logMessage"></param>
            /// <param name="w"></param>
            private static void Write(string logMessage, TextWriter w)
            {
                w.WriteLine(logMessage);
                w.Flush();// Update the underlying file.
            }
            /// <summary>
            /// Returns the log
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            public static string ShowExport()
            {
                try
                {
                    using (
                        var reader =
                            File.OpenText(
                                HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.ExportFileKey])))
                    {
                        // While not at the end of the file, read and write lines.
                        var sb = new StringBuilder();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            sb.AppendLine("<br/>");
                            sb.AppendLine(line);
                        }
                        reader.Close();
                        return sb.ToString();
                    }
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    return ex.Message;
                }
            }
            public static string DeleteExport()
            {
                try
                {

                    File.Delete(
                        HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.ExportFileKey]));
                    return "Deleted Export file in " + HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.ExportFileKey]);
                }
                catch (Exception ex)
                {   //check here why it failed and ask user to retry if the file is in use. 
                    return ex.Message;
                }
            }

        }
        public static class Sizes
        {
           
            public const double ChartWidthLandscape =400;
            public const double ChartHeightLandscape = 293;
        }
        public static class CurrentServer
        {
            public static bool IsLocal
            {
                get { return HttpContext.Current.Request.Url.Authority.Contains("localhost:"); }
            }
        }
    }
