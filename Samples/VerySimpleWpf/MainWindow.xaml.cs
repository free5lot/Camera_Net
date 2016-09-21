using System;
using System.Windows;
using Camera_NET;

namespace VerySimpleWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Camera choice
            CameraChoice _CameraChoice = new CameraChoice();

            // Get List of devices (cameras)
            _CameraChoice.UpdateDeviceList();

            // To get an example of camera and resolution change look at other code samples 
            if (_CameraChoice.Devices.Count > 0)
            {
                // Device moniker. It's like device id or handle.
                // Run first camera if we have one
                var camera_moniker = _CameraChoice.Devices[0].Mon;

                // Set selected camera to camera control with default resolution
                cameraControl.CameraControl.SetCamera(camera_moniker, null);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Close camera. It's safe to call CloseCamera() even if no camera was set.
            cameraControl.CameraControl.CloseCamera();
        }
    }
}
