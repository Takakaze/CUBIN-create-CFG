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
    /// Arrow.xaml 的交互逻辑
    /// </summary>
    public partial class Arrow : UserControl
    {
        public Arrow()
        {
            InitializeComponent();
        }

        public static implicit operator RotateTransform(Arrow v)
        {
            throw new NotImplementedException();
        }
    }
}
