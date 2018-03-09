using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sm4shAIEditor.FileTypes
{
    public static class static_file_def
    {
        public static string[] Names = 
        {
            "attack_data.bin",
            "param.bin",
            "param_nfp.bin",
            "script.bin"
        };
        public static Int32[] Magic =
        {
            0x444b5441,
            0x44504941,
            0x44504941,
            0x00000000
        };
        public static bool IsValidFile(string fileDirectory)
        {
            if (!File.Exists(fileDirectory))
                throw new Exception("file directory does not exist!");

            bool validity = false;
            string fileParentDir = Directory.GetParent(fileDirectory).FullName;
            string fileName = fileDirectory.Remove(0, fileParentDir.Length + 1);

            int typeNumber = -1;
            for (int i = 0; i < Names.Length; i++)
            {
                if (fileName == Names[i])
                {
                    typeNumber = i;
                    break;
                }
            }
            if (typeNumber == -1)
                return validity;

            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));
            Int32 magic = binReader.ReadInt32();
            binReader.Close();

            if (magic == Magic[typeNumber])
                validity = true;

            return validity;
        }
    }
}
