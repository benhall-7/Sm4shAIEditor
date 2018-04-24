using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sm4shAIEditor.Static;

/*
Some assistance with the file format is credit to Sammi Husky. 
Preliminary commands names and some if_checks are credit to Bero.
*/

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
            public UInt32 ScriptFloatOffset { get; set; }
            public UInt16 VarCount { get; set; }
            public Dictionary<UInt32, float> ScriptFloats { get; set; }

            public List<Cmd> CmdList { get; set; }

            public Act(ref BinaryReader binReader, UInt32 actPosition)
            {
                binReader.BaseStream.Seek(actPosition, SeekOrigin.Begin);
                ID = task_helper.ReadReverseUInt32(ref binReader);
                ScriptOffset = task_helper.ReadReverseUInt32(ref binReader);
                ScriptFloatOffset = task_helper.ReadReverseUInt32(ref binReader);
                VarCount = task_helper.ReadReverseUInt16(ref binReader);
                binReader.BaseStream.Seek(ScriptOffset + actPosition, SeekOrigin.Begin);

                ScriptFloats = new Dictionary<uint, float>();

                //Commands
                CmdList = new List<Cmd>();
                UInt32 relOffset = ScriptOffset;
                while (relOffset < ScriptFloatOffset)
                {
                    Cmd cmd = new Cmd(ref binReader);
                    CmdList.Add(cmd);

                    //add values to the script float list
                    foreach (UInt32 cmdParam in cmd.ParamList)
                    {
                        if (cmdParam >= 0x2000 &&
                            cmdParam < 0x3000 &&
                            !ScriptFloats.ContainsKey(cmdParam) &&
                            cmd.ID != 0x1b)
                        {
                            ScriptFloats.Add(cmdParam, GetScriptFloat(ref binReader, cmdParam, actPosition, ScriptFloatOffset));
                        }
                    }

                    relOffset += cmd.Size;
                }
            }

            protected float GetScriptFloat(ref BinaryReader binReader, UInt32 cmdParam, UInt32 actPosition, UInt32 floatOffset)
            {
                float scriptFloat;

                Int32 binPosition = (Int32)binReader.BaseStream.Position;
                if (cmdParam < 0x2000)
                    throw new Exception("The command parameter is invalid");

                cmdParam -= 0x2000;
                binReader.BaseStream.Seek(actPosition + floatOffset + cmdParam * 4, SeekOrigin.Begin);
                scriptFloat = task_helper.ReadReverseFloat(ref binReader);
                binReader.BaseStream.Seek(binPosition, SeekOrigin.Begin);
                return scriptFloat;
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
                    int readParams = 0;
                    while (readParams < paramCount)
                    {
                        ParamList.Add(task_helper.ReadReverseUInt32(ref binReader));
                        readParams += 1;
                    }
                }
            }

            public string get_script_value(UInt32 paramID)
            {
                if (paramID < 0x1000)
                    return "var" + paramID;
                if (paramID >= 0x2000 && ScriptFloats.ContainsKey(paramID))
                    return ScriptFloats[paramID].ToString();
                else
                {
                    string value;
                    if (script_data.script_value_special1.ContainsKey(paramID))
                        value = script_data.script_value_special1[paramID];
                    else
                        value = "0x" + paramID.ToString("X4");

                    return value;
                }
            }

            public string get_if_chk(UInt32[] cmdParams)
            {
                UInt32 reqID = cmdParams[0];
                string requirement = "";
                switch (reqID)
                {
                    case 0x1002://Checks if execution frame is greater than provided argument
                        requirement = "TimerPassed " + get_script_value(cmdParams[1]);
                        break;
                    case 0x1005:
                        requirement = "ai_aerial";
                        break;
                    case 0x1007://Greater than
                        requirement = get_script_value(cmdParams[1]) + " > " + get_script_value(cmdParams[2]);
                        break;
                    case 0x1008://Less than
                        requirement = get_script_value(cmdParams[1]) + " < " + get_script_value(cmdParams[2]);
                        break;
                    case 0x1009://Greater than or Equal
                        requirement = get_script_value(cmdParams[1]) + " >= " + get_script_value(cmdParams[2]);
                        break;
                    case 0x100a://Less than or Equal
                        requirement = get_script_value(cmdParams[1]) + " <= " + get_script_value(cmdParams[2]);
                        break;
                    case 0x100d:
                        requirement = "ai_off_stage";
                        break;
                    case 0x100f://action
                        requirement = "ai_action == " + "0x" + cmdParams[1].ToString("X");
                        break;
                    case 0x101b:
                        requirement = "tgt_aerial";
                        break;
                    case 0x101E://ai character
                        requirement = "ai_char " + script_data.fighters[(int)cmdParams[1]];
                        break;
                    case 0x101F://target character
                        requirement = "tgt_char " + script_data.fighters[(int)cmdParams[1]];
                        break;
                    case 0x1024://subaction
                        requirement = "ai_subaction == " + "0x" + cmdParams[1].ToString("X");
                        break;
                    default:
                        for (int i = 0; i < cmdParams.Length; i++)
                        {
                            //NOTE:
                            //Don't use the "get_script_value" because normal integers can be used as arguments for some checks
                            if (ScriptFloats.ContainsKey(cmdParams[i]))
                                requirement += ScriptFloats[cmdParams[i]];
                            else
                                requirement += "0x" + cmdParams[i].ToString("X");

                            if (i != cmdParams.Length - 1)
                                requirement += ", ";
                        }
                        break;
                }
                return requirement;
            }
        }//end of Act class
    }//end of Script class
}
