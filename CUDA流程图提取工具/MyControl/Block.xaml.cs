using System;
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

namespace CUDA流程图提取工具.MyControl
{
    /// <summary>
    /// Block.xaml 的交互逻辑
    /// </summary>
    public partial class Block : UserControl
    {
        /// <summary>
        /// block工具所显示的文字内容获取
        /// </summary>
        public string Text
        { get
            {
                return this.tb.Text;//从外部获取Block中所显示的内容
            }
            set
            {
                this.tb.Text = value;//将获取的内容显示在界面中
            }
        }

        /// <summary>
        /// block工具的自我刷新
        /// </summary>
        public Block()
        {
            InitializeComponent();
        }
    }
}
