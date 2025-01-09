using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Configuration;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.Models
{
    /// <summary>
    /// The entities in this project
    /// </summary>
    public partial class ProjectEntities : DbContext
    {
        /// <summary>
        /// Constructor, uses the connection string
        /// </summary>
        public ProjectEntities()
            : base("ApplicationServices")
        {
        }
        // base("ApplicationServices") { }
        /// <summary>
        /// The users in the database
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// The roles in the database
        /// </summary>
        public DbSet<Role> Roles { get; set; }
        /// <summary>
        /// Roles and their users
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }
        /// <summary>
        /// Password reset requests
        /// </summary>
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        /// <summary>
        /// The current Trade Blotter
        /// </summary>
        public DbSet<BlotterEntry> TradeBlotter { get; set; }
        /// <summary>
        /// All transactions with pnl
        /// </summary>
        public DbSet<TradeHistoryEntry> TradeHistory { get; set; }
        /// <summary>
        /// Swap log of trade signals
        /// </summary>
        public DbSet<TradeSignal> NewTradeSignals { get; set; }
        /// <summary>
        /// Historical log of all trade signals
        /// </summary>
        public DbSet<TradeSignalHistory> TradeSignalHistory { get; set; }
        /// <summary>
        /// Market data for the universe
        /// </summary>
        public DbSet<MarketData> MarketData { get; set; }

        public DbSet<TradingSettings> TradingSettings { get; set; }

        /// <summary>
        /// Market data for the back testing
        /// </summary>
        public DbSet<BackTestData> BackTestData { get; set; }
        /// <summary>
        /// The protfolios in yahoo that we use
        /// </summary>
        public DbSet<PortfolioEntry> Portfolios { get; set; }
        /// <summary>
        /// Site audit records like last data fetch and signal fetches
        /// </summary>
        public DbSet<SiteAuditRecord> SiteAuditRecords { get; set; }
    }
    public partial class Role : IReadOnly
    {
        /// <summary>
        /// The identity of the record
        /// </summary>
        [Key]
        public int RoleId { get; set; }
        /// <summary>
        /// The role name
        /// </summary>
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The description of the role
        /// </summary>
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// The role photo file name
        /// </summary>
        [Display(Name = "Photo")]
        public string Photo { get; set; }

        [Display(Name = "Member Count")]
        public int? MemberCount { get; set; }
        [Required]
        public string AppName { get; set; }
        /// <summary>
        /// Readonly roles cannot be renamed or deleted. Readonly members cannot be removed.
        /// </summary>
        [Required]
        [Display(Name = "Is Read Only")]
        public bool IsReadOnly { get; set; }
    }
    /// <summary>
    /// Class that represents a user
    /// </summary>
    public partial class User : IReadOnly
    {
        /// <summary>
        /// The identity of the record
        /// </summary>
        [Key]
        public int UserId { get; set; }
        /// <summary>
        /// User's first name
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        /// <summary>
        /// Username, required and must be unique
        /// </summary>
        [Display(Name = "Username")]
        [RegularExpression(Constants.ValidEntries, ErrorMessage = ErrorConstants.EntryInvalid)]
        public string UserName { get; set; }

        /// <summary>
        /// Email address of the user, required and must be unique
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }
        /// <summary>
        /// The users password, must be of minimum length set
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Constants.MinimuPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// The users photo filename
        /// </summary>
        [Display(Name = "Photo")]
        public string Photo { get; set; }
        /// <summary>
        /// If the user is online or not
        /// </summary>
        [Display(Name = "Is Online")]
        public bool IsOnline { get; set; }

        /// <summary>
        /// The last time the user logged on
        /// </summary>
        [Display(Name = "Last Logon")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLogon { get; set; }
        /// <summary>
        /// Returns the users first and last name, and thier photo.
        /// </summary>
        /// <returns></returns>
        public string Tooltip()
        {
            string photo = string.Empty;
            if (!string.IsNullOrEmpty(Photo))
                photo = " (" + Photo + ")";
            return "[" + UserName + "] " + FirstName + " " + LastName + photo;
        }
        [Required]
        public string AppName { get; set; }

        /// <summary>
        /// Readonly users cannot be renamed or deleted. They, themselves, can edit only certain properties.
        /// </summary>
        [Required]
        [Display(Name = "Is Read Only")]
        public bool IsReadOnly { get; set; }
    }
    public partial class UserRole
    {
        /// <summary>
        /// The identity of the record
        /// </summary>
        [Key]
        public int UserRoleId { get; set; }
        /// <summary>
        /// The role id
        /// </summary>
        [Required]
        public int RoleId { get; set; }
        /// <summary>
        /// The user id
        /// </summary>
        [Required]
        public int UserId { get; set; }

    }
    /// <summary>
    /// Class that represents a password request for a user
    /// </summary>
    public partial class PasswordResetRequest
    {
        /// <summary>
        /// The identity of the record
        /// </summary>
        [Key]
        public int RequestId { get; set; }
        /// <summary>
        /// The users id for this request
        /// </summary>
        [Required]
        public int UserId { get; set; }
        /// <summary>
        /// Unique has for this request
        /// </summary>
        [Required]
        public string HashId { get; set; }
        /// <summary>
        /// When the resquest expires
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiresOn { get; set; }


    }
    /// <summary>
    /// Custom paging class
    /// </summary>
    public class Pager
    {
        private int _page;
        /// <summary>
        /// Current page number.
        /// </summary>
        public int Page
        {
            get { return _page < 1 ? 1 : _page; }
            set { _page = value; }
        }

        private int _perpage;
        /// <summary>
        /// Number of items per page to display
        /// </summary>
        public int Perpage
        {
            get { return _perpage < 1 ? Convert.ToInt32(ConfigurationManager.AppSettings[Constants.NumberOfItemsPerPageKey]) : _perpage; }
            set { _perpage = value; }
        }

        /// <summary>
        /// The lowest item in a page
        /// </summary>
        public int LowerBound
        {
            get { return RecordsPerPage - Perpage + 1; }
        }
        int _itemcount = 0;
        /// <summary>
        /// Total records. If the pagenumber is greater than the last page, the pagenumber is set to the last page number
        /// </summary>
        public int ItemCount
        {
            get { return _itemcount; }
            set
            {
                _itemcount = value;
                if (Page > Lastpage) { Page = Lastpage; }
            }
        }
        /// <summary>
        /// Number of records per page
        /// </summary>
        public int RecordsPerPage { get { return Page * Perpage; } }
        /// <summary>
        /// Max record, if the last page has less than a full page worth
        /// </summary>
        public int UpperBound
        {
            get
            {
                var upperBound = LowerBound + RecordsPerPage - 1;
                if (upperBound > ItemCount) //there are less items than the records per page
                {
                    return ItemCount;
                }
                return upperBound;
            }
        }


        /// <summary>
        /// Returns the current set from the total number of items and the  current page
        /// </summary>
        /// <returns></returns>
        public string CurrentSet()
        {

            return LowerBound + " to " + UpperBound + " of " + ItemCount;

        }
        /// <summary>
        /// If there are more records from the current page
        /// </summary>
        public bool HasMore
        {
            get { return Page < Lastpage; }
        }
        /// <summary>
        /// If there are more records from the current page
        /// </summary>
        public bool HasLess
        {
            get { return Page > 1; }
        }
        /// <summary>
        /// The previous page number
        /// </summary>
        public int PreviousPage
        {
            get { return Page - 1; }
        }
        /// <summary>
        /// The next page number or last page
        /// </summary>
        public int NextPage
        {
            get { return (Page == Lastpage) ? Page : Page + 1; }
        }
        /// <summary>
        /// The last page available
        /// </summary>
        public int Lastpage
        {
            get { return (int)Math.Ceiling((ItemCount / (double)Perpage)); }
        }

    }
    /// <summary>
    /// Interfaces to control object changes
    /// </summary>
    public interface IReadOnly
    {
        /// <summary>
        /// The object cannot be changed at all. Once created, the state stays
        /// </summary>
        bool IsReadOnly { get; set; }
    }
    /// <summary>
    /// Class represents the forms fields to edit a user
    /// </summary>
    public class EditUser : IReadOnly
    {
        /// <summary>
        /// The user's key
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The username
        /// </summary>
        [Display(Name = "Username")]
        [Required]
        [RegularExpression(Constants.ValidEntries, ErrorMessage = ErrorConstants.EntryInvalid)]
        public string UserName { get; set; }
        /// <summary>
        /// The users new first name
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        /// <summary>
        /// The users new lastname
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        /// <summary>
        /// The email address
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Photo")]
        public string Photo { get; set; }
        [Display(Name = "Is Online")]
        public bool IsOnline { get; set; }
        [Display(Name = "Last Logon")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLogon { get; set; }

        /// <summary>
        /// Only the user themselves can edit or delete readonly users. Readonly users cannot be renamed.
        /// </summary>
        [Required]
        [Display(Name = "Is Read Only")]
        public bool IsReadOnly { get; set; }
    }

    public class SiteSettings
    {
        [Display(Name = "ApplicationName")]
        public string ApplicationName { get; set; }
        [Display(Name = "LogFile")]
        public string LogFile { get; set; }
        [Display(Name = "Number Of Items Per Page")]
        public int NumberOfItemsPerPage { get; set; }
        [Display(Name = "DropRecreateDatabase?")]
        public bool DropRecreateDatabase { get; set; }
        [Display(Name = "Create Sample Roles And Users?")]
        public bool CreateSampleRolesAndUsers { get; set; }
        [Display(Name = "Number Of Sample Users")]
        public int NumberOfSampleUsers { get; set; }
        [Display(Name = "Domain Url")]
        public string DomainUrl { get; set; }
        [Display(Name = "Domain Email Suffix")]
        public string DomainEmailSuffix { get; set; }

        [Display(Name = "Use Password Strength Indicator?")]
        public bool UsePasswordStrength { get; set; }
        [Display(Name = "Throw an error wehn deleting populated roles?")]
        public bool ThrowErrorOnDeletingPopulatedRoles { get; set; }
        [Display(Name = "Role Images Root Path")]
        public string RoleImagesRootPath { get; set; }
        [Display(Name = "Default Role Photo")]
        public string DefaultRolePhoto { get; set; }

        [Display(Name = "User Images Root Path")]
        public string UserImagesRootPath { get; set; }

        [Display(Name = "Default User Photo")]
        public string DefaultUserPhoto { get; set; }

        [Display(Name = "SmtpServer")]
        public string SmtpServer { get; set; }
        [Display(Name = "SmtpServerPort")]
        public int SmtpServerPort { get; set; }

        [Display(Name = "Send Welcome Email?")]
        public bool SendWelcomeEmail { get; set; }
        [Display(Name = "Welcome Email Sender")]
        public string WelcomeEmailSender { get; set; }
        [Display(Name = "Welcome Email Subject")]
        public string WelcomeEmailSubject { get; set; }
        [Display(Name = "Welcome Email Body")]
        public string WelcomeEmailBody { get; set; }

        [Display(Name = "Send Reset Password Email?")]
        public bool SendResetPasswordEmail { get; set; }
        [Display(Name = "Reset Password Email Sender")]
        public string ResetPasswordSender { get; set; }
        [Display(Name = "Reset Password  Email Subject")]
        public string ResetPasswordSubject { get; set; }
        [Display(Name = "Reset Password Email Body")]
        public string ResetPasswordEmailBody { get; set; }
        [Display(Name = "Reset Password Link")]
        public string ResetPasswordLink { get; set; }

        [Display(Name = "Email ResetLink Marker")]
        public string EmailResetLinkMarker { get; set; }

        [Display(Name = "Password Reset Expire In Days")]
        public int PasswordResetExpireInDays { get; set; }

        [Display(Name = "User Name Marker")]
        public string UserNameMarker { get; set; }
        [Display(Name = "User Email Marker")]
        public string UserEmailMarker { get; set; }
        [Display(Name = "Domain Url Marker")]
        public string DomainUrlMarker { get; set; }

        [Display(Name = "Database Provider")]
        public string DatabaseProvider { get; set; }
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }
        [Display(Name = "Database Server Version")]
        public string DatabaseVersion { get; set; }



    }

    /// <summary>
    /// Represents the credentials for a logon
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// The username or email
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        [RegularExpression(Constants.ValidEntries, ErrorMessage = ErrorConstants.EntryInvalid)]
        public string UserName { get; set; }
        /// <summary>
        /// The user's password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// Wether or not to createa persistent cookie
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class RegisterModel
    {
        
        [Display(Name = "User name")]
        [RegularExpression(Constants.ValidEntries, ErrorMessage = ErrorConstants.EntryInvalid)]
        public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Constants.MinimuPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class AccountRequestModel
    {
        [Required]
        [Display(Name = "Requested UserName")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
         
        [Display(Name = "Middle Name (Optional)")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Street Adress 1")]
        public string StreetAddress1 { get; set; }
         
        [Display(Name = "Street Adress 2")]
        public string StreetAddress2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip")]
        [StringLength(5, ErrorMessage = "The {0} must be {2} characters long.",MinimumLength =5)]
        public string Zip { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The email and confirmation email entires do not match.")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string ConfirmEmail { get; set; }

        
    }
    public class ContactModel
    { 
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }
        [Required]       
        [Display(Name = "Comments or Concerns")] 
        public string Message { get; set; } 
    }
    /// <summary>
    /// Class represents what is needed to rest a password 
    /// </summary>
    public class PasswordResetModel
    {
        /// <summary>
        /// The userid this password reset is for
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The password request key
        /// </summary>
        public int RequestId { get; set; }
        /// <summary>
        /// The password request hash
        /// </summary>
        public string HashId { get; set; }
        /// <summary>
        /// The user's name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The new password
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }
        /// <summary>
        /// Confirmation of new password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Represents the information collected to change a password
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// The user's current password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
        /// <summary>
        /// The new password, must be a certain length
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Constants.MinimuPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }
        /// <summary>
        /// Confirmation of new password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public partial class ResetPasswordModel
    {
        /// <summary>
        /// The email address for the user
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = ErrorConstants.NotAValidEmail)]
        public string Email { get; set; }


    }

    public class Tweet
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public string TimeAgo { get; set; }
        public string ID { get; set; }
        public string ImageUrl { get; set; }
    }
    public class Rss
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicationDate { get; set; }
    }
}