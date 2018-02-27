using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*
AI ATKD File Format
Authors: Jam1garner, Ben Hall
struct HEADER{
    char magic[4];
    int entryCount;
    int firstSpecialSubactionIndex;
    int specialSubactionCount;
}
struct ENTRY{
    ushort subactionIndex;
    ushort unk;
    ushort firstFrame;
    ushort lastFrame;
    float xmin;
    float xmax;
    float ymin;
    float ymax;
}
*/

namespace Sm4shAIEditor
{
    class attack_data
    {
        public Int32 EntryCount { get; set; }
        public Int32 SpecialMoveIndex { get; set; }
        public Int32 SpecialIndexCount { get; set; }
        public List<attack_entry> attacks { get; set; }
        public attack_data(string fileDirectory)
        {
            byte[] temp4Bytes = new byte[4];
            using (BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory)))
            {
                binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                EntryCount = BitConverter.ToInt32(temp4Bytes, 0);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                SpecialMoveIndex = BitConverter.ToInt32(temp4Bytes, 0);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                SpecialIndexCount = BitConverter.ToInt32(temp4Bytes, 0);

                InitializeEntries(EntryCount, binReader);
            }
        }
        
        private void InitializeEntries(Int32 entryCount, BinaryReader binReader)
        {
            attacks = new List<attack_entry>();
            byte[] temp2Bytes = new byte[2];
            byte[] temp4Bytes = new byte[4];
            //who needs a real method to reverse endianness when you could repeat a task for every value...
            for (Int32 i = 0; i < entryCount; i++)
            {
                //have to initialize the thing or get the error thing
                attacks.Add(new attack_entry());
                //ints
                temp2Bytes = binReader.ReadBytes(2);
                Array.Reverse(temp2Bytes);
                attacks[i].SubactionID = BitConverter.ToInt16(temp2Bytes, 0);
                temp2Bytes = binReader.ReadBytes(2);
                Array.Reverse(temp2Bytes);
                attacks[i].Unk_1 = BitConverter.ToInt16(temp2Bytes, 0);
                temp2Bytes = binReader.ReadBytes(2);
                Array.Reverse(temp2Bytes);
                attacks[i].FirstFrame = BitConverter.ToInt16(temp2Bytes, 0);
                temp2Bytes = binReader.ReadBytes(2);
                Array.Reverse(temp2Bytes);
                attacks[i].LastFrame = BitConverter.ToInt16(temp2Bytes, 0);

                //floats
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                attacks[i].X1 = BitConverter.ToSingle(temp4Bytes, 0);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                attacks[i].X2 = BitConverter.ToSingle(temp4Bytes, 0);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                attacks[i].Y1 = BitConverter.ToSingle(temp4Bytes, 0);
                temp4Bytes = binReader.ReadBytes(4);
                Array.Reverse(temp4Bytes);
                attacks[i].Y2 = BitConverter.ToSingle(temp4Bytes, 0);
            }
        }
    }

    class attack_entry
    {
        public Int16 SubactionID { get; set; }
        public Int16 Unk_1 { get; set; }
        public Int16 FirstFrame { get; set; }
        public Int16 LastFrame { get; set; }
        public Single X1 { get; set; }
        public Single X2 { get; set; }
        public Single Y1 { get; set; }
        public Single Y2 { get; set; }
    }
}
