using System;
using System.Threading.Tasks;
using System.Net.Http;
using Parqueadero.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Parqueadero.Helpers;

namespace Parqueadero.Services
{
    public class PrintService
    {
        public async Task<bool> PrintCheckIn(VehicleRecord vehicle)
        {
            return await Print(vehicle);
        }

        public async Task<bool> PrintCheckOut(VehicleRecord vehicle)
        {
            return await Print(vehicle);
        }

        private async Task<bool> Print(VehicleRecord vehicle)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 10);
                    var resolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
                    var jsonSettings = new JsonSerializerSettings() { ContractResolver = resolver };
                    string content = JsonConvert.SerializeObject(vehicle, jsonSettings);
                    StringContent body = new StringContent(content, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(Constants.PrinterAddress, body);
                    var data = await result.Content.ReadAsStringAsync();

                    return result.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
