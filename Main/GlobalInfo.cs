using HBGKTest;
using Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PlayCamera
{
    public class GlobalInfo
    {
        private static GlobalInfo _instance = null;
        private static readonly object syncRoot = new object();

        public static GlobalInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalInfo();
                        }
                    }
                }
                return _instance;
            }
        }

        public List<CameraInfo> CameraInfoList = new List<CameraInfo>(); // 摄像头
        public List<ChannelInfo> ChList = new List<ChannelInfo>();//摄像头信息列表
        public List<ICameraFactory> CameraList = new List<ICameraFactory>(); //摄像头
        public List<Grid> fourGdList = new List<Grid>(); //4画面
        public List<Grid> nineGdList = new List<Grid>(); // 9画面
        public NowPanel nowPanel { get; set; }
        public List<CameraWithPlayPanel> cameraWithPlayPanelList = new List<CameraWithPlayPanel>();
        public List<CameraGroup> GroupList = new List<CameraGroup>();
        //public List<Expander> expanderList = new List<Expander>(); // 保存所有分组
        public List<ListBox> listBoxeList = new List<ListBox>(); // 保存所有分组列表
        public List<ListBoxItem> ListBoxItemList = new List<ListBoxItem>(); // 保存所有分组列表子选项
        //public List<StackPanel> stackPanelList = new List<StackPanel>(); // 保存所有分组列表摄像头
        public List<TextBlock> textBlockList = new List<TextBlock>(); // 保存所有分组摄像头名称
        public CameraWithPlayPanel SelectCamera { get; set; }
        public CameraGroup SelectGroup { get; set; }

        public Node SelectNode { get; set; } // 选择的树节点
        public List<Node> CamList = new List<Node>(); // 树节点列表
        public Node SelectGroupNode { get; set; } // 选择类型未组的树节点
        public Grid SelectGrid { get; set; } // 选择的播放面

        public GloConfig GloConfig { get; set; }
    }
    /// <summary>
    /// 全局配置
    /// </summary>
    public class GloConfig
    {
        /// <summary>
        /// 本地IP
        /// </summary>
        public string LocalIP { get; set; }
        /// <summary>
        /// 本地端口
        /// </summary>
        public int LocalPort { get; set; }
        /// <summary>
        /// 远端IP
        /// </summary>
        public string RemoteIP { get; set; }
        /// <summary>
        /// 远端端口
        /// </summary>
        public int RemotePort { get; set; }
        /// <summary>
        /// 通信是否打开 0-关闭； 1-打开
        /// </summary>
        public int Open { get; set; }
    }
    /// <summary>
    /// 摄像头分屏枚举
    /// </summary>
    public enum NowPanel
    { 
        Four =1,
        Nine =2
    }
}
