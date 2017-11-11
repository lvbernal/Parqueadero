using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Collections.ObjectModel;
using Parqueadero.Models;
using Parqueadero.Helpers;

namespace Parqueadero.Services
{
    public class DataService
    {
        private MobileServiceClient client;
        private IMobileServiceSyncTable<VehicleRecord> vehicleTable;

        public MobileServiceClient Client
        {
            get { return client; }
        }

        public DataService()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);

            var store = new MobileServiceSQLiteStore("parqueaderostore.db");
            store.DefineTable<VehicleRecord>();

            client.SyncContext.InitializeAsync(store);
            vehicleTable = client.GetSyncTable<VehicleRecord>();
        }

        public async Task<ObservableCollection<VehicleRecord>> GetVehiclesAsync()
        {
            try
            {
                await SyncAsync();
                IEnumerable<VehicleRecord> vehicles = await vehicleTable.Where(v => v.ParkingLotId == Constants.ParkingLotId && !v.Done).ToEnumerableAsync();
                return new ObservableCollection<VehicleRecord>(vehicles);
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error loading vehicles." + e.Message);
            }

            return null;
        }

        public async Task<VehicleRecord> GetVehicle(string plate)
        {
            try
            {
                await SyncAsync();
                var results = await vehicleTable.Where(v => v.ParkingLotId == Constants.ParkingLotId && !v.Done && v.Plate == plate).ToListAsync();
                return results.Count > 0 ? results[0] : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveVehicle(VehicleRecord vehicle)
        {
            if (vehicle.Id == null)
            {
                await vehicleTable.InsertAsync(vehicle);
            }
            else
            {
                await vehicleTable.UpdateAsync(vehicle);
            }
        }

        public async Task SyncAsync()
        {
            try
            {
                await client.SyncContext.PushAsync();
                await vehicleTable.PullAsync("allVehicleRecords", vehicleTable.CreateQuery());
            }
            catch (MobileServicePushFailedException err)
            {
                foreach (var e in err.PushResult.Errors)
                {
                    Console.WriteLine(e.TableName + " " + e.RawResult);
                }
            }
        }
    }
}
