using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

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
            EntryCount = util.ReadReverseUInt32(ref binReader);
            SpecialMoveIndex = util.ReadReverseUInt32(ref binReader);
            SpecialIndexCount = util.ReadReverseUInt32(ref binReader);

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
                attacks[i].SubactionID = util.ReadReverseUInt16(ref binReader);
                attacks[i].Unk_1 = util.ReadReverseUInt16(ref binReader);
                attacks[i].FirstFrame = util.ReadReverseUInt16(ref binReader);
                attacks[i].LastFrame = util.ReadReverseUInt16(ref binReader);

                //floats
                attacks[i].X1 = util.ReadReverseFloat(ref binReader);
                attacks[i].X2 = util.ReadReverseFloat(ref binReader);
                attacks[i].Y1 = util.ReadReverseFloat(ref binReader);
                attacks[i].Y2 = util.ReadReverseFloat(ref binReader);
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
