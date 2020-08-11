using HBGKTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Main
{
    /// <summary>
    /// 摄像头绑定播放窗口类
    /// </summary>
    public class CameraWithPlayPanel
    {
        /// <summary>
        /// 摄像头信息
        /// </summary>
        public ICameraFactory camera { get; set;}
        /// <summary>
        /// 播放Grid
        /// </summary>
        public Grid PlayGrid { get; set; }
    }
}
