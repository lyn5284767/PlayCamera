using HBGKTest;
using HBGKTest.YiTongCamera;
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
                    camera.SetSize(this.ActualHeight, this.ActualWidth);
                }
                else
                {
                   
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
                gridCamera.Children.Add(cameraInitImage);
            }
        }

        private void CancelFullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanelFullScreenEvent != null)
            {
                gridCamera.Children.Clear();
                CanelFullScreenEvent();
            }
        }
    }
}
