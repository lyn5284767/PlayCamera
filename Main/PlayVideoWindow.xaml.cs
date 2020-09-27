using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// PlayVideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayVideoWindow : Window
    {
        //获取输出目录
        private static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        //vlc文件的地址
        private DirectoryInfo vlcLibDirectory = new DirectoryInfo(appPath + @"VLC");
        //配置项
        string[] options = new string[]
        {
            // VLC options can be given here. Please refer to the VLC command line documentation.
        };
        private string FilePath { get; set; }

        private bool Init = false;
        public PlayVideoWindow(string file)
        {
            this.FilePath = file;
            InitializeComponent();
            try
            {
                this.timelineSlider.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(timelineSlider_MouseUp), true);
                //创建播放器
                this.vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory, options);
                //本地视频
                this.vlcControl.SourceProvider.MediaPlayer.Play(new Uri(FilePath));
                //添加播放结束事件
                //其他事件 请自行查看
                this.vlcControl.SourceProvider.MediaPlayer.EndReached += MediaPlayerEndEvent;
                this.vlcControl.SourceProvider.MediaPlayer.Opening += MediaPlayer_Opening;
                this.vlcControl.SourceProvider.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            }
            catch (Exception ex)
            {
                
            }

        }
        private void MediaPlayer_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (Init)
            {
                Init = false;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.timelineSlider.Minimum = 0.0;
                    this.timelineSlider.Maximum = this.vlcControl.SourceProvider.MediaPlayer.Length;
                    int minute = (int)(this.vlcControl.SourceProvider.MediaPlayer.Length) / 1000 / 60;
                    int second = ((int)(this.vlcControl.SourceProvider.MediaPlayer.Length) / 1000) % 60;
                    this.tbTotal.Text = minute + ":" + second;
                }));
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.vlcControl.SourceProvider.MediaPlayer.Time > this.timelineSlider.Value + 500)
                {
                    lock (locker)
                    {
                        this.timelineSlider.Value = this.vlcControl.SourceProvider.MediaPlayer.Time;
                    }
                }
                int minute = (int)(this.vlcControl.SourceProvider.MediaPlayer.Time) / 1000 / 60;
                int second = ((int)(this.vlcControl.SourceProvider.MediaPlayer.Time) / 1000) % 60;
                this.tbNow.Text = minute + ":" + second;
            }));
        }

        private void MediaPlayer_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            Init = true;
        }

        private void MediaPlayerEndEvent(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            ////播放结束，循环播放本地视频
            //Task.Factory.StartNew(() => {
            //    this.vlcControl.Dispose();
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
            //        this.vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);
            //        this.vlcControl.SourceProvider.MediaPlayer.Play(new Uri("E:/20200609101808.avi"));
            //        this.vlcControl.SourceProvider.MediaPlayer.EndReached += MediaPlayerEndEvent;
            //    }));
            //});
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.timelineSlider.Width = this.ActualWidth - 60;
        }
        public static object locker = new object();
        private void timelineSlider_MouseUp(object sender, RoutedEventArgs e)
        {
            if (this.vlcControl.SourceProvider.MediaPlayer == null) return;
            lock (locker)
            {
                this.vlcControl.SourceProvider.MediaPlayer.Position = (float)(this.timelineSlider.Value / this.timelineSlider.Maximum);
            }
        }
    }
}
