using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sm4shAIEditor.Filetypes;

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
        public UInt32 EntryCount { get; set; }
        public UInt32 SpecialMoveIndex { get; set; }
        public UInt32 SpecialIndexCount { get; set; }
        public List<attack_entry> attacks { get; set; }

        public attack_data(string fileDirectory)
        {
            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));

            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            EntryCount = task_helper.ReadReverseUInt32(ref binReader);
            SpecialMoveIndex = task_helper.ReadReverseUInt32(ref binReader);
            SpecialIndexCount = task_helper.ReadReverseUInt32(ref binReader);

            InitializeEntries(EntryCount, binReader);

            binReader.Close();
        }
        
        private void InitializeEntries(UInt32 entryCount, BinaryReader binReader)
        {
            attacks = new List<attack_entry>();
            for (Int32 i = 0; i < entryCount; i++)
            {
                //have to initialize the thing or get the error thing
                attacks.Add(new attack_entry());
                //ints
                attacks[i].SubactionID = task_helper.ReadReverseUInt16(ref binReader);
                attacks[i].Unk_1 = task_helper.ReadReverseUInt16(ref binReader);
                attacks[i].FirstFrame = task_helper.ReadReverseUInt16(ref binReader);
                attacks[i].LastFrame = task_helper.ReadReverseUInt16(ref binReader);

                //floats
                attacks[i].X1 = task_helper.ReadReverseFloat(ref binReader);
                attacks[i].X2 = task_helper.ReadReverseFloat(ref binReader);
                attacks[i].Y1 = task_helper.ReadReverseFloat(ref binReader);
                attacks[i].Y2 = task_helper.ReadReverseFloat(ref binReader);
            }
        }

        public class attack_entry
        {
            public UInt16 SubactionID { get; set; }
            public UInt16 Unk_1 { get; set; }
            public UInt16 FirstFrame { get; set; }
            public UInt16 LastFrame { get; set; }
            public Single X1 { get; set; }
            public Single X2 { get; set; }
            public Single Y1 { get; set; }
            public Single Y2 { get; set; }
        }
    }
}
