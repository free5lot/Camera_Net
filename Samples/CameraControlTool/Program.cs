#region License of Sample

/*
CameraControlSample - is a simple sample of usage of CameraControl
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

#region Using directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace CameraControlTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormCameraControlTool());
        }
    }
}
