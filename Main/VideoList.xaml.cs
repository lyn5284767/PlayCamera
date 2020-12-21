using PlayCamera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// VideoList.xaml 的交互逻辑
    /// </summary>
    public partial class VideoList : UserControl
    {
        private static VideoList _instance = null;
        private static readonly object syncRoot = new object();

        public static VideoList Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new VideoList();
                        }
                    }
                }
                return _instance;
            }
        }
        public VideoList()
        {
            InitializeComponent();
            this.Loaded += VideoList_Loaded;
        }

        private void VideoList_Loaded(object sender, RoutedEventArgs e)
        {
            //this.beginTime.SelectedDateTime = DateTime.Now.AddDays(-1);
            //this.endTime.SelectedDateTime = DateTime.Now.AddDays(1);
            this.beginTime.SelectedDate = DateTime.Now.AddDays(-1);
            this.endTime.SelectedDate = DateTime.Now.AddDays(1);
        }
        List<DirectoryInfo> dirList = new List<DirectoryInfo>();
        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            if (GlobalInfo.Instance.SelectNode == null || !(GlobalInfo.Instance.SelectNode.Tag is CameraInfo))
            {
                MessageBox.Show("请选择摄像头!");
                return;
            }
            string filePath = System.Environment.CurrentDirectory + "\\video" + "\\video" + (GlobalInfo.Instance.SelectNode.Tag as CameraInfo).CAMERANAME;
            if (!Directory.Exists(filePath))
            {
                MessageBox.Show("未查询到录像");
                return;
            }
            DateTime bTime = this.beginTime.SelectedDate.Value;
            DateTime eTime = this.endTime.SelectedDate.Value;
            if (eTime <= bTime)
            {
                MessageBox.Show("结束时间必须大于开始时间");
                return;
            }
            dirList.Clear();
            ForeachDir(filePath, bTime, eTime);
            List<VideoInfo> list = new List<VideoInfo>();
            foreach (DirectoryInfo dir in dirList)
            {
                string[] files;
                if ((GlobalInfo.Instance.SelectNode.Tag as CameraInfo).CAMERATYPE == 1)
                {
                    files = Directory.GetFiles(dir.FullName, "*.h264");
                    for (int i = 0; i < files.Count(); i++)
                    {
                        VideoInfo info = new VideoInfo();
                        info.ID = i;
                        string time = files[i].Substring(files[i].Length - 19, 14);
                        info.SaveTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        info.ShowTime = info.SaveTime.ToString();
                        info.FileName = files[i].Substring(files[i].Length - 19, 19);
                        info.FullPath = dir.FullName;
                        info.CameraType = 1;
                        list.Add(info);
                    }
                }
                else
                {
                    files = Directory.GetFiles(dir.FullName, "*.avi");

                    for (int i = 0; i < files.Count(); i++)
                    {
                        VideoInfo info = new VideoInfo();
                        info.ID = i;
                        string time = files[i].Substring(files[i].Length - 18, 14);
                        info.SaveTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        info.ShowTime = info.SaveTime.ToString();
                        info.FileName = files[i].Substring(files[i].Length - 18, 18);
                        info.FullPath = dir.FullName;
                        info.CameraType = 0;
                        list.Add(info);
                    }
                }
            }
            list = list.Where(w => w.SaveTime > bTime & w.SaveTime < eTime).OrderBy(o => o.SaveTime).ToList();
            this.lvRecord.ItemsSource = list;
        }

        /// <summary>
        /// 递归获取文件夹
        /// </summary>
        /// <param name="path">总目录</param>
        public void ForeachDir(string path,DateTime bTime ,DateTime eTime)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
            // 录像总目录
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            //遍历摄像头目录
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                DateTime time = new DateTime();
                DateTime.TryParse(NextFolder.Name,out time);
                if (time > bTime && time < eTime)
                {
                    dirList.Add(NextFolder);
                }
                ForeachDir(NextFolder.FullName, bTime, eTime);
            }
        }

        private void lvRecord_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VideoInfo info = new VideoInfo();
            if ((sender as DataGrid).SelectedItem is VideoInfo)
                info = (sender as DataGrid).SelectedItem as VideoInfo;
            if (info != null)
            {
                string file = info.FullPath + "\\" + info.FileName;
                if (info.CameraType == 0)
                {
                    PlayVideoWindow window = new PlayVideoWindow(file);
                    window.ShowDialog();
                }
                else
                {
                    HBGKTest.YiTongCamera.PlayBackForm playBackForm = new HBGKTest.YiTongCamera.PlayBackForm(file);
                    playBackForm.ShowDialog();
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WinAPI.TextBox_Name_GotFocus(null, null);
        }
    }
}
