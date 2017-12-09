using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Parqueadero.ViewModels;

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
                async () =>
                {
                    scanner.IsAnalyzing = false;

                    var valid = await context.LoadVehicle(result.Text);

                    if (valid)
                    {
                        scanner.IsScanning = false;
                    }
                    else
                    {
                        scanner.IsAnalyzing = true;
                    }
                }
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
                async () =>
                {
                    scanner.IsAnalyzing = false;

                    var valid = await context.LoadVehicle(NoReceiptPlate.Text);

                    if (valid)
                    {
                        scanner.IsScanning = false;
                    }
                    else
                    {
                        scanner.IsAnalyzing = true;
                    }
                }
            );
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
