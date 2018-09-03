using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

//Some assistance with the file format is credit to Sammi Husky

namespace Sm4shAIEditor
{
    class script
    {
        public UInt32 actCount { get; set; }
        public Dictionary<Act, UInt32> acts { get; set; }

        public script(string binDirectory)
        {
            acts = new Dictionary<Act, uint>();

            BinaryReader binReader = new BinaryReader(File.OpenRead(binDirectory));
            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            actCount = util.ReadReverseUInt32(binReader);
            for (int i = 0; i < actCount; i++)
            {
                binReader.BaseStream.Seek(i * 4 + 0x10, SeekOrigin.Begin);
                UInt32 actOffset = util.ReadReverseUInt32(binReader);
                Act act = new Act(binReader, actOffset);

                acts.Add(act, actOffset);
            }

            binReader.Close();
        }
        public script(Dictionary<uint, string> decompiledActs)
        {
            acts = new Dictionary<Act, uint>();

            List<script.Act> actData = new List<script.Act>();
            foreach (var act in decompiledActs)
            {
                actData.Add(new Act(act.Key, act.Value));
            }
            actCount = (uint)actData.Count;
            uint offset = util.Align0x10(0x10 + (4 * actCount));
            foreach (script.Act act in actData)
            {
                acts.Add(act, offset);
                offset += act.GetSize();
            }
        }

        public class Act
        {
            public UInt32 ID { get; set; }
            public UInt32 ScriptOffset { get; set; }
            public UInt32 ScriptFloatOffset { get; set; }
            public UInt16 VarCount { get; set; }
            public Dictionary<UInt32, float> ScriptFloats { get; set; }

            public List<Cmd> CmdList { get; set; }

            private List<string> LabelNames { get; set; }

            public Act(BinaryReader binReader, UInt32 actPosition)
            {
                binReader.BaseStream.Seek(actPosition, SeekOrigin.Begin);
                ID = util.ReadReverseUInt32(binReader);
                ScriptOffset = util.ReadReverseUInt32(binReader);
                ScriptFloatOffset = util.ReadReverseUInt32(binReader);
                VarCount = util.ReadReverseUInt16(binReader);
                binReader.BaseStream.Seek(ScriptOffset + actPosition, SeekOrigin.Begin);

                ScriptFloats = new Dictionary<uint, float>();

                //Commands
                CmdList = new List<Cmd>();
                UInt32 relOffset = ScriptOffset;
                while (relOffset < ScriptFloatOffset)
                {
                    Cmd cmd = new Cmd(binReader);
                    CmdList.Add(cmd);

                    //add values to the script float list
                    foreach (UInt32 cmdParam in cmd.ParamList)
                    {
                        if (cmdParam >= 0x2000 &&
                            cmdParam < 0x2100 &&
                            !ScriptFloats.ContainsKey(cmdParam) &&
                            cmd.ID != 0x1b)
                        {
                            ScriptFloats.Add(cmdParam, GetScriptFloat(binReader, cmdParam, actPosition, ScriptFloatOffset));
                        }
                    }

                    relOffset += cmd.Size;
                }
            }
            //for compiling
            public Act(UInt32 ID, string text)
            {
                this.ID = ID;
                ScriptOffset = 0x10;//since every act and header is 0x10 aligned this is guaranteed to be 0x10
                ScriptFloatOffset = ScriptOffset;//temporarily set to this; every command increases the offset
                CmdList = new List<Cmd>();
                ScriptFloats = new Dictionary<uint, float>();
                LabelNames = new List<string>();

                CustomStringReader sReader = new CustomStringReader(text);
                bool isIfArg = false;
                while (!sReader.EndString)
                {
                    Cmd cmd = new Cmd(sReader, ref isIfArg, this);
                    cmd.ParamCount = (byte)cmd.ParamList.Count;
                    cmd.Size = (UInt16)(4 + (4 * cmd.ParamCount));
                    CmdList.Add(cmd);
                    sReader.SkipWhiteSpace();
                    ScriptFloatOffset += cmd.Size;
                }
                //warnings
                if (VarCount > 25)
                    Console.WriteLine("NOTICE: (Act {0}) variable count exceeded 25", ID);
            }

            public class Cmd
            {
                private Act parent;

                public byte ID { get; set; }
                public byte ParamCount { get; set; }
                public UInt16 Size { get; set; }

                public List<UInt32> ParamList { get; set; }

                public Cmd(BinaryReader binReader)
                {
                    ID = binReader.ReadByte();
                    ParamCount = binReader.ReadByte();
                    Size = util.ReadReverseUInt16(binReader);
                    ParamList = new List<UInt32>(ParamCount);
                    int readParams = 0;
                    while (readParams < ParamCount)
                    {
                        ParamList.Add(util.ReadReverseUInt32(binReader));
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
                public Cmd(CustomStringReader sReader, ref bool isIfArg, Act parent)
                {
                    this.parent = parent;
                    ParamList = new List<uint>();
                    string word;
                    bool canHaveArgs = true;
                    if (isIfArg)//parsing the if statements for their args
                    {
                        word = sReader.ReadIfSymbols();
                        if (word == "||")
                            ID = 0x16;
                        else if (word == "&&")
                            ID = 0x18;
                        else if (word == null)
                        {
                            string nextChar = sReader.ReadChar();
                            if (nextChar == ")")
                                isIfArg = false;
                            else
                                throw new Exception("syntax; expected '&&', '||', or ')' keys");
                        }

                        if (isIfArg)
                        {
                            sReader.SkipWhiteSpace();
                            if (sReader.ReadChar() == "!")
                                ID++;
                            else
                                sReader.Position--;

                            ConvertIfParams(sReader);
                        }
                        else
                            sReader.ReadUntilAnyOfChars("{", true);
                    }
                    if (!isIfArg)
                    {
                        if (parent.CmdList.Count > 0 && parent.CmdList[parent.CmdList.Count - 1].ID == 8)//if last command was Else
                        {
                            sReader.SkipWhiteSpace();
                            if (sReader.ReadChar() != "{")//basically we just want to skip to past the { sign so we can read more commands
                                sReader.Position--;//if Else is followed by a real command, don't skip the first letter
                        }
                        word = sReader.ReadWord();
                        int tempIndex = -1;
                        if (word == null)
                        {
                            string nextChar = sReader.ReadChar();
                            if (nextChar == "}")
                            {
                                sReader.SkipWhiteSpace();
                                int tempPosition = sReader.Position;
                                word = sReader.ReadWord();
                                if (word != cmd_info[8].name)//if next command isn't Else
                                {
                                    ID = 9;//then the } represents an Endif command
                                    sReader.Position = tempPosition;//move the position back so the next loop reads the next command
                                }
                                else
                                    ID = 8;//if next command is Else, then the command ID skips straight to Else
                                canHaveArgs = false;
                            }
                        }
                        else if ((tempIndex = cmd_info.FindIndex(item => item.name == word)) != -1)
                        {
                            ID = (byte)tempIndex;
                            if (ID == 6)//if statement
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                if (sReader.ReadChar() == "!")
                                    ID++;//IfNot
                                else
                                    sReader.Position--;

                                ConvertIfParams(sReader);
                                sReader.SkipWhiteSpace();
                                string nextChar = sReader.ReadChar();
                                if (nextChar != ")")
                                {
                                    sReader.Position--;
                                    isIfArg = true;
                                }
                                else
                                {
                                    isIfArg = false;
                                    canHaveArgs = false;
                                    sReader.ReadUntilAnyOfChars("{", true);
                                }
                            }
                            else if (ID == 8 || ID == 9)
                                canHaveArgs = false;
                        }
                        else if (word.StartsWith("var") || word.StartsWith("vec"))
                        {
                            ParamList.Add(UInt32.Parse(word.Substring(3)));//the numeric ID is the first arg
                            string op = sReader.ReadEqnSymbols();//operation
                            bool isVec;
                            if (word.Substring(0, 3) == "vec")
                                isVec = true;
                            else
                                isVec = false;

                            switch (op)//set command ID
                            {
                                case "=":
                                    ID = 1;
                                    break;
                                case "+=":
                                    ID = 0xc;
                                    break;
                                case "-=":
                                    ID = 0xd;
                                    break;
                                case "*=":
                                    ID = 0xe;
                                    break;
                                case "/=":
                                    ID = 0xf;
                                    break;
                                default:
                                    throw new Exception(string.Format("unrecognized variable assignment operator {0}", op));
                            }
                            if (isVec)
                            {
                                if (ID >= 0xc)
                                    ID += 4;
                                else
                                    ID++;
                            }
                            parent.UpdateVarCount(uint.Parse(word.Substring(3)), isVec);
                        }
                        else throw new Exception(string.Format("Unrecognized command {0}", word));
                        if (!isIfArg && canHaveArgs)
                            ConvertCmdParams(sReader);
                    }
                }

                private void ConvertCmdParams(CustomStringReader sReader)
                {
                    switch (ID)
                    {
                        case 0x01:
                        case 0x02:
                            ParamList.Add(parent.GetScriptValueID(sReader.ReadWord()));
                            break;
                        case 0x03://label
                        case 0x05://search label
                        case 0x1c://jump to label
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                string word = sReader.ReadWord();
                                sReader.SkipWhiteSpace();
                                string append = sReader.ReadChar();
                                if (append != ")")
                                    throw new Exception(string.Format("Syntax error in {0} arg: {1}", cmd_info[ID].name, append));
                                if (word == null) break;
                                if (parent.LabelNames.Contains(word))
                                    ParamList.Add((uint)parent.LabelNames.IndexOf(word));
                                else
                                {
                                    ParamList.Add((uint)parent.LabelNames.Count);
                                    parent.LabelNames.Add(word);
                                }
                                break;
                            }
                        case 0x0b://button
                            uint arg = 0;
                            while (true)
                            {
                                sReader.ReadUntilAnyOfChars("(", true);
                                string word = sReader.ReadWord();
                                sReader.SkipWhiteSpace();
                                string append = sReader.ReadChar();
                                if (word == null)
                                    throw new Exception("SetButton argument cannot be null");
                                if (!buttons.Contains(word))
                                    throw new Exception(string.Format("unrecognized button {0}", word));
                                uint buttonID = (uint)(1 << buttons.IndexOf(word));
                                arg |= buttonID;
                                if (append != "+" && append != "," && append != ")")
                                    throw new Exception(string.Format("syntax error in SetButton args: {0}", append));
                                if (append == ")") break;
                            }
                            ParamList.Add(arg);
                            break;
                        case 0x0c://assignment for vars and vecs
                        case 0x0d:
                        case 0x0e:
                        case 0x0f:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                        case 0x13:
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                if (word == null) throw new Exception("variable assignment argument cannot be null");
                                ParamList.Add(parent.GetScriptValueID(word));

                                sReader.SkipWhiteSpace();//spaces won't cause exceptions here /shrug

                                string append = sReader.ReadChar();
                                if (append != ",")
                                {
                                    if (append != null) sReader.Position--;
                                    break;
                                }
                            }
                            break;
                        case 0x1e:
                            sReader.ReadUntilAnyOfChars("(", true);
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                sReader.SkipWhiteSpace();
                                if (word == null || !word.StartsWith("var"))
                                    throw new Exception(string.Format("invalid argument in VarAbs command: {0}", word));
                                uint varID = uint.Parse(word.Substring(3));
                                ParamList.Add(varID);
                                parent.UpdateVarCount(varID, false);
                                string append = sReader.ReadChar();
                                if (append != ",")
                                {
                                    if (append == ")") break;
                                    else throw new Exception(string.Format("syntax error in VarAbs args: {0}", append));
                                }
                            }
                            break;
                        default:
                            sReader.ReadUntilAnyOfChars("(", true);
                            int readParams = 0;
                            while (true)
                            {
                                string word = sReader.ReadWord();
                                string append = sReader.ReadChar();
                                if (word == null)
                                {
                                    if (append == ",")
                                        throw new Exception(string.Format("null argument in {0}", cmd_info[ID].name));
                                    else if (append != ")")
                                        throw new Exception(string.Format("syntax error in {0} args: {1}", cmd_info[ID].name, append));
                                }
                                else
                                {
                                    int type = 0;
                                    if (readParams < cmd_info[ID].args.Length)
                                        type = cmd_info[ID].args[readParams];
                                    ParamList.Add(parent.GetParamIDFromType(word, type));
                                    
                                    readParams++;
                                }

                                if (append == ")") break;
                            }
                            break;
                    }
                }

                private void ConvertIfParams(CustomStringReader sReader)
                {
                    string word = sReader.ReadWord();
                    UInt32 reqID = 0;
                    if (word.StartsWith("req_"))
                        reqID = UInt32.Parse(word.Substring(4), System.Globalization.NumberStyles.HexNumber);
                    else if (if_chks.ContainsValue(word))
                    {
                        //get the Key used to index the value. This is probably inefficient and will need to be changed in the future
                        foreach (UInt32 key in if_chks.Keys)
                        {
                            if (if_chks[key] == word)
                            {
                                reqID = key;
                                break;
                            }
                        }
                    }
                    else if (param.checks.Contains(word))
                        reqID = (uint)param.checks.IndexOf(word);
                    else
                        throw new Exception(string.Format("unrecognized 'requirement' {0}", word));

                    ParamList.Add(reqID);
                    if (sReader.ReadChar() != "(")
                        throw new Exception("syntax error: requirement ID must be followed by parentheses");

                    bool readNextArg = true;
                    while (readNextArg)
                    {
                        word = sReader.ReadWord();
                        if (if_chk_args.ContainsKey(reqID) && word != null)
                        {
                            switch (if_chk_args[reqID])
                            {
                                case 0:
                                    ParamList.Add(parent.GetScriptValueID(word));
                                    break;
                                case 1:
                                    ParamList.Add((uint)fighters.IndexOf(word));
                                    break;
                            }
                        }
                        else
                        {
                            if (word != null)
                            {
                                if (word.StartsWith("0x"))
                                    ParamList.Add(uint.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber));
                                else
                                    ParamList.Add(uint.Parse(word));
                            }
                        }

                        string nextChar = sReader.ReadChar();
                        if (nextChar == ")")
                            readNextArg = false;
                        else if (nextChar != ",")
                            throw new Exception(string.Format("invalid syntax in If command: {0}", nextChar));
                    }
                }
            }

            public override string ToString()
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
                            cmdParams += GetScriptValue(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x02://SetVec, uses notation [vecX = Y]
                            cmdString += "vec" + cmd.ParamList[0] + " = ";
                            cmdParams += GetScriptValue(cmd.ParamList[1]);
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x06://If
                        case 0x07://IfNot
                            cmdString += cmd_info[0x6].name + "(";
                            if (cmd.ID == 0x7)
                                cmdString += "!";
                            int cmdAfterIndex = 1;
                            while (cmdIndex + cmdAfterIndex < CmdList.Count)
                            {
                                Cmd cmdCurr = CmdList[cmdIndex + cmdAfterIndex - 1];
                                cmdParams += GetIfChk(cmdCurr.ParamList.ToArray());
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
                                cmdString += cmd_info[cmd.ID].name + " ";
                            else
                                cmdString += cmd_info[cmd.ID].name + " {" + "\r\n";
                            text += cmdString;
                            break;
                        case 0x09://EndIf
                            cmdString += "}" + "\r\n";//use the symbol instead of the name
                            text += ifPadding + cmdString;
                            break;
                        case 0x0b://SetButton
                            cmdString += cmd_info[cmd.ID].name + "(";
                            List<string> cmdButtons = new List<string>();
                            //generate buttons from command
                            for (int i = 0; i < 4; i++)
                            {
                                int mask = 1 << i;
                                if ((cmd.ParamList[0] & mask) == mask)
                                {
                                    cmdButtons.Add(buttons[i]);
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
                                cmdParams += GetScriptValue(cmd.ParamList[i]);

                                if (i != cmd.ParamCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        case 0x1e://VarAbs, which supports arbitrary number of args
                            cmdString += cmd_info[cmd.ID].name + "(";
                            for (int i = 0; i < cmd.ParamCount; i++)
                            {
                                cmdParams += "var" + cmd.ParamList[i];

                                if (i != cmd.ParamCount - 1)
                                    cmdParams += ", ";
                            }
                            cmdString += cmdParams + ")" + "\r\n";
                            text += ifPadding + cmdString;
                            break;
                        default:
                            cmdString += cmd_info[cmd.ID].name + "(";
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
                string cmdParams = "";
                for (int i = 0; i < cmd.ParamCount; i++)
                {
                    byte type = 0;
                    if (i < cmd_info[cmd.ID].args.Length)
                        type = cmd_info[cmd.ID].args[i];

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
                            cmdParams += GetScriptValue(cmd.ParamList[i]);
                            break;
                        default:
                            break;
                    }
                    if (i != cmd.ParamCount - 1)
                        cmdParams += ", ";
                }
                return cmdParams;
            }

            public uint GetSize()
            {
                return ScriptFloatOffset + (4 * (uint)ScriptFloats.Count);
            }

            public UInt32 GetParamIDFromType(string param, int type)
            {
                UInt32 id;
                switch (type)
                {
                    case 0:
                        if (param.StartsWith("0x"))
                            id = UInt32.Parse(param.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        else
                            id = UInt32.Parse(param);
                        break;
                    case 1:
                    case 2:
                        if (!param.StartsWith("var") && !param.StartsWith("vec"))
                            throw new Exception(string.Format("param type {0} was neither a variable nor a vector", param));
                        id = UInt32.Parse(param.Substring(3));
                        UpdateVarCount(id, param.StartsWith("vec"));
                        break;
                    case 3:
                        id = GetScriptValueID(param);
                        break;
                    default:
                        throw new Exception("Invalid param type");
                }
                return id;
            }

            protected float GetScriptFloat(BinaryReader binReader, UInt32 cmdParam, UInt32 actPosition, UInt32 floatOffset)
            {
                float scriptFloat;
                Int32 binPosition = (Int32)binReader.BaseStream.Position;
                cmdParam -= 0x2000;
                binReader.BaseStream.Seek(actPosition + floatOffset + cmdParam * 4, SeekOrigin.Begin);
                scriptFloat = util.ReadReverseFloat(binReader);
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

            public string GetScriptValue(UInt32 paramID)
            {
                if (paramID < 0x1000)
                    return "var" + paramID;
                if (paramID >= 0x2000 && ScriptFloats.ContainsKey(paramID))
                    return ScriptFloats[paramID].ToString();
                else
                {
                    string value;
                    if (script_value_special.ContainsKey(paramID))
                        value = script_value_special[paramID];
                    else
                        value = "0x" + paramID.ToString("X4");

                    return value;
                }
            }

            public UInt32 GetScriptValueID(string param)
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
                else if (script_value_special.ContainsValue(param))
                {
                    foreach (UInt32 valueID in script_value_special.Keys)
                    {
                        if (param == script_value_special[valueID])
                        {
                            ID = valueID;
                            break;
                        }
                    }
                }
                else
                    throw new Exception(string.Format("unrecognized script_value {0}", param));

                return ID;
            }

            public string GetIfChk(UInt32[] cmdParams)
            {
                UInt32 reqID = cmdParams[0];
                string requirement = "";

                if (reqID < 0x1000)
                    requirement += param.checks[(int)reqID];
                else if (if_chks.ContainsKey(reqID))
                    requirement += if_chks[reqID];
                else
                    requirement += "req_" + reqID.ToString("X");

                requirement += "(";
                for (int i = 1; i < cmdParams.Length; i++)
                {
                    UInt32 currentParam = cmdParams[i];
                    if (if_chk_args.ContainsKey(reqID))
                    {
                        switch (if_chk_args[reqID])
                        {
                            case 0:
                                requirement += GetScriptValue(currentParam);
                                break;
                            case 1:
                                requirement += fighters[(int)currentParam];
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

            public void UpdateVarCount(uint varID, bool isVec)
            {
                if (isVec)
                    varID++;
                if (varID > VarCount)
                    VarCount = (ushort)(varID + 1);
            }
        }//end of Act class

        public class CmdInfo
        {
            public string name { get; private set; }
            public string desc { get; private set; }
            public byte[] args { get; private set; }
            //0 = int
            //1 = var
            //2 = vec
            //3 = script_value

            public CmdInfo(string name, string desc)
            {
                this.name = name;
                this.desc = desc;
                args = new byte[0];
            }
            public CmdInfo(string name, string desc, byte[] args)
            {
                this.name = name;
                this.desc = desc;
                this.args = args;
            }
        }

        public static List<CmdInfo> cmd_info = new List<CmdInfo>()
        {
            new CmdInfo("End","Ends execution of the script"),
            new CmdInfo("SetVar","Sets a variable using an ID.\nSpecial syntax: varX = Y"),
            new CmdInfo("SetVec","Sets two consecutive variables with an ID.\nSpecial syntax vecX = Y"),
            new CmdInfo("Label","Saves a script position to memory. Starts a loop to run logic within", new byte[] { 0 }),
            new CmdInfo("Return","If a label is set, exits the script and returns to the label on the next frame"),
            new CmdInfo("Search","Sets a new label without changing script position. If no argument is given, searches for the next label. Otherwise, searches for the label of the ID given", new byte[] { 0 }),
            new CmdInfo("If","Checks conditions before running code.\nNOTE: the conditions can require their own arguments and the code to run is enclosed in { } blocks"),
            new CmdInfo("IfNot","Same as If, except this negates the first condition. To use this, negate the first condition with a '!' symbol"),
            new CmdInfo("Else","If a prior If statement was false, runs this code"),
            new CmdInfo("Endif","The end of each { } block in an If statement. Do not use this directly"),
            new CmdInfo("Stick","Adds a value to the current stickX relative to facing direction. If a second argument is given, StickY", new byte[] { 3, 3 }),
            new CmdInfo("Button","Sets the AI's held buttons: Attack, Special, Shield, Jump. Buttons can be set in separate commands or in the same command using a + or | operator"),
            new CmdInfo("VarAdd","Adds a value to a variable.\nSpecial syntax: varX += Y, Z, etc"),
            new CmdInfo("VarSub","Subtracts a value from a variable.\nSpecial syntax: varX -= Y, Z, etc"),
            new CmdInfo("VarMul","Multiplies a value with a variable.\nSpecial syntax: varX *= Y, Z, etc"),
            new CmdInfo("VarDiv","Divides a value from a variable.\nSpecial syntax: varX /= Y, Z, etc"),
            new CmdInfo("VecAdd","Adds a value to a vector.\nSpecial syntax: vecX += Y, Z, etc"),
            new CmdInfo("VecSub","Subtracts a value from a vector.\nSpecial syntax: vecX -= Y, Z, etc"),
            new CmdInfo("VecMul","Multiplies a value with a vector.\nSpecial syntax: vecX *= Y, Z, etc"),
            new CmdInfo("VecDiv","Divides a value from a vector.\nSpecial syntax: vecX /= Y, Z, etc"),
            new CmdInfo("GoToCurrentLabel","Changes script position immediately to the set position"),
            new CmdInfo("SetVarRandf","Sets a variable to a random float, with some options.\n1 args: var = [0,1]\n2 args: var = arg2 + [0,1]\n3 args: var = arg2 + arg3*[0,1]\n5 args: var = arg2 + var3*[0,1], + arg4 if arg5 % chance probability is met", new byte[] { 1, 3, 3, 3, 3 }),
            new CmdInfo("Or","For use only in If statements. Use || instead"),
            new CmdInfo("OrNot","For use only in If statements. Use || !(condition) instead"),
            new CmdInfo("And","For use only in If statements. Use && instead"),
            new CmdInfo("AndNot","For use only in If statemnets. Use && !(condition) instead"),
            new CmdInfo("SetFrame","Sets a value representing the execution frame, continues counting up every frame", new byte[] { 3 }),
            new CmdInfo("SetAct","Sets a new Act ID, to be called when the script finishes", new byte[] { 0 }),
            new CmdInfo("Jump","Jumps to a specified label immediately. When a return command is reached, jump back immediately", new byte[] { 0 }),
            new CmdInfo("GetNearestCliff","", new byte[] { 2 }),
            new CmdInfo("VarAbs","Sets a variable to the absolute value of itself. Supports multiple args"),
            new CmdInfo("StickAbs","Adds a value to the current StickX independent of facing direction. If a second arg is given, StickY", new byte[] { 3, 3 }),
            new CmdInfo("BreakIfAerial","If AI is in the air, exits the script reader loop. Returns to the current position after a frame"),
            new CmdInfo("BreakIfGroundFree","If AI is free to act on the ground, exits the script reader loop. Returns to the current position after a frame"),
            new CmdInfo("SetResetFrames","Sets a countdown which 'encourages the CPU to act' once it reaches 0", new byte[] { 3 }),
            new CmdInfo("SetCliffResetDistance","Sets a distance such that when the AI are outside the given distance from the Cliff, they change acts", new byte[] { 3 }),
            new CmdInfo("CalcArriveFrameX","Sets a variable to the estimated time until the AI's target reaches the given X position", new byte[] { 1, 3 }),
            new CmdInfo("CalcArriveFrameY","Sets a variable to the estimated time until the AI's target reaches the given Y position", new byte[] { 1, 3 }),
            new CmdInfo("SetVarShieldHP","Sets a variable to the Shield HP value", new byte[] { 1 }),
            new CmdInfo("StagePtRand","", new byte[] { 2 }),
            new CmdInfo("CalcArrivePosX","Sets a variable to the estimated X position the AI's target reaches after given frames", new byte[] { 1, 3 }),
            new CmdInfo("CalcArrivePosY","Sets a variable to the estimated Y position the AI's target reaches after given frames", new byte[] { 1, 3 }),
            new CmdInfo("AttackDiceRoll",""),
            new CmdInfo("Null_2b","This command serves no function?"),
            new CmdInfo("Norm","Sets a variable to the magnitude of the given X and Y parameters", new byte[] { 1, 3, 3 }),
            new CmdInfo("Dot",""),
            new CmdInfo("CalcTargetVector","Sets the first and second variables to the expected X and Y displacement after a certain unit of time (14 frames?). If arg3 is given, multiplies the unit of time by it", new byte[] { 1, 1, 3 }),
            new CmdInfo("Unk_2f","", new byte[] { 1, 3 }),
            new CmdInfo("SwingChkSet","Sets a value indicating that the AI should 'respond' to successfully hitting the target. Affects a value in memory which affects act frequency"),
            new CmdInfo("GetNearestCliffAbs","", new byte[] { 2 }),
            new CmdInfo("ClearStick","If arg is not given, sets both StickX and StickY to 0. Otherwise, if the arg is 0, only sets StickX to 0, else sets StickY to 0", new byte[] { 0 }),
            new CmdInfo("Unk_Stick","Add values to StickX and StickY, but multiplies the arguments by an unknown value", new byte[] { 3, 3 }),
            new CmdInfo("Null_34","This command serves no function?"),
            new CmdInfo("Null_35","This command serves no function?"),
            new CmdInfo("StickAngleFront","Sets StickX and StickY using an angle measure in degrees, clockwise", new byte[] { 3 }),
            new CmdInfo("StickAngleBack","Sets StickX and StickY using an angle measure in degrees, counter-clockwise", new byte[] { 3 }),
            new CmdInfo("ACos","Sets a variable to the arccosine of itself. Will throw an error if the absolute value > 1", new byte[] { 1 }),
            new CmdInfo("Unk_39","")
        };

        public static Dictionary<UInt32, string> script_value_special = new Dictionary<UInt32, string>()
        {
            //0x1000 = par_work_update value. Represents distance of some sort?
            //0x1001 = next par_work_update value
            {0x1002, "lr" },
            {0x1003, "lr_tgt" },
            {0x1004, "pos" },//vector
            {0x1005, "tgt_pos" },//vector
            {0x1006, "edge_dist_lr_tgt" },
            {0x1007, "timer" },
            {0x1008, "spd" },//vector
            {0x1009, "zero" },
            {0x100a, "one" },
            {0x100b, "pos_y" },
            {0x100c, "tgt_pos_y" },
            {0x100d, "spd_y" },
            {0x100e, "randf"},
            //0x100f = {Ground = 2, Air = 1}? Based on usage this may have changed in Smash 4
            {0x1010, "edge_dist_front" },
            {0x1011, "rank" },
            //{0x1012, "" }, some byte
            //{0x1013, "" }, next byte
            {0x1014, "ctrl_weapon_pos" },
            {0x1015, "ctrl_weapon_pos_y" },
            {0x1016, "tgt_spd" },
            {0x1017, "tgt_spd_y" },
            {0x1018, "ctrl_weapon_spd" },
            {0x1019, "ctrl_weapon_spd_y" },
            {0x101a, "hipn_pos" },
            {0x101b, "hipn_pos_y" },
            {0x101c, "tgt_hipn_pos" },
            {0x101d, "tgt_hipn_pos" },
            {0x101e, "dmg" },
            {0x101f, "tgt_dmg" },
            //0x1020 is some ai param
            {0x1021, "jump_height" },
            {0x1022, "jump_length" },
            {0x1023, "air_jump_height" },
            {0x1024, "air_jump_length" },
            {0x1025, "tgt_lr" },
            {0x1026, "tgt_jumps_remain" },
            {0x1027, "rhombus_height" },
            {0x1028, "tgt_rhombus_height" },
            {0x1029, "edge_dist_back" },
            {0x102a, "blastzone_bottom" },
            {0x102b, "blastzone_top" },
            {0x102c, "blastzone_left" },
            {0x102d, "blastzone_right" },
            {0x102e, "stage_length" },
        };//maximum value = 0x1044

        public static List<string> buttons = new List<string>
        {
            "attack", "special", "shield", "jump"
        };

        public static Dictionary<UInt32, string> if_chks = new Dictionary<UInt32, string>()
        {
            //{0x1000, "tgt_dist" },?
            //{0x1001, "tgt_dist_x" },?
            {0x1002, "timer_passed" },
            {0x1003, "ground_free" },
            {0x1004, "dashing" },
            {0x1005, "aerial" },
            {0x1006, "null_1006" },
            {0x1007, "greater" },
            {0x1008, "less" },
            {0x1009, "geq" },
            {0x100a, "leq" },
            {0x100d, "off_stage" },
            {0x100f, "status" },
            //{0x1010, "" },? related to tgt_dist
            {0x1012, "air_free" },
            {0x1014, "catch_hold" },
            //{0x1016, "" },? related to tgt_dist
            {0x1019, "hitbox_active" },
            {0x101a, "tgt_try_catch" },
            {0x101b, "tgt_aerial" },
            {0x101c, "tgt_caught" },
            {0x101e, "char" },
            {0x101f, "tgt_char" },
            {0x1024, "motion" },
            {0x102a, "tgt_dmg_geq" }
        };//maximum value = 0x1059

        //Key = ID, Value = type of arguments:
        //0 = get_script_value
        //1 = fighter name
        public static Dictionary<UInt32, byte> if_chk_args = new Dictionary<uint, byte>()
        {
            {0x1000, 0},
            {0x1001, 0},
            {0x1002, 0},
            {0x1007, 0},
            {0x1008, 0},
            {0x1009, 0},
            {0x100a, 0},
            {0x1010, 0},
            {0x1016, 0},
            {0x1017, 0},
            {0x101e, 1},
            {0x101f, 1},
            {0x1020, 0},
            {0x1021, 0},
            {0x1022, 0},
            {0x1023, 0},
            {0x1026, 0},
            {0x1027, 0},
            {0x102a, 0},
            {0x102b, 0},
            {0x102c, 0},
            {0x1047, 0},
            {0x1054, 0},
        };

        public static List<string> fighters = new List<string>()
        {
            "miifighter",
            "miiswordsman",
            "miigunner",
            "mario",
            "donkey",
            "link",
            "samus",
            "yoshi",
            "kirby",
            "fox",
            "pikachu",
            "luigi",
            "captain",
            "ness",
            "peach",
            "koopa",
            "zelda",
            "sheik",
            "marth",
            "gamewatch",
            "ganon",
            "falco",
            "wario",
            "metaknight",
            "pit",
            "szerosuit",
            "pikmin",
            "diddy",
            "dedede",
            "ike",
            "lucario",
            "robot",
            "toonlink",
            "lizardon",
            "sonic",
            "purin",
            "mariod",
            "lucina",
            "pitb",
            "rosetta",
            "wiifit",
            "littlemac",
            "murabito",
            "palutena",
            "reflet",
            "duckhunt",
            "koopajr",
            "shulk",
            "gekkouga",
            "pacman",
            "rockman",
            "mewtwo",
            "ryu",
            "lucas",
            "roy",
            "cloud",
            "bayonetta",
            "kamui",
            "koopag",
            "warioman",
            "littlemacg",
            "lucariom",
            "miienemyf",
            "miienemys",
            "miienemyg"
        };
    }//end of Script class
}
