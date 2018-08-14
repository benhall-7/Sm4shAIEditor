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
                string genObject = string.Format("Generating {0} object... ", fileType);
                string asm = string.Format("Assembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {
                    
                }
                else if (type == AITree.AIType.script)
                {
                    Console.Write(genObject);
                    Dictionary<uint, string> acts = new Dictionary<uint, string>();
                    string[] IDs = File.ReadAllLines(pathIn + "acts.txt");
                    foreach(string ID in IDs)
                    {
                        acts.Add(uint.Parse(ID, NumberStyles.HexNumber), File.ReadAllText(pathIn + ID + ".txt"));
                    }
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
                string genObject = string.Format("Generating {0} object... ", fileType);
                string disasm = string.Format("Disassembling to {0}... ", pathOut);

                if (type == AITree.AIType.attack_data)
                {

                }
                else if (type == AITree.AIType.script)
                {
                    Console.Write(genObject);
                    script script = new script(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "acts.txt"));
                    foreach(var act in script.acts.Keys)
                    {
                        writer.WriteLine(act.ID.ToString("X4"));
                        File.WriteAllText(pathOut + act.ID.ToString("X4") + ".txt", act.ToString());
                    }
                    writer.Dispose();
                    Console.WriteLine("Done");
                }
                else //param and param_nfp will use same methods
                {
                    Console.WriteLine(genObject);
                    param param = new param(pathIn);
                    Console.Write(disasm);
                    if (!Directory.Exists(pathOut))
                        Directory.CreateDirectory(pathOut);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "situation.txt"));//pls change name to something better
                    for (int i = 0; i < param.freqs.Length; i++)
                    {
                        var freq = param.freqs[i];
                        writer.WriteLine(freq.ToString());
                    }
                    writer.Dispose();
                    Console.WriteLine("Done");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }
    }
}
