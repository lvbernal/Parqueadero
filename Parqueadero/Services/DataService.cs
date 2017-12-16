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
        public MobileServiceClient Client
        {
            get { return client; }
        }
  
        private IMobileServiceSyncTable<VehicleRecord> vehicleTable;
		private ParkingLot _parking;


        public DataService(ParkingLot parking)
        {
            _parking = parking;
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
                IEnumerable<VehicleRecord> vehicles = await vehicleTable.Where(v => v.ParkingLotId == _parking.Id && !v.Done).ToEnumerableAsync();
                var vCollection = new ObservableCollection<VehicleRecord>(vehicles);
                return vCollection;
            }
            catch
            {
                return null;
            }
        }

        public async Task<VehicleRecord> GetVehicle(string plate)
        {
            try
            {
                var results = await vehicleTable.Where(v => v.ParkingLotId == _parking.Id && !v.Done && v.Plate == plate).ToListAsync();
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

            await SyncAsync();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                var query = vehicleTable.CreateQuery().Where(v => v.ParkingLotId == _parking.Id && !v.Done);
                await vehicleTable.PullAsync("allVehicleRecords", query);
                await client.SyncContext.PushAsync();
                await vehicleTable.PurgeAsync(vehicleTable.CreateQuery().Where(v => v.Done));
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Other error");
                Console.WriteLine(exc.StackTrace);
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
