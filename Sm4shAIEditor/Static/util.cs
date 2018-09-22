using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Sm4shAIEditor.Static
{
    public static class util
    {
        public static string workDir
        {
            get { return Properties.Settings.Default.work_directory; }
            set { Properties.Settings.Default.work_directory = value; Properties.Settings.Default.Save(); }
        }
        public static string compDir
        {
            get { return Properties.Settings.Default.export_directory; }
            set { Properties.Settings.Default.export_directory = value; Properties.Settings.Default.Save(); }
        }
        public static string gameFtDir
        {
            get { return Properties.Settings.Default.game_fighter_directory; }
            set { Properties.Settings.Default.game_fighter_directory = value; Properties.Settings.Default.Save(); }
        }
        public static Dictionary<AITree.AIType, uint> fileMagic = new Dictionary<AITree.AIType, uint>()
        {
            { AITree.AIType.attack_data, 0x444b5441 },
            { AITree.AIType.param, 0x44504941 },
            { AITree.AIType.param_nfp, 0x44504941 },
            { AITree.AIType.script, 0x00000000 }
        };
        public static string GetFileName(string directory)
        {
            int index = directory.LastIndexOf('\\');
            return directory.Remove(0, index + 1);
        }
        public static string GetFolderName(string directory)
        {
            if (directory[directory.Length - 1] == '\\')
                directory = directory.Remove(directory.Length - 1, 1);
            int index = directory.LastIndexOf('\\');
            return directory.Remove(0, index + 1);
        }
        public static string CorrectFormatFolderPath(string directory)
        {
            if (directory[directory.Length - 1] != '\\')
                directory += '\\';
            return directory;
        }
        public static void WriteReverseByteArray(BinaryWriter binWriter, byte[] bytes)
        {
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static float ReadReverseFloat(BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static void WriteReverseFloat(BinaryWriter binWriter, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static UInt16 ReadReverseUInt16(BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static void WriteReverseUInt16(BinaryWriter binWriter, UInt16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static UInt32 ReadReverseUInt32(BinaryReader binReader)
        {
            byte[] bytes = binReader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static void WriteReverseUInt32(BinaryWriter binWriter, UInt32 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            binWriter.Write(bytes);
        }
        public static uint Align(uint position, uint alignTo)
        {
            return (position + alignTo - 1) / alignTo * alignTo;
        }
    }
}
