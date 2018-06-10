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
        public string Content { get; set; }//基本块部分所包含的内容
        public int height { get; set; }//基本块所占据的高度
        public string type { get; set; }//基本块部分的类型（基本块的入口或出口）
        public string name { get; set; }//基本块部分所属基本块的名称
    }
}
