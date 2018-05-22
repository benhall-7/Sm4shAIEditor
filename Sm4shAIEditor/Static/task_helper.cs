using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sm4shAIEditor.Static
{
    public static class task_helper
    {
        public static Dictionary<string, Int32> fileMagic = new Dictionary<string, int>()
        {
            { "attack_data.bin", 0x444b5441 },
            { "param.bin", 0x44504941 },
            { "param_nfp.bin", 0x44504941 },
            { "script.bin", 0x00000000 }
        };
        public static string GetFileName(string directory)
        {
            string parent = Directory.GetParent(directory).FullName;
            return directory.Remove(0, parent.Length + 1);
        }
        public static float ReadReverseFloat(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static void WriteReverseFloat(ref BinaryWriter binWriter, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static UInt16 ReadReverseUInt16(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static void WriteReverseUInt16(ref BinaryWriter binWriter, UInt16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static UInt32 ReadReverseUInt32(ref BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static void WriteReverseUInt32(ref BinaryWriter binWriter, UInt32 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
    }
}
