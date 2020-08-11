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
    public delegate void AddGroup(CameraGroup group);
    public delegate void ModifyGroup(CameraGroup group);
    /// <summary>
    /// AddOrModifyGroup.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrModifyGroup : Window
    {
        private int GroupID { get; set; }
        public AddOrModifyGroup()
        {
            InitializeComponent();
        }

        public void InitWin(string title, CameraGroup group)
        {
            this.Title = title;
            this.tbGroupName.Text = group.Name;
            this.GroupID = group.Id;
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

        public AddGroup AddGroupEvent;
        public ModifyGroup ModifyGroupEvent;
        /// <summary>
        /// 保存/修改分组
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbGroupName.Text == string.Empty)
            {
                MessageBox.Show("组名不允许为空");
                return;
            }
            if (this.Title == "添加分组")
            {
                if (AddGroupEvent != null)
                {
                    CameraGroup group = new CameraGroup();
                    group.Name = this.tbGroupName.Text;
                    AddGroupEvent(group); 
                }
            }
            else
            {
                if (ModifyGroupEvent != null)
                {
                    CameraGroup group = new CameraGroup();
                    group.Name = this.tbGroupName.Text;
                    group.Id = this.GroupID;
                    ModifyGroupEvent(group);
                }
            }
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


}
