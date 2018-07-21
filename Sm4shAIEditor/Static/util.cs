using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Sm4shAIEditor.Static
{
    public static class util
    {
        public static string workDirectory
        {
            get { return ConfigurationManager.AppSettings.Get("work_directory"); }
            set { ConfigurationManager.AppSettings.Set("work_directory", value); }
        }
        public static string compileDirectory
        {
            get { return ConfigurationManager.AppSettings.Get("export_directory"); }
            set { ConfigurationManager.AppSettings.Set("export_directory", value); }
        }
        public static string gameFighterDirectory
        {
            get { return ConfigurationManager.AppSettings.Get("game_fighter_directory"); }
            set { ConfigurationManager.AppSettings.Set("game_fighter_directory", value); }
        }
        public static Dictionary<string, Int32> fileMagic = new Dictionary<string, int>()
        {
            { "attack_data.bin", 0x444b5441 },
            { "param.bin", 0x44504941 },
            { "param_nfp.bin", 0x44504941 },
            { "script.bin", 0x00000000 }
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
        public static void WriteReverseByteArray(ref BinaryWriter binWriter, byte[] bytes)
        {
            Array.Reverse(bytes);
            binWriter.Write(bytes);
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
        public static uint Align0x10(uint position)
        {
            return ((position + 0xf) / 0x10) * 0x10;
        }
    }
}
