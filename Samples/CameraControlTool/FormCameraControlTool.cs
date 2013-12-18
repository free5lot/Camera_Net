#region License of Sample

/*
CameraControlTool - is a complicated sample of CameraControl usage
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

namespace CameraControlTool
{
    #region Using directives

    using System;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.InteropServices.ComTypes;

    using Camera_NET;

    using DirectShowLib;

    #endregion

    public partial class FormCameraControlTool : Form
    {
        #region Vars

        // Camera object
        //private Camera _Camera;

        // Rect selection with mouse
        private NormalizedRect _MouseSelectionRect = new NormalizedRect(0, 0, 0, 0);
        private bool _bDrawMouseSelection = false;

        // Zooming
        private bool _bZoomed = false;
        private double _fZoomValue = 1.0;

        // Camera choice
        private CameraChoice _CameraChoice = new CameraChoice();

        #endregion

        #region Winforms stuff

        // Constructor
        public FormCameraControlTool()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        // On load of Form
        private void FormCameraControlTool_Load(object sender, EventArgs e)
        {
            // Fill camera list combobox with available cameras
            FillCameraList();

            // Select the first one
            if (comboBoxCameraList.Items.Count > 0)
            {
                comboBoxCameraList.SelectedIndex = 0;
            }

            // Fill camera list combobox with available resolutions
            FillResolutionList();
        }

        // On close of Form
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Close camera
            cameraControl.CloseCamera();
        }

        // Update buttons of GUI based on Camera state
        private void UpdateGUIButtons()
        {
            buttonCrossbarSettings.Enabled = (cameraControl.CrossbarAvailable);
        }

        #endregion

        #region Camera creation and destroy

        // Set current camera to camera_device
        private void SetCamera(IMoniker camera_moniker, Resolution resolution)
        {
            try
            {
                // NOTE: You can debug with DirectShow logging:
                //cameraControl.DirectShowLogFilepath = @"C:\YOUR\LOG\PATH.txt";

                // Makes all magic with camera and DirectShow graph
                cameraControl.SetCamera(camera_moniker, resolution);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Error while running camera");
            }

            if (!cameraControl.CameraCreated)
                return;

            // If you are using Direct3D surface overlay
            // (see documentation about rebuild of library for it)
            //cameraControl.UseGDI = false;

            cameraControl.MixerEnabled = true;

            cameraControl.OutputVideoSizeChanged += Camera_OutputVideoSizeChanged;

            UpdateCameraBitmap();            


            // gui update
            UpdateGUIButtons();
        }

        #endregion

        #region Functions Buttons

        private void buttonMixerOnOff_Click(object sender, EventArgs e)
        {
            cameraControl.MixerEnabled = !cameraControl.MixerEnabled;
        }
        
        private void buttonSnapshotOutputFrame_Click(object sender, EventArgs e)
        {
            if (!cameraControl.CameraCreated)
                return;

            Bitmap bitmap = cameraControl.SnapshotOutputImage();

            if (bitmap == null)
                return;

            pictureBoxScreenshot.Image = bitmap;
            pictureBoxScreenshot.Update();
        }

        private void buttonSnapshotNextSourceFrame_Click(object sender, EventArgs e)
        {
            if (!cameraControl.CameraCreated)
                return;

            Bitmap bitmap = null;
            try
            {
                bitmap = cameraControl.SnapshotSourceImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error while getting a snapshot");
            }

            if (bitmap == null)
                return;

            pictureBoxScreenshot.Image = bitmap;
            pictureBoxScreenshot.Update();
        }

        private void buttonClearSnapshotFrame_Click(object sender, EventArgs e)
        {
            pictureBoxScreenshot.Image = null;
            pictureBoxScreenshot.Update();
        }

        // Example of crossbar usage
        private void buttonCrossbarSettings_Click(object sender, EventArgs e)
        {
            if (cameraControl.CameraCreated)
            {
                cameraControl.DisplayPropertyPage_Crossbar(this.Handle);
            }
        }

        // Example of TVMode usage
        private void buttonTVMode_Click(object sender, EventArgs e)
        {
            if (cameraControl.CameraCreated)
            {
                MessageBox.Show(cameraControl.GetTVMode().ToString());
            }            
        }

        private void buttonCameraSettings_Click(object sender, EventArgs e)
        {
            if (cameraControl.CameraCreated)
            {
                Camera.DisplayPropertyPage_Device(cameraControl.Moniker, this.Handle);
            }
        }

        #endregion

        #region Camera event handlers

        // Event handler for OutputVideoSizeChanged event
        private void Camera_OutputVideoSizeChanged(object sender, EventArgs e)
        {
            // Update camera's bitmap (new size needed)
            UpdateCameraBitmap();

            // Place Zoom button in correct place on form
            UpdateUnzoomButton();
        }

        #endregion

        #region Overlay bitmaps stuff

        // Generate bitmap for overlay
        private void UpdateCameraBitmap()
        {
            if (!cameraControl.MixerEnabled)
                return;

            cameraControl.OverlayBitmap = GenerateColorKeyBitmap(false);

            #region D3D bitmap mixer
            //if (cameraControl.UseGDI)
            //{
            //    cameraControl.OverlayBitmap = GenerateColorKeyBitmap(false);
            //}
            //else
            //{
            //    cameraControl.OverlayBitmap = GenerateAlphaBitmap();
            //}
            #endregion
        }

        // NOTE: This function is an example of overlay bitmap usage
        // Create bitmap with selection rect, text or any other overlay
        private Bitmap GenerateColorKeyBitmap(bool useAntiAlias)
        {
            int w = cameraControl.OutputVideoSize.Width;
            int h = cameraControl.OutputVideoSize.Height;

            if (w <= 0 || h <= 0)
                return null;

            // Create RGB bitmap
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);

            // configure antialiased drawing or not
            if (useAntiAlias)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
            else
            {
                g.SmoothingMode = SmoothingMode.None;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            }

            // Clear the bitmap with the color key
            g.Clear(cameraControl.GDIColorKey);

            // Draw selection rect --------------------------
            if (_bDrawMouseSelection && IsMouseSelectionRectCorrect())
            {
                Color color_of_pen = Color.Gray;
                if (IsMouseSelectionRectCorrectAndGood())
                {
                    color_of_pen = Color.Green;
                }

                Pen pen = new Pen(color_of_pen, 2.0f);

                Rectangle rect = new Rectangle(
                        (int)(_MouseSelectionRect.left * w),
                        (int)(_MouseSelectionRect.top * h),
                        (int)((_MouseSelectionRect.right - _MouseSelectionRect.left) * w),
                        (int)((_MouseSelectionRect.bottom - _MouseSelectionRect.top) * h)
                    );



                g.DrawLine(pen, rect.Left - 5, rect.Top, rect.Right + 5, rect.Top);
                g.DrawLine(pen, rect.Left - 5, rect.Bottom, rect.Right + 5, rect.Bottom);
                g.DrawLine(pen, rect.Left, rect.Top - 5, rect.Left, rect.Bottom + 5);
                g.DrawLine(pen, rect.Right, rect.Top - 5, rect.Right, rect.Bottom + 5);

                pen.Dispose();

            }

            // Draw zoom text if needed
            if (_bZoomed)
            {
                Font font = new Font("Tahoma", 16);
                Brush textColorKeyed = new SolidBrush(Color.DarkBlue);

                g.DrawString("Zoom: " + Math.Round(_fZoomValue, 1).ToString("0.0") + "x", font, textColorKeyed, 4, 4);

                font.Dispose();
                textColorKeyed.Dispose();
            }

            // Draw text logo for example
            {
                Font font = new Font("Tahoma", 16);
                Brush textColorKeyed = new SolidBrush(Color.Yellow);

                g.DrawString("Sample project for Camera_NET component", font, textColorKeyed, 4, h - 30);

            }


            // dispose Graphics
            g.Dispose();

            // return the bitmap
            return bmp;
        }

        #region D3D bitmap mixer
        //public Bitmap GenerateAlphaBitmap()
        //{
        //    // Alpha values
        //    int alpha50 = (int)(255 * 0.50f); // 50% opacity

        //    // Some drawing tools needed later
        //    Pen blackBorder = new Pen(Color.Black, 2.0f);
        //    Brush red50 = new SolidBrush(Color.FromArgb(alpha50, Color.Red));
        //    Font font = new Font("Tahoma", 16);

        //    int w = _Camera.OutputVideoSize.Width;
        //    int h = _Camera.OutputVideoSize.Height;

        //    // Create a ARGB bitmap
        //    Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        //    Graphics g = Graphics.FromImage(bmp);

        //    // Do antialiased drawings
        //    g.SmoothingMode = SmoothingMode.AntiAlias;
        //    g.TextRenderingHint = TextRenderingHint.AntiAlias;

        //    // Clear the bitmap with complete transparency
        //    g.Clear(Color.Transparent);

        //    // Draw a green circle with black border in the middle
        //    //g.FillEllipse(green, 320 * w / 640, 240 * h / 480, 155 * w / 640, 155 * h / 480);
        //    g.FillEllipse(red50, w / 2 - 70, h / 2 - 70, 140, 140);
        //    g.DrawEllipse(blackBorder, w / 2 - 70, h / 2 - 70, 140, 140);


        //    // Release GDI+ objects
        //    blackBorder.Dispose();
        //    red50.Dispose();
        //    g.Dispose();

        //    // return the bitmap
        //    return bmp;
        //}
        #endregion

        // Swritch GDI/D3D
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    cameraControl.MixerEnabled = false;

        //    cameraControl.UseGDI = !cameraControl.UseGDI;
        //    UpdateCameraBitmap();

        //    if (!cameraControl.MixerEnabled)
        //        cameraControl.MixerEnabled = true;
        //}

        #endregion

        #region Mouse selection stuff

        
        
        private void cameraControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (!cameraControl.CameraCreated)
                return;

            if (_bZoomed)
                return;

            PointF point = cameraControl.ConvertWinToNorm(new PointF(e.X, e.Y));
            _MouseSelectionRect = new NormalizedRect(point.X, point.Y, point.X, point.Y);

            _bDrawMouseSelection = true;
            UpdateCameraBitmap();
        }

        private void cameraControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_bZoomed)
                return;

            if (!_bDrawMouseSelection)
                return;

            // Zoom
            if (!IsMouseSelectionRectCorrectAndGood())
            {
                // Doesn't allow zoom too much

                _bDrawMouseSelection = false;
                UpdateCameraBitmap();
                return;
            }


            int w = cameraControl.Resolution.Width;
            int h = cameraControl.Resolution.Height;

            double rect_w = w * (_MouseSelectionRect.right - _MouseSelectionRect.left);
            double rect_h = h * (_MouseSelectionRect.bottom - _MouseSelectionRect.top);


            // Save frame proportion

            double ratio_video = ((double)w) / h;
            double ratio_rect  = ((double)rect_w) / rect_h;

            //NormalizedRect norm_rect;



            if (ratio_video >= ratio_rect)
            {
                // calculate width
                double needed_rect_width = rect_h * ratio_video;

                _MouseSelectionRect.left -= (float)(((needed_rect_width - rect_w) / 2) / w);
                _MouseSelectionRect.right += (float)(((needed_rect_width - rect_w) / 2) / w);

                _fZoomValue = (double)h / rect_h;
            }
            else
            {
                // calculate height
                double needed_rect_height = rect_w / ratio_video;

                _MouseSelectionRect.top -= (float)(((needed_rect_height - rect_h) / 2) / h);
                _MouseSelectionRect.bottom += (float)(((needed_rect_height - rect_h) / 2) / h);

                _fZoomValue = (double)w / rect_w;
            }



            Rectangle rect = new Rectangle(
                    (int)(_MouseSelectionRect.left * w),
                    (int)(_MouseSelectionRect.top * h),
                    (int)((_MouseSelectionRect.right - _MouseSelectionRect.left) * w),
                    (int)((_MouseSelectionRect.bottom - _MouseSelectionRect.top) * h)
                );


            cameraControl.ZoomToRect(rect);
            _bZoomed = true;
            _bDrawMouseSelection = false;

            UpdateCameraBitmap();

            // Place Zoom button in correct place on form
            UpdateUnzoomButton();

        }

        private void cameraControl_MouseMove(object sender, MouseEventArgs e)
        
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (!cameraControl.CameraCreated)
                return;

            if (_bZoomed)
                return;

            if (!_bDrawMouseSelection)
                return;

            PointF point = cameraControl.ConvertWinToNorm(new PointF(e.X, e.Y));
            _MouseSelectionRect.right = point.X;
            _MouseSelectionRect.bottom = point.Y;


            UpdateCameraBitmap();
        }

        private bool IsMouseSelectionRectCorrect()
        {
            if (Math.Abs(_MouseSelectionRect.right - _MouseSelectionRect.left) < float.Epsilon*10 ||
                Math.Abs(_MouseSelectionRect.bottom - _MouseSelectionRect.top) < float.Epsilon*10)
            {
                return false;
            }

            if (_MouseSelectionRect.left >= _MouseSelectionRect.right ||
                _MouseSelectionRect.top >= _MouseSelectionRect.bottom)
            {
                return false;
            }

            if (_MouseSelectionRect.left < 0 ||
                _MouseSelectionRect.top < 0 ||
                _MouseSelectionRect.right > 1.0 ||
                _MouseSelectionRect.bottom > 1.0)
            {
                return false;
            }
            return true;
        }

        private bool IsMouseSelectionRectCorrectAndGood()
        {
            if (! IsMouseSelectionRectCorrect())
            {
                return false;

            }

            // Zoom
            if (Math.Abs(_MouseSelectionRect.right - _MouseSelectionRect.left) < 0.1f ||
                Math.Abs(_MouseSelectionRect.bottom - _MouseSelectionRect.top) < 0.1f)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Zooming stuff

        // Unzoom on video double-click
        private void cameraControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UnzoomCamera();
        }

        // Unzoom with unzoom button
        private void buttonUnZoom_Click(object sender, EventArgs e)
        {
            UnzoomCamera();
        }


        // Unzoom camera and update gui
        private void UnzoomCamera()
        {
            cameraControl.ZoomToRect(new Rectangle(0, 0, cameraControl.Resolution.Width, cameraControl.Resolution.Height));

            _bZoomed = false;
            _fZoomValue = 1.0;

            // gui updates
            UpdateCameraBitmap();
            UpdateUnzoomButton();

            _bDrawMouseSelection = false;
        }


        // Place Zoom button in correct place on form
        private void UpdateUnzoomButton()
        {
            if (_bZoomed)
            {
                buttonUnZoom.Left = cameraControl.Left + (cameraControl.ClientRectangle.Width - cameraControl.OutputVideoSize.Width) / 2 + 4;
                buttonUnZoom.Top = cameraControl.Top + (cameraControl.ClientRectangle.Height - cameraControl.OutputVideoSize.Height) / 2 + 40;
                buttonUnZoom.Visible = true;
            }
            else
            {
                buttonUnZoom.Visible = false;
            }
        }

        #endregion

        #region Camera and resolution selection

        private void comboBoxCameraList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCameraList.SelectedIndex < 0)
            {
                cameraControl.CloseCamera();
            }
            else
            {
                // Set camera
                SetCamera(_CameraChoice.Devices[ comboBoxCameraList.SelectedIndex ].Mon, null);
            }

            FillResolutionList();
        }

        private void comboBoxResolutionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cameraControl.CameraCreated)
                return;

            int comboBoxResolutionIndex = comboBoxResolutionList.SelectedIndex;
            if (comboBoxResolutionIndex < 0)
            {
                return;
            }
            ResolutionList resolutions = Camera.GetResolutionList(cameraControl.Moniker);

            if ( resolutions == null )
                return; 

            if ( comboBoxResolutionIndex >= resolutions.Count )
                return; // throw

            if (0 == resolutions[comboBoxResolutionIndex].CompareTo(cameraControl.Resolution))
            {
                // this resolution is already selected
                return;
            }

            // Recreate camera
            SetCamera(cameraControl.Moniker, resolutions[comboBoxResolutionIndex]);

        }

        private void FillResolutionList()
        {
            comboBoxResolutionList.Items.Clear();

            if (!cameraControl.CameraCreated)
                return;

            ResolutionList resolutions = Camera.GetResolutionList(cameraControl.Moniker);

            if (resolutions == null)
                return;


            int index_to_select = -1;

            for (int index = 0; index < resolutions.Count; index++)
            {
                comboBoxResolutionList.Items.Add(resolutions[index].ToString());

                if (resolutions[index].CompareTo(cameraControl.Resolution) == 0)
                {
                    index_to_select = index;
                }
            }

            // select current resolution
            if (index_to_select >= 0)
            {
                comboBoxResolutionList.SelectedIndex = index_to_select;
            }
        }

        private void FillCameraList()
        {
            comboBoxCameraList.Items.Clear();

            _CameraChoice.UpdateDeviceList();

            foreach (var camera_device in _CameraChoice.Devices)
            {
                comboBoxCameraList.Items.Add(camera_device.Name);
            }
        }

        #endregion

        private void buttonPinOutputSettings_Click(object sender, EventArgs e)
        {
            if (cameraControl.CameraCreated)
            {
                cameraControl.DisplayPropertyPage_SourcePinOutput(this.Handle);
            }
        }




    }
}
