using HBGKTest;
using HBGKTest.YiTongCamera;
using Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer cameraSaveThreadTimer = new System.Timers.Timer();// 摄像头存储定时器
        BrushConverter bc = new BrushConverter();// 全局画刷，标记选中颜色
        //ViewModel viewModel = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.gdAll.Children.Add(FullPlayCamera.Instance);// 首先添加一次用于获得全屏尺寸
            GlobalInfo.Instance.nowPanel = NowPanel.Four;
            string sql = "Select * from GloConfig";
            GlobalInfo.Instance.GloConfig = SQLiteFac.Instance.ExecuteList<GloConfig>(sql).FirstOrDefault();
            if (GlobalInfo.Instance.GloConfig.Open == 1)
            {
                IConnect con = new UDPConnect(GlobalInfo.Instance.GloConfig.LocalIP, GlobalInfo.Instance.GloConfig.LocalPort, GlobalInfo.Instance.GloConfig.RemoteIP, GlobalInfo.Instance.GloConfig.RemotePort);
                con.GetPlayCameraEvent += Con_GetPlayCameraEvent;
                con.OpenConnect();
            }
            this.Loaded += MainWindow_Loaded;
        }
        /// <summary>
        /// 根据UDP传输数据得播放摄像头回调
        /// </summary>
        /// <param name="camIP">摄像头IP</param>
        private void Con_GetPlayCameraEvent(string camIP)
        {
            try
            {
                Action<String> playFullScreenAct = new Action<string>(PlayFullScreen);
                this.Dispatcher.BeginInvoke(playFullScreenAct, camIP);
            }
            catch (Exception ex)
            {
                
            }
        }
        /// <summary>
        /// 全屏播放
        /// </summary>
        /// <param name="camIP">摄像头IP</param>
        private void PlayFullScreen(string camIP)
        {
            this.gdMain.Visibility = Visibility.Collapsed;

            if (this.gdAll.Children.Contains(FullPlayCamera.Instance))
            {
                this.gdAll.Children.Remove(FullPlayCamera.Instance);
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            else
            {
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            FullPlayCamera.Instance.CanelFullScreenEvent -= Instance_CanelFullScreenEvent;
            FullPlayCamera.Instance.CanelFullScreenEvent += Instance_CanelFullScreenEvent;
            ICameraFactory cam = GlobalInfo.Instance.CameraList.Where(w => w.Info.RemoteIP == camIP).FirstOrDefault();
            if (cam != null)
            {
                FullPlayCamera.Instance.PlayCameraFromUdp(cam);
            }
            else
            {
                System.Windows.MessageBox.Show("未配置" + camIP + "摄像头");
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitCameraTree();
            InitCameraSaveTimeThread();
            InitPlayPanel();
            this.gdAll.Children.Remove(FullPlayCamera.Instance);

        }
        //object _async = new object();
        //public bool Connect()
        //{
        //    lock (_async)
        //    {
        //        if (con.IsClosed)
        //        {
        //            if (con.OpenConnect())
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        }
        //    }
        //    return false;
        //}
        /// <summary>
        /// Step.1:初始化摄像头树列表
        /// </summary>
        private void InitCameraTree()
        {
            this.tvCamera.Height = this.Height;
            string sql = "Select * from CameraGroup";
            GlobalInfo.Instance.GroupList = SQLiteFac.Instance.ExecuteList<CameraGroup>(sql);
            sql = "Select * from CameraInfo";
            GlobalInfo.Instance.CameraInfoList = SQLiteFac.Instance.ExecuteList<CameraInfo>(sql);
            Node root = new Node();
            root.NodeId = 0;
            root.NodeName = "视频监控";
            GlobalInfo.Instance.CamList.Add(root);
            foreach (CameraGroup group in GlobalInfo.Instance.GroupList)
            {
                Node node = AddGroup(group, root);
                List<CameraInfo> CameraList = GlobalInfo.Instance.CameraInfoList.Where(w => w.CamGroup == group.Id).ToList();
                foreach (CameraInfo info in CameraList)
                {
                    Node camnode = new Node();
                    camnode.NodeId = info.Id;
                    camnode.NodeName = info.CAMERANAME;
                    camnode.NodeImg = "/Images/Play.png";
                    camnode.Tag = info;
                    camnode.Parent = node;
                    node.Nodes.Add(camnode);
                    // 添加全局摄像头
                    ChannelInfo clInfo = CameraInfoToChannelInfo(info);
                    switch (clInfo.CameraType)
                    {
                        case 0:
                            {
                                GlobalInfo.Instance.CameraList.Add(new UIControl_HBGK1(clInfo));
                                break;
                            }
                        case 1:
                            {
                                GlobalInfo.Instance.CameraList.Add(new YiTongCameraControl(clInfo));
                                break;
                            }
                    }
                }
            }
            //viewModel.TreeNodes = GlobalInfo.Instance.CamList;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //ExpandTree(GlobalInfo.Instance.CamList.FirstOrDefault());
            //SelectTheCurrentNode(8);
        }
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="group">摄像头组</param>
        private Node AddGroup(CameraGroup group,Node root)
        {
            Node node = new Node();
            node.NodeId = group.Id;
            node.NodeName = group.Name;
            node.Tag = group;
            //node.NodeImg = "/Images/Play.png";
            root.Nodes.Add(node);
            return node;
        }
        /// <summary>
        /// 树列表选择变换事件
        /// </summary>
        private void tvCamera_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Node node = this.tvCamera.SelectedItem as Node;
            if (node != null)
            {
                GlobalInfo.Instance.SelectNode = node;
            }
            if (GlobalInfo.Instance.SelectNode.Tag is CameraGroup)
            {
                ExpandTree(GlobalInfo.Instance.SelectNode);
            }
        }
        /// <summary>
        /// 展开树
        /// </summary>
        /// <param name="node"></param>
        private void ExpandTree(Node node)
        {
            DependencyObject dependencyObject = this.tvCamera.ItemContainerGenerator.ContainerFromItem(node);
            if (dependencyObject != null)//第一次打开程序，dependencyObject为null，会出错
            {
                ((TreeViewItem)dependencyObject).ExpandSubtree();
            }
        }

        /// <summary>
        /// Step 1.初始化摄像头组（弃用）
        /// </summary>
        private void InitCameraGroup()
        {
            string sql = "Select * from CameraGroup";
            GlobalInfo.Instance.GroupList = SQLiteFac.Instance.ExecuteList<CameraGroup>(sql);
            bool isFirst = true;
            foreach (CameraGroup group in GlobalInfo.Instance.GroupList)
            {
                Expander exp = InitExpander(group);
                spCameraList.Children.Add(exp);
                exp.Content = InitListBox(group);
                if (isFirst)
                {
                    isFirst = false;
                    exp.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Step 2.初始化摄像头信息（弃用）
        /// </summary>
        private void InitCameraInfo()
        {
            string sql = "Select * from CameraInfo";
            GlobalInfo.Instance.CameraInfoList = SQLiteFac.Instance.ExecuteList<CameraInfo>(sql);
            foreach (CameraInfo info in GlobalInfo.Instance.CameraInfoList)
            {
                GlobalInfo.Instance.ChList.Add(CameraInfoToChannelInfo(info));
            }
            foreach (ChannelInfo info in GlobalInfo.Instance.ChList)
            {
                CameraWithPlayPanel tmp = new CameraWithPlayPanel();
                switch (info.CameraType)
                {
                    case 0:
                        {
                            tmp.camera = new UIControl_HBGK1(info);
                            //GlobalInfo.Instance.CameraList.Add(new UIControl_HBGK1(info));
                            break;
                        }
                    case 1:
                        {
                            tmp.camera = new YiTongCameraControl(info);
                            //GlobalInfo.Instance.CameraList.Add(new YiTongCameraControl(info));
                            break;
                        }
                }
                GlobalInfo.Instance.cameraWithPlayPanelList.Add(tmp);
            }
        }

        /// <summary>
        /// Step 3.摄像头绑定播放Grid（弃用）
        /// </summary>
        private void CameraBindGrid()
        {
            if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
            {
                foreach (CameraWithPlayPanel item in GlobalInfo.Instance.cameraWithPlayPanelList.Where(w=>w.camera.Info.CamGroup == 1))
                {
                    Grid gd = GlobalInfo.Instance.fourGdList.Where(w => w.Tag.ToString() == item.camera.Info.ID.ToString()).FirstOrDefault();
                    if (gd != null)
                    {
                        item.PlayGrid = gd;
                    }
                }
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Nine)
            {
                foreach (CameraWithPlayPanel item in GlobalInfo.Instance.cameraWithPlayPanelList.Where(w => w.camera.Info.CamGroup == 1))
                {
                    Grid gd = GlobalInfo.Instance.nineGdList.Where(w => w.Tag.ToString() == item.camera.Info.ID.ToString()).FirstOrDefault();
                    if (gd != null)
                    {
                        item.PlayGrid = gd;
                    }
                }
            }
        }
        /// <summary>
        /// Step 4.初始化播放列表（弃用）
        /// </summary>
        private void InitPlayList()
        {
            foreach (System.Windows.Controls.ListBox lb in GlobalInfo.Instance.listBoxeList)
            {
                List<CameraWithPlayPanel> tmpList = GlobalInfo.Instance.cameraWithPlayPanelList.Where(w => w.camera.Info.CamGroup.ToString() == lb.Tag.ToString()).ToList();
                foreach (CameraWithPlayPanel panel in tmpList)
                {
                    lb.Items.Add(InitListBoxItem(panel));
                }
            }
        }

        /// <summary>
        /// step 2.初始化摄像头录像线程
        /// </summary>
        private void InitCameraSaveTimeThread()
        {
            cameraSaveThreadTimer.Interval = 1;
            cameraSaveThreadTimer.Start();
            cameraSaveThreadTimer.Elapsed += CameraSaveThreadTimer_Elapsed;
        }

        /// <summary>
        /// step 3.初始化播放画面
        /// </summary>
        private void InitPlayPanel()
        {
            
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(FourPanel.Instance);
            FourPanel.Instance.FullScreenEvent -= Instance_FullScreenEvent;
            FourPanel.Instance.SelectCameraEvent -= Instance_SelectCameraEvent;
            FourPanel.Instance.IsCameraPlayEvent -= Instance_IsCameraPlayEvent;
            FourPanel.Instance.FullScreenEvent += Instance_FullScreenEvent;
            FourPanel.Instance.SelectCameraEvent += Instance_SelectCameraEvent;
            FourPanel.Instance.IsCameraPlayEvent += Instance_IsCameraPlayEvent;
            //this.lbCamera.Focus();
            GlobalInfo.Instance.fourGdList.Clear();
            GlobalInfo.Instance.fourGdList.Add(FourPanel.Instance.gridCamera1);
            GlobalInfo.Instance.fourGdList.Add(FourPanel.Instance.gridCamera2);
            GlobalInfo.Instance.fourGdList.Add(FourPanel.Instance.gridCamera3);
            GlobalInfo.Instance.fourGdList.Add(FourPanel.Instance.gridCamera4);
            NinePanel.Instance.FullScreenEvent -= Instance_FullScreenEvent;
            NinePanel.Instance.SelectCameraEvent -= Instance_SelectCameraEvent;
            NinePanel.Instance.IsCameraPlayEvent -= Instance_IsCameraPlayEvent;
            NinePanel.Instance.FullScreenEvent += Instance_FullScreenEvent;
            NinePanel.Instance.SelectCameraEvent += Instance_SelectCameraEvent;
            NinePanel.Instance.IsCameraPlayEvent += Instance_IsCameraPlayEvent;
            GlobalInfo.Instance.nineGdList.Clear();
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera1);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera2);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera3);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera4);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera5);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera6);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera7);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera8);
            GlobalInfo.Instance.nineGdList.Add(NinePanel.Instance.ninegridCamera9);
            SixPanel.Instance.FullScreenEvent -= Instance_FullScreenEvent;
            SixPanel.Instance.SelectCameraEvent -= Instance_SelectCameraEvent;
            SixPanel.Instance.IsCameraPlayEvent -= Instance_IsCameraPlayEvent;
            SixPanel.Instance.FullScreenEvent += Instance_FullScreenEvent;
            SixPanel.Instance.SelectCameraEvent += Instance_SelectCameraEvent;
            SixPanel.Instance.IsCameraPlayEvent += Instance_IsCameraPlayEvent;
            GlobalInfo.Instance.sixGdList.Clear();
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera1);
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera2);
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera3);
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera4);
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera5);
            GlobalInfo.Instance.sixGdList.Add(SixPanel.Instance.sixgridCamera6);
        }

        /// <summary>
        /// 初始化Expander
        /// </summary>
        /// <param name="group">摄像头组信息</param>
        private Expander InitExpander(CameraGroup group)
        {
            Expander expander = new Expander();
            expander.Header = group.Name;
            expander.FontSize = 24;
            expander.Margin = new Thickness(0, 5, 5, 0);
            expander.Tag = group;
            expander.Name = "exp" + group.Id;
            expander.MouseRightButtonDown += Expander_MouseRightButtonDown;
            //GlobalInfo.Instance.expanderList.Add(expander);
            return expander;
        }
        /// <summary>
        /// 初始化组内listbox
        /// </summary>
        /// <param name="group">所属分组</param>
        private System.Windows.Controls.ListBox InitListBox(CameraGroup group)
        {
            System.Windows.Controls.ListBox listBox = new System.Windows.Controls.ListBox();
            listBox.Name = "lb" + group.Id;
            listBox.Tag = group.Id;
            listBox.SelectionChanged += ListBox_SelectionChanged;
            GlobalInfo.Instance.listBoxeList.Add(listBox);
            return listBox;
        }

        /// <summary>
        /// 切换摄像头
        /// </summary>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListBox listBox = sender as listBox;
            ListBoxItem tmpItem = (sender as System.Windows.Controls.ListBox).SelectedItem as ListBoxItem;
            CameraWithPlayPanel tempCamera;
            if (tmpItem.Tag is CameraWithPlayPanel)
            {
                tempCamera = tmpItem.Tag as CameraWithPlayPanel;
                GlobalInfo.Instance.SelectGroup = GlobalInfo.Instance.GroupList.Where(w => w.Id == tempCamera.camera.Info.ID).FirstOrDefault();
                GlobalInfo.Instance.SelectCamera = tempCamera;
            }
        }

        /// <summary>
        /// 右键选择分组
        /// </summary>
        private void Expander_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            GlobalInfo.Instance.SelectGroup = ((sender as Expander).Tag as CameraGroup);
            GlobalInfo.Instance.SelectCamera = null;
        }
        /// <summary>
        /// 弃用
        /// </summary>
        private ListBoxItem InitListBoxItem(CameraWithPlayPanel item)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.FontSize = 24;
            listBoxItem.Margin = new Thickness(0, 10, 0, 0);
            listBoxItem.Foreground = (Brush)bc.ConvertFrom("#000000");
            listBoxItem.Tag = item;
            GlobalInfo.Instance.ListBoxItemList.Add(listBoxItem);
            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
            sp.MouseLeftButtonDown += MouseDown_PlayCamera;
            sp.Tag = item;
            TextBlock tb = new TextBlock();
            tb.Text = item.camera.Info.CameraName;
            tb.Tag = item;
            sp.Children.Add(tb);
            Image img = new Image();
            img.Name = "imgCamera" + item.camera.Info.ID;
            img.Source = new BitmapImage(new Uri("/Images/Play.png", UriKind.Relative));
            img.Width = 30;
            img.Height = 30;
            sp.Children.Add(img);
            listBoxItem.Content = sp;
            GlobalInfo.Instance.textBlockList.Add(tb);

            return listBoxItem;
        }
        /// <summary>
        /// 保存摄像头视频
        /// </summary>
        private void CameraSaveThreadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (cameraSaveThreadTimer.Interval == 1) cameraSaveThreadTimer.Interval = 1 * 60 * 1000;
            try
            {
                foreach (ICameraFactory camera in GlobalInfo.Instance.CameraList)
                {
                    string mainPath = System.Environment.CurrentDirectory + "\\video" + "\\video" + camera.Info.CameraName;
                    string secondPath = "\\" + DateTime.Now.Year + "年\\" + DateTime.Now.Month + "月\\" + DateTime.Now.ToString("yyyy-MM-dd");
                    string filePath = mainPath + secondPath;
                    string fileName = string.Empty;
                    if (camera.GetType().Name == "UIControl_HBGK1")
                    {
                        fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi";
                    }
                    else if (camera.GetType().Name == "YiTongCameraControl")
                    {
                        fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".h264";
                    }
                    camera.StopFile();
                    camera.SaveFile(filePath, fileName);
                }
                DeleteOldFileName(System.Environment.CurrentDirectory + "\\video");
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// CameraInfo转为ChannelInfo
        /// </summary>
        /// <param name="info">摄像头信息</param>
        /// <returns>摄像头通道信息</returns>
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
        List<DirectoryInfo> dirList = new List<DirectoryInfo>();
        /// <summary>
        /// 删除最老的视频文件
        /// </summary>
        /// <param name="path"></param>
        private void DeleteOldFileName(string path)
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            string[] disk = path.Split('\\');
            // 硬盘空间小于2G，开始清理录像
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == disk[0] + "\\" && drive.TotalFreeSpace / (1024 * 1024) < 1024*2)
                {
                    dirList.Clear();
                    ForeachDir(path);
                    DateTime date = dirList.Min(m => m.CreationTime).Date;
                    foreach (DirectoryInfo info in dirList)
                    {
                        if (info.CreationTime.Date == date)
                        {
                            info.Delete(true);
                        }
                    }
                    //DirectoryInfo root = new DirectoryInfo(path);
                    //List<FileInfo> fileList = root.GetFiles().OrderBy(s => s.CreationTime).Take(10).ToList();
                    //foreach (FileInfo file in fileList)
                    //{
                    //    file.Delete();
                    //}
                }
            }
        }
        /// <summary>
        /// 递归获取文件夹
        /// </summary>
        /// <param name="path">总目录</param>
        public void ForeachDir(string path)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
            // 录像总目录
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            if (dirInfo.Count() == 0)
            {
                if (theFolder.Name.Contains("年") || theFolder.Name.Contains("月"))
                {
                    theFolder.Delete();
                }
                dirList.Add(theFolder);
            }
            //遍历摄像头目录
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                ForeachDir(NextFolder.FullName);
            }
        }
        /// <summary>
        /// 删除文件夹及子文件内文件
        /// </summary>
        /// <param name="path">待删除文件</param>
        public void DeleteFiles(string path)
        {
            DirectoryInfo fatherFolder = new DirectoryInfo(path);
            //删除当前文件夹内文件
            FileInfo[] files = fatherFolder.GetFiles();
            foreach (FileInfo file in files)
            {
                //string fileName = file.FullName.Substring((file.FullName.LastIndexOf("\\") + 1), file.FullName.Length - file.FullName.LastIndexOf("\\") - 1);
                string fileName = file.Name;
                try
                {
                    if (!fileName.Equals("index.dat"))
                    {
                        File.Delete(file.FullName);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            //递归删除子文件夹内文件
            foreach (DirectoryInfo childFolder in fatherFolder.GetDirectories())
            {
                DeleteFiles(childFolder.FullName);
            }
        }
        /// <summary>
        /// 4分屏
        /// </summary>
        private void FourImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(FourPanel.Instance);
            imgFour.Source = new BitmapImage(new Uri("/Images/FourSelect.png", UriKind.Relative));
            imgNine.Source = new BitmapImage(new Uri("/Images/Nine.png", UriKind.Relative));
            imgSix.Source = new BitmapImage(new Uri("/Images/Six.png", UriKind.Relative));
            //this.gdFour.Background = (Brush)bc.ConvertFrom("#A6CFDC");
            //this.gdNine.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            GlobalInfo.Instance.nowPanel = NowPanel.Four;
            //CameraBindGrid();
        }
        /// <summary>
        /// 双击全屏
        /// </summary>
        /// <param name="camId"></param>
        private void Instance_FullScreenEvent(int camId)
        {
            FullPlayCamera.Instance.CanelFullScreenEvent -= Instance_CanelFullScreenEvent;
            FullPlayCamera.Instance.CanelFullScreenEvent += Instance_CanelFullScreenEvent;
            this.gdMain.Visibility = Visibility.Collapsed;
            if (!this.gdAll.Children.Contains(FullPlayCamera.Instance))
            {
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            else
            {
                this.gdAll.Children.Remove(FullPlayCamera.Instance);
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            FullPlayCamera.Instance.CamId = camId;
            FullPlayCamera.Instance.PlayCamera(camId);
        }

        /// <summary>
        /// 9分屏
        /// </summary>
        private void NineImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(NinePanel.Instance);
            //this.gdFour.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            //this.gdNine.Background = (Brush)bc.ConvertFrom("#A6CFDC");
            imgFour.Source = new BitmapImage(new Uri("/Images/Four.png", UriKind.Relative));
            imgNine.Source = new BitmapImage(new Uri("/Images/NineSelect.png", UriKind.Relative));
            imgSix.Source = new BitmapImage(new Uri("/Images/Six.png", UriKind.Relative));
            GlobalInfo.Instance.nowPanel = NowPanel.Nine;
            //CameraBindGrid();
        }
        /// <summary>
        /// 退出
        /// </summary>
        private void ExitImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("确认退出？", "提示", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
                foreach (ICameraFactory camera in GlobalInfo.Instance.CameraList)
                {
                    string mainPath = System.Environment.CurrentDirectory + "\\video" + "\\video" + camera.Info.CameraName;
                    string secondPath = "\\" + DateTime.Now.Year + "年\\" + DateTime.Now.Month + "月\\" + DateTime.Now.ToString("yyyy-MM-dd");
                    string filePath = mainPath + secondPath;
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi";
                    camera.StopFile();
                    camera.SaveFile(filePath, fileName);
                }
            }
        }
        /// <summary>
        /// 全屏
        /// </summary>
        private void FullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FullPlayCamera.Instance.CanelFullScreenEvent -= Instance_CanelFullScreenEvent;
            FullPlayCamera.Instance.CanelFullScreenEvent += Instance_CanelFullScreenEvent;
            this.gdMain.Visibility = Visibility.Collapsed;
            if (!this.gdAll.Children.Contains(FullPlayCamera.Instance))
            {
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            else
            {
                this.gdAll.Children.Remove(FullPlayCamera.Instance);
                this.gdAll.Children.Add(FullPlayCamera.Instance);
            }
            Grid dg = GlobalInfo.Instance.SelectGrid;
            if (dg != null && dg.Tag is ICameraFactory)
            {
                FullPlayCamera.Instance.PlayCamera((dg.Tag as ICameraFactory).Info.ID);
            }
        }
        /// <summary>
        /// 取消全屏
        /// </summary>
        private void Instance_CanelFullScreenEvent()
        {
            FullPlayCamera.Instance.CanelFullScreenEvent -= Instance_CanelFullScreenEvent;
            this.gdMain.Visibility = Visibility.Visible;
            if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
            {
                FourPanel.Instance.PlayCamera();
            }
            else if(GlobalInfo.Instance.nowPanel == NowPanel.Nine)
            {
                NinePanel.Instance.PlayCamera();
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Six)
            {
                SixPanel.Instance.PlayCamera();
            }
            this.gdAll.Children.Remove(FullPlayCamera.Instance);
        }
        /// <summary>
        /// 播放摄像头更改图标
        /// </summary>
        /// <param name="camId">摄像头Id</param>
        /// <param name="isPlay">播放成功</param>
        private void Instance_IsCameraPlayEvent(int camId, bool isPlay)
        {
            //Node node = FindPlayNode(GlobalInfo.Instance.CamList, camId);
            //if (node != null)
            //{
            //    if (isPlay)
            //    {
            //        node.NodeImg = "/Images/Pause.png";
            //    }
            //    else
            //    {
            //        node.NodeImg = "/Images/Play.png";
            //    }
            //}
            //this.tvCamera.ItemsSource = null;
            //this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            List<int> playIdList = new List<int>();
            if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
            {
                foreach (Grid gd in GlobalInfo.Instance.fourGdList)
                {
                    if (gd.Tag is ICameraFactory)
                    {
                        bool play = (gd.Tag as ICameraFactory).Info.IsPlay;
                        if (play) playIdList.Add((gd.Tag as ICameraFactory).Info.ID);
                    }
                }
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Nine)
            {
                foreach (Grid gd in GlobalInfo.Instance.nineGdList)
                {
                    if (gd.Tag is ICameraFactory)
                    {
                        bool play = (gd.Tag as ICameraFactory).Info.IsPlay;
                        if (play) playIdList.Add((gd.Tag as ICameraFactory).Info.ID);
                    }
                }
            }
            else if (GlobalInfo.Instance.nowPanel == NowPanel.Six)
            {
                foreach (Grid gd in GlobalInfo.Instance.sixGdList)
                {
                    if (gd.Tag is ICameraFactory)
                    {
                        bool play = (gd.Tag as ICameraFactory).Info.IsPlay;
                        if (play) playIdList.Add((gd.Tag as ICameraFactory).Info.ID);
                    }
                }
            }

            FindPlayNode(GlobalInfo.Instance.CamList, playIdList);
            this.tvCamera.ItemsSource = null;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //this.tvCamera.ItemsSource = null;
            //this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //ExpandAllItems(this.tvCamera);
            //foreach (Image image in FindVisualChildren<Image>(this.lbCamera))
            //{
            //    if (image.Tag != null && image.Tag.ToString() == camId.ToString())
            //    {
            //        if (isPlay)
            //        {
            //            path = "/Images/Pause.png";
            //        }
            //        else
            //        {
            //            path = "/Images/Play.png";
            //        }
            //        var uriSource = new Uri(path, UriKind.Relative);
            //        image.Source = new BitmapImage(uriSource);
            //        break;
            //    }
            //}
        }
        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="control"></param>
        private void ExpandAllItems(ItemsControl control)
        {
            if (control == null)
            {
                return;
            }

            foreach (Object item in control.Items)
            {

                System.Windows.Controls.TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (treeItem == null || !treeItem.HasItems)
                {
                    continue;
                }

                treeItem.IsExpanded = true;
                ExpandAllItems(treeItem as ItemsControl);
            }
        }
        /// <summary>
        /// 递归方法：根据播放状态修改播放图标
        /// </summary>
        /// <param name="nodeList">待寻找节点</param>
        /// <param name="playIdList">播放摄像头ID列表</param>
        private void FindPlayNode(List<Node> nodeList, List<int> playIdList)
        {
            foreach (Node node in nodeList)
            {
                if (node.Tag is CameraInfo)
                {
                    if (playIdList.Contains(node.NodeId))
                        node.NodeImg = "/Images/Pause.png";
                    else
                        node.NodeImg = "/Images/Play.png";
                }
                else
                {
                    if (node.Nodes != null && node.Nodes.Count > 0)
                    {
                        FindPlayNode(node.Nodes, playIdList);
                    }
                }
            }
        }
        /// <summary>
        /// 选择摄像头
        /// </summary>
        /// <param name="camId">摄像头ID</param>
        private void Instance_SelectCameraEvent(int camId)
        {
            SelectTheCurrentNode(camId);
        }


        /// <summary>
        ///  查找子元素
        /// </summary>
        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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
        /// 弃用
        /// </summary>
        private void lbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Border bd in FindVisualChildren<Border>(FourPanel.Instance.gdMain))
            {
                var bc = new BrushConverter();
                //if (bd.Tag != null && bd.Tag.ToString() == (this.lbCamera.SelectedIndex +1).ToString())
                //{
                //    bd.BorderBrush = (Brush)bc.ConvertFrom("#002DFF");
                //}
                //else
                //{
                //    bd.BorderBrush = (Brush)bc.ConvertFrom("#686868");
                //}
            }
        }
        /// <summary>
        /// 双击播放或者停止摄像头
        /// </summary>
        private void MouseDown_PlayCamera(object sender, MouseButtonEventArgs e)
        {
            int tag = int.Parse((sender as StackPanel).Tag.ToString());
            if (e.ClickCount == 2)
            {
                if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
                {
                    Grid gdfour = GlobalInfo.Instance.fourGdList.Where(w => w.Tag.ToString() == tag.ToString()).FirstOrDefault();
                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == tag).FirstOrDefault();
                    if (gdfour != null && camera != null)
                    {
                        if (camera.Info.IsPlay)
                        {
                            FourPanel.Instance.StopCamera(gdfour, camera);
                        }
                        else
                        {
                            FourPanel.Instance.PlaySelectCamera(gdfour, camera);
                        }
                    }
                }
                else if (GlobalInfo.Instance.nowPanel == NowPanel.Nine)
                {
                    Grid gdnine = GlobalInfo.Instance.nineGdList.Where(w => w.Tag.ToString() == tag.ToString()).FirstOrDefault();
                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == tag).FirstOrDefault();
                    if (gdnine != null && camera != null)
                    {
                        if (camera.Info.IsPlay)
                        {
                            NinePanel.Instance.StopCamera(gdnine, camera);
                        }
                        else
                        {
                            NinePanel.Instance.PlaySelectCamera(gdnine, camera);
                        }
                    }
                }
                else if (GlobalInfo.Instance.nowPanel == NowPanel.Six)
                {
                    Grid gdsix = GlobalInfo.Instance.sixGdList.Where(w => w.Tag.ToString() == tag.ToString()).FirstOrDefault();
                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == tag).FirstOrDefault();
                    if (gdsix != null && camera != null)
                    {
                        if (camera.Info.IsPlay)
                        {
                            SixPanel.Instance.StopCamera(gdsix, camera);
                        }
                        else
                        {
                            SixPanel.Instance.PlaySelectCamera(gdsix, camera);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 左列表隐藏
        /// </summary>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (firstCol.Width.Value > 0)
            {
                firstCol.Width = new System.Windows.GridLength(0);
            }
            else
            {
                firstCol.Width = new GridLength(1, GridUnitType.Star);
            }
        }
        /// <summary>
        /// 添加分组
        /// </summary>
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddOrModifyGroup addOrModifyGroup = new AddOrModifyGroup();
            addOrModifyGroup.InitWin("添加分组", new CameraGroup() { Id = 0, Name = string.Empty });
            addOrModifyGroup.AddGroupEvent += Main_AddGroupEvent;
            addOrModifyGroup.ShowDialog();
        }
        /// <summary>
        /// 添加分组回调事件
        /// </summary>
        /// <param name="group"></param>
        private void Main_AddGroupEvent(CameraGroup group)
        {
            string sql = string.Format("Insert Into CameraGroup (Name) Values ('{0}')", group.Name);
            int groupID = SQLiteFac.Instance.InsertWithBack(sql);
            //sql = "Select Max(Id) From CameraGroup";
            //List<CameraGroup> nowgroup = SQLiteFac.Instance.ExecuteList<CameraGroup>(sql);
            //spCameraList.Children.Add(InitExpander(group));
            group.Id = groupID;
            AddGroup(group,GlobalInfo.Instance.CamList[0]);
            this.tvCamera.ItemsSource = null;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
        }
        /// <summary>
        /// 修改分组事件
        /// </summary>
        private void ModifyGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!(GlobalInfo.Instance.SelectNode.Tag is CameraGroup))
            {
                System.Windows.MessageBox.Show("未选中组");
                return;
            }
            AddOrModifyGroup addOrModifyGroup = new AddOrModifyGroup();
            addOrModifyGroup.InitWin("修改分组", GlobalInfo.Instance.SelectNode.Tag as CameraGroup);
            addOrModifyGroup.ModifyGroupEvent += Main_ModifyGroupEvent;
            addOrModifyGroup.ShowDialog();
        }
        /// <summary>
        /// 修改分组回调
        /// </summary>
        /// <param name="group"></param>
        private void Main_ModifyGroupEvent(CameraGroup group)
        {
            string sql = string.Format("Update CameraGroup Set Name= '{0}' Where Id= '{1}'", group.Name, group.Id);
            SQLiteFac.Instance.ExecuteNonQuery(sql);
            GlobalInfo.Instance.CamList[0].Nodes.Where(w => w.NodeId == group.Id).FirstOrDefault().NodeName = group.Name;
            this.tvCamera.ItemsSource = null;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //foreach (var child in spCameraList.Children)
            //{
            //    if ((child is Expander) && (child as Expander).Tag != null && ((child as Expander).Tag as CameraGroup).Id == group.Id)
            //    {
            //        ((child as Expander).Tag as CameraGroup).Name = group.Name;
            //        (child as Expander).Header = group.Name;
            //        break;
            //    }
            //}
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        private void DelGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!(GlobalInfo.Instance.SelectNode.Tag is CameraGroup))
            {
                System.Windows.MessageBox.Show("未选中组");
                return;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show("确认删除分组及组下包含的摄像头？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string sql = string.Format("Delete From CameraGroup Where Id= '{0}'", GlobalInfo.Instance.SelectNode.NodeId);
                SQLiteFac.Instance.ExecuteNonQuery(sql);
                sql = string.Format("Delete From CameraInfo Where CamGroup= '{0}'", GlobalInfo.Instance.SelectNode.NodeId);
                SQLiteFac.Instance.ExecuteNonQuery(sql);
                GlobalInfo.Instance.CamList[0].Nodes.Remove(GlobalInfo.Instance.SelectNode);
                this.tvCamera.ItemsSource = null;
                this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            }
            //foreach (var child in spCameraList.Children)
            //{
            //    if ((child is Expander) && (child as Expander).Tag != null && (child as Expander).Tag == GlobalInfo.Instance.SelectGroup)
            //    {
            //        spCameraList.Children.Remove(child as Expander);
            //        //GlobalInfo.Instance.expanderList.Remove(child as Expander);
            //        break;
            //    }
            //}
        }

        /// <summary>
        /// 添加摄像头
        /// </summary>
        private void AddCamera_Click(object sender, RoutedEventArgs e)
        {
            Node node = GetRootNode(GlobalInfo.Instance.SelectNode);
            if (!(node.Tag is CameraGroup))
            {
                System.Windows.MessageBox.Show("选择分组出错");
                return;
            }

            AddOrModifyCamera addOrModifyCamera = new AddOrModifyCamera("添加摄像头");
            addOrModifyCamera.InitWin("添加摄像头", node.Tag as CameraGroup, null);
            addOrModifyCamera.AddCameraEvent += Main_AddCameraEvent;
            addOrModifyCamera.ShowDialog();
        }
        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <param name="node">当前节点</param>
        private Node GetRootNode(Node node)
        {
            if (node.Parent != null)
            {
                return node.Parent;
            }
            return node;
        }

        /// <summary>
        /// 添加摄像头事件回调
        /// </summary>
        /// <param name="group"></param>
        private void Main_AddCameraEvent(CameraInfo info)
        {
            string sql = string.Format("Insert Into CameraInfo (CHLID,REMOTECHANNLE,REMOTEIP,REMOTEPORT,REMOTEUSER,REMOTEPWD,NPLAYPORT,PTZPORT,CAMERATYPE,CameraName,CamGroup) " +
                "Values ('1', '1', '{0}', '{1}', '{2}', '{3}', '{4}', '8091', '{5}', '{6}', '{7}')",
                info.REMOTEIP,info.REMOTEPORT,info.REMOTEUSER,info.REMOTEPWD,info.NPLAYPORT,info.CAMERATYPE,info.CAMERANAME,info.CamGroup);
            int id = SQLiteFac.Instance.InsertWithBack(sql);
            info.Id = id;
            Node camnode = new Node();
            camnode.NodeId = info.Id;
            camnode.NodeName = info.CAMERANAME;
            camnode.NodeImg = "/Images/Play.png";
            camnode.Tag = info;
            // 选择的是摄像头不是组
            if (GlobalInfo.Instance.SelectNode.Tag is CameraInfo)
            {
                GlobalInfo.Instance.SelectGroupNode = GetRootNode(GlobalInfo.Instance.SelectNode);
                camnode.Parent = GlobalInfo.Instance.SelectGroupNode;
                GlobalInfo.Instance.SelectGroupNode.Nodes.Add(camnode);
            }
            else if (GlobalInfo.Instance.SelectNode.Tag is CameraGroup)
            {
                camnode.Parent = GlobalInfo.Instance.SelectNode;
                GlobalInfo.Instance.SelectNode.Nodes.Add(camnode);
            }
            else
            {
                
            }
            ChannelInfo chInfo =  CameraInfoToChannelInfo(info);
            switch (chInfo.CameraType)
            {
                case 0:
                    {
                        GlobalInfo.Instance.CameraList.Add(new UIControl_HBGK1(chInfo));
                        break;
                    }
                case 1:
                    {
                        GlobalInfo.Instance.CameraList.Add(new YiTongCameraControl(chInfo));
                        break;
                    }
            }
            this.tvCamera.ItemsSource = null;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //ChannelInfo chInfo = CameraInfoToChannelInfo(info);
            //GlobalInfo.Instance.ChList.Add(chInfo);
            //CameraWithPlayPanel tmp = new CameraWithPlayPanel();
            //switch (info.CAMERATYPE)
            //{
            //    case 0:
            //        {
            //            tmp.camera = new UIControl_HBGK1(chInfo);
            //            break;
            //        }
            //    case 1:
            //        {
            //            tmp.camera = new YiTongCameraControl(chInfo);
            //            break;
            //        }
            //}
            //GlobalInfo.Instance.cameraWithPlayPanelList.Add(tmp);
            //foreach (System.Windows.Controls.ListBox item in GlobalInfo.Instance.listBoxeList)
            //{
            //    if (item.Name == "lb" + info.CamGroup)
            //    {
            //        item.Items.Add(InitListBoxItem(tmp));
            //    }
            //}
        }
        /// <summary>
        /// 修改摄像头
        /// </summary>
        private void ModifyCamera_Click(object sender, RoutedEventArgs e)
        {
            if (!(GlobalInfo.Instance.SelectNode.Tag is CameraInfo))
            {
                System.Windows.MessageBox.Show("未选中摄像头");
                return;
            }
            Node node = GetRootNode(GlobalInfo.Instance.SelectNode);
            AddOrModifyCamera addOrModifyCamera = new AddOrModifyCamera("修改摄像头");
            addOrModifyCamera.InitWin("修改摄像头", node.Tag as CameraGroup, GlobalInfo.Instance.SelectNode.Tag as CameraInfo);
            addOrModifyCamera.ModifyCameraEvent += Main_ModifyCameraEvent;
            addOrModifyCamera.ShowDialog();
        }

        /// <summary>
        /// 修改摄像头事件回调
        /// </summary>
        /// <param name="group"></param>
        private void Main_ModifyCameraEvent(CameraInfo info)
        {
            string sql = string.Format("update CameraInfo Set REMOTEIP = '{0}',REMOTEPORT='{1}',REMOTEUSER='{2}',REMOTEPWD='{3}',NPLAYPORT='{4}',CAMERATYPE='{5}',CameraName='{6}',CamGroup='{7}' Where Id={8} ",
                info.REMOTEIP, info.REMOTEPORT, info.REMOTEUSER, info.REMOTEPWD,info.NPLAYPORT, info.CAMERATYPE, info.CAMERANAME, info.CamGroup, info.Id);
            SQLiteFac.Instance.ExecuteNonQuery(sql);
            GlobalInfo.Instance.SelectNode.Tag = info;
            GlobalInfo.Instance.SelectNode.NodeName = info.CAMERANAME;
            //GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault().Info = CameraInfoToChannelInfo(info);
            ChannelInfo chInfo = CameraInfoToChannelInfo(info);
            GlobalInfo.Instance.CameraList.RemoveAll(w => w.Info.ID == info.Id);
            switch (chInfo.CameraType)
            {
                case 0:
                    {
                        GlobalInfo.Instance.CameraList.Add(new UIControl_HBGK1(chInfo));
                        break;
                    }
                case 1:
                    {
                        GlobalInfo.Instance.CameraList.Add(new YiTongCameraControl(chInfo));
                        break;
                    }
            }
            this.tvCamera.ItemsSource = null;
            this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            //GlobalInfo.Instance.cameraWithPlayPanelList.RemoveAll(w=>w.camera.Info.ID == info.camera.Info.ID);
            //GlobalInfo.Instance.cameraWithPlayPanelList.Add(info);
            //foreach (System.Windows.Controls.ListBox item in GlobalInfo.Instance.listBoxeList)
            //{
            //    if (item.Name == "lb" + info.camera.Info.CamGroup)
            //    {
            //       GlobalInfo.Instance.ListBoxItemList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == info.camera.Info.ID).FirstOrDefault().Tag = info;
            //        GlobalInfo.Instance.textBlockList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == info.camera.Info.ID).FirstOrDefault().Tag = info;
            //        GlobalInfo.Instance.textBlockList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == info.camera.Info.ID).FirstOrDefault().Text = info.camera.Info.CameraName;
            //    }
            //}
        }
        /// <summary>
        /// 删除摄像头
        /// </summary>
        private void DelCamera_Click(object sender, RoutedEventArgs e)
        {
            if (!(GlobalInfo.Instance.SelectNode.Tag is CameraInfo))
            {
                System.Windows.MessageBox.Show("未选中摄像头");
                return;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show("确认删除摄像头？","提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string sql = string.Format("Delete From CameraInfo Where Id= '{0}'", (GlobalInfo.Instance.SelectNode.Tag as CameraInfo).Id);
                SQLiteFac.Instance.ExecuteNonQuery(sql);
                GlobalInfo.Instance.CameraList.RemoveAll(w => w.Info.ID == (GlobalInfo.Instance.SelectNode.Tag as CameraInfo).Id);
                DeleteSelectCamera(GlobalInfo.Instance.SelectNode,GlobalInfo.Instance.CamList);
                //Node node = GetRootNode(GlobalInfo.Instance.SelectNode);
                //foreach (Node tmp in node.Nodes)
                //{
                //    if (tmp == GlobalInfo.Instance.SelectNode)
                //    {
                //        tmp.Nodes.Remove(GlobalInfo.Instance.SelectNode);
                //    }
                //}
                this.tvCamera.ItemsSource = null;
                this.tvCamera.ItemsSource = GlobalInfo.Instance.CamList;
            }
            //foreach (System.Windows.Controls.ListBox item in GlobalInfo.Instance.listBoxeList)
            //{
            //    //ListBoxItem lbItem =  GlobalInfo.Instance.ListBoxItemList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == GlobalInfo.Instance.SelectCamera.camera.Info.ID).FirstOrDefault();
            //    if (item.Items.Contains(GlobalInfo.Instance.ListBoxItemList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == GlobalInfo.Instance.SelectCamera.camera.Info.ID).FirstOrDefault()))
            //    {
            //        item.Items.Remove(GlobalInfo.Instance.ListBoxItemList.Where(w => (w.Tag as CameraWithPlayPanel).camera.Info.ID == GlobalInfo.Instance.SelectCamera.camera.Info.ID).FirstOrDefault());
            //    }
            //}
        }
        /// <summary>
        /// 递归方法：递归寻找待删除摄像头
        /// </summary>
        /// <param name="selectNode">选中得节点</param>
        /// <param name="NodeList">节点列表</param>
        private void DeleteSelectCamera(Node selectNode,List<Node> NodeList)
        {
            foreach (Node tmp in NodeList)
            {
                if (tmp == selectNode)
                {
                    NodeList.Remove(tmp);
                    return;
                }
                if (tmp.Nodes.Count>0)
                {
                    DeleteSelectCamera(selectNode, tmp.Nodes);
                }
            }
        }
        /// <summary>
        /// 双击播放
        /// </summary>
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (GlobalInfo.Instance.nowPanel == NowPanel.Four)
                {
                    if (GlobalInfo.Instance.SelectNode != null)
                    {
                        if (GlobalInfo.Instance.SelectGrid != null)
                        {
                            CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                            ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                            if (camera != null)
                            {
                                FourPanel.Instance.PlaySelectCamera(GlobalInfo.Instance.SelectGrid, camera);
                            }
                        }
                        else
                        {
                            foreach (Grid gd in GlobalInfo.Instance.fourGdList)
                            {
                                if (gd.Children.Count > 0 && gd.Children[0].GetType().Name != "UIControl_HBGK1" && gd.Children[0].GetType().Name != "YiTongCameraControl")
                                {
                                    CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                                    if (camera != null)
                                    {
                                        FourPanel.Instance.PlaySelectCamera(gd, camera);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (GlobalInfo.Instance.nowPanel == NowPanel.Nine)
                {
                    if (GlobalInfo.Instance.SelectNode != null)
                    {
                        if (GlobalInfo.Instance.SelectGrid != null)
                        {
                            CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                            ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                            if (camera != null)
                            {
                                NinePanel.Instance.PlaySelectCamera(GlobalInfo.Instance.SelectGrid, camera);
                            }
                        }
                        else
                        {
                            foreach (Grid gd in GlobalInfo.Instance.nineGdList)
                            {
                                if (gd.Children.Count > 0 && gd.Children[0].GetType().Name != "UIControl_HBGK1" && gd.Children[0].GetType().Name != "YiTongCameraControl")
                                {
                                    CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                                    if (camera != null)
                                    {
                                        NinePanel.Instance.PlaySelectCamera(gd, camera);
                                    }
                                    break;
                                }
                            }
                        }
                    }


                }
                else if (GlobalInfo.Instance.nowPanel == NowPanel.Six)
                {
                    if (GlobalInfo.Instance.SelectNode != null)
                    {
                        if (GlobalInfo.Instance.SelectGrid != null)
                        {
                            CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                            ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                            if (camera != null)
                            {
                                NinePanel.Instance.PlaySelectCamera(GlobalInfo.Instance.SelectGrid, camera);
                            }
                        }
                        else
                        {
                            foreach (Grid gd in GlobalInfo.Instance.sixGdList)
                            {
                                if (gd.Children.Count > 0 && gd.Children[0].GetType().Name != "UIControl_HBGK1" && gd.Children[0].GetType().Name != "YiTongCameraControl")
                                {
                                    CameraInfo info = GlobalInfo.Instance.SelectNode.Tag as CameraInfo;
                                    ICameraFactory camera = GlobalInfo.Instance.CameraList.Where(w => w.Info.ID == info.Id).FirstOrDefault();
                                    if (camera != null)
                                    {
                                        SixPanel.Instance.PlaySelectCamera(gd, camera);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 树列表右键选中
        /// </summary>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                Node currentNode = treeViewItem.Header as Node;
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        private DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
        /// <summary>
        /// 选中摄像头节点
        /// </summary>
        /// <param name="SelectID"></param>
        private void SelectTheCurrentNode(int SelectID)
        {
            if (this.tvCamera.Items != null && this.tvCamera.Items.Count > 0)
            {
                foreach (var item in this.tvCamera.Items)
                {
                    DependencyObject dependencyObject = this.tvCamera.ItemContainerGenerator.ContainerFromItem(item);
                    if (dependencyObject != null)//第一次打开程序，dependencyObject为null，会出错
                    {
                        TreeViewItem tvi = (TreeViewItem)dependencyObject;
                        if (((tvi.Header as Node).Tag is CameraInfo) && (tvi.Header as Node).NodeId == SelectID)
                        {
                            tvi.IsSelected = true;
                            tvi.Focus();
                        }
                        else
                        {
                            SetNodeSelected(tvi, SelectID);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 递归选中树节点
        /// </summary>
        /// <param name="Item"></param>
        private void SetNodeSelected(TreeViewItem Item, int SelectID)
        {
            foreach (var item in Item.Items)
            {
                DependencyObject dependencyObject = Item.ItemContainerGenerator.ContainerFromItem(item);
                if (dependencyObject != null)
                {
                    TreeViewItem treeViewItem = (TreeViewItem)dependencyObject;
                    if (((treeViewItem.Header as Node).Tag is CameraInfo) && (treeViewItem.Header as Node).NodeId == SelectID)
                    {
                        treeViewItem.IsSelected = true;
                        treeViewItem.Focus();
                    }
                    else
                    {
                        treeViewItem.IsSelected = false;
                        if (treeViewItem.HasItems)
                        {
                            SetNodeSelected(treeViewItem, SelectID);
                        }
                    }
                }
            }
        }

        private void Video_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(VideoList.Instance);
        }
        /// <summary>
        /// 选择6画面
        /// </summary>
        private void SixImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(SixPanel.Instance);
            imgFour.Source = new BitmapImage(new Uri("/Images/Four.png", UriKind.Relative));
            imgNine.Source = new BitmapImage(new Uri("/Images/Nine.png", UriKind.Relative));
            imgSix.Source = new BitmapImage(new Uri("/Images/SixSelect.png", UriKind.Relative));
            GlobalInfo.Instance.nowPanel = NowPanel.Six;
        }
    }

    public class Node
    {
        public Node()
        {
            this.IsDeleted = false;
            this.Nodes = new List<Node>();
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeId { get; set; }
        /// <summary>
        ///  节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点图片
        /// </summary>
        public string NodeImg { get; set; }

        /// <summary>
        /// 被删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 绑定类型
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<Node> Nodes { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public Node Parent { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpand { get; set; }
        /// <summary>
        /// 是否处于播放状态
        /// </summary>
        public bool IsPlay { get; set; }
    }
    //public class ViewModel : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private List<Node> treenodes = new List<Node>();
    //    public List<Node> TreeNodes
    //    {
    //        get { return treenodes; }
    //        set
    //        {
    //            treenodes = value;
    //            if (PropertyChanged != null)
    //                PropertyChanged(this, new PropertyChangedEventArgs("TreeNodes"));
    //        }
    //    }

    //    public ViewModel()
    //    {
            
    //    }
    //}
}
