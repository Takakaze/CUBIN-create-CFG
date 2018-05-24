using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDA流程图提取工具
{
    /// <summary>
    /// BlockData类的用处是存放和设定所有构成CFG的TEXTBLOCK的所有属性
    /// </summary>
    public class BlockData
    {
        public string Content { get; set; }
        public int height { get; set; }
        public string type { get; set; }
    }
}
