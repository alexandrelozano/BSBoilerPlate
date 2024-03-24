using BSBoilerPlate.Models;
using BSBoilerPlate.Services;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Diagnostics;

namespace BSBoilerPlate.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BSBoilerPlateContext context)
        {
            lock (context)
            {
                context.Database.Migrate();
            }

            foreach (string logActionName in Enum.GetNames(typeof(LogAction.Names)))
            {
                if (!context.LogActions.Where(p => p.Name == logActionName).Any())
                {
                    LogAction logAction = new LogAction { Name = logActionName };
                    context.LogActions.Add(logAction);
                }
            }
            context.SaveChanges();

            if (!context.Applications.Any())
            {
                var application = new Application();
                application.Name = "BSApplication";
                application.CompanyName = "Contoso Ltd.";

                SKBitmap logoIcon = new SKBitmap(64, 64);
                SKCanvas canvas = new SKCanvas(logoIcon);

                canvas.Clear(SKColors.White);
                using (var paint = new SKPaint())
                {
                    paint.TextSize = 42f;
                    paint.IsAntialias = true;
                    paint.Color = new SKColor(0x42, 0x81, 0xA4);
                    paint.IsStroke = false;

                    canvas.DrawText(application.Name.Substring(0,2), logoIcon.Width / 14f, logoIcon.Height - (logoIcon.Height / 4f), paint);
                }

                SKData d = SKImage.FromBitmap(logoIcon).Encode(SKEncodedImageFormat.Png, 100);
                application.logoIcon = d.ToArray();


                SKBitmap logo = new SKBitmap(256, 256);
                canvas = new SKCanvas(logo);

                canvas.Clear(SKColors.White);
                using (var paint = new SKPaint())
                {
                    paint.TextSize = 200f;
                    paint.IsAntialias = true;
                    paint.Color = new SKColor(0x42, 0x81, 0xA4);
                    paint.IsStroke = false;

                    canvas.DrawText(application.Name.Substring(0, 2), logo.Width / 14f, logo.Height - (logo.Height / 4f), paint);
                }

                d = SKImage.FromBitmap(logo).Encode(SKEncodedImageFormat.Png, 100);
                application.logo = d.ToArray();

                context.Applications.Add(application);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {

                var roles = new Role[]
                {
                    new Role{Name="Administrator"},
                    new Role{Name="Administrative"},
                    new Role{Name="Nurse"},
                    new Role{Name="Doctor"},
                };
                foreach (Role r in roles)
                {
                    context.Roles.Add(r);
                }
                context.SaveChanges();

                string defaultPassword = AppService.CreateMD5("1234");

                var users = new User[]
                {
                new User{UserName="jsmith", Password=defaultPassword, GivenName="John", FirstSurname="Smith", SecondSurname="Doe", DOB=DateTime.Parse("1965-09-01"), RoleID=(from r in roles where r.Name=="Administrator" select r.ID).First<int>()},
                new User{UserName="msimone", Password=defaultPassword, GivenName="Maria", FirstSurname="Simone", SecondSurname="", DOB=DateTime.Parse("1978-12-23"), RoleID=(from r in roles where r.Name=="Administrative" select r.ID).First<int>()},
                new User{UserName="lsimpson", Password=defaultPassword, GivenName="Lisa", FirstSurname="Simpson", SecondSurname="McCallahan", DOB=DateTime.Parse("2000-09-11"), RoleID=(from r in roles where r.Name=="Nurse" select r.ID).First<int>()},
                new User{UserName="dcallahan", Password=defaultPassword, GivenName="Daniel", FirstSurname="Callahan", SecondSurname="", DOB=DateTime.Parse("1998-02-05"), RoleID=(from r in roles where r.Name=="Doctor" select r.ID).First<int>()},
                };
                foreach (var user in users)
                {
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }
        }
    }
}