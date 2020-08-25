using HBGKTest;
using HBGKTest.YiTongCamera;
using Main;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public partial class FourPanel : System.Windows.Controls.UserControl,IDisposable
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
            Node node = GlobalInfo.Instance.CamList[0].Nodes.FirstOrDefault();
            if (node != null)
            {
                int groupID = node.NodeId;
                List<ICameraFactory> camList = GlobalInfo.Instance.CameraList.Where(w => w.Info.CamGroup == groupID).ToList();
                for (int i = 0; i < GlobalInfo.Instance.fourGdList.Count; i++)
                {
                    if (camList.Count > i)
                    {
                        GlobalInfo.Instance.fourGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.fourGdList[i], camList[i] });
                    }
                    else
                    {
                        GlobalInfo.Instance.fourGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.fourGdList[i], null });
                    }
                }
            }
            //if (camList.Count >= 1)
            //    this.gridCamera1.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera1, camList[0]});
            //if (camList.Count >= 2)
            //    this.gridCamera2.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera2, camList[1]});
            //if (camList.Count >= 3)
            //    this.gridCamera3.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera3, camList[2]});
            //if (camList.Count >= 4)
            //    this.gridCamera4.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera4, camList[3]});
        }

        public void PlaySelectCamera(Grid gridCamera, ICameraFactory camera)
        {
            gridCamera.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { gridCamera, camera });
        }

        public void ReSizeCamera()
        {
            double height = this.bdOne.ActualHeight;
            double width = this.bdOne.ActualWidth;
        }
        /// <summary>
        /// 摄像头播放
        /// </summary>
        /// <param name="gridCamera">所属区域</param>
        /// <param name="camera">播放摄像头</param>
        private void PlayAction(Grid gridCamera, ICameraFactory camera)
        {
            Image cameraInitImage = new Image();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            try
            {
                if (gridCamera != null && camera != null)
                {
                    GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
                    //if (camera is UIControl_HBGK1)
                    //{
                    //    UIControl_HBGK1 cam = new UIControl_HBGK1(camera.Info);
                    //    cam.StopCamera();
                    //    if (cam.InitCamera(camera.Info))
                    //    {
                    //        camera.Info.IsPlay = true;
                    //        gridCamera.Children.Clear();
                    //        gridCamera.Children.Add(cam);
                    //    }
                    //}
                    //else if (camera is YiTongCameraControl)
                    //{
                    //    gridCamera.Children.Add(camera as YiTongCameraControl);
                    //}
                    camera.StopCamera();
                    bool isPlay = camera.InitCamera(camera.Info);
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
                        camera.SetSize(this.bdOne.ActualHeight, this.bdOne.ActualWidth);
                        camera.FullScreenEvent -= Camera_FullScreenEvent;
                        camera.SelectCameraEvent -= Camera_SelectCameraEvent;
                        camera.FullScreenEvent += Camera_FullScreenEvent;
                        camera.SelectCameraEvent += Camera_SelectCameraEvent;
                        gridCamera.Tag = camera;
                        if (gridCamera.Parent != null && gridCamera.Parent is Viewbox)
                        {
                            Viewbox vb = gridCamera.Parent as Viewbox;
                            if (vb.Parent != null && vb.Parent is Border)
                            {
                                (vb.Parent as Border).Tag = camera;
                            }
                        }
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
                System.Windows.MessageBox.Show("摄像头已经在其他窗口播放");
                gridCamera.Children.Add(cameraInitImage);
            }
            //try
            //{
            //    Image cameraInitImage = new Image();
            //    cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            //    if (gridCamera != null && cameraWithPanel != null)
            //    {
            //        if (gridCamera.Parent is Viewbox)
            //        {
            //            Viewbox vb = gridCamera.Parent as Viewbox;
            //            if (vb.Parent is Border)
            //            {
            //                (vb.Parent as Border).Tag = cameraWithPanel;
            //            }
            //        }
            //        GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
            //        cameraWithPanel.camera.StopCamera();
            //        ChannelInfo info = cameraWithPanel.camera.Info;
            //        bool isPlay = cameraWithPanel.camera.InitCamera(info);
            //        if (isPlay)
            //        {
            //            cameraWithPanel.camera.Info.IsPlay = true;
            //            gridCamera.Children.Clear();
            //            if (cameraWithPanel.camera is UIControl_HBGK1)
            //            {
            //                gridCamera.Children.Add(cameraWithPanel.camera as UIControl_HBGK1);
            //            }
            //            else if (cameraWithPanel.camera is YiTongCameraControl)
            //            {
            //                gridCamera.Children.Add(cameraWithPanel.camera as YiTongCameraControl);
            //            }
            //            else
            //            {
            //                gridCamera.Children.Add(cameraInitImage);
            //            }
            //            cameraWithPanel.camera.SetSize(this.bdOne.ActualHeight, this.bdOne.ActualWidth);
            //            cameraWithPanel.camera.FullScreenEvent -= Camera_FullScreenEvent;
            //            cameraWithPanel.camera.SelectCameraEvent -= Camera_SelectCameraEvent;
            //            cameraWithPanel.camera.FullScreenEvent += Camera_FullScreenEvent;
            //            cameraWithPanel.camera.SelectCameraEvent += Camera_SelectCameraEvent;
            //            cameraWithPanel.PlayGrid = gridCamera;
            //        }
            //        else
            //        {
            //            cameraWithPanel.camera.Info.IsPlay = false;
            //            gridCamera.Children.Add(cameraInitImage);
            //        }
            //        if (IsCameraPlayEvent != null)
            //        {
            //            IsCameraPlayEvent(cameraWithPanel.camera.Info.ID, cameraWithPanel.camera.Info.IsPlay);
            //        }
            //    }
            //    else
            //    {
            //        gridCamera.Children.Add(cameraInitImage);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.StackTrace);
            //}
        }

        private void Play(ICameraFactory camera,ChannelInfo clInfo, Grid gridCamera)
        {
            Image cameraInitImage = new Image();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            camera.StopCamera();
            bool isPlay = camera.InitCamera(clInfo);
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
        /// <summary>
        /// CameraInfo转为ChannelInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private ChannelInfo CameraInfoToChannelInfo(CameraInfo info)
        {
            ChannelInfo ch1 = new ChannelInfo();
            ch1.ID = info.Id;
            ch1.ChlID = info.CHLID.ToString();
            ch1.RemoteChannle = info.REMOTECHANNLE.ToString();
            ch1.RemoteIP = info.REMOTEIP;
            ch1.RemotePort = info.REMOTEPORT;
            ch1.RemoteUser = info.REMOTEUSER;
            ch1.RemotePwd = info.REMOTEPWD;
            ch1.nPlayPort = info.NPLAYPORT;
            ch1.PtzPort = info.PTZPORT;
            ch1.CameraType = info.CAMERATYPE;
            ch1.CameraName = info.CAMERANAME;
            ch1.CamGroup = info.CamGroup;
            return ch1;
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
                System.Windows.MessageBox.Show(ex.StackTrace);
            }
        }
        /// <summary>
        /// 已绑定摄像头播放后事件回调
        /// </summary>
        /// <param name="cameraID"></param>
        private void Camera_SelectCameraEvent(int cameraID)
        {
            var bc = new BrushConverter();
            foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
            {
                if (bd.Tag != null && (bd.Tag as ICameraFactory).Info.ID == cameraID)
                {
                    bd.BorderBrush = (Brush)bc.ConvertFrom("#002DFF");
                    GlobalInfo.Instance.SelectGrid = GetGridInBorder(bd);
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
        /// <summary>
        /// 获取Border中的Grid
        /// </summary>
        private Grid GetGridInBorder(Border border)
        {
            if (border.Child != null && border.Child is Viewbox)
            {
                Viewbox vb = border.Child as Viewbox;
                if (vb.Child != null && vb.Child is Grid)
                {
                    return vb.Child as Grid;
                }
            }
            return null;
        }

        private void Camera_FullScreenEvent(int cameraID)
        {
            if (FullScreenEvent != null)
            {
                FullScreenEvent(cameraID);
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
        /// <summary>
        /// 选择播放面版（未有摄像头播放）
        /// </summary>
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
                GlobalInfo.Instance.SelectGrid = GetGridInBorder(border);
                if (SelectCameraEvent != null)
                {
                    SelectCameraEvent(0);
                }
            }
            else if (e.ClickCount == 2)
            {
                if (FullScreenEvent != null)
                {
                    if (border.Tag == null)
                    {
                        System.Windows.MessageBox.Show("未检测到播放摄像头");
                    }
                    else
                    {
                        FullScreenEvent(int.Parse(border.Tag.ToString()));
                    }
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}
