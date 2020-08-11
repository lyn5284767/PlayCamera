using HBGKTest;
using HBGKTest.YiTongCamera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer cameraSaveThreadTimer = new System.Timers.Timer();
        BrushConverter bc = new BrushConverter();
        public MainWindow()
        {
            InitializeComponent();
            InitCameraInfo();
            InitCameraSaveTimeThread();
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(FourPanel.Instance);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitCameraInfo()
        {
            string sql = "Select * from CameraInfo";
            GlobalInfo.Instance.ProtocolList = SQLiteFac.Instance.ExecuteList<CameraInfo>(sql);
            foreach (CameraInfo info in GlobalInfo.Instance.ProtocolList)
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
                GlobalInfo.Instance.ChList.Add(ch1);
            }
            foreach (ChannelInfo info in GlobalInfo.Instance.ChList)
            {
                switch (info.CameraType)
                {
                    case 0:
                        {
                            GlobalInfo.Instance.CameraList.Add(new UIControl_HBGK1(info));
                            break;
                        }
                    case 1:
                        {
                            GlobalInfo.Instance.CameraList.Add(new YiTongCameraControl(info));
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// 初始化摄像头录像线程
        /// </summary>
        private void InitCameraSaveTimeThread()
        {
            cameraSaveThreadTimer.Interval = 1;
            cameraSaveThreadTimer.Start();
            cameraSaveThreadTimer.Elapsed += CameraSaveThreadTimer_Elapsed;
        }

        private void CameraSaveThreadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (cameraSaveThreadTimer.Interval == 1) cameraSaveThreadTimer.Interval = 60 * 1000;
            try
            {
                foreach (ICameraFactory camera in GlobalInfo.Instance.CameraList)
                {
                    string filePath = System.Environment.CurrentDirectory + "\\video" + "\\video" + camera.Info.ID;
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi";
                    camera.StopFile();
                    camera.SaveFile(filePath, fileName);

                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 删除最老的视频文件
        /// </summary>
        /// <param name="path"></param>
        private void DeleteOldFileName(string path)
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            string[] disk = path.Split('\\');
            // 硬盘空间小于1G，开始清理录像
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == disk[0] + "\\" && drive.TotalFreeSpace / (1024 * 1024) < 1024)
                {
                    DirectoryInfo root = new DirectoryInfo(path);
                    var file = root.GetFiles().OrderBy(s => s.CreationTime).FirstOrDefault();
                    file.Delete();
                }
            }
        }
        /// <summary>
        /// 4分屏
        /// </summary>
        private void FourImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(FourPanel.Instance);
            this.gdFour.Background = (Brush)bc.ConvertFrom("#A6CFDC");
            this.gdNine.Background = (Brush)bc.ConvertFrom("#FFFFFF");
        }
        /// <summary>
        /// 9分屏
        /// </summary>
        private void NineImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(NinePanel.Instance);
            this.gdFour.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            this.gdNine.Background = (Brush)bc.ConvertFrom("#A6CFDC");
        }
        /// <summary>
        /// 退出
        /// </summary>
        private void ExitImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("确认退出？", "提示", MessageBoxButton.OKCancel);
            this.Close();
        }

        private void FullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FullPlayCamera.Instance.CanelFullScreenEvent += Instance_CanelFullScreenEvent;
            this.gdMain.Visibility = Visibility.Collapsed;
            this.gdAll.Children.Add(FullPlayCamera.Instance);
        }

        private void Instance_CanelFullScreenEvent()
        {
            FullPlayCamera.Instance.CanelFullScreenEvent -= Instance_CanelFullScreenEvent;
            this.gdMain.Visibility = Visibility.Visible;
            this.gdAll.Children.Remove(FullPlayCamera.Instance);
        }
    }
}
