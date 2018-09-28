using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sm4shAIEditor.Static
{
    public static class aism
    {
        public static void AssembleFolder(string pathIn, string pathOut)
        {
            //check path is folder ; check folder name ; check folder contents ; proceed
            try
            {
                string tempPath = pathIn;

                pathIn = util.CorrectFormatFolderPath(pathIn);
                if (!Directory.Exists(pathIn))
                    throw new Exception("Input path directory not found");
                pathIn = Path.GetFullPath(pathIn);
                string fileType = util.GetFolderName(pathIn);
                if (!AITree.StringToAIType.ContainsKey(fileType))
                    throw new Exception(string.Format("Nonexistant file type {0}", fileType));
                
                pathOut = util.CorrectFormatFolderPath(pathOut);

                AITree.AIType type = AITree.StringToAIType[fileType];
                Console.Write("Initializing {0}...", tempPath);
                string asm = string.Format("Assembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {
                    string[] sub_counts = File.ReadAllLines(pathIn + "subactions.txt");
                    attack_data atkd = new attack_data(uint.Parse(sub_counts[0]), uint.Parse(sub_counts[1]), pathIn + "attack_data.txt");
                    Console.Write(asm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    BinaryWriter binWriter = new BinaryWriter(File.Create(pathOut + fileType + ".bin"));
                    binWriter.Write(util.fileMagic[type]);
                    util.WriteReverseUInt32(binWriter, atkd.count);
                    util.WriteReverseUInt32(binWriter, atkd.common_subactions);
                    util.WriteReverseUInt32(binWriter, atkd.special_subactions);
                    foreach (var attack in atkd.attacks)
                    {
                        util.WriteReverseUInt16(binWriter, attack.subaction);
                        binWriter.BaseStream.Position += 2;
                        util.WriteReverseUInt16(binWriter, attack.start);
                        util.WriteReverseUInt16(binWriter, attack.end);
                        util.WriteReverseFloat(binWriter, attack.x1);
                        util.WriteReverseFloat(binWriter, attack.x2);
                        util.WriteReverseFloat(binWriter, attack.y1);
                        util.WriteReverseFloat(binWriter, attack.y2);
                    }
                    binWriter.Dispose();
                }
                else if (type == AITree.AIType.script)
                {
                    Dictionary<uint, string> acts = new Dictionary<uint, string>();
                    string[] IDs = File.ReadAllLines(pathIn + "acts.txt");
                    foreach(string ID in IDs)
                        acts.Add(uint.Parse(ID, NumberStyles.HexNumber), File.ReadAllText(pathIn + ID + ".txt"));
                    script script = new script(acts);
                    Console.Write(asm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    BinaryWriter binWriter = new BinaryWriter(File.Create(pathOut + fileType + ".bin"));
                    binWriter.Write((int)0);//pad
                    util.WriteReverseUInt32(binWriter, script.actCount);
                    binWriter.Write((long)0);//pad
                    foreach (var act in script.acts.Keys)
                        util.WriteReverseUInt32(binWriter, script.acts[act]);
                    foreach (var act in script.acts.Keys)
                    {
                        binWriter.BaseStream.Position = script.acts[act];
                        util.WriteReverseUInt32(binWriter, act.ID);
                        util.WriteReverseUInt32(binWriter, act.ScriptOffset);
                        util.WriteReverseUInt32(binWriter, act.ScriptFloatOffset);
                        util.WriteReverseUInt16(binWriter, act.VarCount);
                        binWriter.Write((short)0);
                        foreach (var cmd in act.CmdList)
                        {
                            binWriter.Write(cmd.ID);
                            binWriter.Write(cmd.ParamCount);
                            util.WriteReverseUInt16(binWriter, cmd.Size);
                            foreach (var param in cmd.ParamList)
                                util.WriteReverseUInt32(binWriter, param);
                        }
                        foreach (var value in act.ScriptFloats.Values)
                            util.WriteReverseFloat(binWriter, value);
                    }
                    binWriter.Dispose();
                }
                else //param and param_nfp will use same methods
                {
                    param aipd = new param(pathIn + "section_1.txt",
                        pathIn + "section_2.txt",
                        pathIn + "section_3.txt",
                        pathIn + "situation_return.txt",
                        pathIn + "situation_attack.txt",
                        pathIn + "situation_defend.txt");
                    Console.Write(asm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    BinaryWriter binWriter = new BinaryWriter(File.Create(pathOut + fileType + ".bin"));
                    binWriter.Write(util.fileMagic[type]);
                    util.WriteReverseUInt32(binWriter, aipd.unk_size);
                    binWriter.BaseStream.Position = 0x10;
                    binWriter.Write(aipd.sit_return_start);
                    binWriter.Write(aipd.sit_return_end);
                    binWriter.Write(aipd.sit_attack_start);
                    binWriter.Write(aipd.sit_attack_end);
                    binWriter.Write(aipd.sit_defend_start);
                    binWriter.Write(aipd.sit_defend_end);
                    binWriter.BaseStream.Position = param.val_offset;
                    for (int i = 0; i < param.val_count; i++)
                        binWriter.Write(aipd.vals[i]);
                    binWriter.BaseStream.Position = param.flg_offset;
                    for (int i = 0; i < param.flg_count; i++)
                        binWriter.Write(aipd.flags[i]);
                    //stuff with offsets now
                    long tablePos = param.cmd_offset;
                    long filePos = param.cmd_offset + aipd.get_address_table_size();
                    for (int i = 0; i < param.cmd_count; i++)
                    {
                        binWriter.BaseStream.Position = tablePos;
                        util.WriteReverseUInt32(binWriter, (uint)filePos);
                        tablePos = binWriter.BaseStream.Position;
                        binWriter.BaseStream.Position = filePos;
                        for (int j = 0; j < param.Cmd.unk_count; j++)
                        {
                            var unk = aipd.cmds[i].unks[j];
                            binWriter.Write(unk.index);
                            binWriter.BaseStream.Position += 3;
                            util.WriteReverseUInt16(binWriter, unk.hi_rank_prob);
                            util.WriteReverseUInt16(binWriter, unk.lw_rank_prob);
                        }
                        filePos = binWriter.BaseStream.Position + 1;//extra 00 byte at the end of each section
                    }
                    for (int i = 0; i < aipd.sits.Length; i++)
                    {
                        binWriter.BaseStream.Position = tablePos;
                        util.WriteReverseUInt32(binWriter, (uint)filePos);
                        tablePos = binWriter.BaseStream.Position;
                        binWriter.BaseStream.Position = filePos;
                        var sit = aipd.sits[i];
                        binWriter.Write(sit.condition0);
                        binWriter.Write(sit.condition1);
                        binWriter.Write(sit.flags);
                        binWriter.Write(sit.count);
                        for (int j = 0; j < sit.count; j++)
                        {
                            var action = sit.actions[j];
                            binWriter.Write(action.hi_rank_prob);
                            binWriter.Write(action.lw_rank_prob);
                            binWriter.Write(action.max_rank);
                            binWriter.Write(action.min_rank);
                            util.WriteReverseUInt16(binWriter, action.act);
                        }
                        filePos = binWriter.BaseStream.Position;
                    }
                    binWriter.Dispose();
                }
                Console.WriteLine("Complete");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void DisassembleFile(string pathIn, string pathOut)
        {
            //check path is file ; check file name ; proceed
            try
            {
                string tempPath = pathIn;

                if (!File.Exists(pathIn))
                    throw new Exception("Input path file not found");
                pathIn = Path.GetFullPath(pathIn);
                string fileType = Path.GetFileNameWithoutExtension(pathIn);
                if (!AITree.StringToAIType.ContainsKey(fileType))
                    throw new Exception(string.Format("Nonexistant file type {0}", fileType));

                pathOut = util.CorrectFormatFolderPath(pathOut);

                AITree.AIType type = AITree.StringToAIType[fileType];
                Console.Write("Initializing {0}...", tempPath);
                string disasm = string.Format("Disassembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {
                    attack_data atkd = new attack_data(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    File.WriteAllText(pathOut + "subactions.txt", atkd.common_subactions + "\n" + atkd.special_subactions);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "attack_data.txt"));
                    writer.WriteLine("#format:\n#subaction: [start_frame, end_frame], [x1, x2, y1, y2]");
                    foreach (var attack in atkd.attacks)
                        writer.WriteLine(attack.ToString());
                    writer.Dispose();
                }
                else if (type == AITree.AIType.script)
                {
                    script script = new script(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "acts.txt"));
                    foreach (var act in script.acts.Keys)
                    {
                        writer.WriteLine(act.ID.ToString("X4"));
                        File.WriteAllText(pathOut + act.ID.ToString("X4") + ".txt", act.ToString());
                    }
                    writer.Dispose();
                }
                else //param and param_nfp will use same methods
                {
                    param aipd = new param(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);

                    StreamWriter writer;
                    writer = new StreamWriter(File.Create(pathOut + "section_1.txt"));
                    for (int i = 0; i < aipd.vals.Length; i++)
                        writer.WriteLine("{0}", aipd.vals[i]);
                    writer.Dispose();

                    writer = new StreamWriter(File.Create(pathOut + "section_2.txt"));
                    for (int i = 0; i < aipd.flags.Length; i++)
                        writer.WriteLine("{0}", aipd.flags[i]);
                    writer.Dispose();

                    writer = new StreamWriter(File.Create(pathOut + "section_3.txt"));
                    for (int i = 0; i < aipd.cmds.Length; i++)
                        writer.Write("{0}:\n{1}", i, aipd.cmds[i].ToString());
                    writer.Dispose();

                    writer = new StreamWriter(File.Create(pathOut + "situation_return.txt"));
                    for (int i = aipd.sit_return_start; i <= aipd.sit_return_end; i++)
                        writer.WriteLine(aipd.sits[i].ToString());
                    writer.Dispose();

                    writer = new StreamWriter(File.Create(pathOut + "situation_attack.txt"));
                    for (int i = aipd.sit_attack_start; i <= aipd.sit_attack_end; i++)
                        writer.WriteLine(aipd.sits[i].ToString());
                    writer.Dispose();

                    writer = new StreamWriter(File.Create(pathOut + "situation_defend.txt"));
                    for (int i = aipd.sit_defend_start; i <= aipd.sit_defend_end; i++)
                        writer.WriteLine(aipd.sits[i].ToString());
                    writer.Dispose();
                }
                Console.WriteLine("Complete");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
