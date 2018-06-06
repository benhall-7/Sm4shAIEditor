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
        public Dictionary<string, string> aiFiles { get; } //Key = file directory; Value = owner
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

                    string fighterName = task_helper.GetFileName(fighterDirectory);
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
            string fighterName = task_helper.GetFileName(fighterDirectory);
            foreach (string fileType in task_helper.fileMagic.Keys)
            {
                string subDir = fighterDirectory + @"\script\ai\" + fileType;
                if (File.Exists(subDir) && !aiFiles.Keys.Contains(subDir))
                    aiFiles.Add(subDir, fighterName);
            }
        }

        public void LoadFighterFilesWorkspace(string fighterDirectory)
        {
            string fighterName = task_helper.GetFileName(fighterDirectory);
            foreach (string subDir in new string[] {"atkd", "aipd", "script"})
            {

            }
        }
    }
}
