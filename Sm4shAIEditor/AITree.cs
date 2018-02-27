using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sm4shAIEditor
{
    public class AITree
    {
        private List<AIFighter> fighters { get; set; }
        private List<AIFile> files { get; set; }

        private static string[] fileTypes =
        {
            "attack_data.bin",
            "param.bin",
            "param_nfp.bin",
            "script.bin"
        };
        private static Int32 ATKD_Magic = 0x444b5441;
        private static Int32 AIPD_Magic = 0x44504941;
        private static Int32 Script_Magic = 0;
        
        //initialization nation
        public AITree()
        {
            fighters = new List<AIFighter>();
            files = new List<AIFile>();
        }

        private class AIFighter
        {
            private List<AIFile> files { get; set; }
            private string fighterName { get; set; }

            public AIFighter(string directory, string name)
            {
                files = new List<AIFile>();

                fighterName = name;
                foreach (string fileType in fileTypes)
                {
                    string subDir = directory + @"\script\ai\" + fileType;
                    if (System.IO.File.Exists(subDir))
                    {
                        bool canLoad = CheckFileHeader(subDir, fileType);
                        if (canLoad)
                        {
                            AIFile newFile = new AIFile(subDir, fileType);
                            files.Add(newFile);
                        }
                    }
                }
            }
        }

        private class AIFile
        {
            public AIFile(string dir, string name)
            {
                fileName = name;
                fileDir = dir;
            }
            private string fileName { get; set; }
            private string fileDir { get; set; }
        }

        //including a fighter folder will automatically try to find the files
        public void AddFighter(string fighterDirectory, string fighterName)
        {
            AIFighter newFighter = new AIFighter(fighterDirectory, fighterName);
            fighters.Add(newFighter);
        }

        //will not be a child of fighter, thus you may get the names confused with multiple. They'll display it somehow...
        public void AddFile(string fileDirectory, string fileName)
        {
            bool canLoad = CheckFileHeader(fileDirectory, fileName);
            if (canLoad)
            {
                AIFile newFile = new AIFile(fileDirectory, fileName);
                files.Add(newFile);
            }
            else
                throw new Exception();
        }

        private static bool CheckFileHeader(string fileDirectory, string fileName)
        {
            bool isType = false;
            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));
            Int32 magic = binReader.ReadInt32();
            binReader.Close();
            if (fileName == fileTypes[0] && magic == ATKD_Magic)
                isType = true;
            else if ((fileName == fileTypes[1] || fileName == fileTypes[2]) && magic == AIPD_Magic)
                isType = true;
            else if (fileName == fileTypes[3] && magic == Script_Magic)
                isType = true;

            return isType;
        }
    }
}
