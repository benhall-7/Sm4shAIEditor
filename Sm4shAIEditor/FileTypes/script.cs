using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

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
                            cmdParam < 0x2100 &&
                            !ScriptFloats.ContainsKey(cmdParam) &&
                            cmd.ID != 0x1b)
                        {
                            ScriptFloats.Add(cmdParam, GetScriptFloat(ref binReader, cmdParam, actPosition, ScriptFloatOffset));
                        }
                    }

                    relOffset += cmd.Size;
                }
            }
            //for compiling
            public Act(UInt32 ID, string text)
            {
                int byteOffset = 0;
                Int32 varCount = 0;
                this.ID = ID;
                List<UInt32> cmdList = new List<uint>();
                Dictionary<UInt32, float> scriptFloats = new Dictionary<UInt32, float>();
                CustomStringReader sReader = new CustomStringReader(text);
                
                while (!sReader.EndString)
                {
                    bool isIfArg = false;

                    Cmd cmd = new Cmd(ref sReader, ref isIfArg, this);
                    CmdList.Add(cmd);
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

            public void AddScriptFloat(float value)
            {
                if (!ScriptFloats.ContainsValue(value))
                {
                    UInt32 nextFloatID = (UInt32)(ScriptFloats.Keys.Count + 0x2000);
                    ScriptFloats.Add(nextFloatID, value);
                }
            }

            public class Cmd
            {
                private Act parent;

                public byte ID { get; set; }
                public byte ParamCount { get; set; }
                public UInt16 Size { get; set; }

                public List<UInt32> ParamList { get; set; }

                public Cmd(ref BinaryReader binReader)
                {
                    ID = binReader.ReadByte();
                    ParamCount = binReader.ReadByte();
                    Size = task_helper.ReadReverseUInt16(ref binReader);
                    ParamList = new List<UInt32>(ParamCount);
                    int readParams = 0;
                    while (readParams < ParamCount)
                    {
                        ParamList.Add(task_helper.ReadReverseUInt32(ref binReader));
                        readParams++;
                    }
                }
                public Cmd(byte id, List<UInt32> paramList)
                {
                    ID = id;
                    ParamList = paramList;
                    ParamCount = (byte)paramList.Count;
                    Size = (UInt16)(ParamCount * 4 + 4);
                }
                public Cmd(ref CustomStringReader sReader, ref bool isIfArg, Act parent)
                {
                    this.parent = parent;
                    byte cmdID = 0;
                    string word;
                    if (!isIfArg)
                    {
                        word = sReader.ReadWord();
                        if (script_data.cmds.Contains(word))
                        {
                            cmdID = (byte)script_data.cmds.IndexOf(word);
                            if (cmdID == 6)//if statement
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                if (sReader.ReadChar() == "!")
                                    cmdID++;//IfNot
                                else
                                    sReader.Position--;

                                ConvertIfParams(ref sReader);
                                string nextChar = sReader.ReadChar();
                                if (nextChar == ",")
                                    isIfArg = true;
                                else if (nextChar != ")")
                                    throw new Exception();//add exception text here
                            }
                        }
                        else if (word.StartsWith("var") || word.StartsWith("vec"))
                        {
                            Int32 varID = Int32.Parse(word.Substring(3));//the numeric variable ID
                            string op = sReader.ReadEqnSymbols();//operation
                            bool isVec;
                            if (word.Substring(0, 3) == "vec")
                                isVec = true;
                            else if (word.Substring(0, 3) == "var")
                                isVec = false;
                            else
                                throw new Exception();//add exception text here

                            switch (op)//set command ID
                            {
                                case "=":
                                    cmdID = 1;
                                    break;
                                case "+=":
                                    cmdID = 0xc;
                                    break;
                                case "-=":
                                    cmdID = 0xd;
                                    break;
                                case "*=":
                                    cmdID = 0xe;
                                    break;
                                case "/=":
                                    cmdID = 0xf;
                                    break;
                                default:
                                    throw new Exception();//add exception text here; unrecognized variable assignment
                            }
                            if (isVec)
                            {
                                if (cmdID >= 0xc)
                                    cmdID += 4;
                                else
                                    cmdID++;
                            }
                        }
                    }
                    else//parsing the if statements for their args
                    {
                        word = sReader.ReadIfSymbols();
                        if (word == "||")
                            cmdID = 0x16;
                        else if (word == "&&")
                            cmdID = 0x18;
                        else
                            throw new Exception();//add exception text here

                        if (sReader.ReadChar() == "!")
                            cmdID++;
                        else
                            sReader.Position--;

                        ConvertIfParams(ref sReader);
                    }
                }

                public void ConvertIfParams(ref CustomStringReader sReader)
                {
                    string word = sReader.ReadWord();
                    UInt32 reqID = 0;
                    if (word.StartsWith("req_"))
                    {
                        reqID = UInt32.Parse(word.Substring(4), System.Globalization.NumberStyles.HexNumber);
                    }
                    else if (script_data.if_chks.ContainsValue(word))
                    {
                        //get the Key used to index the value. This is probably inefficient and will need to be changed in the future
                        foreach (UInt32 key in script_data.if_chks.Keys)
                        {
                            if (script_data.if_chks[key] == word)
                            {
                                reqID = key;
                                break;
                            }
                        }
                    }
                    else
                        throw new Exception();//add exception text here: unrecognized 'requirement' parameter

                    ParamList.Add(reqID);
                    if (sReader.ReadChar() != "(")
                        throw new Exception();//add exception text here; syntax error: requirement ID must be followed by ()

                    bool readNextArg = true;
                    while (readNextArg)
                    {
                        word = sReader.ReadWord();
                        if (script_data.if_chk_args.ContainsKey(reqID))
                        {
                            switch (script_data.if_chk_args[reqID])
                            {
                                case 0:
                                    ParamList.Add(parent.get_script_value_id(word));
                                    break;
                                case 1:
                                    ParamList.Add((uint)script_data.fighters.IndexOf(word));
                                    break;
                            }
                        }
                        else
                            ParamList.Add(uint.Parse(word));

                        if (sReader.ReadChar() == ")")
                            readNextArg = false;
                        else if (sReader.ReadChar() != ",")
                            throw new Exception();//add exception text here
                    }
                }
            }

            public string DecompAct()
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
                            cmdString += script_data.cmds[0x6] + "(";
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
                                cmdString += script_data.cmds[cmd.ID] + " ";
                            else
                                cmdString += script_data.cmds[cmd.ID] + " {" + "\r\n";
                            text += cmdString;
                            break;
                        case 0x09://EndIf
                            cmdString += "}" + "\r\n";//use the symbol instead of the name
                            text += ifPadding + cmdString;
                            break;
                        case 0x0b://SetButton
                            cmdString += script_data.cmds[cmd.ID] + "(";
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

                            for (int i = 1; i < cmd.ParamCount; i++)
                            {
                                cmdParams += get_script_value(cmd.ParamList[i]);

                                if (i != cmd.ParamCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        default:
                            cmdString += script_data.cmds[cmd.ID] + "(";
                            string parsed = ParseCmdParams(cmd);
                            if (parsed != null)
                            {
                                cmdParams += parsed;
                            }
                            else
                            {
                                //hopefully I get to the point where this becomes obsolete
                                for (int i = 0; i < cmd.ParamCount; i++)
                                {
                                    cmdParams += "0x" + cmd.ParamList[i].ToString("X");

                                    if (i != cmd.ParamCount - 1)
                                        cmdParams += ", ";
                                }
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                    }
                    lastCmdID = cmd.ID;
                }

                return text;
            }

            public string ParseCmdParams(Cmd cmd)
            {
                int correctIndex = -1;
                //can this be made faster?
                for (int i = 0; i < script_data.cmd_args.Count; i++)
                {
                    if (cmd.ID == script_data.cmd_args[i][0])
                    {
                        if (cmd.ParamCount == script_data.cmd_args[i].Length - 1)
                        {
                            correctIndex = i;
                            break;
                        }
                    }
                }
                if (correctIndex == -1)
                    return null;
                else
                {
                    string cmdParams = "";
                    for (int i = 0; i < cmd.ParamCount; i++)
                    {
                        byte type = script_data.cmd_args[correctIndex][i + 1];
                        switch (type)
                        {
                            case 0:
                                cmdParams += "0x" + cmd.ParamList[i].ToString("X");
                                break;
                            case 1:
                                cmdParams += "var" + cmd.ParamList[i];
                                break;
                            case 2:
                                cmdParams += "vec" + cmd.ParamList[i];
                                break;
                            case 3:
                                cmdParams += get_script_value(cmd.ParamList[i]);
                                break;
                            default:
                                break;
                        }
                        if (i != cmd.ParamCount - 1)
                            cmdParams += ", ";
                    }
                    return cmdParams;
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
                    if (script_data.script_value_special.ContainsKey(paramID))
                        value = script_data.script_value_special[paramID];
                    else
                        value = "0x" + paramID.ToString("X4");

                    return value;
                }
            }

            public UInt32 get_script_value_id(string param)
            {
                UInt32 ID = 0;
                if (param.StartsWith("0x"))
                {
                    ID = UInt32.Parse(param.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }
                else if (float.TryParse(param, out float value))
                {
                    AddScriptFloat(value);
                    foreach (UInt32 floatID in ScriptFloats.Keys)
                    {
                        if (value == ScriptFloats[floatID])
                        {
                            ID = floatID;
                            break;
                        }
                    }
                }
                else if (param.StartsWith("var") || param.StartsWith("vec"))
                {
                    ID = UInt32.Parse(param.Substring(3));
                }
                else if (script_data.script_value_special.ContainsValue(param))
                {
                    foreach (UInt32 valueID in script_data.script_value_special.Keys)
                    {
                        if (param == script_data.script_value_special[valueID])
                        {
                            ID = valueID;
                            break;
                        }
                    }
                }
                else
                    throw new Exception();//add exception text here

                return ID;
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
                    if (script_data.if_chk_args.ContainsKey(reqID))
                    {
                        switch (script_data.if_chk_args[reqID])
                        {
                            case 0:
                                requirement += get_script_value(currentParam);
                                break;
                            case 1:
                                requirement += script_data.fighters[(int)currentParam];
                                break;
                        }
                    }
                    else
                    {
                        requirement += "0x" + currentParam.ToString("X");
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
