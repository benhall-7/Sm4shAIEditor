using Sm4shAIEditor.Static;
using System;
using System.IO;

namespace Sm4shAIEditor
{
    class attack_data
    {
        public UInt32 count { get; set; }
        public UInt32 common_subactions { get; set; }
        public UInt32 special_subactions { get; set; }
        public attack[] attacks { get; set; }

        public attack_data(string fileDirectory)
        {
            BinaryReader binReader = new BinaryReader(File.OpenRead(fileDirectory));
            binReader.BaseStream.Seek(0x4, SeekOrigin.Begin);
            count = util.ReadReverseUInt32(binReader);
            common_subactions = util.ReadReverseUInt32(binReader);
            special_subactions = util.ReadReverseUInt32(binReader);
            InitializeEntries(count, binReader);
            binReader.Dispose();
        }
        
        private void InitializeEntries(UInt32 entryCount, BinaryReader binReader)
        {
            attacks = new attack[entryCount];
            for (Int32 i = 0; i < entryCount; i++)
            {
                attacks[i] = new attack();
                attacks[i].subaction = util.ReadReverseUInt16(binReader);
                binReader.BaseStream.Position += 0x2;//padding
                attacks[i].start_frame = util.ReadReverseUInt16(binReader);
                attacks[i].end_frame = util.ReadReverseUInt16(binReader);
                attacks[i].x1 = util.ReadReverseFloat(binReader);
                attacks[i].x2 = util.ReadReverseFloat(binReader);
                attacks[i].y1 = util.ReadReverseFloat(binReader);
                attacks[i].y2 = util.ReadReverseFloat(binReader);
            }
        }

        public struct attack
        {
            public UInt16 subaction { get; set; }
            public UInt16 start_frame { get; set; }
            public UInt16 end_frame { get; set; }
            public Single x1 { get; set; }
            public Single x2 { get; set; }
            public Single y1 { get; set; }
            public Single y2 { get; set; }
        }
    }
}
