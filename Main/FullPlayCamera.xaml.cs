using System;
using System.Collections.Generic;
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

namespace PlayCamera
{
    /// <summary>
    /// FullPlayCamera.xaml 的交互逻辑
    /// </summary>
    public partial class FullPlayCamera : UserControl
    {
        private static FullPlayCamera _instance = null;
        private static readonly object syncRoot = new object();

        public static FullPlayCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new FullPlayCamera();
                        }
                    }
                }
                return _instance;
            }
        }

        public delegate void CanelFullScreenHandler();

        public event CanelFullScreenHandler CanelFullScreenEvent;
        public FullPlayCamera()
        {
            InitializeComponent();
        }

        private void CancelFullImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanelFullScreenEvent != null)
            {
                CanelFullScreenEvent();
            }
        }
    }
}
