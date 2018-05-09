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
                string test = "";
            }
            //larger half of compilation logic
            public Act(UInt32 ID, string text)
            {
                int byteOffset = 0;
                this.ID = ID;
                CustomStringReader sReader = new CustomStringReader(text);
                CustomStringReader lReader = new CustomStringReader("");
                
                while (!sReader.EndString)
                {
                    string str = sReader.ReadLine();
                    if (str != null)
                    {
                        lReader.CharArray = str.ToCharArray();
                        string word = lReader.ReadWord();
                        if (script_data.CmdNames.Contains(word))
                        {
                            Int32 index = script_data.CmdNames.IndexOf(word);
                        }
                        else
                        {

                        }
                    }
                }
            }

            protected float GetScriptFloat(ref BinaryReader binReader, UInt32 cmdParam, UInt32 actPosition, UInt32 floatOffset)
            {
                float scriptFloat;
                Int32 binPosition = (Int32)binReader.BaseStream.Position;
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

            public string decomp_act()
            {
                string text = "";
                byte lastCmdID = 0xff;
                int ifNestLevel = 0;
                byte relID = 0xFF;
                for (int cmdIndex = 0; cmdIndex < CmdList.Count; cmdIndex++)
                {
                    script.Act.Cmd cmd = CmdList[cmdIndex];

                    //control the nested level spaces
                    string ifPadding = "";
                    if (cmd.ID == 8 || cmd.ID == 9)
                        ifNestLevel--;
                    for (int i = 0; i < ifNestLevel; i++)
                    {
                        ifPadding += "\t";
                    }
                    //account for the "else if" statement, which messes up the nest level
                    if (((cmd.ID == 6 || cmd.ID == 7) && lastCmdID != 8) || cmd.ID == 8)
                        ifNestLevel++;

                    string cmdString = "";
                    string cmdParams = "";
                    switch (cmd.ID)
                    {
                        case 0x01://SetVar, uses notation [varX = Y]
                            cmdString += "var" + cmd.ParamList[0] + " = ";
                            cmdParams += get_script_value(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x02://SetVec, uses notation [vecX = Y]
                            cmdString += "vec" + cmd.ParamList[0] + " = ";
                            cmdParams += get_script_value(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x06://If
                        case 0x07://IfNot
                            cmdString += script_data.CmdNames[0x6] + "(";
                            if (cmd.ID == 0x7)
                                cmdString += "!";
                            int cmdAfterIndex = 1;
                            while (cmdIndex + cmdAfterIndex < CmdList.Count)
                            {
                                script.Act.Cmd cmdCurr = CmdList[cmdIndex + cmdAfterIndex - 1];
                                cmdParams += get_if_chk(cmdCurr.ParamList.ToArray());
                                //commands 0x16 to 0x19 (Or + OrNot + And + AndNot)
                                //believe it or not this next check is actually what the source code does
                                relID = (byte)(CmdList[cmdIndex + cmdAfterIndex].ID - 0x16);
                                if (relID <= 3)
                                {
                                    cmdParams += " ";
                                    if (relID / 2 == 0)
                                        cmdParams += "|| ";
                                    else
                                        cmdParams += "&& ";

                                    if (relID % 2 != 0)
                                        cmdParams += "!";
                                    cmdAfterIndex++;
                                }
                                else
                                {
                                    cmdIndex += cmdAfterIndex - 1;
                                    break;
                                }
                            }
                            cmdString += cmdParams + ") {" + "\r\n";
                            if (lastCmdID != 0x8)
                                text += ifPadding;
                            text += cmdString;
                            break;
                        case 0x08://Else
                            cmdString += ifPadding + "}" + "\r\n" + ifPadding;
                            //if next command is an "if" or "ifNot" don't put it on a separate line
                            if (CmdList[cmdIndex + 1].ID == 0x6 || CmdList[cmdIndex + 1].ID == 0x7)
                                cmdString += script_data.CmdNames[cmd.ID] + " ";
                            else
                                cmdString += script_data.CmdNames[cmd.ID] + " {" + "\r\n";
                            text += cmdString;
                            break;
                        case 0x09://EndIf
                            cmdString += "}" + "\r\n";//use the symbol instead of the name
                            text += ifPadding + cmdString;
                            break;
                        case 0x0a://SetStickRel
                        case 0x1f://SetStickAbs
                            cmdString += script_data.CmdNames[cmd.ID] + "(";
                            for (int i = 0; i < cmd.paramCount; i++)
                            {
                                cmdParams += get_script_value(cmd.ParamList[i]);

                                if (i != cmd.paramCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x0b://SetButton
                            cmdString += script_data.CmdNames[cmd.ID] + "(";
                            List<string> cmdButtons = new List<string>();
                            //generate buttons from command
                            for (int i = 0; i < 4; i++)
                            {
                                int mask = 1 << i;
                                if ((cmd.ParamList[0] & mask) == mask)
                                {
                                    cmdButtons.Add(script_data.buttons[i]);
                                }
                            }
                            //write out button list
                            for (int i = 0; i < cmdButtons.Count; i++)
                            {
                                if (i != 0)
                                    cmdParams += "+";//surprisingly never used in vanilla smash 4 scripts, but we should support it
                                cmdParams += cmdButtons[i];
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x0c://var operators
                        case 0x0d:
                        case 0x0e:
                        case 0x0f:
                        case 0x10://vec operators
                        case 0x11:
                        case 0x12:
                        case 0x13:
                            relID = (byte)(cmd.ID - 0xc);
                            if (relID < 4)
                                cmdString += "var" + cmd.ParamList[0];
                            else
                                cmdString += "vec" + cmd.ParamList[0];

                            if (relID % 4 == 0)
                                cmdString += " += ";
                            else if (relID % 4 == 1)
                                cmdString += " -= ";
                            else if (relID % 4 == 2)
                                cmdString += " *= ";
                            else
                                cmdString += " /= ";

                            for (int i = 1; i < cmd.paramCount; i++)
                            {
                                cmdParams += get_script_value(cmd.ParamList[i]);

                                if (i != cmd.paramCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x1b://SetAct
                            cmdString += script_data.CmdNames[cmd.ID] + "(";
                            if (cmd.paramCount != 0)
                                cmdParams += "0x" + cmd.ParamList[0].ToString("X");
                            else
                            {
                                cmdParams += "";
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x1d://cliff vector stuff
                        case 0x27:
                        case 0x31:
                            cmdString += script_data.CmdNames[cmd.ID] + "(";
                            cmdParams += "vec" + cmd.ParamList[0].ToString();
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x2c://Norm = length of vector with given components
                            cmdString += "var" + cmd.ParamList[0] + " = " + script_data.CmdNames[cmd.ID] + "(";
                            cmdParams += get_script_value(cmd.ParamList[1]) + ", " + get_script_value(cmd.ParamList[2]);
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        default:
                            cmdString += script_data.CmdNames[cmd.ID] + "(";
                            for (int i = 0; i < cmd.paramCount; i++)
                            {
                                if (ScriptFloats.ContainsKey(cmd.ParamList[i]))
                                    cmdParams += ScriptFloats[cmd.ParamList[i]];
                                else
                                    cmdParams += "0x" + cmd.ParamList[i].ToString("X");

                                if (i != cmd.paramCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                    }
                    lastCmdID = cmd.ID;
                }

                return text;
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
                    if (script_data.script_value_special.ContainsKey(paramID))
                        value = script_data.script_value_special[paramID];
                    else
                        value = "0x" + paramID.ToString("X4");

                    return value;
                }
            }

            public string get_if_chk(UInt32[] cmdParams)
            {
                UInt32 reqID = cmdParams[0];
                string requirement = "";
                if (script_data.if_chks.ContainsKey(reqID))
                {
                    requirement += script_data.if_chks[reqID];
                }
                else
                {
                    requirement += "req_" + reqID.ToString("X");
                }
                requirement += "(";
                for (int i = 1; i < cmdParams.Length; i++)
                {
                    UInt32 currentParam = cmdParams[i];
                    switch (reqID)
                    {
                        //known if_chks that use get_script_value
                        case 0x1002:
                        case 0x1007:
                        case 0x1008:
                        case 0x1009:
                        case 0x100a:
                        case 0x1022:
                        case 0x102a:
                            requirement += get_script_value(currentParam);
                            break;
                        //character IDs
                        case 0x101e:
                        case 0x101f:
                            requirement += script_data.fighters[(int)currentParam];
                            break;
                        default:
                            requirement += "0x" + currentParam.ToString("X");
                            break;
                    }
                    //deal with adding commas for multiple args
                    if (i != cmdParams.Length - 1)
                        requirement += ", ";
                }
                return requirement + ")";
            }
        }//end of Act class
    }//end of Script class
}
