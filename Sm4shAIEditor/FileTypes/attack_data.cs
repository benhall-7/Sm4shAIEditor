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
                attacks[i].start = util.ReadReverseUInt16(binReader);
                attacks[i].end = util.ReadReverseUInt16(binReader);
                attacks[i].x1 = util.ReadReverseFloat(binReader);
                attacks[i].x2 = util.ReadReverseFloat(binReader);
                attacks[i].y1 = util.ReadReverseFloat(binReader);
                attacks[i].y2 = util.ReadReverseFloat(binReader);
            }
        }

        public struct attack
        {
            public ushort subaction { get; set; }
            public ushort start { get; set; }
            public ushort end { get; set; }
            public float x1 { get; set; }
            public float x2 { get; set; }
            public float y1 { get; set; }
            public float y2 { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: [{1}, {2}], [{3}, {4}, {5}, {6}]", subaction, start, end, x1, x2, y1, y2);
            }
        }
    }
}
