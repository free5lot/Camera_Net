Camera_NET
----------
A .NET library for camera easy use via DirectShow
----------

_**NOTE: There is an [article on CodeProject.com about Camera Net library](http://www.codeproject.com/Articles/671407/Camera_Net-Library "Article about Camera_Net Library on CodeProject") and some additional information. Take a look.**_

### Features of the library:
1. Select camera
2. Select resolution
3. Show camera's output
4. Overlay image on the frame
5. Take snapshots of the frame
6. Change TV mode (PAL, NTSC, etc.)
7. Display dialog windows of the camera (from drivers)
8. Get a list of available cameras and resolutions

![Complicated sample of Camera_Net library use. Stranger on television.](https://raw.github.com/free5lot/Camera_Net/master/screenshots/CameraControlTool_sample_screenshot.png "Complicated sample of Camera_Net library use. Stranger on television.")

### About the library
The library was created because I've failed to find a library among dozens of open source solutions that would support what I wanted: easy use of camera as a component, ability to change the camera resolution, take frame snapshots, show overlay images over the frame.

The main classes in the library are CameraControl and Camera.
CameraControl is a UserControl-wrapper for Camera class, easy to use out of box. This approach is recommended.
The library includes two samples of use in WinForms, in particular an example of a simple implementation of the component in your application, and more complicated example that shows almost all features of the library. 
Camera is an object for interaction with cameras.

Additionally there are several public classes: Resolution, VideoInput, camera selection class and etc. 
The library uses DirectShowLib (license LGPL 2.1 or later), which is a very thin wrapper of DirectX COM-interfaces, so that the losses of productivity from the use of managed code instead of the native one are minimal.

DirectShow is MS Windows only, so the library is also available only for Windows.

### License
##### Camera_NET license
This library is free software; you can redistribute it and/or modify it under the terms of the **GNU Lesser General Public License** as published by the Free Software Foundation; either version 3.0 of the License, or (at your option) any later version.
This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
##### Samples license
While the Camera_Net library is covered by LGPL, the samples are released as **PUBLIC DOMAIN**.
So, you can use code from these samples in your  free or proprietary project without any limitations.

![Simple sample of Camera_Net library use.](https://raw.github.com/free5lot/Camera_Net/master/screenshots/Simple_sample_screenshot.png "Simple sample of Camera_Net library use.")

### Links to related open source projects:
- [DirectShow.NET –  CodeProject](http://www.codeproject.com/Articles/2615/DirectShow-NET)
- [DirectX.Capture Class Library –  CodeProject](http://www.codeproject.com/Articles/3566/DirectX-Capture-Class-Library)
- [Aforge.NET –  Computer Vision, Artificial Intelligence, Robotics](http://www.aforgenet.com/)
- [Touchless SDK  –  Codeplex](https://touchless.codeplex.com/releases/view/17986)
- [Emgu CV: OpenCV in .NET](http://www.emgu.com/wiki/index.php/Main_Page)
- [OpenCV | OpenCV](http://opencv.org/)
- [Image Processing Lab in C# –  CodeProject](http://www.codeproject.com/Articles/9727/Image-Processing-Lab-in-C)
- [Versatile WebCam C# library –  CodeProject](http://www.codeproject.com/Articles/125478/Versatile-WebCam-C-library)
- [Camera Vision (video surveillance on C#) –  CodeProject](http://www.codeproject.com/Articles/15537/Camera-Vision-video-surveillance-on-C)
