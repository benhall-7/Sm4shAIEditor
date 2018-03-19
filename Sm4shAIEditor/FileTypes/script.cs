using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            public UInt32 RoutineID { get; set; }
            public UInt32 Unk_1 { get; set; }
            public UInt32 ConstOffset { get; set; }
            public UInt32 VarCount { get; set; }

            public List<Command> CommandList { get; set; }
            public List<float> ConstantList { get; set; }

            public class Command
            {
                byte CmdID { get; set; }
                byte paramCount { get; set; }
                UInt16 CmdSize { get; set; }

                public List<UInt32> ParamList { get; set; }
            }
        }

        public static Dictionary<byte, string> CmdNames = new Dictionary<byte, string>()
        {
            { 0x00, "End" },
            { 0x02, "SetVar" },
            { 0x03, "SetLabel" },
            { 0x04, "Return" },
            { 0x06, "If" },
            { 0x09, "EndIf" },
            { 0x0a, "SetStick" },
            { 0x0b, "SetButton" },
            { 0x0e, "VarMult" },
            { 0x18, "And" },
            { 0x19, "AndNot" },
            { 0x22, "Wait" },
        };
    }
}
