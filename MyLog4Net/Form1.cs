using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace MyLog4Net
{
    //添加配置文件
    public partial class Form1 : Form
    {
        //config中没有的logger,根据root初始化
        ILog tmplog = LogManager.GetLogger("tmplog");

        //按config中的mylog定义初始化
        ILog mylog = LogManager.GetLogger("mylog");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF

            try
            {
                //level warn
                tmplog.Warn("tmp warn");
                //tmplog.Info("tmp info");//×
                //tmplog.Debug("tmp debug");//×

                ////level debug
                //mylog.Warn("my warn");
                //mylog.Info("my info");
                //mylog.Debug("my debug");//×

                //Convert.ToInt32("xx");//throw
            }
            catch (Exception ex)
            {
                tmplog.Fatal("tmp fatal", ex);
                mylog.Fatal("my fatal", ex);
            }
        }
    }
}
