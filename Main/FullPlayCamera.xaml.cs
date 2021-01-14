using HBGKTest;
using HBGKTest.YiTongCamera;
using Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// FullPlayCamera.xaml 的交互逻辑
    /// </summary>
    public partial class FullPlayCamera : UserControl
    {
        private static FullPlayCamera _instance = null;
        private static readonly object syncRoot = new object();

        public static FullPlayCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new FullPlayCamera();
                        }
                    }
                }
                return _instance;
            }
        }
        public double NowHeight { get; set; }
        public double NowWidth { get; set; }
        public int CamId { get; set; }
        public delegate void CanelFullScreenHandler();

        public event CanelFullScreenHandler CanelFullScreenEvent;
        public FullPlayCamera()
        {
            InitializeComponent();
            this.Loaded += FullPlayCamera_Loaded;
        }

        private void FullPlayCamera_Loaded(object sender, RoutedEventArgs e)
        {
            this.NowHeight = this.ActualHeight;
            this.NowWidth = this.ActualWidth;
            //PlayCamera(this.CamId);
            viewboxCamera2.Visibility = Visibility.Collapsed;
            viewboxCamera3.Visibility = Visibility.Collapsed;
            viewboxCamera4.Visibility = Visibility.Collapsed;
            viewboxCamera5.Visibility = Visibility.Collapsed;
        }

        public void PlayCamera(int camId)
        {
            Image cameraInitImage = new Image();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            try
            {
                GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.sixGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
                ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == camId).FirstOrDefault();
                camera.StopCamera();
                bool isPlay = camera.InitCamera(camera.Info);
                if (isPlay)
                {
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
                    camera.SetSize(this.ActualHeight, this.ActualWidth);
                }
                else
                {
                    gridCamera.Children.Clear();
                    gridCamera.Children.Add(cameraInitImage);
                }

                //ICameraFactory camera2 = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == 28).FirstOrDefault();
                //camera2.StopCamera();
                //bool isPlay2 = camera2.InitCamera(camera2.Info);
                //if (isPlay2)
                //{
                //    gridCamera2.Children.Clear();
                //    if (camera2 is UIControl_HBGK1)
                //    {
                //        gridCamera2.Children.Add(camera2 as UIControl_HBGK1);
                //    }
                //    else if (camera2 is YiTongCameraControl)
                //    {
                //        gridCamera2.Children.Add(camera2 as YiTongCameraControl);
                //    }
                //    else if (camera2 is RTSPControl)
                //    {
                //        this.viewboxCamera2.Visibility = Visibility.Visible;
                //        gridCamera2.Children.Add(camera2 as RTSPControl);
                //    }
                //    //camera2.SetSize(this.ActualHeight, this.ActualWidth);
                //}
                //else
                //{
                //    gridCamera2.Children.Clear();
                //    gridCamera2.Children.Add(cameraInitImage);
                //}
                //    if (camera is UIControl_HBGK1)
                //{
                //    UIControl_HBGK1 cam = new UIControl_HBGK1(camera.Info);
                //    cam.StopCamera();

                //    if (isPlay)
                //    {
                //        gridCamera.Children.Clear();
                //        gridCamera.Children.Add(cam);
                //    }
                //    cam.SetSize(this.ActualHeight, this.ActualWidth);
                //}
                //else if (camera is YiTongCameraControl)
                //{
                //    YiTongCameraControl cam = new YiTongCameraControl(camera.Info);
                //    cam.StopCamera();
                //    if (cam.InitCamera(camera.Info))
                //    {
                //        gridCamera.Children.Clear();
                //        gridCamera.Children.Add(cam);
                //    }
                //    cam.SetSize(this.ActualHeight, this.ActualWidth);
                //}
                //else
                //{
                //    camera.Info.IsPlay = false;
                //    gridCamera.Children.Add(cameraInitImage);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("播放失败，资源被占用");
                gridCamera.Children.Clear();
                gridCamera.Children.Add(cameraInitImage);
            }
        }

        public void PlayCameraFromUdp(ICameraFactory camera)
        {
            Image cameraInitImage = new Image();
            cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
            try
            {
                GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.sixGdList.ForEach(o => o.Children.Clear());
                GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
                camera.StopCamera();
                bool isPlay = camera.InitCamera(camera.Info);
                if (isPlay)
                {
                    gridCamera.Children.Clear();
                    if (camera is UIControl_HBGK1)
                    {
                        gridCamera.Children.Add(camera as UIControl_HBGK1);
                    }
                    else if (camera is YiTongCameraControl)
                    {
                        gridCamera.Children.Add(camera as YiTongCameraControl);
                    }
                    camera.SetSize(this.ActualHeight, this.ActualWidth);
                }
                else
                {
                    gridCamera.Children.Clear();
                    gridCamera.Children.Add(cameraInitImage);
                }
                //    if (camera is UIControl_HBGK1)
                //{
                //    UIControl_HBGK1 cam = new UIControl_HBGK1(camera.Info);
                //    cam.StopCamera();

                //    if (isPlay)
                //    {
                //        gridCamera.Children.Clear();
                //        gridCamera.Children.Add(cam);
                //    }
                //    cam.SetSize(this.ActualHeight, this.ActualWidth);
                //}
                //else if (camera is YiTongCameraControl)
                //{
                //    YiTongCameraControl cam = new YiTongCameraControl(camera.Info);
                //    cam.StopCamera();
                //    if (cam.InitCamera(camera.Info))
                //    {
                //        gridCamera.Children.Clear();
                //        gridCamera.Children.Add(cam);
                //    }
                //    cam.SetSize(this.ActualHeight, this.ActualWidth);
                //}
                //else
                //{
                //    camera.Info.IsPlay = false;
                //    gridCamera.Children.Add(cameraInitImage);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("播放失败，资源被占用");
                gridCamera.Children.Clear();
                gridCamera.Children.Add(cameraInitImage);
            }
        }
        private delegate void PlayDelegate(Grid gridCamera, ICameraFactory camera);
        public void PlayCameraFromUdp(List<ICameraFactory> cameraList)
        {
            if (cameraList.Count == 0)
            {
                MessageBox.Show("未接收到播放摄像头");
                return;
            }
            if (cameraList.Count > 5)
            {
                cameraList = cameraList.Take(5).ToList();
            }
            for (int i = 0; i < cameraList.Count; i++)
            {
                if(i==0) this.gridCamera.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera, cameraList[i] });
                if (i == 1)
                {
                    this.viewboxCamera2.Visibility = Visibility;
                    this.gridCamera2.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera2, cameraList[i] });
                }
                if (i == 2)
                {
                    this.viewboxCamera3.Visibility = Visibility;
                    this.gridCamera3.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera3, cameraList[i] });
                }
                if (i == 3)
                {
                    this.viewboxCamera4.Visibility = Visibility;
                    this.gridCamera4.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera4, cameraList[i] });
                }
                if (i == 4)
                {
                    this.viewboxCamera5.Visibility = Visibility;
                    this.gridCamera5.Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { this.gridCamera5, cameraList[i] });
                }
            }
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
                        gridCamera.Children.Clear();
                        camera.Info.IsPlay = false;
                        gridCamera.Children.Add(cameraInitImage);
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

        private void CancelFullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanelFullScreenEvent != null)
            {
                gridCamera.Children.Clear();
                gridCamera2.Children.Clear();
                gridCamera3.Children.Clear();
                gridCamera4.Children.Clear();
                gridCamera5.Children.Clear();
                CanelFullScreenEvent();
            }
        }
    }
}
