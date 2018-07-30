using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sm4shAIEditor
{
    public class AITree
    {
        public List<AIFt> fighters = new List<AIFt>();

        public class AIFt
        {
            public string name { get; set; }
            public List<AIFile> files = new List<AIFile>();

            public AIFt(string name, List<AIFile> files)
            {
                this.name = name;
                this.files = files;
            }

            public class AIFile
            {
                //properties
                private string parentName { get; set; }

                public AIType type { get; private set; }
                public AISource source { get; private set; }
                public string folder_address { get { return GetFolderPath(parentName, type, source); } }

                //constructors
                public AIFile(string parentName, AIType type, AISource source)
                {
                    this.type = type;
                    this.source = source;
                    this.parentName = parentName;
                    SetAsWork();
                }

                private void SetAsWork()
                {
                    if (source == AISource.work)
                        return;
                    string pathIn = folder_address + AITypeToString[type] + ".bin";
                    string pathOut = util.workDir + parentName + "\\" + AITypeToString[type] + "\\";
                    aism.DisassembleFile(pathIn, pathOut);
                    source = AISource.work;
                }

                //methods
                public static string GetFolderPath(string name, AIType type, AISource source)
                {
                    if (source == AISource.work) return util.workDir + name + "\\" + AITypeToString[type];
                    else if (source == AISource.compiled) return util.compDir + name + "\\";
                    else return util.gameFtDir + name + "\\script\\ai\\";
                }
            }
        }

        public void InitNewProject(string[] fighters, AIType[] types)
        {
            foreach (string name in fighters)
            {
                int fileCount = 0;
                foreach (AIType type in types)
                {
                    if (File.Exists(AIFt.AIFile.GetFolderPath(name, type, AISource.game_file) + AITypeToString[type] + ".bin"))
                    {
                        try
                        {
                            AIFt.AIFile file = new AIFt.AIFile(name, type, AISource.game_file);
                            fileCount++;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error on file load ({0}: {1})", name, AITypeToString[type]);
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                if (fileCount == 0) Console.WriteLine("NOTICE: no AI files loaded from fighter '{0}'", name);
                InitOpenProject();
            }
        }

        public void InitOpenProject()
        {
            //set data from the workspace. Fighter list is empty here
            foreach (string dir in Directory.EnumerateDirectories(util.workDir))
            {
                string name = util.GetFolderName(dir);
                List<AIFt.AIFile> files = new List<AIFt.AIFile>();
                foreach (string subDir in Directory.EnumerateDirectories(dir).ToArray())
                {
                    AIType type = StringToAIType[util.GetFileName(subDir)];
                    files.Add(new AIFt.AIFile(name, type, AISource.work));
                }
                if (files.Count > 0) this.fighters.Add(new AIFt(name, files));
            }
        }

        public void AddProjectFiles(string[] fighters, AIType[] types, AISource source)
        {
            foreach (string name in fighters)
            {
                List<AIFt.AIFile> newFiles = new List<AIFt.AIFile>();
                int ftIndex = this.fighters.FindIndex(ft => ft.name == name);
                foreach (AIType type in types)
                {
                    if (File.Exists(AIFt.AIFile.GetFolderPath(name, type, source) + AITypeToString[type] + ".bin")
                        && (ftIndex == -1 || !this.fighters[ftIndex].files.Exists(file => file.type == type)))
                    {
                        try
                        {
                            AIFt.AIFile file = new AIFt.AIFile(name, type, source);
                            if (Directory.Exists(file.folder_address))
                                newFiles.Add(file);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error on file load ({0}: {1})", name, AITypeToString[type]);
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                //if the fighter doesn't exist, add it
                if (ftIndex == -1 && newFiles.Count > 0) this.fighters.Add(new AIFt(name, newFiles));
                else foreach (var file in newFiles) this.fighters[ftIndex].files.Add(file);
                Sort();
            }
        }

        public void Sort()
        {
            //sorting the custom class, thanks to https://stackoverflow.com/a/3163963
            foreach (AIFt fighter in fighters)
                fighter.files.Sort(delegate (AIFt.AIFile f1, AIFt.AIFile f2) { return f1.type.CompareTo(f2.type); });
            fighters.Sort(delegate (AIFt ft1, AIFt ft2) { return ft1.name.CompareTo(ft2.name); });
        }

        //defunct methods, maybe save for later?
        public void Refresh()
        {
            //TODO
        }

        public void CheckNoFiles()
        {
            foreach(AIFt ft in fighters.ToArray())
            {
                if (ft.files.Count == 0)
                {
                    fighters.Remove(ft);
                }
            }
        }

        public enum AIType { attack_data, param, param_nfp, script }
        public enum AISource { work, compiled, game_file }
        public static Dictionary<AIType, string> AITypeToString = new Dictionary<AIType, string>()
        {
            { AIType.attack_data, "attack_data" },
            { AIType.param, "param" },
            { AIType.param_nfp, "param_nfp" },
            { AIType.script, "script" }
        };
        public static Dictionary<string, AIType> StringToAIType = new Dictionary<string, AIType>()
        {
            { "attack_data", AIType.attack_data },
            { "param", AIType.param },
            { "param_nfp", AIType.param_nfp },
            { "script", AIType.script }
        };
    }
}
