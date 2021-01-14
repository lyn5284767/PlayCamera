using HBGKTest;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// RTSPControl.xaml 的交互逻辑
    /// </summary>
    public partial class RTSPControl : UserControl,ICameraFactory
    {
        //获取输出目录
        private static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        //vlc文件的地址
        private DirectoryInfo vlcLibDirectory = new DirectoryInfo(appPath + @"VLC");
        //配置项
        string[] options = new string[]
        {
            // VLC options can be given here. Please refer to the VLC command line documentation.
            ":network-caching=1000"
        };
        public RTSPControl(ChannelInfo info)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.Info = info;
        }

        public ChannelInfo Info { get ; set ; }

        public event ChangeVideo ChangeVideoEvent;
        public event FullScreen FullScreenEvent;
        public event SelectCamera SelectCameraEvent;

        public bool InitCamera(ChannelInfo chInfo)
        {
            try
            {
                this.Info = chInfo;
                PlayCamera();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void PlayCamera()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //创建播放器
                this.vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory, options);
                //本地视频
                //this.vlcControl.SourceProvider.MediaPlayer.Play(new Uri("rtmp://58.200.131.2:1935/livetv/hunantv"));
                this.vlcControl.SourceProvider.MediaPlayer.Play(new Uri(Info.RemoteIP));
            }));
        }

        public void SaveFile(string filePath, string fileName)
        {
           
        }

        public void SetSize(double height, double width)
        {
            this.Height = height;
            this.Width = width;
        }

        public void StopCamera()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (this.vlcControl.SourceProvider.MediaPlayer != null)
                {
                    //if(this.vlcControl.SourceProvider.MediaPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                    //{
                    //    this.vlcControl.SourceProvider.MediaPlayer.Stop();
                    //}
                    this.vlcControl.SourceProvider.MediaPlayer.ResetMedia();
                    //this.vlcControl.SourceProvider.MediaPlayer.Dispose();
                }
            }));
        }

        public void StopFile()
        {
            
        }
        /// <summary>
        /// 双击全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FullScreenEvent != null)
            {
                FullScreenEvent(this.Info.ID);
            }
        }
        /// <summary>
        /// 单机选中摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectCameraEvent != null)
            {
                SelectCameraEvent(this.Info.ID);
            }
        }
    }
}
