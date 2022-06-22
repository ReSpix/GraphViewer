using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    class Instruments
    {
        public enum Tool
        {
            None, PointAdd, PointRemove, PointMove, LineAdd, LineRemove
        }

        public static Tool currentTool;
    }
}
