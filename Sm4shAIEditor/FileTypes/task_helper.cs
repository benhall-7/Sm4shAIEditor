using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sm4shAIEditor.FileTypes
{
    public static class task_helper
    {
        public static Dictionary<string, Int32> fileAttributes = new Dictionary<string, int>()
        {
            { "attack_data.bin", 0x444b5441 },
            { "param.bin", 0x44504941 },
            { "param_nfp.bin", 0x44504941 },
            { "script.bin", 0x00000000 }
        };
        public static bool IsValidFile(string fileDirectory)
        {
            if (!File.Exists(fileDirectory))
                throw new Exception("file directory does not exist!");

            string fileParentDir = Directory.GetParent(fileDirectory).FullName;
            string fileName = fileDirectory.Remove(0, fileParentDir.Length + 1);

            if (!fileAttributes.ContainsKey(fileName))
                return false;

            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));
            Int32 magic = binReader.ReadInt32();
            binReader.Close();

            if (magic != fileAttributes[fileName])
                return false;

            return true;
        }
        public static float ReadReverseFloat(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static UInt16 ReadReverseUInt16(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static UInt32 ReadReverseUInt32(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
