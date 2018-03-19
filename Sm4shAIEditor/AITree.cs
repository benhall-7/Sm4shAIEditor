using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Sm4shAIEditor.FileTypes;

namespace Sm4shAIEditor
{
    public class AITree
    {
        public Dictionary<string, string> aiFiles { get; } //Key = file directory; Value = owner
        public List<string> fighters { get; } //this list only generated as a convenience. Equivalent to the owner Value list
        
        //initialization nation
        public AITree()
        {
            aiFiles = new Dictionary<string, string>(0);
            fighters = new List<string>(0);
        }

        //automatically will load the files that belong here
        public void AddFighters(string[] directoryList, ref RichTextBox messageBox)
        {
            foreach (string fighterDirectory in directoryList)
            {
                try
                {
                    if (!Directory.Exists(fighterDirectory))
                        throw new ProgramException(Properties.Resources.FighterException3, fighterDirectory);

                    string fighterParent = Directory.GetParent(fighterDirectory).FullName;
                    string fighterName = fighterDirectory.Remove(0, fighterParent.Length + 1);
                    string[] currentNames = fighters.ToArray();
                    if (currentNames.Contains(fighterName))
                        throw new ProgramException(Properties.Resources.FighterException1, fighterName);

                    AddFighterFiles(fighterDirectory);
                    fighters.Add(fighterName);
                }
                catch (ProgramException exception)
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
            foreach (string fileType in static_file_def.FileMagic.Keys)
            {
                string subDir = fighterDirectory + @"\script\ai\" + fileType;
                if (File.Exists(subDir))
                {
                    bool canLoad = static_file_def.IsValidFile(subDir);
                    if (canLoad)
                    {
                        aiFiles.Add(subDir, fighterName);
                        empty = false;
                    }
                }
            }
            if (empty)
                throw new ProgramException(Properties.Resources.FighterException2, fighterName);
        }
        
        public void AddFiles(string[] directoryList, ref RichTextBox messageBox)
        {
            foreach (string fileDirectory in directoryList)
            {
                try
                {
                    //don't load the same file twice pls
                    if (aiFiles.ContainsKey(fileDirectory))
                        throw new ProgramException(Properties.Resources.FileException1, fileDirectory);

                    bool canLoad = static_file_def.IsValidFile(fileDirectory);
                    if (canLoad)
                        aiFiles.Add(fileDirectory, null);
                    else
                        throw new ProgramException(Properties.Resources.FileException2, fileDirectory);
                }
                catch (ProgramException exception)
                {
                    messageBox.Text += exception.Message + Environment.NewLine;
                }
            }
        }
        public void GetFighterFileInfoFromName(string fighterName)
        {
            //temporarily nulled
        }

        public void GetFileInfo()
        {
            //temporarily nulled
        }
    }
}
