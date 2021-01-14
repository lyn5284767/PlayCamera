using HBGKTest;
using HBGKTest.YiTongCamera;
using Main;
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

        bool FirstLoad = false;

        private void FourPanel_Loaded(object sender, RoutedEventArgs e)
        {
            FirstLoad = true;
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
        private delegate void GetPlayCamList(Grid gd);
        List<ICameraFactory> camList = new List<ICameraFactory>();
        /// <summary>
        /// 跨线程调用播放视频
        /// </summary>
        public void PlayCamera()
        {
            camList.Clear();
            for (int i = 0; i < GlobalInfo.Instance.nineGdList.Count; i++) // 获取播放面板绑定得摄像头
            {
                var data = GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new GetPlayCamList(GetPlayCamera), new object[] { GlobalInfo.Instance.nineGdList[i] });
            }
            this.Dispatcher.Invoke(new Action(() =>
            {
                GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.sixGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.nineGdList.ForEach(o => o.Tag = null);
            }));
            if (camList.Count == 0) // 如果没有半丁摄像头则获取第一组摄像头
            {
                Node node = GlobalInfo.Instance.CamList[0].Nodes.FirstOrDefault();
                if (node != null)
                {
                    int groupID = node.NodeId;
                    camList = GlobalInfo.Instance.CameraList.Where(w => w.Info.CamGroup == groupID).ToList();
                }
            }
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
        /// <summary>
        /// 获取面板绑定摄像头
        /// </summary>
        /// <param name="gd"></param>
        private void GetPlayCamera(Grid gd)
        {
            if (gd.Tag is ICameraFactory)
            {
                camList.Add(gd.Tag as ICameraFactory);
            }
        }
        /// <summary>
        /// 播放选中摄像头
        /// </summary>
        /// <param name="gridCamera">播放面板</param>
        /// <param name="camera">播放摄像头</param>
        public void PlaySelectCamera(Grid gridCamera, ICameraFactory camera)
        {
            gridCamera.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { gridCamera, camera });
        }
        /// <summary>
        /// 播放摄像头
        /// </summary>
        /// <param name="gridCamera">播放面板</param>
        /// <param name="camera">播放摄像头</param>
        private void PlayAction(Grid gridCamera, ICameraFactory camera)
        {
            Image cameraInitImage = new Image();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            try
            {
                if (gridCamera != null && camera != null)
                {
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
                        else if (camera is RTSPControl)
                        {
                            gridCamera.Children.Add(camera as RTSPControl);
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
                        gridCamera.Children.Clear();
                        gridCamera.Children.Add(cameraInitImage);
                    }
                    if (IsCameraPlayEvent != null)
                    {
                        IsCameraPlayEvent(camera.Info.ID, camera.Info.IsPlay);
                    }
                }
                else
                {
                    gridCamera.Children.Clear();
                    gridCamera.Children.Add(cameraInitImage);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("摄像头已经在其他窗口播放");
                gridCamera.Children.Clear();
                gridCamera.Children.Add(cameraInitImage);
            }
        }
        /// <summary>
        /// 选中摄像头绑定播放列表
        /// </summary>
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
                    gridCamera.Children.Clear();
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
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="cameraID"></param>
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
        /// <summary>
        /// 查询子元素
        /// </summary>
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
        /// 选中播放平面（未绑定摄像头）
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
                if (SelectCameraEvent != null && border.Tag!=null)
                {
                    SelectCameraEvent(int.Parse(border.Tag.ToString()));
                }
            }
            else if (e.ClickCount == 2)
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
        /// <summary>
        /// 动态获取播放面板大小
        /// </summary>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (FirstLoad)
            {
                PlayCameraInThread();
            }
        }
    }
}
