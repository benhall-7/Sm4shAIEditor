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
        public Dictionary<Routine,UInt32> Routines { get; set; }

        public script(string fileDirectory)
        {
            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));

            //initialization process

            binReader.Close();
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

        //is this really the right way to make such a list?
        public static Dictionary<byte, string> CmdNames = new Dictionary<byte, string>()
        {
            { 0x00, "End" },
            { 0x01, "SetVar" },
            { 0x02, "SetVec" },
            { 0x03, "SetLabel" },
            { 0x04, "Return" },
            { 0x05, "Seek" },
            { 0x06, "If" },
            { 0x07, "IfNot" },
            { 0x08, "Else" },
            { 0x09, "EndIf" },
            { 0x0a, "SetStickRel" },
            { 0x0b, "SetButton" },
            { 0x0c, "VarAdd" },
            { 0x0d, "VarSub" },
            { 0x0e, "VarMult" },
            { 0x0f, "VarDiv" },
            { 0x10, "VecAdd" },
            { 0x11, "VecSub" },
            { 0x12, "VecMult" },
            { 0x13, "VecDiv" },
            { 0x14, "Jump" },
            { 0x15, "Randf" },
            { 0x16, "Or" },
            { 0x17, "OrNot" },
            { 0x18, "And" },
            { 0x19, "AndNot" },
            { 0x1A, "SetFrame" },
            { 0x1B, "Call" },
            { 0x1C, "Goto" },
            { 0x1D, "GetNearestCliffRel" },
            { 0x1E, "Abs" },
            { 0x1F, "SetStickAbs" },
            { 0x20, "Unk_Cmd20" },
            { 0x21, "Unk_Cmd21" },
            { 0x22, "Wait" },
            { 0x23, "CliffCheck" },
            { 0x24, "EstTimePassX" },
            { 0x25, "EstTimePassY" },
            { 0x26, "EstTimeShield" },
            { 0x27, "RandStagePoint" },
            { 0x28, "EstX" },
            { 0x29, "EstY" },
            { 0x2A, "AtkDiceRoll" },
            { 0x2B, "Break" },
            { 0x2C, "Norm" },
            { 0x2D, "Dot" },
            { 0x2E, "EstPosVec_sec" },
            { 0x2F, "Unk_Cmd2F_atkd" },
            { 0x30, "Unk_Cmd30" },
            { 0x31, "GetNearestCliffAbs" },
            { 0x32, "ClearStick" },
        };
    }
}
