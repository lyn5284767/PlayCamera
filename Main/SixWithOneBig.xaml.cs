using HBGKTest;
using HBGKTest.YiTongCamera;
using PlayCamera;
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

namespace Main
{
    /// <summary>
    /// SixWithOneBig.xaml 的交互逻辑
    /// </summary>
    public partial class SixWithOneBig : UserControl
    {
        private static SixWithOneBig _instance = null;
        private static readonly object syncRoot = new object();

        public static SixWithOneBig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new SixWithOneBig();
                        }
                    }
                }
                return _instance;
            }
        }
        public SixWithOneBig()
        {
            InitializeComponent();
            this.Loaded += SixWithOneBig_Loaded;
        }

        private void SixWithOneBig_Loaded(object sender, RoutedEventArgs e)
        {
            PlayCameraInThread();
        }
        /// <summary>
        /// 播放摄像头线程
        /// </summary>
        public void PlayCameraInThread()
        {
            Task.Factory.StartNew(() =>
            {
                PlayCamera();
            });
        }
        List<ICameraFactory> camList = new List<ICameraFactory>();
        private delegate void PlayDelegate(Grid gridCamera, ICameraFactory camera);
        /// <summary>
        /// 跨线程调用播放视频
        /// </summary>
        public void PlayCamera()
        {
            camList.Clear();
            //for (int i = 0; i < GlobalInfo.Instance.sixGdList.Count; i++) // 获取播放面板绑定得摄像头
            //{
            //    var data = GlobalInfo.Instance.sixGdList[i].Dispatcher.Invoke(new GetPlayCamList(GetPlayCamera), new object[] { GlobalInfo.Instance.sixGdList[i] });
            //}
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            //    GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.sixGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.sixGdList.ForEach(o => o.Tag = null);
            //}));
            if (camList.Count == 0) // 如果没有半丁摄像头则获取第一组摄像头
            {
                Node node = GlobalInfo.Instance.CamList[0].Nodes.FirstOrDefault();
                if (node != null)
                {
                    int groupID = node.NodeId;
                    camList = GlobalInfo.Instance.CameraList.Where(w => w.Info.CamGroup == groupID).ToList();
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (camList.Count > i)
                {
                    GlobalInfo.Instance.sixWithOneBigGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.sixWithOneBigGdList[i], camList[i] });
                }
                else
                {
                    GlobalInfo.Instance.sixWithOneBigGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.sixWithOneBigGdList[i], null });
                }
            }
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
                        camera.SetSize((gridCamera.Parent as Border).ActualHeight, (gridCamera.Parent as Border).ActualWidth);
                        //camera.FullScreenEvent -= Camera_FullScreenEvent;
                        //camera.SelectCameraEvent -= Camera_SelectCameraEvent;
                        //camera.FullScreenEvent += Camera_FullScreenEvent;
                        //camera.SelectCameraEvent += Camera_SelectCameraEvent;
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
        bool test = true;
        private void bd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.bdOne.Child = null;
            this.bdTwo.Child = null;
            if (test)
            {
                this.bdOne.Child = this.sixgridCamera2;
                this.bdTwo.Child = this.sixgridCamera1;
                (this.sixgridCamera2.Children[0] as ICameraFactory).SetSize((this.sixgridCamera2.Parent as Border).ActualHeight, (this.sixgridCamera2.Parent as Border).ActualWidth);
                (this.sixgridCamera1.Children[0] as ICameraFactory).SetSize((this.sixgridCamera1.Parent as Border).ActualHeight, (this.sixgridCamera1.Parent as Border).ActualWidth);
                test = false;
            }
            else
            {
                this.bdOne.Child = this.sixgridCamera1;
                this.bdTwo.Child = this.sixgridCamera2;
                (this.sixgridCamera2.Children[0] as ICameraFactory).SetSize((this.sixgridCamera2.Parent as Border).ActualHeight, (this.sixgridCamera2.Parent as Border).ActualWidth);
                (this.sixgridCamera1.Children[0] as ICameraFactory).SetSize((this.sixgridCamera1.Parent as Border).ActualHeight, (this.sixgridCamera1.Parent as Border).ActualWidth);
                test = true;
            }
        }
        /// <summary>
        /// 退出全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
