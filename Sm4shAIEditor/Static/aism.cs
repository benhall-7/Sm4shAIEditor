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
                util.CorrectFormatFolderPath(ref pathIn);
                if (!Directory.Exists(pathIn))
                    throw new Exception("Input path directory not found");
                pathIn = Path.GetFullPath(pathIn);
                string fileType = util.GetFolderName(pathIn);
                if (!AITree.StringToAIType.ContainsKey(fileType))
                    throw new Exception(string.Format("Nonexistant file type {0}", fileType));

                util.CorrectFormatFolderPath(ref pathOut);
                if (!Directory.Exists(pathOut))
                    Directory.CreateDirectory(pathOut);
                pathOut = Path.GetFullPath(pathOut);//is this actually necessary for anything?

                AITree.AIType type = AITree.StringToAIType[fileType];
                string genObject = string.Format("Generating {0} object... ", fileType);
                string asm = string.Format("Assembling to {0}... ", pathOut);

                Console.WriteLine(genObject);
                if (type == AITree.AIType.attack_data)
                {
                    
                }
                else if (type == AITree.AIType.script)
                {
                    Dictionary<uint, string> acts = new Dictionary<uint, string>();
                    string[] IDs = File.ReadAllLines(pathIn + "acts.txt");
                    foreach(string ID in IDs)
                    {
                        acts.Add(uint.Parse(ID, NumberStyles.HexNumber), File.ReadAllText(pathIn + ID + ".txt"));
                    }
                    script script = new script(acts);
                    
                    Console.Write(asm);
                    BinaryWriter binWriter = new BinaryWriter(File.Create(pathOut + fileType + ".bin"));
                    binWriter.Write((int)0);//pad
                    util.WriteReverseUInt32(ref binWriter, script.actScriptCount);
                    binWriter.Write((long)0);//pad
                    foreach (var act in script.acts.Keys)
                        util.WriteReverseUInt32(ref binWriter, script.acts[act]);
                    foreach (var act in script.acts.Keys)
                    {
                        binWriter.BaseStream.Position = script.acts[act];
                        util.WriteReverseUInt32(ref binWriter, act.ID);
                        util.WriteReverseUInt32(ref binWriter, act.ScriptOffset);
                        util.WriteReverseUInt32(ref binWriter, act.ScriptFloatOffset);
                        util.WriteReverseUInt16(ref binWriter, act.VarCount);
                        binWriter.Write((short)0);
                        foreach (var cmd in act.CmdList)
                        {
                            binWriter.Write(cmd.ID);
                            binWriter.Write(cmd.ParamCount);
                            util.WriteReverseUInt16(ref binWriter, cmd.Size);
                            foreach (var param in cmd.ParamList)
                                util.WriteReverseUInt32(ref binWriter, param);
                        }
                        foreach (var value in act.ScriptFloats.Values)
                            util.WriteReverseFloat(ref binWriter, value);
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

                util.CorrectFormatFolderPath(ref pathOut);
                if (!Directory.Exists(pathOut))
                    Directory.CreateDirectory(pathOut);
                pathOut = Path.GetFullPath(pathOut);//is this actually necessary for anything?

                AITree.AIType type = AITree.StringToAIType[fileType];
                string genObject = string.Format("Generating {0} object... ", fileType);
                string disasm = string.Format("Disassembling to {0}... ", pathOut);

                Console.WriteLine(genObject);
                if (type == AITree.AIType.attack_data)
                {

                }
                else if (type == AITree.AIType.script)
                {
                    script script = new script(pathIn);
                    Console.Write(disasm);
                    StreamWriter writer = new StreamWriter(File.Create(pathOut + "acts.txt"));
                    foreach(var act in script.acts.Keys)
                    {
                        writer.WriteLine(act.ID.ToString("x4"));
                        File.WriteAllText(pathOut + act.ID.ToString("x4") + ".txt", act.DecompAct());
                    }
                    writer.Dispose();
                    Console.WriteLine("Done");
                }
                else //param and param_nfp will use same methods
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }
    }
}
