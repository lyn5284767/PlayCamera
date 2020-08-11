using HBGKTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public List<CameraInfo> ProtocolList = new List<CameraInfo>();
        public List<ChannelInfo> ChList = new List<ChannelInfo>();//摄像头信息列表
        public List<ICameraFactory> CameraList = new List<ICameraFactory>(); //摄像头
    }
}
