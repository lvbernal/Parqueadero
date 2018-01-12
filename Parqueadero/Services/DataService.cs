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
        private string applicationUrl = "APPLICATION_URL";

        private MobileServiceClient client;
        private IMobileServiceSyncTable<VehicleRecord> vehicleTable;

        public DataService()
        {
            var store = new MobileServiceSQLiteStore("parqueaderostore.db");
            store.DefineTable<VehicleRecord>();

            client = new MobileServiceClient(applicationUrl, new DataServiceHandler());
            client.SyncContext.InitializeAsync(store);
            vehicleTable = client.GetSyncTable<VehicleRecord>();
        }

        public async Task<IEnumerable<VehicleRecord>> GetVehiclesAsync()
        {
            try
            {
                var results = await vehicleTable.Where(v => v.ParkingLotId == Settings.ParkingLotId && !v.Done).OrderBy(v => v.VehicleType).ToEnumerableAsync();
                return results;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<VehicleRecord> GetVehicle(string plate)
        {
            try
            {
                var results = await vehicleTable.Where(v => v.ParkingLotId == Settings.ParkingLotId && !v.Done && v.Plate == plate).ToListAsync();
                return results.Count == 1 ? results[0] : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
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

            await SyncAsync();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await client.SyncContext.PushAsync();
                await vehicleTable.PullAsync("allVehicleRecords", vehicleTable.CreateQuery().Where(v => v.ParkingLotId == Settings.ParkingLotId));
                await vehicleTable.PurgeAsync(vehicleTable.CreateQuery().Where(v => v.Done));
            }
            catch (MobileServicePushFailedException exc)
            {
                syncErrors = exc.PushResult?.Errors;
            }

            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        await error.CancelAndDiscardItemAsync();
                    }

                    Console.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
    }
}
