using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Globalization;
using System.Web.Helpers;
using System.Web.Security;
using System.Data.Entity;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class RebootInitializer: DropCreateDatabaseIfModelChanges<ProjectEntities>
        {
            private readonly string _appName;

            public RebootInitializer(string applicationName)
            {
                _appName = applicationName;
            }
        
            /// <summary>
            /// Seeds the data base with table values
            /// </summary>
            /// <param name="context"></param>
            protected override void Seed(ProjectEntities context)
            {
                //add the admin
                var admin = UserService.CreateAdmin(_appName);
                context.Users.Add(admin);
                context.SaveChanges();

                //Add the Administrators Role
                var adminRol = RebootRoleProvider.CreateAdminRole(_appName);
                var userRole = RebootRoleProvider.CreateUsersRole(_appName);
                context.Roles.Add(adminRol);
                context.SaveChanges();
                context.Roles.Add(userRole);
                context.SaveChanges();
                //only one admin
                var adminuser = context.Users.Single(p => p.UserName == Constants.Admin && p.AppName == _appName);
                var usersRole = context.Roles.Single(t => t.Name == Constants.UsersRole && t.AppName == _appName);
                var adminRole = context.Roles.Single(p => p.Name == Constants.AdministratorsRole && p.AppName == _appName);
                context.UserRoles.Add(new UserRole
                {
                    RoleId = adminRole.RoleId,
                    UserId = adminuser.UserId,
                });
                Logger.WriteLine(MessageType.Information, Constants.Adding + adminuser.UserName + Constants.To + adminRole.Name);
                context.SaveChanges();
                context.UserRoles.Add(new UserRole
                {
                    RoleId = usersRole.RoleId,
                    UserId = adminuser.UserId,
                });
                Logger.WriteLine(MessageType.Information, Constants.Adding + adminuser.UserName + Constants.To + usersRole.Name);
                context.SaveChanges();
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.CreateSampleRolesAndUsersKey])) //add sample roles and users if sepcified
                {
                    Logger.WriteLine(MessageType.Information, "Sample users and roles not requested.");
                    base.Seed(context);
                    return;
                }
                Logger.WriteLine(MessageType.Information, "Sample users and roles creation requested.");
                //now add the roles
                var sampleRoles = new List<Role>
                                             {                                                
                                                 new Role
                                                     {
                                                          
                                                         Name = Constants.Level1Role,
                                                         Description = Constants.Level1RoleDescription,
                                                         AppName = _appName,
                                                          IsReadOnly = false
                                                     },
                                                 new Role
                                                     {
                                                          
                                                         Name = Constants.Level2Role,
                                                         Description = Constants.Level2RoleDescription,
                                                         AppName = _appName,
                                                          IsReadOnly = false
                                                     },
                                                 new Role
                                                     {
                                                         
                                                         Name = Constants.PremiumRole,
                                                         Description = Constants.PremiumRoleRoleDescription,
                                                         AppName = _appName,
                                                          IsReadOnly = false
                                                     }
                                             };
                foreach (var sampleRole in sampleRoles)
                {
                    context.Roles.Add(sampleRole);
                    Logger.WriteLine(MessageType.Information, Constants.Adding + sampleRole.Name);

                }
                context.SaveChanges();

                //add sample users
                int sampleUserCount = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.NumberOfSampleUsersKey]);
                if (sampleUserCount < 1)
                {
                    sampleUserCount = 1;
                }
                for (var i = 1; i < sampleUserCount; i++)
                {
                    var newUser = UserService.CreateUser(_appName,
                                                  i.ToString(CultureInfo.InvariantCulture) + Constants.SampleUserName,
                                                  i.ToString(CultureInfo.InvariantCulture),
                                                  Constants.SampleUserName,
                                                  Crypto.HashPassword(Constants.SampleUserPassword),
                                                    Constants.SampleUserName + i.ToString(CultureInfo.InvariantCulture) + ConfigurationManager.AppSettings[Constants.DomainEmailSuffixKey],
                                                   false);
                    context.Users.Add(newUser);

                    Logger.WriteLine(MessageType.Information, Constants.Adding + newUser.UserName);
                }
                context.SaveChanges();
                //everyone is a user

                foreach (User usr1 in context.Users)
                {
                    if (usr1.UserName == Constants.Admin) { continue; } //admin already added
                    var usrRole = new UserRole
                    {
                        RoleId = usersRole.RoleId,
                        UserId = usr1.UserId
                    };
                    context.UserRoles.Add(usrRole);
                    Logger.WriteLine(MessageType.Information, Constants.Adding + usr1.UserName + Constants.To + usersRole.Name);
                }
                context.SaveChanges();
                //add random users to roles
                int j = 1;
                foreach (var r in sampleRoles)
                {
                    int t = j;
                    var randomName = t.ToString(CultureInfo.InvariantCulture);
                    var users = from p in context.Users where p.UserName.Contains(randomName) && p.AppName == _appName select p;
                    foreach (var user in users)
                    {
                        var usrRole = new UserRole
                        {
                            RoleId = r.RoleId,
                            UserId = user.UserId
                        };
                        context.UserRoles.Add(usrRole);
                        Logger.WriteLine(MessageType.Information, Constants.Adding + user.UserName + Constants.To + r.Name);
                    }
                    j++;
                }
                context.SaveChanges();
                base.Seed(context);
            }

        }
    }