using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sm4shAIEditor.FileTypes;

namespace Sm4shAIEditor
{
    class script
    {
        public UInt32 RoutineCount { get; set; }
        public Dictionary<Routine,UInt32> Routines { get; set; }

        public script(string fileDirectory)
        {
            Routines = new Dictionary<Routine, uint>();

            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));

            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            RoutineCount = task_helper.ReadReverseUInt32(ref binReader);
            //fill the Routines
            for (int i = 0; i < RoutineCount; i++)
            {
                binReader.BaseStream.Seek(i * 4 + 0x10, SeekOrigin.Begin);
                UInt32 routineOffset = task_helper.ReadReverseUInt32(ref binReader);
                Routine routine = new Routine(ref binReader, routineOffset);

                Routines.Add(routine, routineOffset);
            }

            binReader.Close();
        }

        public class Routine
        {
            public UInt32 RoutineID { get; set; }
            public UInt32 Unk_1 { get; set; }
            public UInt32 ConstOffset { get; set; }
            public UInt16 VarCount { get; set; }

            public List<Command> CommandList { get; set; }

            public Routine(ref BinaryReader binReader, UInt32 offset)
            {
                binReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                RoutineID = task_helper.ReadReverseUInt32(ref binReader);
                Unk_1 = task_helper.ReadReverseUInt32(ref binReader);
                ConstOffset = task_helper.ReadReverseUInt32(ref binReader);
                VarCount = task_helper.ReadReverseUInt16(ref binReader);
                binReader.ReadInt16();//padding

                //Commands
                CommandList = new List<Command>();
                int relOffset = 0;
                while (relOffset < ConstOffset)
                {
                    Command cmd = new Command(ref binReader);
                    CommandList.Add(cmd);
                    
                    relOffset = (int)binReader.BaseStream.Position - (int)offset;
                }
            }

            public class Command
            {
                public byte CmdID { get; set; }
                public byte paramCount { get; set; }
                public UInt16 CmdSize { get; set; }

                public List<UInt32> ParamList { get; set; }

                public Command(ref BinaryReader binReader)
                {
                    CmdID = binReader.ReadByte();
                    paramCount = binReader.ReadByte();
                    CmdSize = task_helper.ReadReverseUInt16(ref binReader);
                    ParamList = new List<UInt32>(paramCount);
                    int readBytes = 4;
                    int readParams = 0;
                    while (readBytes < CmdSize && readParams < paramCount)
                    {
                        ParamList.Add(task_helper.ReadReverseUInt32(ref binReader));
                        readBytes += 4;
                        readParams += 1;
                    }
                }
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
            { 0x33, "Unk_Cmd33" },
            { 0x34, "Unk_Cmd34" },
            { 0x35, "Unk_Cmd35" },
            { 0x36, "Unk_Cmd36" },
            { 0x37, "Unk_Cmd37" },
            { 0x38, "Unk_Cmd38" },
            { 0x39, "Unk_Cmd39" },
            { 0x3A, "Unk_Cmd3A" },
            { 0x3B, "Unk_Cmd3B" },
            { 0x3C, "Unk_Cmd3C" },//I don't know exactly how many commands there are, but there are at least 52
        };
    }
}
