using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PlayCamera
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        /// <param name="model">要序列化的实体</param>
        /// <returns>序列化后的Json字符串</returns>
        public static string SerializeObject(object model)
        {
            return JsonConvert.SerializeObject(model);
        }
        /// <summary>
        /// 把对象序列化为Json字符串
        /// </summary>
        /// <param name="model">要序列化的实体</param>
        /// <returns>序列化后的Json字符串</returns>
        public static string ToJson(this object model)
        {
            return JsonHelper.SerializeObject(model);
        }
        /// <summary>
        /// 把Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">要反序列化的对象类型</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns>反序列化后的实体对象</returns>
        public static T DeserializeObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        /// <summary>
        /// 把Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">要反序列化的对象类型</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns>反序列化后的实体对象</returns>
        public static T ToModel<T>(this string jsonString)
        {
            try
            {
                return JsonHelper.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 序列化为JObject对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static JObject Parse(this string jsonString)
        {
            return JObject.Parse(jsonString);
        }
    }

    public static class WinAPI
    {

        //参数说明：section：INI文件中的段落；key：INI文件中的关键字；val：INI文件中关键字的数值；filePath：INI文件的完整的路径和名称。
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        //参数说明：section：INI文件中的段落名称；key：INI文件中的关键字；def：无法读取时候时候的缺省数值；retVal：读取数值；size：数值的大小；filePath：INI文件的完整路径和名称。
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        public const Int32 WM_SYSCOMMAND = 274;
        public const UInt32 SC_CLOSE = 61536;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        public static void TextBox_Name_LostFocus(object sender, EventArgs e)
        {
            IntPtr TouchhWnd = new IntPtr(0);
            //TouchhWnd = FindWindow("IPTip_Main_Window", null);
            TouchhWnd = FindWindow(null, "屏幕键盘");
            if (TouchhWnd == IntPtr.Zero)
                return;
            PostMessage(TouchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        public static void TextBox_Name_GotFocus(object sender, EventArgs e)
        {
            try
            {
                dynamic file = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
                if (!System.IO.File.Exists(file))
                {
                    MessageBox.Show("打开系统键盘失败，路径:C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe");
                    return;
                }
                System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process process in processList)
                {
                    if (process.ProcessName.Contains("TabTip"))
                    {
                        process.Kill();
                    }
                }
                Process.Start(file);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }

            //打开软键盘
            //try
            //{

            //    //System.Diagnostics.Process softKey = System.Diagnostics.Process.Start("C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe");
            //    // 上面的语句在打开软键盘后，系统还没用立刻把软键盘的窗口创建出来了。所以下面的代码用循环来查询窗口是否创建，只有创建了窗口
            //    // FindWindow才能找到窗口句柄，才可以移动窗口的位置和设置窗口的大小。这里是关键。
            //    IntPtr intptr = IntPtr.Zero;
            //    intptr = FindWindow(null, "屏幕键盘");
            //    if (IntPtr.Zero == intptr)
            //    {
            //        System.Diagnostics.Process softKey = System.Diagnostics.Process.Start("C:\\Windows\\System32\\osk.exe");
            //        intptr = FindWindow("IPTip_Main_Window", null);
            //    }
            //        //IntPtr intptr = IntPtr.Zero;
            //    ////while (IntPtr.Zero == intptr)
            //    ////{
            //    ////    System.Threading.Thread.Sleep(100);
            //    ////intptr = FindWindow("IPTip_Main_Window", null);
            //    ////}
            //    //intptr = FindWindow(null, "屏幕键盘");

            //    // 获取屏幕尺寸
            //    int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
            //    int iActulaHeight = Screen.PrimaryScreen.Bounds.Height;


            //    // 设置软键盘的显示位置，底部居中
            //    int posX = (iActulaWidth - 1000) / 2;
            //    int posY = (iActulaHeight - 300);


            //    //设定键盘显示位置
            //    MoveWindow(intptr, posX, posY, 1000, 300, true);


            //    //设置软键盘到前端显示
            //    SetForegroundWindow(intptr);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
    }
}
