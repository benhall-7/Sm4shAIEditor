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
        static string workDirectory { get; set; }
        static string compileDirectory { get; set; }
        static string gameFighterDirectory { get; set; }

        List<AIFighter> fighters = new List<AIFighter>();

        public class AIFighter
        {
            string name { get; set; }
            List<AIFile> files = new List<AIFile>();

            public class AIFile
            {
                //enums
                public enum Type { attack_data, param, param_nfp, script }
                public enum Source { work, compiled, game_file }

                //properties
                public AIFighter parent { get; private set; }

                public Type type { get; private set; }
                public Source source { get; private set; }
                public string folder_address
                {
                    get
                    {
                        if (source == Source.work)
                        {
                            string subDirectory = "";
                            if (type == Type.attack_data) subDirectory = @"atkd\";
                            else if (type == Type.param || type == Type.param_nfp) subDirectory = @"aipd\";
                            else if (type == Type.script) subDirectory = @"script\";
                            else throw new Exception("invalid AI File type");

                            return workDirectory + parent.name + @"\" + subDirectory;
                        }
                        else if (source == Source.compiled) return compileDirectory + parent.name + @"\";
                        else if (source == Source.game_file) return gameFighterDirectory + parent.name + @"\script\ai\";
                        else throw new Exception("invalid AI File source");
                    }
                }
            }
        }
        /*public Dictionary<string, string> aiFiles { get; } //Key = file directory; Value = owner
        public List<string> fighters { get; } //this list used to generate the tree parent nodes
        
        public AITree()
        {
            aiFiles = new Dictionary<string, string>();
            fighters = new List<string>();
        }

        //automatically will load the files that belong here
        public void AddFighters(string[] directoryList, ref RichTextBox messageBox)
        {
            foreach (string fighterDirectory in directoryList)
            {
                try
                {
                    if (!Directory.Exists(fighterDirectory))
                        throw new Exception(string.Format("Attempt to load file info from non-existant fighter '{0}'", fighterDirectory));

                    string fighterName = util.GetFileName(fighterDirectory);
                    string[] currentNames = fighters.ToArray();

                    LoadFighterFilesSource(fighterDirectory);
                    if (!fighters.Contains(fighterName))
                        fighters.Add(fighterName);
                    fighters.Sort();
                }
                catch (Exception exception)
                {
                    messageBox.Text += exception.Message + Environment.NewLine;
                }
            }
        }

        private void LoadFighterFilesSource(string fighterDirectory)
        {
            string fighterName = util.GetFileName(fighterDirectory);
            foreach (string fileType in util.fileMagic.Keys)
            {
                string subDir = fighterDirectory + @"\script\ai\" + fileType;
                if (File.Exists(subDir) && !aiFiles.Keys.Contains(subDir))
                    aiFiles.Add(subDir, fighterName);
            }
        }

        public void LoadFighterFilesWorkspace(string fighterDirectory)
        {
            string fighterName = util.GetFileName(fighterDirectory);
            foreach (string subDir in new string[] {"atkd", "aipd", "script"})
            {

            }
        }*/
    }
}
