using DDDCqrsEs.Application.Models.AuthenticationModels;
using DDDCqrsEs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using DDDCqrsEs.Common;
using Quotation.Domain.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDDCqrsEs.Infrastructure.Services
{
    [MapServiceDependency(nameof(UserService))]
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void DeleteUser(Guid id)
        {
            var user = _userManager.FindByIdAsync(id.ToString()).Result;
            _userManager.DeleteAsync(user).Wait();
        }

        public bool ExistsUser(Guid id)
        {
            return _userManager.FindByIdAsync(id.ToString()).Result != null;
        }

        public bool IsUsernameUnique(string username)
        {
            return !_userManager.Users.Any(u => u.UserName.ToUpper() == username.ToUpper());
        }

        public bool IsEmailUnique(string email)
        {
            return !_userManager.Users.Any(u => u.Email.ToUpper() == email.ToUpper());
        }

        public List<UserModel> GetUsers()
        {
            return _userManager.Users.Select(u => new UserModel
            {
                Id = Guid.Parse(u.Id),
                Username = u.UserName
            }).ToList();
        }

        public UserModel GetUsersById(Guid id)
        {
            var user = _userManager.FindByIdAsync(id.ToString()).Result;
            if (user != null)
            {
                var userRoles = _userManager.GetRolesAsync(user).Result;
                var rolesList = new List<RoleModel>();
                foreach (var role in userRoles)
                {
                    var roleEntity = _roleManager.FindByNameAsync(role).Result;
                    rolesList.Add(new RoleModel
                    {
                        Id = roleEntity.Id,
                        RoleName = roleEntity.Name,
                        RoleType = roleEntity.RoleType
                    });
                }

                return new UserModel
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName,
                    Roles = rolesList
                };
            }
            return null;
        }

        public UserModel Register(RegisterUserModel newUser)
        {
            try
            {
                var rolesList = new List<RoleModel>();
                foreach (var role in newUser.Roles)
                {
                    var roleFindResult = _roleManager.FindByNameAsync(role.RoleName).Result;
                    if (roleFindResult == null)
                    {
                        return null;
                    }
                    else
                    {
                        rolesList.Add(new RoleModel
                        {
                            Id = roleFindResult.Id,
                            RoleName = roleFindResult.Name,
                            RoleType = roleFindResult.RoleType
                        });
                    };
                }

                if (newUser != null)
                {
                    var user = new User
                    {
                        UserName = newUser.Username,
                        Email = newUser.Email
                    };
                    var result = _userManager.CreateAsync(user, newUser.Password).Result;
                    if (result != IdentityResult.Success)
                    {
                        return null;
                    }

                    var registeredUser = _userManager.FindByNameAsync(user.UserName).Result;
                    foreach (var role in newUser.Roles)
                    {
                        var roleAddResult = _userManager.AddToRoleAsync(registeredUser, role.RoleName).Result;

                        if (roleAddResult != IdentityResult.Success)
                        {
                            return null;
                        }
                    }

                    return new UserModel
                    {
                        Id = Guid.Parse(registeredUser.Id),
                        Username = newUser.Username,
                        Roles = rolesList
                    };
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public UserModel SignIn(string username, string password)
        {
            var existingUser = _userManager.FindByNameAsync(username).Result;

            if (existingUser != null)
            {
                var result = _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false).Result;

                if (result.Succeeded)
                {
                    var userRoles = _userManager.GetRolesAsync(existingUser).Result;
                    var rolesList = new List<RoleModel>();
                    foreach (var role in userRoles)
                    {
                        var roleEntity = _roleManager.FindByNameAsync(role).Result;
                        rolesList.Add(new RoleModel
                        {
                            Id = roleEntity.Id,
                            RoleName = roleEntity.Name,
                            RoleType = roleEntity.RoleType
                        });
                    }

                    return new UserModel
                    {
                        Username = username,
                        Id = Guid.Parse(existingUser.Id),
                        Name = username,
                        Roles = rolesList
                    };
                }
            }

            return null;
        }

        public void SignOut()
        {
            _signInManager.SignOutAsync().Wait();
        }

        public bool ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            var result = _userManager.ChangePasswordAsync(user, oldPassword, newPassword).Result;
            return result.Succeeded;
        }
    }
}
