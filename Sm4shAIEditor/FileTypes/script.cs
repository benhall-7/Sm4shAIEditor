using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sm4shAIEditor
{
    class script
    {
        public UInt32 RoutineCount { get; set; }
        public List<Routine> RoutineList { get; set; }

        public script(string fileDirectory)
        {

        }

        public class Routine
        {
            UInt32 RoutineID { get; set; }
            UInt32 Unk_1 { get; set; }
            UInt32 ConstOffset { get; set; }
            UInt32 VarCount { get; set; }

            public class Command
            {
                byte CmdID { get; set; }
                byte paramCount { get; set; }
                UInt16 CmdSize { get; set; }
            }
        }
    }
}
