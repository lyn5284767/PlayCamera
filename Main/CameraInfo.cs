using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayCamera
{
    /// <summary>
    /// 摄像头信息
    /// </summary>
    public class CameraInfo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int CHLID { get; set; }
        /// <summary>
        /// 远端渠道
        /// </summary>
        public int REMOTECHANNLE { get; set; }
        /// <summary>
        /// 远端IP
        /// </summary>
        public string REMOTEIP { get; set; }
        /// <summary>
        /// 远端端口
        /// </summary>
        public int REMOTEPORT { get; set; }
        /// <summary>
        /// 远端用户
        /// </summary>
        public string REMOTEUSER { get; set; }
        /// <summary>
        /// 远端密码
        /// </summary>
        public string REMOTEPWD { get; set; }
        /// <summary>
        /// 远端端口
        /// </summary>
        public int NPLAYPORT { get; set; }
        /// <summary>
        /// 远端端口
        /// </summary>
        public int PTZPORT { get; set; }
        /// <summary>
        /// 摄像头类型0:宏英 1：一通
        /// </summary>
        public int CAMERATYPE { get; set; }
        /// <summary>
        /// 摄像头名
        /// </summary>
        public string CAMERANAME { get; set; }
        /// <summary>
        /// 所属分组
        /// </summary>
        public int CamGroup { get; set; }
    }
    /// <summary>
    /// 摄像头组
    /// </summary>
    public class CameraGroup
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }
    }

    public class VideoInfo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 保存日期
        /// </summary>
        public DateTime SaveTime { get; set; }
        /// <summary>
        /// 列表显示时间
        /// </summary>
        public string ShowTime { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 录像路径
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 摄像头类型 0-宏英；1-一通
        /// </summary>
        public int CameraType { get; set; }
    }

    public class DirLink
    {
        /// <summary>
        /// 文件夹名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父文件夹名
        /// </summary>
        public string ParentName { get; set; }
    }
}
