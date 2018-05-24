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
        public string Text
        { get
            {
                return this.tb.Text;
            }
            set
            {
                this.tb.Text = value;
            }
        }
        public Block()
        {
            InitializeComponent();
        }
    }
}
