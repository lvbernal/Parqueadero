using System;
using System.Threading.Tasks;
using System.Net.Http;
using Parqueadero.Models;
using Parqueadero.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Parqueadero.Services
{
    public class PrintService
    {
        public async Task<bool> Print(VehicleRecord vehicle)
        {
            try
            {
                var printerUrl = Settings.PrinterUrl;

                if (String.IsNullOrWhiteSpace(printerUrl))
                {
                    return true;
                }

                using (HttpClient client = new HttpClient())
                {
                    var resolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
                    var jsonSettings = new JsonSerializerSettings() { ContractResolver = resolver };

                    string content = JsonConvert.SerializeObject(vehicle, jsonSettings);
                    StringContent body = new StringContent(content, Encoding.UTF8, "application/json");

                    client.Timeout = new TimeSpan(0, 0, 10);
                    var result = await client.PostAsync(printerUrl, body);
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
