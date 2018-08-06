using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sm4shAIEditor
{
    class param
    {
        public uint unk_size { get; set; }
        //padding for 0x10 alignment
        public byte unk_index0 { get; set; }
        public byte unk_index1 { get; set; }
        public byte unk_index2 { get; set; }
        public byte unk_index3 { get; set; }
        public byte unk_index4 { get; set; }
        public byte last_index { get; set; }
        //hword padding, for some reason there is "0x10" alignment after these indeces but offset by 0x8
        private byte[] Bytes_1 = new byte[0x28];
        public byte[] bytes_1 { get { return Bytes_1; } set { Bytes_1 = value; } }
        //dword padding
        private byte[] Flags = new byte[0x118];
        public byte[] flags { get { return Flags; } set { Flags = value; } }
        //dword padding
        private Unk1[] Unk1s = new Unk1[0x1c];
        public Unk1[] unk1s { get { return Unk1s; } set { Unk1s = value; } }
        public ActFreqDef[] freqs { get; set; }

        public param(string fileDir)
        {
            using (BinaryReader bR = new BinaryReader(File.OpenRead(fileDir)))
            {
                bR.BaseStream.Position = 0x4;
                unk_size = util.ReadReverseUInt32(bR);
                bR.BaseStream.Position = 0x10;
                unk_index0 = bR.ReadByte();//always 0. Is this padding?
                unk_index1 = bR.ReadByte();
                unk_index2 = bR.ReadByte();
                unk_index3 = bR.ReadByte();
                unk_index4 = bR.ReadByte();
                last_index = bR.ReadByte();
                bR.BaseStream.Position = 0x18;
                for (int i = 0; i < bytes_1.Length; i++)
                    bytes_1[i] = bR.ReadByte();
                bR.BaseStream.Position = 0x48;
                for (int i = 0; i < flags.Length; i++)
                    flags[i] = bR.ReadByte();
                bR.BaseStream.Position = 0x168;
                //record the offsets
                uint[] unk1_offsets = new uint[0x1c];
                for (int i = 0; i < 0x1c; i++)
                    unk1_offsets[i] = util.ReadReverseUInt32(bR);
                uint[] freq_offsets = new uint[last_index + 1];
                for (int i = 0; i < freq_offsets.Length; i++)
                    freq_offsets[i] = util.ReadReverseUInt32(bR);
                for (int i = 0; i < 0x1c; i++)
                {
                    bR.BaseStream.Position = unk1_offsets[i];
                    Unk1s[i] = new Unk1(bR);
                }
                freqs = new ActFreqDef[last_index + 1];
                for (int i = 0; i <= last_index; i++)
                {
                    bR.BaseStream.Position = freq_offsets[i];
                    freqs[i] = new ActFreqDef(bR);
                }
            }
        }

        public class Unk1
        {
            private static int count = 0x2e;
            private field[] Things = new field[count];
            public field[] things { get { return Things; } set { Things = value; } }

            public Unk1(BinaryReader bR)
            {
                byte i = 1;
                while (i <= count)
                {
                    byte index = bR.ReadByte();
                    bR.BaseStream.Position += 4;
                    things[i - 1] = new field(index, bR.ReadByte(), bR.ReadByte(), bR.ReadByte());
                    i++;
                }
            }

            public struct field
            {
                public byte index;
                public int pad;
                public byte unk1;
                public byte unk2;
                public byte unk3;
                public field(byte i, byte unk1, byte unk2, byte unk3)
                {
                    index = i;
                    pad = 0;
                    this.unk1 = unk1;
                    this.unk2 = unk2;
                    this.unk3 = unk3;
                }
            }
        }

        public class ActFreqDef
        {
            public byte condition { get; set; }
            public byte unk1 { get; set; }
            public byte unk2 { get; set; }
            public byte count { get; set; }
            public data[] events { get; set; }

            public ActFreqDef(BinaryReader bR)
            {
                condition = bR.ReadByte();
                unk1 = bR.ReadByte();
                unk2 = bR.ReadByte();
                count = bR.ReadByte();
                events = new data[count];
                for (int i = 0; i < count; i++)
                    events[i] = new data(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), util.ReadReverseUInt16(bR));
            }

            public struct data
            {
                byte min_prob;
                byte max_prob;
                byte max_rank;
                byte min_rank;
                ushort act;

                public data(byte min_prob, byte max_prob, byte max_rank, byte min_rank, ushort act)
                {
                    this.min_prob = min_prob;
                    this.max_prob = max_prob;
                    this.max_rank = max_rank;
                    this.min_rank = min_rank;
                    this.act = act;
                }
            }
        }

        public static string[] conditions = new string[]
        {
            "true",
            "unk_01",
            "unk_02",
            "unk_03",
            "tgt_front_far",
            "tgt_front_mid",
            "tgt_front_close",
            "unk_07",
            "unk_08",
            "unk_09",
            "unk_0a",
            "unk_0b",
            "unk_0c",
            "unk_0d",
            "unk_0e",
            "false",
            "unk_10",
            "unk_11",
            "unk_12",
            "unk_13",
            "unk_14",
            "unk_15",
            "unk_16",
            "unk_17",
            "unk_18",
            "unk_19",
            "unk_1a",
            "unk_1b",
            "unk_1c",
            "unk_1d",
            "unk_1e",
            "unk_1f",
            "unk_20",
            "unk_21",
            "unk_22",
            "unk_23",
            "unk_24",
            "unk_25",
            "unk_26",
            "unk_27",
            "unk_28",
            "unk_29",
            "unk_2a",
            "unk_2b",
            "unk_2c",
            "unk_2d",
            "unk_2e",
            "unk_2f",
            "unk_30",
            "unk_31",
            "unk_32",
            "unk_33",
            "unk_34",
            "unk_35",
            "unk_36",
            "unk_37",
            "unk_38",
            "unk_39",
            "unk_3a",
            "unk_3b",
            "unk_3c",
            "unk_3d",
            "unk_3e",
            "unk_3f",
            "unk_40",
            "unk_41",
            "unk_42",
            "unk_43",
            "unk_44"
        };
    }
}
