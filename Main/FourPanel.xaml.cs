using HBGKTest;
using HBGKTest.YiTongCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayCamera
{
    /// <summary>
    /// FourPanel.xaml 的交互逻辑
    /// </summary>
    public partial class FourPanel : UserControl
    {
        private static FourPanel _instance = null;
        private static readonly object syncRoot = new object();

        public static FourPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new FourPanel();
                        }
                    }
                }
                return _instance;
            }
        }
        public FourPanel()
        {
            InitializeComponent();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            PlayCameraInThread();
        }
        public void PlayCameraInThread()
        {
            Task.Factory.StartNew(() =>
            {
                PlayCamera();
            });
        }

        private delegate void PlayDelegate(Grid gridCamera, ICameraFactory camera);
        /// <summary>
        /// 跨线程调用播放视频
        /// </summary>
        public void PlayCamera()
        {
            this.gridCamera1.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera1,GlobalInfo.Instance.CameraList.Where(w=>w.Info.ID==1).FirstOrDefault()});
            this.gridCamera2.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera1, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 2).FirstOrDefault() });
            this.gridCamera3.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera1, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 3).FirstOrDefault() });
            this.gridCamera4.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera1, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 4).FirstOrDefault() });
        }
        Image cameraInitImage = new Image();
        private void PlayAction(Grid gridCamera, ICameraFactory camera)
        {
            ////SFCameraFullScreen.Instance.gridCamera1.Children.Clear();
            //ICameraFactory cameraOne = GlobalData.Instance.cameraList.Where(w => w.Info.ID == 1).FirstOrDefault();
            camera.StopCamera();
            ChannelInfo info = camera.Info;
            bool isPlay = camera.InitCamera(info);
            //CameraVideoStart1();
            //cameraOne.SetSize(220, 380);
            if (isPlay)
            {
                camera.Info.IsPlay = true;
                gridCamera.Children.Clear();
                if (camera is UIControl_HBGK1)
                {
                    gridCamera.Children.Add(camera as UIControl_HBGK1);
                    //(cameraOne as UIControl_HBGK1).SetValue(Grid.RowProperty, 0);
                    //(cameraOne as UIControl_HBGK1).SetValue(Grid.ColumnProperty, 0);
                }
                else if (camera is YiTongCameraControl)
                {
                    gridCamera.Children.Add(camera as YiTongCameraControl);
                    //(cameraOne as YiTongCameraControl).SetValue(Grid.RowProperty, 0);
                    //(cameraOne as YiTongCameraControl).SetValue(Grid.ColumnProperty, 0);
                }
                else
                {
                    gridCamera.Children.Add(cameraInitImage);
                }
            }
            camera.Info.IsPlay = false;
            camera.FullScreenEvent -= CameraOne_FullScreenEvent;
            camera.FullScreenEvent += CameraOne_FullScreenEvent;
            ////viewboxCameral1.Height = 300;
            ////viewboxCameral1.Width = 403;
        }

        private void Camera_FullScreenEvent(int cameraID)
        {
            throw new NotImplementedException();
        }

        public delegate void FullScreenHandler(int camId);

        public event FullScreenHandler FullScreenEvent;
        private void CameraOne_FullScreenEvent()
        {
            if (FullScreenEvent != null)
            {
                FullScreenEvent(1);
            }
        }

        /// <summary>
        /// 选中
        /// </summary>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var bc = new BrushConverter();
            foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
            {
                bd.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            }

            Border border = sender as Border;
            border.Background = (Brush)bc.ConvertFrom("#F5F6FA");
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
