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
                pathIn = util.CorrectFormatFolderPath(pathIn);
                if (!Directory.Exists(pathIn))
                    throw new Exception("Input path directory not found");
                pathIn = Path.GetFullPath(pathIn);
                string fileType = util.GetFolderName(pathIn);
                if (!AITree.StringToAIType.ContainsKey(fileType))
                    throw new Exception(string.Format("Nonexistant file type {0}", fileType));
                
                pathOut = util.CorrectFormatFolderPath(pathOut);

                AITree.AIType type = AITree.StringToAIType[fileType];
                string init = string.Format("Initializing {0}... ", fileType);
                string asm = string.Format("Assembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {
                    Console.Write(init);
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
                    Console.WriteLine("Done");
                }
                else if (type == AITree.AIType.script)
                {
                    Console.Write(init);
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
                    Console.WriteLine("Done");
                }
                else //param and param_nfp will use same methods
                {
                    //can't be bothered to assemble the param files until I finish analysis + disassembly
                }
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
                if (!File.Exists(pathIn))
                    throw new Exception("Input path file not found");
                pathIn = Path.GetFullPath(pathIn);
                string fileType = Path.GetFileNameWithoutExtension(pathIn);
                if (!AITree.StringToAIType.ContainsKey(fileType))
                    throw new Exception(string.Format("Nonexistant file type {0}", fileType));

                pathOut = util.CorrectFormatFolderPath(pathOut);

                AITree.AIType type = AITree.StringToAIType[fileType];
                string init = string.Format("Initializing {0}... ", fileType);
                string disasm = string.Format("Disassembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {
                    Console.Write(init);
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
                    Console.WriteLine("Done");
                }
                else if (type == AITree.AIType.script)
                {
                    Console.Write(init);
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
                    Console.WriteLine("Done");
                }
                else //param and param_nfp will use same methods
                {
                    Console.Write(init);
                    param aipd = new param(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "situation_return.txt"));
                    for (int i = aipd.sec1_start; i <= aipd.sec1_end; i++)
                    {
                        var freq = aipd.sits[i];
                        writer.WriteLine(freq.ToString());
                    }
                    writer.Dispose();
                    writer = new StreamWriter(File.Create(pathOut + "situation_attack.txt"));
                    for (int i = aipd.sec2_start; i <= aipd.sec2_end; i++)
                    {
                        var freq = aipd.sits[i];
                        writer.WriteLine(freq.ToString());
                    }
                    writer.Dispose();
                    writer = new StreamWriter(File.Create(pathOut + "situation_defend.txt"));
                    for (int i = aipd.sec3_start; i <= aipd.sec3_end; i++)
                    {
                        var freq = aipd.sits[i];
                        writer.WriteLine(freq.ToString());
                    }
                    writer.Dispose();
                    Console.WriteLine("Done");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
