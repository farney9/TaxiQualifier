using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            var admin = await CheckUserAsync("1010", "Farney", "Jimenez", "farney9@gmail.com", "315 258 4641", "Calle Luna Calle Sol", UserType.Admin);
            var driver = await CheckUserAsync("2020", "Farney", "Jimenez", "farney9@outlook.com", "315 258 4641", "Carrera 58 # 57 - 06 Piso 4", UserType.Driver);
            var user1 = await CheckUserAsync("3030", "Farney", "Jimenez", "fcybercafe@gmail.com", "315 258 4641", "Calle 96B sur # 55-102", UserType.User);
            var user2 = await CheckUserAsync("4040", "Farney", "Jimenez ITM", "farneyjimenez178518@correo.itm.edu.co", "350 634 2747", "Carrera 20 # 2 Sur 240", UserType.User);
            await CheckTaxisAsync(driver, user1, user2);
        }

        private async Task<UserEntity> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTaxisAsync(
            UserEntity driver,
            UserEntity user1,
            UserEntity user2)
        {
            if (!_dataContext.Taxis.Any())
            {
                _dataContext.Taxis.Add(new TaxiEntity
                {
                    User = driver,
                    Plaque = "TPQ123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user1
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user1
                        }
                    }
                });

                _dataContext.Taxis.Add(new TaxiEntity
                {
                    Plaque = "THW321",
                    User = driver,
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Auctor urna nunc id cursus metus. Imperdiet massa tincidunt nunc pulvinar. Facilisi cras fermentum odio eu. Aliquam vestibulum morbi blandit cursus risus at ultrices mi tempus. Netus et malesuada fames ac turpis egestas. Erat imperdiet sed euismod nisi. Id porta nibh venenatis cras sed felis eget velit. Eu volutpat odio facilisis mauris sit amet massa. Feugiat scelerisque varius morbi enim nunc. Magna etiam tempor orci eu lobortis elementum nibh tellus molestie. Senectus et netus et malesuada fames ac turpis egestas. Pulvinar pellentesque habitant morbi tristique senectus et netus. Ut ornare lectus sit amet. Arcu felis bibendum ut tristique et egestas quis ipsum suspendisse. Dictum varius duis at consectetur lorem donec. Cursus sit amet dictum sit amet justo donec. Facilisi etiam dignissim diam quis enim.",
                            User = user2
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Ut morbi tincidunt augue interdum velit. Diam ut venenatis tellus in metus vulputate eu scelerisque. Nunc vel risus commodo viverra maecenas accumsan. Quam pellentesque nec nam aliquam sem. Semper feugiat nibh sed pulvinar proin.",
                            User = user2
                        }
                    }
                });

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
