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
        public List<string> fighters { get; } //this list only generated as a convenience. Equivalent to the owner Value list
        
        //initialization nation
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
                    if (currentNames.Contains(fighterName))
                        throw new Exception(string.Format("Fighter '{0}' not loaded; it is already a member of the tree", fighterName));

                    AddFighterFiles(fighterDirectory);
                    fighters.Add(fighterName);
                    fighters.Sort();
                }
                catch (Exception exception)
                {
                    messageBox.Text += exception.Message + Environment.NewLine;
                }
            }
        }

        private void AddFighterFiles(string fighterDirectory)
        {
            string fighterParent = Directory.GetParent(fighterDirectory).FullName;
            string fighterName = fighterDirectory.Remove(0, fighterParent.Length + 1);
            bool empty = true;
            foreach (string fileType in task_helper.fileMagic.Keys)
            {
                string subDir = fighterDirectory + @"\script\ai\" + fileType;
                if (File.Exists(subDir))
                {
                    aiFiles.Add(subDir, fighterName);
                    empty = false;
                }
            }
            if (empty)
                throw new Exception(string.Format("Fighter '{0}' not loaded; could not find AI files", fighterName));
        }
    }
}
