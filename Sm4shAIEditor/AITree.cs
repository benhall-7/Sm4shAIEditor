using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        public AITree(string[] directoryList, bool fighterOption)
        {
            aiFiles = new Dictionary<string, string>(0);
            fighters = new List<string>(0);
            if (fighterOption)
            {
                AddFighters(directoryList);
            }
            else //raw file directories with no owner
            {
                AddFiles(directoryList);
            }
        }

        //automatically will load the files that belong here
        public void AddFighters(string[] directoryList)
        {
            foreach (string fighterDirectory in directoryList)
            {
                if (!Directory.Exists(fighterDirectory))
                    throw new Exception(); //PLEASE SPECIFY

                string fighterParent = Directory.GetParent(fighterDirectory).FullName;
                string fighterName = fighterDirectory.Remove(0, fighterParent.Length + 1);
                string[] currentNames = fighters.ToArray();
                if (currentNames.Contains(fighterName))
                    throw new ProgramException(Properties.Resources.FighterException1, fighterName);
                else
                    fighters.Add(fighterName);

                AddFighterFiles(fighterDirectory);
            }
        }

        private void AddFighterFiles(string fighterDirectory)
        {
            string fighterParent = Directory.GetParent(fighterDirectory).FullName;
            string fighterName = fighterDirectory.Remove(0, fighterParent.Length + 1);
            bool empty = true;
            foreach (string fileType in static_file_def.Names)
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

        //will not be a child of fighter, thus you may get the names confused with multiple. They'll display it somehow...
        public void AddFiles(string[] directoryList)
        {
            foreach (string fileDirectory in directoryList)
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
