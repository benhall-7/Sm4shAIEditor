using Sm4shAIEditor.Static;
using System.Collections.Generic;
using System.IO;

namespace Sm4shAIEditor
{
    class param
    {
        public uint unk_size { get; set; }
        //padding for 0x10 alignment
        public byte sec1_start { get; set; }
        public byte sec1_end { get; set; }
        public byte sec2_start { get; set; }
        public byte sec2_end { get; set; }
        public byte sec3_start { get; set; }
        public byte sec3_end { get; set; }
        //hword padding, for some reason there is "0x10" alignment after these indeces but offset by 0x8
        private byte[] Bytes_1 = new byte[0x28];
        public byte[] bytes_1 { get { return Bytes_1; } set { Bytes_1 = value; } }
        //dword padding
        private byte[] Flags = new byte[0x118];
        public byte[] flags { get { return Flags; } set { Flags = value; } }
        //dword padding
        private Unk1[] Unk1s = new Unk1[0x1c];
        public Unk1[] unk1s { get { return Unk1s; } set { Unk1s = value; } }
        public Situation[] sits { get; set; }

        public param(string fileDir)
        {
            using (BinaryReader bR = new BinaryReader(File.OpenRead(fileDir)))
            {
                bR.BaseStream.Position = 0x4;
                unk_size = util.ReadReverseUInt32(bR);
                bR.BaseStream.Position = 0x10;
                sec1_start = bR.ReadByte();//always 0. Is this padding?
                sec1_end = bR.ReadByte();
                sec2_start = bR.ReadByte();
                sec2_end = bR.ReadByte();
                sec3_start = bR.ReadByte();
                sec3_end = bR.ReadByte();
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
                uint[] freq_offsets = new uint[sec3_end + 1];
                for (int i = 0; i < freq_offsets.Length; i++)
                    freq_offsets[i] = util.ReadReverseUInt32(bR);
                for (int i = 0; i < 0x1c; i++)
                {
                    bR.BaseStream.Position = unk1_offsets[i];
                    Unk1s[i] = new Unk1(bR);
                }
                sits = new Situation[sec3_end + 1];
                for (int i = 0; i <= sec3_end; i++)
                {
                    bR.BaseStream.Position = freq_offsets[i];
                    sits[i] = new Situation(bR);
                }
            }
        }

        public class Unk1
        {
            public field[] things { get; set; }

            public Unk1(BinaryReader bR)
            {
                List<field> ls = new List<field>();
                for (byte i = bR.ReadByte(); i > 0; i = bR.ReadByte())
                {
                    bR.BaseStream.Position += 3;//3 bytes padding
                    ls.Add(new field(i, util.ReadReverseUInt16(bR), util.ReadReverseUInt16(bR)));
                }
                things = ls.ToArray();
            }

            public struct field
            {
                public byte index;
                public ushort hi_rank_prob;
                public ushort lw_rank_prob;
                public field(byte index, ushort hi_rank_prob, ushort lw_rank_prob)
                {
                    this.index = index;
                    this.hi_rank_prob = hi_rank_prob;
                    this.lw_rank_prob = lw_rank_prob;
                }
            }
        }

        public class Situation
        {
            public byte condition0 { get; set; }
            public byte condition1 { get; set; }
            public byte flags { get; set; }
            //0x1 -> negate condition0
            //0x2 -> negate condition1
            //0x4 -> && the conditions
            //0x8 -> 
            public byte count { get; set; }
            public action[] actions { get; set; }

            public Situation(BinaryReader bR)
            {
                condition0 = bR.ReadByte();
                condition1 = bR.ReadByte();
                flags = bR.ReadByte();
                count = bR.ReadByte();
                actions = new action[count];
                for (int i = 0; i < count; i++)
                    actions[i] = new action(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), util.ReadReverseUInt16(bR));
            }

            public struct action
            {
                public byte hi_rank_prob;
                public byte lw_rank_prob;
                public byte max_rank;
                public byte min_rank;
                public ushort act;

                public action(byte hi_rank_prob, byte lw_rank_prob, byte max_rank, byte min_rank, ushort act)
                {
                    this.hi_rank_prob = hi_rank_prob;
                    this.lw_rank_prob = lw_rank_prob;
                    this.max_rank = max_rank;
                    this.min_rank = min_rank;
                    this.act = act;
                }
            }

            public override string ToString()
            {
                string arg0 = "";
                if ((flags & 0x1) == 0x1)
                    arg0 = "!";
                arg0 += checks[condition0];
                string arg1 = "";
                if ((flags & 0x2) == 0x2)
                    arg1 = "!";
                arg1 += checks[condition1];
                if ((flags & 0x8) == 0x8)
                    arg1 += " (flag 0x8)";
                string op;
                if ((flags & 0x4) == 0x4) op = "||";
                else op = "&&";
                string ActOdds = "";
                foreach (action action in actions)
                    ActOdds += string.Format("\n\t{0}\t{1}\t{2}\t{3}\t{4}",
                        action.hi_rank_prob, action.lw_rank_prob, action.max_rank, action.min_rank, action.act.ToString("x4"));
                return string.Format("if {0} {1} {2}:{3}", arg0, op, arg1, ActOdds);
            }
        }

        public static string[] checks = new string[]
        {
            "true",
            "unk_01",
            "unk_02",
            "unk_03",
            "tgt_front_far",
            "tgt_front_mid",
            "tgt_front_close",
            "unk_07",
            "tgt_back",
            "unk_09",
            "unk_0a",
            "unk_0b",
            "unk_0c",
            "mario_0d",
            "unk_0e",
            "null_0f",//always false ; used for Pokemon Trainer (Brawl) when low stamina
            "diddy_10",
            "unk_11",
            "falling",
            "aerial",
            "tgt_front_very_close",
            "tgt_very_close",
            "unk_16",
            "peach_17",
            "unk_18",
            "unk_19",
            "unk_1a",
            "unk_1b",
            "unk_1c",
            "diddy_1d",
            "rosetta_1e",
            "rosetta_1f",
            "rosetta_20",
            "rosetta_21",
            "rosetta_22",
            "rosetta_23",
            "rosetta_24",
            "littlemac_25",
            "wiifit_26",
            "wiifit_27",
            "rockman_28",
            "lucario_29",
            "murabito_2a",
            "murabito_2b",
            "murabito_2c",
            "murabito_2d",
            "murabito_2e",
            "reflet_2f",
            "reflet_30",
            "reflet_31",
            "reflet_32",
            "reflet_33",
            "sheik_34",
            "fox_falco_35",
            "szerosuit_36",
            "szerosuit_37",
            "palutena_38",
            "miifighter_39",
            "miifighter_3a",
            "ryu_3b",
            "ryu_3c",
            "ryu_3d",
            "ryu_3e",
            "ryu_3f",
            "ryu_40",
            "ryu_41",
            "ryu_42",
            "bayo_43",
            "bayo_44"
        };
    }
}
