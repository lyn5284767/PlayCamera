using PlayCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main
{
    public delegate void AddCamera(CameraInfo info);
    public delegate void ModifyCamera(CameraInfo info);
    /// <summary>
    /// AddOrModifyCamera.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrModifyCamera : Window
    {
        private int GroupID { get; set; }
        private CameraInfo CameraInfo { get; set; }
        private string NowTitle { get; set; }
        public AddOrModifyCamera(string title)
        {
            this.NowTitle = title;
            InitializeComponent();
        }

        #region 去除标题栏ICON

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        #endregion 去除标题栏ICON

        /// <summary>
        /// 初始化信息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="group">组</param>
        /// <param name="info">摄像头</param>
        public void InitWin(string title, CameraGroup group,CameraInfo info)
        {
            this.Title = title;
            this.GroupID = group.Id;
            if (info != null)
            {
                this.tbCameraIP.Text = info.REMOTEIP;
                this.tbCameraPort.Text = info.REMOTEPORT.ToString();
                this.tbCameraUser.Text = info.REMOTEUSER;
                this.tbCameraPwd.Text = info.REMOTEPWD;
                this.cbCameraType.SelectedIndex = info.CAMERATYPE;
                this.tbCameraName.Text = info.CAMERANAME;
                this.tbPlayPort.Text = info.NPLAYPORT.ToString();
                this.CameraInfo = info;
            }
        }

        public AddCamera AddCameraEvent;
        public ModifyCamera ModifyCameraEvent;
        /// <summary>
        /// 保存/修改摄像头
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbCameraIP.Text == string.Empty || this.tbCameraPort.Text == string.Empty || this.tbCameraUser.Text == string.Empty
               || this.tbCameraName.Text == string.Empty)
            {
                MessageBox.Show("摄像头信息填写不完整，请重新填写");
                return;
            }
            if (this.Title == "添加摄像头")
            {
                CameraInfo info = new CameraInfo();
                info.REMOTEIP = this.tbCameraIP.Text;
                info.REMOTEPORT = int.Parse(this.tbCameraPort.Text);
                info.REMOTEUSER = this.tbCameraUser.Text;
                info.REMOTEPWD = this.tbCameraPwd.Text;
                info.CAMERATYPE= this.cbCameraType.SelectedIndex;
                info.CAMERANAME = this.tbCameraName.Text;
                info.NPLAYPORT = int.Parse(this.tbPlayPort.Text);
                info.CamGroup = this.GroupID;
                if (AddCameraEvent != null)
                {
                    AddCameraEvent(info);
                }
            }
            else
            {
                this.CameraInfo.REMOTEIP = this.tbCameraIP.Text;
                this.CameraInfo.REMOTEPORT = int.Parse(this.tbCameraPort.Text);
                this.CameraInfo.REMOTEUSER = this.tbCameraUser.Text;
                this.CameraInfo.REMOTEPWD = this.tbCameraPwd.Text;
                this.CameraInfo.CAMERATYPE = this.cbCameraType.SelectedIndex;
                this.CameraInfo.CAMERANAME = this.tbCameraName.Text;
                this.CameraInfo.CamGroup = this.GroupID;
                this.CameraInfo.NPLAYPORT = int.Parse(this.tbPlayPort.Text);
                if (ModifyCameraEvent != null)
                {
                    ModifyCameraEvent(this.CameraInfo);
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 选择不同摄像头厂家补全数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.NowTitle == "添加摄像头")
            {
                ComboBox cb = sender as ComboBox;
                if (cb.SelectedIndex == 0)
                {
                    this.tbCameraPort.Text = "554";
                    this.tbCameraUser.Text = "admin";
                    this.tbCameraPwd.Text = "123456";
                    if (GlobalInfo.Instance.CameraList.Count > 0)
                    {
                        this.tbPlayPort.Text = (GlobalInfo.Instance.CameraList.Max(m => m.Info.nPlayPort) + 1).ToString();
                    }
                    else
                    {
                        this.tbPlayPort.Text = "1";
                    }
                }
                else if (cb.SelectedIndex == 1)
                {
                    this.tbCameraPort.Text = "34567";
                    this.tbCameraUser.Text = "admin";
                    this.tbCameraPwd.Text = "";
                    if (GlobalInfo.Instance.CameraList.Count > 0)
                    {
                        this.tbPlayPort.Text = (GlobalInfo.Instance.CameraList.Max(m => m.Info.nPlayPort) + 1).ToString();
                    }
                    else
                    {
                        this.tbPlayPort.Text = "1";
                    }
                }
            }
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            WinAPI.TextBox_Name_GotFocus(null, null);
        }
    }
}
