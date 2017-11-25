using System;
using System.Threading.Tasks;
using Parqueadero.Models;

namespace Parqueadero.Services
{
    public class PrintService
    {
        public async Task<bool> PrintCheckIn(VehicleRecord vehicle)
        {
            return true;
        }

        public async Task<bool> PrintCheckOut(VehicleRecord vehicle)
        {
            return true;
        }
    }
}
