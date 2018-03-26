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
        public UInt32 actScriptCount { get; set; }
        public Dictionary<Act,UInt32> acts { get; set; }

        public script(string fileDirectory)
        {
            acts = new Dictionary<Act, uint>();

            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));
            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            actScriptCount = task_helper.ReadReverseUInt32(ref binReader);
            for (int i = 0; i < actScriptCount; i++)
            {
                binReader.BaseStream.Seek(i * 4 + 0x10, SeekOrigin.Begin);
                UInt32 actOffset = task_helper.ReadReverseUInt32(ref binReader);
                Act act = new Act(ref binReader, actOffset);

                acts.Add(act, actOffset);
            }

            binReader.Close();
        }

        public class Act
        {
            public UInt32 ID { get; set; }
            public UInt32 ScriptOffset { get; set; }
            public UInt32 ScriptValueOffset { get; set; }
            public UInt16 VarCount { get; set; }

            public List<Cmd> CmdList { get; set; }

            public Act(ref BinaryReader binReader, UInt32 offset)
            {
                binReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                ID = task_helper.ReadReverseUInt32(ref binReader);
                ScriptOffset = task_helper.ReadReverseUInt32(ref binReader);
                ScriptValueOffset = task_helper.ReadReverseUInt32(ref binReader);
                VarCount = task_helper.ReadReverseUInt16(ref binReader);
                binReader.BaseStream.Seek(ScriptOffset + offset, SeekOrigin.Begin);

                //Commands
                CmdList = new List<Cmd>();
                UInt32 relOffset = 0;
                while (relOffset < ScriptValueOffset)
                {
                    Cmd cmd = new Cmd(ref binReader);
                    CmdList.Add(cmd);
                    
                    relOffset = (UInt32)binReader.BaseStream.Position - offset;
                }
            }

            public class Cmd
            {
                public byte ID { get; set; }
                public byte paramCount { get; set; }
                public UInt16 Size { get; set; }

                public List<UInt32> ParamList { get; set; }

                public Cmd(ref BinaryReader binReader)
                {
                    ID = binReader.ReadByte();
                    paramCount = binReader.ReadByte();
                    Size = task_helper.ReadReverseUInt16(ref binReader);
                    ParamList = new List<UInt32>(paramCount);
                    int readBytes = 4;
                    int readParams = 0;
                    while (readBytes < Size && readParams < paramCount)
                    {
                        ParamList.Add(task_helper.ReadReverseUInt32(ref binReader));
                        readBytes += 4;
                        readParams += 1;
                    }
                }
            }
        }

        //when I get a chance overloads and parameters will be included
        public class CmdInfo
        {
            public byte ID;
            public string Name;
            public string Description;

            public CmdInfo(byte id, string name, string description)
            {
                ID = id;
                Name = name;
                Description = description;
            }
        }
        public static List<CmdInfo> CmdData = new List<CmdInfo>()
        {
            new CmdInfo(0x00, "End", ""),
            new CmdInfo(0x01, "SetVar", "Stores the value retrieved by arg2 to the variable ID specified by arg1"),
            new CmdInfo(0x02, "SetVec2D", ""),
            new CmdInfo(0x03, "SetLabel", ""),
            new CmdInfo(0x04, "Return", ""),
            new CmdInfo(0x05, "SearchLabel", ""),
            new CmdInfo(0x06, "If", ""),
            new CmdInfo(0x07, "IfNot", ""),
            new CmdInfo(0x08, "Else", ""),
            new CmdInfo(0x09, "EndIf", ""),
            new CmdInfo(0x0a, "SetStickRel", ""),
            new CmdInfo(0x0b, "SetButton", ""),
            new CmdInfo(0x0c, "VarAdd", ""),
            new CmdInfo(0x0d, "VarSub", ""),
            new CmdInfo(0x0e, "VarMul", ""),
            new CmdInfo(0x0f, "VarDiv", ""),
            new CmdInfo(0x10, "VecAdd", ""),
            new CmdInfo(0x11, "VecSub", ""),
            new CmdInfo(0x12, "VecMul", ""),
            new CmdInfo(0x13, "VecDiv", ""),
            new CmdInfo(0x14, "GoToCurrentLabel", ""),
            new CmdInfo(0x15, "SetVarRandf", ""),
            new CmdInfo(0x16, "Or", ""),
            new CmdInfo(0x17, "OrNot", ""),
            new CmdInfo(0x18, "And", ""),
            new CmdInfo(0x19, "AndNot", ""),
            new CmdInfo(0x1a, "SetFrame", ""),
            new CmdInfo(0x1b, "SetAct", ""),
            new CmdInfo(0x1c, "GoToLabel", ""),
            new CmdInfo(0x1d, "GetNearestCliffRel", ""),
            new CmdInfo(0x1e, "VarAbs", ""),
            new CmdInfo(0x1f, "SetStickAbs", ""),
            new CmdInfo(0x20, "Unk_20", ""),
            new CmdInfo(0x21, "Unk_21", ""),
            new CmdInfo(0x22, "SetWait", ""),
            new CmdInfo(0x23, "CliffCheck", ""),
            new CmdInfo(0x24, "CalcArriveFrameX", ""),
            new CmdInfo(0x25, "CalcArriveFrameY", ""),
            new CmdInfo(0x26, "GetShieldHP", ""),
            new CmdInfo(0x27, "StagePtRand", ""),
            new CmdInfo(0x28, "CalcArrivePosX", ""),
            new CmdInfo(0x29, "CalcArrivePosY", ""),
            new CmdInfo(0x2a, "AtkdDiceRoll", ""),
            new CmdInfo(0x2b, "Break", ""),
            new CmdInfo(0x2c, "Norm", ""),
            new CmdInfo(0x2d, "Dot", ""),
            new CmdInfo(0x2e, "CalcArrivePos_Sec", ""),
            new CmdInfo(0x2f, "Unk_2f", ""),
            new CmdInfo(0x30, "Unk_30", ""),
            new CmdInfo(0x31, "SetVar", ""),
            new CmdInfo(0x32, "GetNearestCliffAbs", ""),
            new CmdInfo(0x33, "ClearStick", ""),
            new CmdInfo(0x34, "Unk_34", ""),
            new CmdInfo(0x35, "Unk_35", ""),
            new CmdInfo(0x36, "Unk_36", ""),
            new CmdInfo(0x37, "Unk_37", ""),
            new CmdInfo(0x38, "Unk_38", ""),
            new CmdInfo(0x39, "Unk_39", ""),
            new CmdInfo(0x3a, "Unk_3a", ""),
            new CmdInfo(0x3b, "Unk_3b", ""),
            new CmdInfo(0x3c, "Unk_3c", ""),
        };
    }
}
