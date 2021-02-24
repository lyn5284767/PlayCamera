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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// NewPlayPanel.xaml 的交互逻辑
    /// </summary>
    public partial class NewPlayPanel : System.Windows.Controls.UserControl
    {
        private static NewPlayPanel _instance = null;
        private static readonly object syncRoot = new object();

        public static NewPlayPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new NewPlayPanel();
                        }
                    }
                }
                return _instance;
            }
        }

        public delegate void SelectCameraHandler(int camId);

        public event SelectCameraHandler SelectCameraEvent;

        public NewPlayPanel()
        {
            InitializeComponent();
            this.Loaded += NewPlayPanel_Loaded;
        }

        private void NewPlayPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //SetPlayPanelSize(NowPanel.Four);
        }
        /// <summary>
        /// 设置播放画面大小
        /// </summary>
        public void SetPlayPanelSize(NowPanel nowPanel)
        {
            double realWidth;
            double realHeight;
            this.bdOne.Child = null;
            this.bdTwo.Child = null;
            this.bdThree.Child = null;
            this.bdFour.Child = null;
            this.bdFive.Child = null;
            this.bdSix.Child = null;
            this.bdSeven.Child = null;
            this.bdEight.Child = null;
            this.bdNine.Child = null;
            Grid.SetColumn(this.bdOne, 0);
            Grid.SetRow(this.bdOne, 0);
            Grid.SetColumn(this.bdTwo, 1);
            Grid.SetRow(this.bdTwo, 0);
            Grid.SetColumn(this.bdThree, 2);
            Grid.SetRow(this.bdThree, 0);
            Grid.SetColumn(this.bdFour, 0);
            Grid.SetRow(this.bdFour, 1);
            Grid.SetColumn(this.bdFive, 1);
            Grid.SetRow(this.bdFive, 1);
            Grid.SetColumn(this.bdSix, 2);
            Grid.SetRow(this.bdSix, 1);
            Grid.SetColumn(this.bdSeven, 0);
            Grid.SetRow(this.bdSeven, 2);
            Grid.SetColumn(this.bdEight, 1);
            Grid.SetRow(this.bdEight, 2);
            Grid.SetColumn(this.bdNine, 2);
            Grid.SetRow(this.bdNine, 2);
            Grid.SetColumnSpan(this.bdOne, 1);
            Grid.SetRowSpan(this.bdOne, 1);
            if (nowPanel == NowPanel.Four)
            {
                realWidth = this.ActualWidth / 2;
                realHeight = this.ActualHeight / 2;
                this.colOne.Width = new System.Windows.GridLength(realWidth);
                this.colTwo.Width = new System.Windows.GridLength(realWidth);
                this.rowOne.Height = new System.Windows.GridLength(realHeight);
                this.rowTwo.Height = new System.Windows.GridLength(realHeight);
                this.bdOne.Child = this.viewboxCameral1;
                this.bdTwo.Child = this.viewboxCameral2;
                this.bdFour.Child = this.viewboxCameral3;
                this.bdFive.Child = this.viewboxCameral4;
            }
            else if (nowPanel == NowPanel.Six)
            {
                realWidth = this.ActualWidth / 3;
                realHeight = this.ActualHeight / 3;
                this.colOne.Width = new System.Windows.GridLength(realWidth);
                this.colTwo.Width = new System.Windows.GridLength(realWidth);
                this.colThree.Width = new System.Windows.GridLength(realWidth);
                this.rowOne.Height = new System.Windows.GridLength(realHeight);
                this.rowTwo.Height = new System.Windows.GridLength(realHeight);
                this.rowThree.Height = new System.Windows.GridLength(realHeight);
                Grid.SetColumnSpan(this.bdOne, 2);
                Grid.SetRowSpan(this.bdOne, 2);
                this.bdOne.Child = this.viewboxCameral1;
                Grid.SetColumn(this.bdTwo, 2);
                Grid.SetRow(this.bdTwo, 0);
                this.bdTwo.Child = this.viewboxCameral2;
                Grid.SetColumn(this.bdThree, 2);
                Grid.SetRow(this.bdThree, 1);
                this.bdThree.Child = this.viewboxCameral3;
                Grid.SetColumn(this.bdFour, 2);
                Grid.SetRow(this.bdFour, 2);
                this.bdFour.Child = this.viewboxCameral4;
                Grid.SetColumn(this.bdFive, 1);
                Grid.SetRow(this.bdFive, 2);
                this.bdFive.Child = this.viewboxCameral5;
                Grid.SetColumn(this.bdSix, 0);
                Grid.SetRow(this.bdSix, 2);
                this.bdSix.Child = this.viewboxCameral6;
            }
            else if (nowPanel == NowPanel.Nine)
            {
                realWidth = this.ActualWidth / 3;
                realHeight = this.ActualHeight / 3;
                this.colOne.Width = new System.Windows.GridLength(realWidth);
                this.colTwo.Width = new System.Windows.GridLength(realWidth);
                this.colThree.Width = new System.Windows.GridLength(realWidth);
                this.rowOne.Height = new System.Windows.GridLength(realHeight);
                this.rowTwo.Height = new System.Windows.GridLength(realHeight);
                this.rowThree.Height = new System.Windows.GridLength(realHeight);
                this.bdOne.Child = this.viewboxCameral1;
                this.bdTwo.Child = this.viewboxCameral2;
                this.bdThree.Child = this.viewboxCameral3;
                this.bdFour.Child = this.viewboxCameral4;
                this.bdFive.Child = this.viewboxCameral5;
                this.bdSix.Child = this.viewboxCameral6;
                this.bdSeven.Child = this.viewboxCameral7;
                this.bdEight.Child = this.viewboxCameral8;
                this.bdNine.Child = this.viewboxCameral9;
            }
            else if (nowPanel == NowPanel.One)
            {
                realWidth = this.ActualWidth;
                realHeight = this.ActualHeight;
                this.colOne.Width = new System.Windows.GridLength(realWidth);
                this.rowOne.Height = new System.Windows.GridLength(realHeight);
            }
            PlayCameraInThread();
        }

        /// <summary>
        /// 设置播放画面大小
        /// </summary>
        public void SetPlayPanelSizeByUDP(string camIP)
        {
            foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
            {
                // 找到需要切换的摄像头
                if (bd.Tag != null && (bd.Tag as ICameraFactory).Info.RemoteIP == camIP)
                {
                    if (bd.Child is Viewbox && this.bdOne.Child is Viewbox)
                    {
                        Viewbox viewboxMain = bd.Child as Viewbox;
                        Viewbox viewboxBack = this.bdOne.Child as Viewbox;
                        bd.Child = null;
                        this.bdOne.Child = null;
                        bd.Child = viewboxBack;
                        this.bdOne.Child = viewboxMain;
                    }
                    break;
                }
            }
            PlayCameraInThread();
        }

        /// <summary>
        /// 尺寸改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPlayPanelSize(GlobalInfo.Instance.nowPanel);
        }
        /// <summary>
        /// 单机选择摄像头，双击全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (SelectCameraEvent != null && border.Tag != null)
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
                    //SetPlayPanelSize(NowPanel.One);
                }
            }
        }

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
        /// 播放摄像头线程
        /// </summary>
        public void PlayCameraInThread()
        {
            Task.Factory.StartNew(() =>
            {
                PlayCamera();
            });
        }


        private delegate void PlayDelegate(Grid gridCamera, ICameraFactory camera);
        List<ICameraFactory> camList = new List<ICameraFactory>();
        /// <summary>
        /// 跨线程调用播放视频
        /// </summary>
        public void PlayCamera()
        {
            camList.Clear();
            //for (int i = 0; i < GlobalInfo.Instance.nineGdList.Count; i++) // 获取播放面板绑定得摄像头
            //{
            //    var data = GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new GetPlayCamList(GetPlayCamera), new object[] { GlobalInfo.Instance.nineGdList[i] });
            //}
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            //    GlobalInfo.Instance.fourGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.sixGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.nineGdList.ForEach(o => o.Children.Clear());
            //    GlobalInfo.Instance.nineGdList.ForEach(o => o.Tag = null);
            //}));
            if (camList.Count == 0) // 如果没有绑定摄像头则获取第一组摄像头
            {
                Node node = GlobalInfo.Instance.CamList[0].Nodes.FirstOrDefault();
                if (node != null)
                {
                    int groupID = node.NodeId;
                    camList = GlobalInfo.Instance.CameraList.Where(w => w.Info.CamGroup == groupID).ToList();
                }
            }
            //for (int i = 0; i < GlobalInfo.Instance.nineGdList.Count; i++)
            //{
            //    if (camList.Count > i)
            //    {
            //        GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.nineGdList[i], camList[i] });
            //    }
            //    else
            //    {
            //        GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.nineGdList[i], null });
            //    }
            //}
            int playNum = 0;
            if (GlobalInfo.Instance.nowPanel == NowPanel.One)
            {
                playNum = 1;
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
            {
                playNum = camList.Count < 4 ? camList.Count : 4;
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Six)
            {
                 playNum = camList.Count < 6 ? camList.Count : 6;
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Nine)
            {
                playNum = camList.Count < 9 ? camList.Count : 9;
            }
            for (int i = 0; i < playNum; i++)
            {
                GlobalInfo.Instance.nineGdList[i].Dispatcher.Invoke(new PlayDelegate(PlayAction), new object[] { GlobalInfo.Instance.nineGdList[i], camList[i] });
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
            try
            {
                if (gridCamera != null && camera != null)
                {
                    camera.StopCamera();
                    bool isPlay = camera.InitCamera(camera.Info);
                    if (isPlay)
                    {
                        if (!camera.Info.IsPlay)
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
                            camera.FullScreenEvent += Camera_FullScreenEvent;
                            camera.SelectCameraEvent += Camera_SelectCameraEvent;
                            gridCamera.Tag = camera;
                            SetCameraSize(gridCamera, camera);
                        }
                        else
                        {
                            SetCameraSize(gridCamera, camera);
                        }
                    }
                    else
                    {
                        cameraInitImage.Source = new BitmapImage(new Uri("../Images/camera.jpg", UriKind.Relative));
                        camera.Info.IsPlay = false;
                        gridCamera.Children.Clear();
                        gridCamera.Children.Add(cameraInitImage);
                    }
                    //if (IsCameraPlayEvent != null)
                    //{
                    //    IsCameraPlayEvent(camera.Info.ID, camera.Info.IsPlay);
                    //}
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
        /// 设置摄像头播放尺寸
        /// </summary>
        /// <param name="gridCamera"></param>
        /// <param name="camera"></param>
        private void SetCameraSize(Grid gridCamera, ICameraFactory camera)
        {
            if (gridCamera.Parent != null && gridCamera.Parent is Viewbox)
            {
                Viewbox vb = gridCamera.Parent as Viewbox;
                if (vb.Parent != null && vb.Parent is Border)
                {
                    (vb.Parent as Border).Tag = camera;
                    camera.SetSize((vb.Parent as Border).ActualHeight, (vb.Parent as Border).ActualWidth);
                }
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
        public void Camera_FullScreenEvent(int cameraID)
        {
            if (!GlobalInfo.Instance.FullScreen)
            {
                GlobalInfo.Instance.FullScreen = true;
                SetPlayPanelSize(NowPanel.One);
                foreach (Border bd in FindVisualChildren<Border>(this.gdMain))
                {
                    if (bd.Tag != null && (bd.Tag as ICameraFactory).Info.ID == cameraID)
                    {
                        Grid.SetColumn(bd, 0);
                        Grid.SetRow(bd, 0);

                        bd.Child = GlobalInfo.Instance.SelectGrid.Parent as Viewbox;
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                GlobalInfo.Instance.FullScreen = false;
                SetPlayPanelSize(GlobalInfo.Instance.nowPanel);
            }
        }
    }
}
