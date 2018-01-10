using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Parqueadero.ViewModels;
using System.Threading.Tasks;

namespace Parqueadero.Pages
{
    public partial class CheckOutPage : ContentPage
    {
        private ZXingScannerView scanner;
        private ZXingDefaultOverlay overlay;
        private CheckOutViewModel context;

        public CheckOutPage()
        {
            InitializeComponent();
            context = ((MainViewModel)BindingContext).CheckOut;
            InitializeScannerPage();
            InitializeNoReceiptOption();
        }

        private void InitializeScannerPage()
        {
            scanner = new ZXingScannerView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            scanner.OnScanResult += (result) => Device.BeginInvokeOnMainThread(
                async () => { await ValidateResult(result.Text); }
            );

            overlay = new ZXingDefaultOverlay()
            {
                ShowFlashButton = scanner.HasTorch
            };

            overlay.FlashButtonClicked += (sender, e) =>
            {
                scanner.IsTorchOn = !scanner.IsTorchOn;
            };

            scannerGrid.Children.Add(scanner, 0, 0);
            scannerGrid.Children.Add(overlay, 0, 0);
        }

        private void InitializeNoReceiptOption()
        {
            NoReceiptButton.Clicked += (s, e) => Device.BeginInvokeOnMainThread(
                async () => { await ValidateResult(NoReceiptPlate.Text); }
            );
        }

        private async Task ValidateResult(string plate)
        {
            if (scanner.IsAnalyzing)
            {
                scanner.IsAnalyzing = false;
                NoReceiptButton.IsEnabled = false;

                var valid = await context.LoadVehicle(plate);

                if (valid)
                {
                    scanner.IsScanning = false;
                }
                else
                {
                    scanner.IsAnalyzing = true;
                    NoReceiptButton.IsEnabled = true;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (context.DoScan)
            {
                scanner.IsScanning = true;
            }
        }

        protected override void OnDisappearing()
        {
            if (context.DoScan)
            {
                scanner.IsScanning = false;
            }

            base.OnDisappearing();
        }
    }
}
