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
using System.Globalization;
using System.IO;
using System.Web;
using System.Drawing;

namespace HedgefundMe.com.Services
{
    public abstract class PhotoService
    {
        /// <summary>
        /// Rootpath to the image storage
        /// </summary>
        protected string RootPath;
        /// <summary>
        /// The application name for different applications photos
        /// </summary>
        protected string AppName;
        /// <summary>
        /// Folder name under the root RootPath for this photo saving location ex: 'Users'
        /// </summary>
        protected string PhotoSubDirectory;

        /// <summary>
        /// The combination of roothpath and the PhotoDirectory
        /// </summary>
        protected string WorkingDirectory;

        /// <summary>
        /// Path for the default images for this photoservice
        /// </summary>
        protected string DefaultPhotoDirectoryPath;

        /// <summary>
        /// The default photo name for this service
        /// </summary>
        protected string DefaultPhoto;

        /// <summary>
        /// Where to find the default photo on disk
        /// </summary>
        protected string DefaultPhotoFullPath;

        /// <summary>
        /// Saves the photo if valid, creates the directory of not yet created.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual string SavePhoto(HttpPostedFileBase fileName)
        {
            PhotoHelper.CreateDirectory(WorkingDirectory);
            //string filePath;
            string savedPhoto;
            if (PhotoHelper.IsValidPhoto(fileName))
            {
                savedPhoto = PhotoHelper.GetUniquePhotoName(fileName.FileName);
                string filePath = Path.Combine(WorkingDirectory, savedPhoto);
                fileName.SaveAs(filePath);
            }
            else
            {
                savedPhoto = string.Empty; // DefaultPhoto;
            }
            return savedPhoto;
        }

        /// <summary>
        /// Returns where to get the default photo full path
        /// </summary>
        /// <returns></returns>
        public virtual string GetDefaultPhotoFullPath()
        {
            return DefaultPhotoFullPath;
        }

        /// <summary>
        /// Updates the existing photo with a new photo if valid, deletes the old photo if its not the default
        /// </summary>
        /// <param name="oldPhoto"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual string UpdatePhoto(string oldPhoto, HttpPostedFileBase fileName)
        {
            if (!PhotoHelper.IsValidPhoto(fileName))
            {
                return oldPhoto;
            }
            CreateWorkingDirectory();
            DeletePhoto(oldPhoto);
            return SavePhoto(fileName);
        }

        /// <summary>
        /// If the photo is not the default, it is deleted.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual void DeletePhoto(string fileName)
        {
            if (fileName == DefaultPhoto)
            {
                return;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            try
            {
                File.Delete(Path.Combine(WorkingDirectory, fileName));
            }
            catch (Exception ex)
            {
                //could notdelete old photo
                Logger.WriteLine(MessageType.Information, ex.Message);
            }

        }
        /// <summary>
        /// Deletes the photos and the subdirectory.
        /// Will not delete the default photo
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllPhotos()
        {
            //delte
            PhotoHelper.DeleteDirectory(WorkingDirectory);
            return true;
        }
        /// <summary>
        /// If the photo is empty, the default photo is returned, otherwise the photo's full working path is returned
        /// </summary>
        /// <param name="named"></param>
        /// <returns></returns>
        public virtual string GetPhoto(string named)
        {
            if (string.IsNullOrEmpty(named))
            {
                return DefaultPhotoFullPath;
            }
            try
            {
                return Path.Combine(WorkingDirectory, named);
            }
            catch
            {
                return DefaultPhotoFullPath;
            }
        }

        /// <summary>
        /// Returns just the default photo
        /// </summary>
        /// <returns></returns>
        public virtual string GetDefaultPhoto()
        {
            return DefaultPhoto;
        }

        /// <summary>
        /// Creates the working directory if it is not created already
        /// </summary>
        public virtual void CreateWorkingDirectory()
        {
            PhotoHelper.CreateDirectory(WorkingDirectory);
        }

        /// <summary>
        /// Gets the content type by checking the extension
        /// </summary>
        public virtual string GetContentType(string fileName)
        {
            return PhotoHelper.GetContentType(fileName);
        }

    }
    /// <summary>
    /// Class has methods for manipulating photos
    /// </summary>
    public static class PhotoHelper
    {
        public static string MakeThumbnail(string ofFile)
        {
            throw new NotImplementedException();
        }
        public static string ScalePhoto(string fileName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// If the directory does not exist it is created
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateDirectory(string filePath)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
        }
        public static void DeleteDirectory(string filePath)
        {
            if (Directory.Exists(filePath))
                Directory.Delete(filePath, true);
        }
        /// <summary>
        /// Creates a unique photo name using datetime ticks
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetUniquePhotoName(string fileName)
        {
            var extnsion = Path.GetExtension(fileName);
            return DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) + extnsion;
        }
        /// <summary>
        /// Determines if the photo is valid
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static bool IsValidPhoto(HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength != 0)
            {
                //check the file name
                if (photo.FileName == null) //they did not give us a valid file name
                {
                    return false;
                }
                Image i = null;
                try
                {
                    i = Image.FromStream(photo.InputStream);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Warning, ex.Message);
                    return false;
                    //not a valid image
                }
                finally
                {
                    if (i != null)
                        i.Dispose();
                }
                var extnsion = Path.GetExtension(photo.FileName);
                if (extnsion == null)
                {
                    return false;
                }
                extnsion = extnsion.ToLower();
                if (extnsion == ".png" || extnsion == ".jpeg" || extnsion == ".jpg" || extnsion == ".bmp")
                {
                    return true;
                }

            }
            return false;
        }
        /// <summary>
        /// checks that this is a valid csv file
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static bool IsValidCsvFile(HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength != 0 && ((photo.ContentLength * 1024f) / (1024f * 1024f) < 500))//must be less than 500 k
            {
                //check the file name
                if (photo.FileName == null) //they did not give us a valid file name
                {
                    return false;
                }
                
                var extnsion = Path.GetExtension(photo.FileName);
                if (extnsion == null)
                {
                    return false;
                }
                extnsion = extnsion.ToLower();
                if (extnsion == ".csv")
                {
                    return true;
                } 
            }
            return false;
        }
        /// <summary>
        /// Returns the content type by checking the file extension
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "image/.jpeg";
            }
            return "image/" + Path.GetExtension(fileName);
        }
        
    }
    /// <summary>
    /// Photo service for role photos
    /// </summary>
    public class RolePhotoService : PhotoService
    {

        public RolePhotoService()
        {
            RootPath =
                HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.RolesImagesRootPathKey]);
            AppName = ConfigurationManager.AppSettings[Constants.ApplicationNameKey];

            if (HttpContext.Current.Session[Constants.ApplicationNameKey] != null)
            {
                AppName = HttpContext.Current.Session[Constants.ApplicationNameKey].ToString();
            }
            PhotoSubDirectory = string.Empty;
            WorkingDirectory = Path.Combine(RootPath, AppName, PhotoSubDirectory);
            DefaultPhoto = ConfigurationManager.AppSettings[Constants.DefaultRolePhotoKey];
            DefaultPhotoDirectoryPath = ConfigurationManager.AppSettings[Constants.RolesImagesRootPathKey];
            DefaultPhotoFullPath = Path.Combine(RootPath, DefaultPhoto);
        }

    }

    /// <summary>
    /// Photo service for user photo
    /// </summary>
    public class UserPhotoService : PhotoService
    {
        public UserPhotoService(string userName)
        {
            RootPath =
                HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.UserImagesRootPathKey]);
            PhotoSubDirectory = userName;
            AppName = ConfigurationManager.AppSettings[Constants.ApplicationNameKey];
            if (HttpContext.Current.Session[Constants.ApplicationNameKey] != null)
            {
                AppName = HttpContext.Current.Session[Constants.ApplicationNameKey].ToString();
            }
            WorkingDirectory = Path.Combine(RootPath, AppName, PhotoSubDirectory);
            DefaultPhoto = ConfigurationManager.AppSettings[Constants.DefaultUserPhotoKey];
            DefaultPhotoDirectoryPath = ConfigurationManager.AppSettings[Constants.UserImagesRootPathKey];
            DefaultPhotoFullPath = Path.Combine(RootPath, DefaultPhoto);
        }
    }

    /// <summary>
    /// Photo service for saving the stock files
    /// </summary>
    public class StockFileService : PhotoService
    {
        public StockFileService(string userName)
        {
            RootPath =   HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[Constants.StockDataRootPath]);
             WorkingDirectory =  RootPath;
            DefaultPhoto = ConfigurationManager.AppSettings[Constants.StockDataFileName];
            DefaultPhotoDirectoryPath = ConfigurationManager.AppSettings[Constants.StockDataRootPath];
            DefaultPhotoFullPath = Path.Combine(RootPath, DefaultPhoto);
        }
        /// <summary>
        /// Creates the directory and saves the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override string SavePhoto(HttpPostedFileBase fileName)
        {
            if (!PhotoHelper.IsValidCsvFile(fileName))
            {
                throw new Exception(ErrorConstants.NotaValidCsv);
            }
            PhotoHelper.CreateDirectory(WorkingDirectory);            
            string savedPhoto = DefaultPhoto;
            string filePath = Path.Combine(WorkingDirectory, savedPhoto);
            fileName.SaveAs(filePath); 
            return savedPhoto;
        }
        /// <summary>
        /// Updates the existing file with a new file if valid, deletes the old file if it exists
        /// </summary>
        /// <param name="oldPhoto"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override string UpdatePhoto(string oldPhoto, HttpPostedFileBase fileName)
        {
            if (!PhotoHelper.IsValidCsvFile(fileName))
            {
                throw new Exception(ErrorConstants.NotaValidCsv);
            }
            CreateWorkingDirectory();
            DeletePhoto(oldPhoto);
            return SavePhoto(fileName);
        }
        /// <summary>
        /// If the file exists it is deleted.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override void DeletePhoto(string fileName)
        {
             
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            try
            {
                File.Delete(Path.Combine(WorkingDirectory, fileName));
            }
            catch (Exception ex)
            {
                //could notdelete old photo
                Logger.WriteLine(MessageType.Information, ex.Message);
            }

        }
        /// <summary>
        /// returns the file path
        /// </summary>
        /// <param name="named"></param>
        /// <returns></returns>
        public override string GetPhoto(string named)
        {
            return DefaultPhotoFullPath;             
        }
    }
}
