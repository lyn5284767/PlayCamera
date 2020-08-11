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
    public partial class NinePanel : UserControl
    {
        private static NinePanel _instance = null;
        private static readonly object syncRoot = new object();

        public static NinePanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new NinePanel();
                        }
                    }
                }
                return _instance;
            }
        }
        public NinePanel()
        {
            InitializeComponent();
            this.Loaded += FourPanel_Loaded;
        }

        private void FourPanel_Loaded(object sender, RoutedEventArgs e)
        {
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
            int groupID = GlobalInfo.Instance.CamList[0].Nodes.FirstOrDefault().NodeId;
            List<ICameraFactory> camList = GlobalInfo.Instance.CameraList.Where(w => w.Info.CamGroup == groupID).ToList();
            for (int i = 0; i < GlobalInfo.Instance.nineGdList.Count; i++)
            {
                if (camList.Count > i)
                {
                    GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.nineGdList[i], camList[i] });
                }
                else
                {
                    GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.nineGdList[i], null });
                }
            }
            //this.ninegridCamera1.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera1, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 1).FirstOrDefault() });
            //this.ninegridCamera2.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera2, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 2).FirstOrDefault() });
            //this.ninegridCamera3.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera3, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 3).FirstOrDefault() });
            //this.ninegridCamera4.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera4, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 4).FirstOrDefault() });
            //this.ninegridCamera5.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera5, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 5).FirstOrDefault() });
            //this.ninegridCamera6.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera6, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 6).FirstOrDefault() });
            //this.ninegridCamera7.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera7, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 7).FirstOrDefault() });
            //this.ninegridCamera8.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera8, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 8).FirstOrDefault() });
            //this.ninegridCamera9.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.ninegridCamera9, GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 9).FirstOrDefault() });
        }

        public void PlaySelectCamera(Grid gridCamera, ICameraFactory camera)
        {
            gridCamera.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { gridCamera, camera });
        }
        private void PlayAction(Grid gridCamera, ICameraFactory camera)
        {
            try
            {
                Image cameraInitImage = new Image();
                cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
                if (gridCamera != null && camera != null)
                {
                    GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
                    camera.StopCamera();
                    ChannelInfo info = camera.Info;
                    bool isPlay = camera.InitCamera(info);
                    if (isPlay)
                    {
                        camera.Info.IsPlay = true;
                        gridCamera.Children.Clear();
                        if (camera is UIControl_HBGK1)
                        {
                            gridCamera.Children.Add(camera as UIControl_HBGK1);
                        }
                        else if (camera is YiTongCameraControl)
                        {
                            gridCamera.Children.Add(camera as YiTongCameraControl);
                        }
                        else
                        {
                            gridCamera.Children.Add(cameraInitImage);
                        }
                        camera.SetSize(this.bdOne.ActualHeight, this.bdOne.ActualWidth);
                        camera.FullScreenEvent -= Camera_FullScreenEvent;
                        camera.SelectCameraEvent -= Camera_SelectCameraEvent;
                        camera.FullScreenEvent += Camera_FullScreenEvent;
                        camera.SelectCameraEvent += Camera_SelectCameraEvent;
                    }
                    else
                    {
                        camera.Info.IsPlay = false;
                        gridCamera.Children.Add(cameraInitImage);
                    }
                    if (IsCameraPlayEvent != null)
                    {
                        IsCameraPlayEvent(camera.Info.ID, camera.Info.IsPlay);
                    }
                }
                else
                {
                    gridCamera.Children.Add(cameraInitImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void Camera_SelectCameraEvent(int cameraID)
        {
            var bc = new BrushConverter();
            foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
            {
                if (bd.Tag != null && bd.Tag.ToString() == cameraID.ToString())
                {
                    bd.BorderBrush = (Brush)bc.ConvertFrom("#002DFF");
                }
                else
                {
                    bd.BorderBrush = (Brush)bc.ConvertFrom("#686868");
                }
            }
            if (SelectCameraEvent != null)
            {
                SelectCameraEvent(cameraID);
            }
        }

        private void Camera_FullScreenEvent(int cameraID)
        {
            if (FullScreenEvent != null)
            {
                FullScreenEvent(cameraID);
            }
        }

        /// <summary>
        /// 停止播放摄像头
        /// </summary>
        /// <param name="gridCamera">摄像头区域</param>
        /// <param name="camera">停止的摄像头</param>
        public void StopCamera(Grid gridCamera, ICameraFactory camera)
        {
            try
            {
                Image cameraInitImage = new Image();
                cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
                if (gridCamera != null && camera != null)
                {
                    camera.StopCamera();
                    gridCamera.Children.Clear();
                    gridCamera.Children.Add(cameraInitImage);
                    camera.Info.IsPlay = false;
                }
                else
                {
                    gridCamera.Children.Add(cameraInitImage);
                }
                if (IsCameraPlayEvent != null)
                {
                    IsCameraPlayEvent(camera.Info.ID, camera.Info.IsPlay);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        public delegate void FullScreenHandler(int camId);

        public event FullScreenHandler FullScreenEvent;

        public delegate void SelectCameraHandler(int camId);

        public event SelectCameraHandler SelectCameraEvent;

        public delegate void IsCameraPlayHandler(int camId, bool isPlay);

        public event IsCameraPlayHandler IsCameraPlayEvent;

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

        private void bd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (e.ClickCount == 1)
            {
                var bc = new BrushConverter();
                foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
                {
                    bd.BorderBrush = (Brush)bc.ConvertFrom("#686868");
                }
                border.BorderBrush = (Brush)bc.ConvertFrom("#002DFF");
                if (SelectCameraEvent != null)
                {
                    SelectCameraEvent(int.Parse(border.Tag.ToString()));
                }
            }
            else if (e.ClickCount == 2)
            {
                if (FullScreenEvent != null)
                {
                    FullScreenEvent(int.Parse(border.Tag.ToString()));
                }
            }
        }
    }
}
