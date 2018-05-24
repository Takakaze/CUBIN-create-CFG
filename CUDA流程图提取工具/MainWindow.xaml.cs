using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
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
        string[] CFGresult = new string[1000];
        string[] CFGtype = new string[1000];

        public MainWindow()
        {
            InitializeComponent();
        }

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
                    commandstr += " -o";
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

        private void EXIT(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void COMPILE_CUDA(object sender, RoutedEventArgs e)
        {
            try
            {
                string commandstr = "nvcc";
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
                output = output.Replace("&exit", "^");
                output = output.Substring(output.IndexOf('^') + 1);
                STRVision.Text = output;
            }
            catch (Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show("Incorrect File!!!", "ERROR!!", MessageBoxButton.OK);
            }
        }

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
                output = output.Replace("\"", "");
                output = output.Replace("[", "");
                output = output.Replace("]", "");
                output = output.Replace("|", "");
                OP = output.ToCharArray();
                List<char> OPList = OP.ToList();
                OPList.RemoveAt(0);
                string[] arcCache = { "", "", "", "", "", "" };
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
                        BlockId++;
                    }
                    else if (OP[i] == '-' && OP[i + 1] == '>')
                    {
                        for (int j = i; (OP[j] != 'e' || OP[j + 1] != 'x' || OP[j + 2] != 'i' || OP[j + 3] != 't' || OP[j + 4] != '0') && (OP[j] != 'e' || OP[j + 1] != 'n' || OP[j + 2] != 't' || OP[j + 3] != 'r' || OP[j + 4] != 'y'); j--)
                        {
                            if (OP[j + 1] == 'x')
                            {
                                arcCache[BlockId - 1] = "exit0";
                                arcCache[BlockId - 2] = "entry";
                            }
                            else
                            {
                                arcCache[BlockId - 1] = "entry";
                                arcCache[BlockId - 2] = "exit0";
                            }
                        }
                    }
                }
                output = new string(OP);
                STRVision.Text = output;
                int margin = 0;
                for (int i = 0; i < BlockId; i++)
                {
                    CreateBlock(CFGresult[i], CFGtype[i]);
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
                    margin = 0;
                    if (arcCache[i] == BlockDatas[i - 1].type && arcCache[i - 1] == BlockDatas[i].type)
                    {
                        for (int j = i - 2; j >= 0; j--)
                        {
                            margin += BlockDatas[j].height;
                        }
                        Drawline(margin, i);
                    }
                }
            }
            catch (Exception ex)
            {
                STRVision.Text = ex.ToString();
                MessageBox.Show("you have NOT reverse complie the .cubin file!!", "ERROR!!", MessageBoxButton.OK);
            }
        }

        private void CreateBlock(string text,string type)
        {
            BlockDatas.Add(new BlockData {Content = text,height = MarginCal(text),type = type });
        }

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

        private void Drawline(int margin, int num)
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            PathGeometry PG = new PathGeometry();
            ArcSegment arc = new ArcSegment(new Point(700, margin+22), new Size(20, 50), 0, false, SweepDirection.Counterclockwise, true);
            PathFigure PF = new PathFigure();
            PF.StartPoint = new Point(700, margin + BlockDatas[num - 1].height+22);
            PF.Segments.Add(arc);
            PG.Figures.Add(PF);
            path.Data = PG;
            path.Stroke = Brushes.Black;
            SP.Children.Add(path);
        }

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
            }
        }

        private void ABOUT(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("made by こしま/Takakaze", "ABOUT", MessageBoxButton.OK);
        }


    }
}
