using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SDK_HANDLE = System.Int32;

namespace HBGKTest.YiTongCamera
{
    public partial class PlayBackForm : Form
    {
        int m_nLocalplayHandle = 0;
        System.Timers.Timer timerLocalPlayBack = new System.Timers.Timer(200);
        NetSDK.fLocalPlayFileCallBack fileEndCallBack;
        public PlayBackForm(string file)
        {
            InitializeComponent();
            trackBarLocalPlayPos.SetRange(0, 1000);
            PlayRecord(file);
        }

        private void PlayRecord(string file)
        {
            String fileName = file;
            // 使用文件名
            m_nLocalplayHandle = Convert.ToInt32(NetSDK.H264_DVR_StartLocalPlay(fileName, pictureBoxVideoWnd.Handle, null, Convert.ToUInt32(0)));
            if (m_nLocalplayHandle > 0)
            {
                //   MessageBox.Show("success");
                timerLocalPlayBack.Elapsed += new System.Timers.ElapsedEventHandler(recordTime);
                timerLocalPlayBack.AutoReset = true;
                timerLocalPlayBack.Enabled = true;
                //fileEndCallBack = new NetSDK.fLocalPlayFileCallBack(FileEndCallBack);
                //NetSDK.H264_DVR_SetFileEndCallBack(m_nLocalplayHandle, fileEndCallBack, this.Handle);
            }
            else
            {
                MessageBox.Show("failed");
            }
        }

        public void recordTime(object source, System.Timers.ElapsedEventArgs e)
        {
            float pos = NetSDK.H264_DVR_GetPlayPos(m_nLocalplayHandle);
            trackBarLocalPlayPos.Value = Convert.ToInt32(pos * 1000);
            if (trackBarLocalPlayPos.Value >995)
            {
                if (NetSDK.H264_DVR_StopLocalPlay(m_nLocalplayHandle))
                {
                    m_nLocalplayHandle = 0;
                }
                timerLocalPlayBack.Enabled = false;
                trackBarLocalPlayPos.Value = 0;
            }
        }

        void FileEndCallBack(SDK_HANDLE lPlayHand, uint nUser)
        {
            if (NetSDK.H264_DVR_StopLocalPlay(m_nLocalplayHandle))
            {
                m_nLocalplayHandle = 0;
            }
            timerLocalPlayBack.Enabled = false;
            trackBarLocalPlayPos.Value = 0;
        }

        private void trackBarLocalPlayPos_Scroll(object sender, EventArgs e)
        {
            int value = trackBarLocalPlayPos.Value;
            NetSDK.H264_DVR_SetPlayPos(m_nLocalplayHandle, (float)(value / 1000.0));
        }
    }
}
