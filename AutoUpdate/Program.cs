using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("正在检查更新...");

            var helper = new Helper();

            //获取客户端指定路径下的应用程序最近一次更新时间 
            string thePreUpdateDate = helper.GetTheLastUpdateTime(Environment.CurrentDirectory, "AutoUpdater.xml");
            //获得从服务器端已下载文档的最近一次更新日期 
            string theLastsUpdateDate = helper.GetTheLastUpdateTime(Environment.CurrentDirectory, "ServerUpdater.xml");

            if (thePreUpdateDate != "")
            {
                //如果客户端将升级的应用程序的更新日期大于服务器端升级的应用程序的更新日期 
                if (Convert.ToDateTime(thePreUpdateDate) >= Convert.ToDateTime(theLastsUpdateDate))
                {
                    Console.WriteLine("当前软件已经是最新的，无需更新！");
                }
                else
                {
                    Console.WriteLine("正在获取更新列表...");

                    //通过动态数组获取下载文件的列表 
                    ArrayList List = helper.GetDownFileList(Environment.CurrentDirectory, "ServerUpdater.xml");
                    string[] urls = new string[List.Count];
                    List.CopyTo(urls, 0);

                    //更新文件
                    if (helper.UpdateFile(Environment.CurrentDirectory, "TempUpdate",urls))
                    {
                        Console.WriteLine("更新完成。");
                    }
                    else
                    {
                        Console.WriteLine("更新失败。");
                    }
                }
            }


            Console.ReadKey(true);
        }
    }

    class Helper
    {
        public string GetTheLastUpdateTime(string Dir, string FireName)
        {
            string LastUpdateTime = "";
            string AutoUpdaterFileName = Dir + "\\" + FireName;
            if (!File.Exists(AutoUpdaterFileName))
                return LastUpdateTime;
            //打开xml文件 
            FileStream myFile = new FileStream(AutoUpdaterFileName, FileMode.Open);
            //xml文件阅读器 
            XmlTextReader xml = new XmlTextReader(myFile);
            while (xml.Read())
            {
                if (xml.Name == "UpdateTime")
                {
                    //获取升级文档的最后一次更新日期 
                    LastUpdateTime = xml.GetAttribute("Date");
                    break;
                }
            }
            xml.Close();
            myFile.Close();
            return LastUpdateTime;
        }

        public ArrayList GetDownFileList(string Dir, string FireName)
        {
            string AutoUpdaterFileName = Dir + "\\" + FireName;
            if (!File.Exists(AutoUpdaterFileName))
                return null;

            //打开xml文件 
            FileStream myFile = new FileStream(AutoUpdaterFileName, FileMode.Open);
            //xml文件阅读器 
            XmlTextReader xml = new XmlTextReader(myFile);

            ArrayList list = new ArrayList();

            while (xml.Read())
            {
                if (xml.Name == "UpdateFile")
                {
                    //获取升级文档的最后一次更新日期 
                    var temp = xml.GetAttribute("FileName");
                    Console.WriteLine(temp);
                    list.Add(temp);
                }
            }
            xml.Close();
            myFile.Close();
            return list;
        }

        public bool UpdateFile(string Dir,string FolderName,string[]urls)
        {
            //关闭原有的应用程序 
            //Console.WriteLine("正在关闭程序....");
            //System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("TIMS");
            //关闭原有应用程序的所有进程 
            //foreach (System.Diagnostics.Process pro in proc)
            //{
            //    pro.Kill();
            //}

            //判断程序是否运行
            if (System.Diagnostics.Process.GetProcessesByName("要获取的程序在进程中的名称").ToList().Count > 0)
            {
                Console.WriteLine("程序正在运行，请先关闭程序！");
                return false;
            }
            Console.WriteLine("准备下载文件...");

            string path = Dir + "\\" + FolderName;

            //清除文件夹下的内容
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            //下载文件并存入临时文件夹
            foreach (string url in urls)
            {
                Console.WriteLine("Downloading:" + url);
            }

            DirectoryInfo theFolder = new DirectoryInfo(path);
            //如果临时文件夹下存在与应用程序所在目录下的文件同名的文件，则删除应用程序目录下的文件 
            foreach (FileInfo theFile in theFolder.GetFiles())
            {
                //if (File.Exists(Application.StartupPath + \\"+Path.GetFileName(theFile.FullName))) 

                //File.Delete(Application.StartupPath + "\\" + Path.GetFileName(theFile.FullName));
                ////将临时文件夹的文件移到应用程序所在的目录下 
                //File.Move(theFile.FullName, Application.StartupPath + \\"+Path.GetFileName(theFile.FullName)); 
                //}
            }

            //启动安装程序
            Console.WriteLine("正在重新启动程序....");
            //System.Diagnostics.Process.Start(Application.StartupPath + "\\" + "TIMS.exe");
            //this.Close();
            return true;
        }
    }
}
