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
        private List<AIFighter> fighters { get; set; }
        private List<AIFile> files { get; set; }
        
        //initialization nation
        public AITree()
        {
            fighters = new List<AIFighter>();
            files = new List<AIFile>();
        }

        private class AIFighter
        {
            public List<AIFile> files { get; set; }
            public string fighterName { get; set; }

            public AIFighter(string directory, string name)
            {
                files = new List<AIFile>();

                fighterName = name;
                foreach (string fileType in static_file_def.Names)
                {
                    string subDir = directory + @"\script\ai\" + fileType;
                    if (System.IO.File.Exists(subDir))
                    {
                        bool canLoad = static_file_def.IsValidFile(subDir);
                        if (canLoad)
                        {
                            AIFile newFile = new AIFile(subDir, fileType, fighterName);
                            files.Add(newFile);
                        }
                    }
                }
                if (files.Count == 0)
                    throw new ProgramException(Properties.Resources.FighterException2, fighterName);
            }
        }

        private class AIFile
        {
            public AIFile(string dir, string name)
            {
                fileName = name;
                fileDir = dir;
                relatedFighter = null;
            }
            public AIFile(string dir, string name, string owner)
            {
                fileName = name;
                fileDir = dir;
                relatedFighter = owner;
            }
            public string fileDir { get; set; }
            public string fileName { get; set; }
            public string relatedFighter { get; set; }
        }
        
        //including a fighter folder will automatically try to find the files
        public void AddFighter(string fighterDirectory, string fighterName)
        {
            string[] currentNames = GetFighterNames();
            if (currentNames.Contains(fighterName))
            {
                throw new ProgramException(Properties.Resources.FighterException1, fighterName);
            }
            AIFighter newFighter = new AIFighter(fighterDirectory, fighterName);
            fighters.Add(newFighter);
        }

        //will not be a child of fighter, thus you may get the names confused with multiple. They'll display it somehow...
        public void AddFile(string fileDirectory, string fileName)
        {
            //don't load the same file twice pls
            string[][] currentFiles = GetFileInfo();
            int currentFileCount = currentFiles.Length;
            for (int i = 0 ; i < currentFileCount; i++)
            {
                if (fileDirectory == currentFiles[i][0])
                    throw new ProgramException(Properties.Resources.FileException1, fileDirectory);
            }

            bool canLoad = static_file_def.IsValidFile(fileDirectory);
            if (canLoad)
            {
                AIFile newFile = new AIFile(fileDirectory, fileName);
                files.Add(newFile);
            }
            else
                throw new ProgramException(Properties.Resources.FileException2,fileDirectory);
        }

        public string[] GetFighterNames()
        {
            int fighterCount = fighters.Count;
            string[] fighterNames = new string[fighterCount];
            for (int i = 0; i < fighterCount; i++)
            {
                fighterNames[i] = fighters[i].fighterName;
            }
            return fighterNames;
        }

        public string[][] GetFighterFileInfoFromName(string fighterName)
        {
            //the name has to exist
            string[] currentNames = GetFighterNames();
            if (!currentNames.Contains(fighterName))
                throw new ProgramException(Properties.Resources.FighterException3, fighterName);
            //get the first index containing the fighter name
            int index = 0;
            while (index < currentNames.Length)
            {
                if (currentNames[index] == fighterName)
                    break;
                index++;
            }
            //this should never fire, but just in case
            if (index >= currentNames.Length)
                throw new Exception();

            /*
            at fighters[index]:
            {
                {fileDir1,fileName1},
                {fileDir2,fileName2},
                {fileDir3,fileName3},
                ... //total = fighters[index].files.Count
            }
            */
            int fighterFileCount = fighters[index].files.Count;
            string[][] fighterFileData = new string[fighterFileCount][];
            for (int i=0; i < fighterFileCount; i++)
            {
                fighterFileData[i] = new string[2];
                fighterFileData[i][0] = fighters[index].files[i].fileDir;
                fighterFileData[i][1] = fighters[index].files[i].fileName;
            }

            return fighterFileData;
        }

        public string[][] GetFileInfo()
        {
            /*
            {
                {fileDir1,fileName1},
                {fileDir2,fileName2},
                {fileDir3,fileName3},
                ... //total = files.Count
            }
            */
            int fileCount = files.Count;
            string[][] fileData = new string[fileCount][];
            for (int i = 0; i < fileCount; i++)
            {
                fileData[i] = new string[3];
                fileData[i][0] = files[i].fileDir;
                fileData[i][1] = files[i].fileName;
                fileData[i][2] = files[i].relatedFighter;
            }
            return fileData;
        }
    }
}
