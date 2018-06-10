﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Security.Permissions;

namespace CUDA流程图提取工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string cubinpath = "";
        string output = "";
        List<BlockData> BlockDatas = new List<BlockData>();
        int BlockId = 0;
        int arcCount = 0;
        string[] CFGresult = new string[1000];
        string[] CFGtype = new string[1000];
        int blocknum = 0;
        int[] Whichblock = new int[1000];
        string[] Blockname = new string[1000];

        /// <summary>
        /// page update/页面更新
        /// </summary>
        public MainWindow()
        {
            if (ConfigurationManager.AppSettings["BGI"] != null&& ConfigurationManager.AppSettings["BGI"] != "")
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(ConfigurationManager.AppSettings["BGI"], UriKind.Absolute));
                imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式  
                this.Background = imageBrush;
            }
            else
            {
                this.Background = Brushes.White;
            }
            InitializeComponent();
        }

        /// <summary>
        /// 打开CUDA程序和C程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FILE_OPEN(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"CUDA程序|*.cu|C/C++程序|*.cpp";
            if (ofd.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);
                string str = sr.ReadToEnd();
                STRVision.Text = str;
                sr.Close();
                cubinpath = ofd.FileName;
            }
            
        }

        /// <summary>
        /// 打开CUBIN文件（打开之后的处理稍有不同）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OPEN_CUBIN(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"CUBIN文件|*.cubin";
            if (ofd.ShowDialog() == true)
            {
                cubinpath = ofd.FileName;
                STRVision.Text = cubinpath;
            }
        }

        /// <summary>
        /// 保存CFG（使用graphviz，未安装的情况下不能保存）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SAVE_PICTURE(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = @"png图像|*.png";
                if (sfd.ShowDialog() == true)
                {
                    output = sfd.FileName;
                    string commandstr = "nvdisasm";
                    commandstr += " ";
                    commandstr += "-cfg";
                    commandstr += " ";
                    commandstr += cubinpath;
                    commandstr += " | ";
                    commandstr += "dot";
                    commandstr += " -o ";
                    commandstr += output;
                    commandstr += " -Tpng";
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                    p.Start();//启动程序
                    p.StandardInput.WriteLine(commandstr + "&exit");
                    p.StandardInput.AutoFlush = true;
                    p.WaitForExit();
                    p.Close();
                }
            }
            catch(Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show("Have you ever opened a .cubin file?", "ERROR!!", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EXIT(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// CUDA程序的编译（需要正确安装和配置NVCC）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void COMPILE_CUDA(object sender, RoutedEventArgs e)
        {
            try
            {
                string commandstr = "nvcc";
                commandstr += " ";
                commandstr += "-ccbin";
                commandstr += " ";
                commandstr += "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\VC\\Tools\\MSVC\\14.14.26428\\bin\\Hostx64\\ x86\\cl.exe\"";
                commandstr += " ";
                commandstr += "-cubin";
                commandstr += " ";
                commandstr += cubinpath;
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序
                p.StandardInput.WriteLine(commandstr + "&exit");
                p.StandardInput.AutoFlush = true;
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                STRVision.Text = output;
            }
            catch (Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show("Incorrect File!!!", "ERROR!!", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// CUBIN程序的反编译（需要正确安装配置nvdisasm）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void COMPILE(object sender, RoutedEventArgs e)
        {
            try
            {
                string commandstr = "nvdisasm";
                commandstr += " ";
                commandstr += "-cfg";
                commandstr += " ";
                commandstr += cubinpath;
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序
                p.StandardInput.WriteLine(commandstr + "&exit");
                p.StandardInput.AutoFlush = true;
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                output = output.Replace("&exit", "^");
                output = output.Substring(output.IndexOf('^') + 1);
                STRVision.Text = output;
            }
            catch (Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show(".cubin file unopened or opened incorrect file!!", "ERROR!!", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// CFG的绘图程序，前提是要成功反编译cubin文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DRAW_CFG(object sender, RoutedEventArgs e)
        {
            try
            {
                output = output.Replace(@"\l", "\r");
                output = output.Replace("\\", "\t");
                output = output.Substring(output.IndexOf("];") + 2);
                char[] OP = output.ToCharArray();
                for (int i = 0; i < OP.Length; i++)
                {
                    if (i == 0)
                    {
                        List<char> LST = OP.ToList();
                        LST.RemoveAt(i);
                        OP = LST.ToArray();
                    }
                    if (OP[i] == 'n' && OP[i + 1] == 'o' && OP[i + 2] == 'd' && OP[i + 3] == 'e')
                    {
                        List<char> LST = OP.ToList();
                        for (int j = i; OP[j - 3] != ';'; j++)
                        {
                            LST.RemoveAt(i);
                        }
                        OP = LST.ToArray();
                    }
                    if (OP[i] == '[' && OP[i + 1] == 's' && OP[i + 2] == 't')
                    {
                        List<char> LST = OP.ToList();
                        for (int j = i; OP[j - 2] != ';'; j++)
                        {
                            LST.RemoveAt(i);
                        }
                        OP = LST.ToArray();
                    }
                }
                output = new string(OP);
                output = output.Replace("label=", "");
                output = output.Replace("{", "");
                output = output.Replace("}", "");
                output = output.Replace("[", "");
                output = output.Replace("]", "");
                output = output.Replace("|", "");
                OP = output.ToCharArray();
                for (int i = 0; i < OP.Length; i++)
                {
                    if (i <= 1 && OP[i] == '\"' && OP[i + 1] == '.' && OP[i - 1] == '\n')
                    {
                        i++;
                        Blockname[blocknum] += OP[i];
                        i++;
                        for (; OP[i] != '\"'; i++)
                        {
                            Blockname[blocknum] += OP[i];
                        }
                        blocknum++;
                    }
                    else if (OP[i] == '\"' && OP[i + 1] == '.'&&OP[i-1]=='\n'&&(OP[i-2]!='\r'|| i-2<=0))
                    {
                        i++;
                        Blockname[blocknum] += OP[i];
                        i++;
                        for (; OP[i] != '\"'; i++)
                        {
                            Blockname[blocknum] += OP[i];
                        }
                        blocknum++;
                    }
                }
                blocknum = 0;
                output = new string(OP);
                output = output.Replace("\"", "");
                OP = output.ToCharArray();
                List<char> OPList = OP.ToList();
                OPList.RemoveAt(0);
                int[] arcCache = new int[200];
                int[] arcCache2 = new int[200];
                for (int i = 2; i < 8; i++)
                {
                    OPList.RemoveAt(OP.Length - i);
                }
                OP = OPList.ToArray();
                for (int i = 0; i < OP.Length; i++)
                {
                    if ((OP[i] == '_' || OP[i] == '1') && OP[i + 1] == '<')
                    {
                        for (int j = 0; j != i + 1 && OP[j] != ';'; j++)
                        {
                            CFGresult[BlockId] += OP[j];
                        }
                        CFGtype[BlockId] = "Name";
                        BlockId++;
                    }
                    else if (OP[i] == '>' && OP[i - 5] == 'e' && OP[i - 4] == 'n' && OP[i - 3] == 't' && OP[i - 2] == 'r' && OP[i - 1] == 'y')
                    {
                        CFGresult[BlockId] += "<entry>\r";
                        i++;
                        for (; OP[i] != '<' || (OP[i - 2] != ';' && OP[i - 2] != ':'); i++)
                        {
                            CFGresult[BlockId] += OP[i];
                        }
                        CFGtype[BlockId] = "entry";
                        CreateBlock(CFGresult[BlockId], CFGtype[BlockId] = "entry", Blockname[blocknum]);
                        BlockId++;
                    }
                    else if (OP[i] == '>' && OP[i - 5] == 'e' && OP[i - 4] == 'x' && OP[i - 3] == 'i' && OP[i - 2] == 't' && OP[i - 1] == '0')
                    {
                        CFGresult[BlockId] += "<exit0>\r";
                        i++;
                        for (; OP[i] != ';' || OP[i + 1] != '\r' || OP[i + 2] != '\r'; i++)
                        {
                            CFGresult[BlockId] += OP[i];
                        }
                        CFGtype[BlockId] = "exit0";
                        CreateBlock(CFGresult[BlockId], CFGtype[BlockId] = "exit0", Blockname[blocknum]);
                        BlockId++;
                        blocknum++;
                    }
                }

                for (int i = 0; i < OP.Length; i++)
                {
                    if (OP[i] == '-' && OP[i + 1] == '>')
                    {
                        for (; OP[i] != '.' || OP[i - 1] != '\n'; i--) ;
                        string match = "";
                        for (; OP[i] != ':'; i++)
                        {
                            match += OP[i];
                        }
                        for (int j = 0; j < Blockname.Length; j++)
                        {
                            if (match == Blockname[j])
                            {
                                match = "";
                                for (i++; OP[i] != ':'; i++)
                                {
                                    match += OP[i];
                                }
                                for (int k = 0; k < BlockId; k++)
                                {
                                    if (match == CFGtype[k])
                                    {
                                        for (int l = 0; l < BlockDatas.Count; l++)
                                        {
                                            if (Blockname[j] == BlockDatas[l].name && CFGtype[k] == BlockDatas[l].type)
                                            {
                                                arcCache[arcCount] = l;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        match = "";
                        for (; OP[i] != '.'; i++) ;
                        for (; OP[i] != ':'; i++)
                        {
                            match += OP[i];
                        }
                        for (int j = 0; j < Blockname.Length; j++)
                        {
                            if (match == Blockname[j])
                            {
                                match = "";
                                for (i++; OP[i] != ':'; i++)
                                {
                                    match += OP[i];
                                }
                                for (int k = 0; k < BlockId; k++)
                                {
                                    if (match == CFGtype[k])
                                    {
                                        for (int l = 0; l < BlockDatas.Count; l++)
                                        {
                                            if (Blockname[j] == BlockDatas[l].name && CFGtype[k] == BlockDatas[l].type)
                                            {
                                                arcCache2[arcCount] = l;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        arcCount++;
                    }
                }
                output = new string(OP);
                STRVision.Text = output;
                int margin = 0;
                int margin2 = 0;
                for (int i = 0; i < BlockId; i++)
                {
                    if (i == 0)
                    {
                        Drawblock(BlockDatas[i].Content, 0);
                    }
                    else
                    {
                        margin = 0;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            margin += BlockDatas[j].height;
                        }
                        Drawblock(BlockDatas[i].Content, margin);
                        SP.Height = margin + 200;
                    }
                }
                for (int i = 1; i < BlockId; i++)
                {
                    for (int j = 0; j < arcCache.Length; j++)
                    {
                        margin = 0;
                        margin2 = 0;
                        if (i == arcCache[j])
                        {
                            for (int k=i-1; k >= 0; k--)
                            {
                                margin += BlockDatas[k].height;
                            }
                            for (int k = arcCache2[j]-1; k >= 0; k--)
                            {
                                margin2 += BlockDatas[k].height;
                            }
                            Drawline(margin, margin2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show("you have NOT reverse complie the .cubin file!!", "ERROR!!", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 创立一个新的block，用于绘制该block到canvas中
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        private void CreateBlock(string text,string type,string blockname)
        {
            BlockDatas.Add(new BlockData { Content = text, height = MarginCal(text), type = type, name = blockname });
        }

        /// <summary>
        /// 绘制一个新的block，前提是要先创立该block
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Margin_Top"></param>
        private void Drawblock(string text, int Margin_Top)
        {
            var block = new MyControl.Block { Text = text };
            Thickness TN = new Thickness();
            TN.Top = Margin_Top;
            block.HorizontalAlignment = HorizontalAlignment.Center;
            block.VerticalAlignment = VerticalAlignment.Top;
            block.Width = 700;
            block.Margin = TN;
            this.SP.Children.Add(block);
        }

        /// <summary>
        /// 计算每一个block所需要的位移（要不然会重叠到一起去）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int MarginCal(string text)
        {
            int MarginReturn = 1;
            foreach (char Enter in text)
            {
                if (Enter == '\r') 
                {
                    MarginReturn++;
                }
            }
            MarginReturn *= 22;
            MarginReturn += 44;
            return MarginReturn;
        }

        /// <summary>
        /// 绘制CFG中需要的弧线
        /// </summary>
        /// <param name="margin"></param>
        /// <param name="num"></param>
        private void Drawline(int margin, int margin2)
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            PathGeometry PG = new PathGeometry();
            ArcSegment arc = new ArcSegment(new Point(700, margin+22), new Size(20,80), 0, true, SweepDirection.Clockwise, true);
            PathFigure PF = new PathFigure();
            PF.StartPoint = new Point(700, margin2+22);
            PF.Segments.Add(arc);
            PG.Figures.Add(PF);
            path.Data = PG;
            path.Stroke = Brushes.Black;
            SP.Children.Add(path);
            DrawStraight(margin2);
        }

        private void DrawStraight(int margin)
        {
            MyControl.Arrow A= new MyControl.Arrow();
            Thickness T = new Thickness();
            T.Top = margin+10;
            T.Left = 690;
            A.HorizontalAlignment = HorizontalAlignment.Center;
            A.VerticalAlignment = VerticalAlignment.Center;
            A.Margin = T;
            SP.Children.Add(A);
        }

        /// <summary>
        /// 改变UI的背景（支持jpg,png,bmp格式文件，画面比例为16：9）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGI_CHANGE(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"图片文件|*.jpg;*.png;*.bmp";
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式  
                this.Background = imageBrush;
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["BGI"].Value = ofd.FileName;
                cfa.Save();
            }
        }

        private void BGI_ERASE(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.White;
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["BGI"].Value = "";
            cfa.Save();
        }
        /// <summary>
        /// 关于（即便里面made by后面有英文和日文，作者就是我老王哒！）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ABOUT(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("made by こしま/Takakaze", "ABOUT", MessageBoxButton.OK);
        }


    }
}
