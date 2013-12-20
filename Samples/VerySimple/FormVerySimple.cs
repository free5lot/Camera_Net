#region License of Sample

/*
VerySimple - is a simple sample of CameraControl usage
Copyright (C) 2013
https://github.com/free5lot/Camera_Net

While the Camera_Net library is covered by LGPL, 
this sample is released as PUBLIC DOMAIN.
So, you can use code from this sample in your 
free or proprietary project without any limitations.

It is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/

#endregion

namespace VerySimple
{
    #region Using directives

    using System;
    using System.Windows.Forms;
    using Camera_NET;

    #endregion

    // FormVerySimple
    public partial class FormVerySimple : Form
    {
        // Constructor
        public FormVerySimple()
        {
            InitializeComponent();
        }

        // On load of Form
        private void FormVerySimple_Load(object sender, EventArgs e)
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
                cameraControl.SetCamera(camera_moniker, null);
            }
        }

        // On close of Form
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Close camera. It's safe to call CloseCamera() even if no camera was set.
            cameraControl.CloseCamera();
        }

    }
}
