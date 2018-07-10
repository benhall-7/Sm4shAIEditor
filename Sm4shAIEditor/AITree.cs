using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public class AITree
    {
        public List<AIFighter> fighters = new List<AIFighter>();

        //no constructor here because it's been moved to two different methods

        public class AIFighter
        {
            public string name { get; set; }
            public List<AIFile> files = new List<AIFile>();

            public AIFighter(string name, List<AIFile> files)
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
                public string folder_address
                {
                    get
                    {
                        if (source == AISource.work) return util.workDirectory + parentName + @"\" + AITypeToString[type];
                        else if (source == AISource.compiled) return util.compileDirectory + parentName + @"\";
                        else if (source == AISource.game_file) return util.gameFighterDirectory + parentName + @"\script\ai\";
                        else throw new Exception("invalid AI file source");
                    }
                }

                //constructors
                public AIFile(AIType type, AISource source, string parentName)
                {
                    this.parentName = parentName;
                    //INCOMPLETE:
                    //I need to think of a good method to parse workspace, compile, and game directories into the ai tree instance
                }

                //methods
            }
        }

        public void InitNewProject(string[] fighters, AIType[] types)
        {
            foreach (string ft in fighters)
            {
                List<AIFighter.AIFile> fighterFiles = new List<AIFighter.AIFile>();
                foreach (AIType type in types)
                {
                    AIFighter.AIFile file = new AIFighter.AIFile(type, AISource.game_file, ft);
                    if (File.Exists(file.folder_address + AITypeToString[type] + ".bin"))
                        fighterFiles.Add(file);
                }
                if (fighterFiles.Count > 0) this.fighters.Add(new AIFighter(ft, fighterFiles));
            }
        }

        public void InitOpenProject()
        {
            //set data from the workspace. Fighter list is empty here
            foreach (string dir in Directory.EnumerateDirectories(util.workDirectory))
            {
                string name = util.GetFileName(dir);
                List<AIFighter.AIFile> files = new List<AIFighter.AIFile>();
                foreach (string subDir in Directory.EnumerateDirectories(dir).ToArray())
                {
                    AIType type = StringToAIType[util.GetFileName(subDir)];
                    files.Add(new AIFighter.AIFile(type, AISource.work, name));
                }
                if (files.Count > 0) this.fighters.Add(new AIFighter(name, files));
            }
            //set any remaining data from the compile directory
            foreach (string dir in Directory.EnumerateDirectories(util.compileDirectory))
            {
                string name = util.GetFileName(dir);
                List<AIFighter.AIFile> newFiles = new List<AIFighter.AIFile>();
                foreach (string file in Directory.EnumerateFiles(dir).ToArray())
                {
                    AIType type = StringToAIType[Path.GetFileNameWithoutExtension(file)];
                    newFiles.Add(new AIFighter.AIFile(type, AISource.compiled, name));
                }
                int ftIndex = this.fighters.FindIndex(ft => ft.name == name);
                if (ftIndex == -1) this.fighters.Add(new AIFighter(name, newFiles));//if the fighter doesn't exist, add it
                else
                {
                    foreach (AIFighter.AIFile newFile in newFiles)
                    {
                        //make sure the file type being added isn't in the list yet
                        if (!this.fighters[ftIndex].files.Exists(file => file.type == newFile.type))
                            this.fighters[ftIndex].files.Add(newFile);
                    }
                }
            }
        }

        public void AddGameFiles(string[] fighters, AIType[] types)
        {
            //TODO
        }

        public void Refresh()
        {
            //TODO
        }

        public void Sort()
        {
            //sorting the custom class, thanks to https://stackoverflow.com/a/3163963
            fighters.Sort(delegate (AITree.AIFighter ft1, AITree.AIFighter ft2) { return ft1.name.CompareTo(ft2.name); });
        }

        public void CheckNoFiles()
        {
            foreach(AIFighter ft in fighters.ToArray())
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
