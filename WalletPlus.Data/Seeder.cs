using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data
{
    public static class Seeder
    {
        public static void Seed(DatabaseContext context)
        {
            //PointSettings
            if (!context.PointSettings.Any())
            {
                var pointSettings = new List<PointSetting>
                {
                    new PointSetting
                    {
                        Id = Guid.NewGuid(),
                        LowAmount = 5000,
                        HighAmount = 10000,
                        Point = 1
                    },
                    new PointSetting
                    {
                        Id = Guid.NewGuid(),
                        LowAmount = 10001,
                        HighAmount = 25000,
                        Point = 2.5m
                    },
                    new PointSetting
                    {
                        Id = Guid.NewGuid(),
                        LowAmount = 25001,
                        Point = 5
                    }
                };

                context.PointSettings.AddRange(pointSettings);
                context.SaveChanges();
            }
        }
    }
}
