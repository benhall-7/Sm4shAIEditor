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

        private class AIFighter
        {
            public AIFighter(string directory, string name)
            {
                fighterName = name;
                foreach (string fileType in fileTypes)
                {
                    string subDir = directory + @"\script\ai\" + fileType;
                    if (System.IO.File.Exists(subDir))
                    {
                        AIFile newFile = new AIFile(fileType, subDir);
                        files.Add(newFile);
                    }
                }
            }
            private string fighterName { get; set; }
            private List<AIFile> files { get; set; }
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

        //will not be a child of fighter, thus you may get the names confused if you have multiple
        public void AddFile(string fileDirectory, string fileName)
        {
            BinaryReader binReader = new BinaryReader(File.OpenRead(fileName));
            Int32 magic = binReader.ReadInt32();
            binReader.Close();
            if (magic == ATKD_Magic)
            {

            }
            else if (magic == AIPD_Magic)
            {

            }
            else if (magic == Script_Magic)
            {

            }
            AIFile newFile = new AIFile(fileDirectory,fileName);
        }
    }
}
