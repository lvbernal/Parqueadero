using System;
using System.Threading.Tasks;
using System.Net.Http;
using Parqueadero.Models;
using Parqueadero.Helpers;

namespace Parqueadero.Services
{
    public class PrintService
    {
        public async Task<bool> PrintCheckIn(VehicleRecord vehicle)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 10);
            }

            return true;
        }

        public async Task<bool> PrintCheckOut(VehicleRecord vehicle)
        {
            return true;
        }
    }
}
