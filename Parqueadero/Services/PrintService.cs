using System;
using System.Threading.Tasks;
using System.Net.Http;
using Parqueadero.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Parqueadero.Services
{
    public class PrintService
    {
        private string _printerUrl;
        public PrintService(string printerUrl)
        {
            _printerUrl = printerUrl;
        }

        public async Task<bool> PrintCheckIn(VehicleRecord vehicle)
        {
            if (_printerUrl == "dev") { return true; }
            return await Print(vehicle);
        }

        public async Task<bool> PrintCheckOut(VehicleRecord vehicle)
        {
            if (_printerUrl == "dev") { return true; }
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

                    var result = await client.PostAsync(_printerUrl, body);
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
