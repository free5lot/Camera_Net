using System.Windows.Controls;

namespace Camera_NET.Public
{
    /// <summary>Interaction logic for CamControlWpf.xaml</summary>
    public partial class CameraControlWpf : UserControl
    {
        // The WPF support is done by referencing WindowsFormsIntegration to references
        // Creating a WindowsFormsHost user control and setting its child to CameraControl
        // The provided public property CameraControl doesn't handle any error checks

        public CameraControlWpf()
        {
            InitializeComponent();
        }

        /// <summary>Underlying CameraControl</summary>
        public CameraControl CameraControl
        {
            get { return (CameraControl)FormsHost.Child; }
            set { FormsHost.Child = value; }
        }
    }
}