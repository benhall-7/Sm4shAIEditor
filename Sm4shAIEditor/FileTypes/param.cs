using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sm4shAIEditor
{
    class param
    {
        public const int val_count = 0x28;
        public const int flg_count = 0x118;
        public const int cmd_count = 0x1c;

        public const int val_offset = 0x18;
        public const int flg_offset = 0x48;
        public const int cmd_offset = 0x168;
        public const int sit_offset = 0x1d8;

        public uint unk_size { get; set; }
        //padding for 0x10 alignment
        public byte sit_return_start { get; set; }
        public byte sit_return_end { get; set; }
        public byte sit_attack_start { get; set; }
        public byte sit_attack_end { get; set; }
        public byte sit_defend_start { get; set; }
        public byte sit_defend_end { get; set; }
        public sbyte[] vals { get; set; } = new sbyte[val_count];
        public byte[] flags { get; set; } = new byte[flg_count];
        public Cmd[] cmds { get; set; } = new Cmd[cmd_count];
        public Situation[] sits { get; set; }

        public param(string fileDir)
        {
            using (BinaryReader bR = new BinaryReader(File.OpenRead(fileDir)))
            {
                bR.BaseStream.Position = 0x4;
                unk_size = util.ReadReverseUInt32(bR);
                bR.BaseStream.Position = 0x10;
                sit_return_start = bR.ReadByte();
                sit_return_end = bR.ReadByte();
                sit_attack_start = bR.ReadByte();
                sit_attack_end = bR.ReadByte();
                sit_defend_start = bR.ReadByte();
                sit_defend_end = bR.ReadByte();
                bR.BaseStream.Position = val_offset;
                for (int i = 0; i < vals.Length; i++)
                    vals[i] = (sbyte)bR.ReadByte();
                bR.BaseStream.Position = flg_offset;
                for (int i = 0; i < flags.Length; i++)
                    flags[i] = bR.ReadByte();
                bR.BaseStream.Position = cmd_offset;
                //record the offsets
                uint[] cmd_offsets = new uint[cmd_count];
                for (int i = 0; i < cmd_count; i++)
                    cmd_offsets[i] = util.ReadReverseUInt32(bR);
                uint[] freq_offsets = new uint[sit_defend_end + 1];
                for (int i = 0; i < freq_offsets.Length; i++)
                    freq_offsets[i] = util.ReadReverseUInt32(bR);
                for (int i = 0; i < cmd_count; i++)
                {
                    bR.BaseStream.Position = cmd_offsets[i];
                    cmds[i] = new Cmd(bR);
                }
                sits = new Situation[sit_defend_end + 1];
                for (int i = 0; i <= sit_defend_end; i++)
                {
                    bR.BaseStream.Position = freq_offsets[i];
                    sits[i] = new Situation(bR);
                }
            }
        }
        public param(string val_dir, string flg_dir, string cmd_dir, string sit1_dir, string sit2_dir, string sit3_dir)
        {
            unk_size = 0xf;//this is the same for all files so I'm not adding it to disassembly until I know what it is
            string[] values = File.ReadAllLines(val_dir);
            for (int i = 0; i < val_count; i++)
                vals[i] = sbyte.Parse(values[i]);
            string[] flags = File.ReadAllLines(flg_dir);
            for (int i = 0; i < flg_count; i++)
                this.flags[i] = byte.Parse(flags[i]);
            //parse cmd file
            CustomStringReader sReader = new CustomStringReader(File.ReadAllText(cmd_dir));
            for (int i = 0; i < cmd_count; i++)
            {
                string word = sReader.ReadWord();
                sReader.SkipToEndLine();
                cmds[i] = new Cmd(sReader);
            }
            //parse situation files
            List<Situation> ls = new List<Situation>();
            byte sit_index = 0;
            foreach (string sit_dir in new string[] { sit1_dir, sit2_dir, sit3_dir })
            {
                if (sit_dir == sit1_dir) sit_return_start = sit_index;
                else if (sit_dir == sit2_dir) sit_attack_start = sit_index;
                else if (sit_dir == sit3_dir) sit_defend_start = sit_index;
                sReader.CharArray = File.ReadAllText(sit_dir).ToCharArray();
                while (true)
                {
                    string word = sReader.ReadWord();
                    if (word != "if")
                    {
                        if (word == null) break;//end of file
                        else throw new Exception("ERROR: expected 'if', received '" + word + "'");
                    }
                    ls.Add(new Situation(sReader));
                    sit_index++;
                }
                if (sit_dir == sit1_dir) sit_return_end = (byte)(sit_index - 1);
                else if (sit_dir == sit2_dir) sit_attack_end = (byte)(sit_index - 1);
                else if (sit_dir == sit3_dir) sit_defend_end = (byte)(sit_index - 1);
            }
            sits = ls.ToArray();
        }

        public class Cmd
        {
            public const int unk_count = 0x2e;
            public Unk[] unks { get; set; } = new Unk[unk_count];

            public Cmd(BinaryReader bR)
            {
                List<Unk> ls = new List<Unk>();
                for (byte i = bR.ReadByte(); i > 0; i = bR.ReadByte())//temporarily entrust that there are always 0x2e of these
                {
                    bR.BaseStream.Position += 3;//3 bytes padding
                    ls.Add(new Unk(i, util.ReadReverseUInt16(bR), util.ReadReverseUInt16(bR)));
                }
                unks = ls.ToArray();
            }
            public Cmd(CustomStringReader sR)
            {
                for (int i = 0; i < unk_count; i++)
                {
                    byte index = byte.Parse(sR.ReadWord());
                    sR.ReadUntilAnyOfChars(",", true);
                    ushort hi_prob = ushort.Parse(sR.ReadWord());
                    sR.ReadUntilAnyOfChars(",", true);
                    ushort lw_prob = ushort.Parse(sR.ReadWord());
                    unks[i] = new Unk(index, hi_prob, lw_prob);
                }
            }

            public struct Unk
            {
                public byte index;
                public ushort hi_rank_prob;
                public ushort lw_rank_prob;
                public Unk(byte index, ushort hi_rank_prob, ushort lw_rank_prob)
                {
                    this.index = index;
                    this.hi_rank_prob = hi_rank_prob;
                    this.lw_rank_prob = lw_rank_prob;
                }
            }

            public override string ToString()
            {
                string str = "";
                for (int i = 0; i < unks.Length; i++)
                    str += string.Format("\t{0}, {1}, {2}\n", unks[i].index, unks[1].hi_rank_prob, unks[i].lw_rank_prob);
                return str;
            }
        }

        public class Situation
        {
            public byte condition0 { get; set; }
            public byte condition1 { get; set; }
            public byte flags { get; set; }
            //0x1 -> negate condition0
            //0x2 -> negate condition1
            //0x4 -> || the conditions
            //0x8 -> ?
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
            public Situation(CustomStringReader sReader)
            {
                flags = 0;
                string word = sReader.ReadWord();
                if (word == null)
                {
                    string pre = sReader.ReadChar();
                    if (pre == "!")
                    {
                        flags |= 0x1;
                        word = sReader.ReadWord();
                    }
                }
                condition0 = (byte)checks.IndexOf(word);
                string op = sReader.ReadIfSymbols();
                if (op != "&&")
                {
                    if (op == "||") flags |= 0x4;
                    else throw new Exception("ERROR: invalid logical operator '" + op + "'");
                }
                word = sReader.ReadWord();
                if (word == null)
                {
                    string pre = sReader.ReadChar();
                    if (pre == "!")
                    {
                        flags |= 0x2;
                        word = sReader.ReadWord();
                    }
                }
                condition1 = (byte)checks.IndexOf(word);
                string post = sReader.ReadChar();
                if (post != ":")
                {
                    if (post == ";") flags |= 0x8;
                    else throw new Exception("ERROR: invalid character '" + post + "'");
                }
                //actions:
                while (true)
                {
                    action action = new action();
                    int oldPosition = sReader.Position;
                    string firstWord = sReader.ReadWord();
                    if (byte.TryParse(firstWord, out action.hi_rank_prob))
                    {
                        action.lw_rank_prob = byte.Parse(sReader.ReadWord());
                        action.max_rank = byte.Parse(sReader.ReadWord());
                        action.min_rank = byte.Parse(sReader.ReadWord());
                        action.act = ushort.Parse(sReader.ReadWord(), System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        sReader.Position = oldPosition;
                        break;
                    }
                }
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
                if ((flags & 0x1) > 0)
                    arg0 = "!";
                arg0 += checks[condition0];
                string arg1 = "";
                if ((flags & 0x2) > 0)
                    arg1 = "!";
                arg1 += checks[condition1];
                string op;
                if ((flags & 0x4) > 0) op = "||";
                else op = "&&";
                string delimiter = ":";
                if ((flags & 0x8) > 0) delimiter = ";";//still not sure about the usage here
                string ActOdds = "";
                foreach (action action in actions)
                    ActOdds += string.Format("\n\t{0}\t{1}\t{2}\t{3}\t{4}",
                        action.hi_rank_prob, action.lw_rank_prob, action.max_rank, action.min_rank, action.act.ToString("x4"));
                return string.Format("if {0} {1} {2}{3}{4}", arg0, op, arg1, delimiter, ActOdds);
            }
        }

        public int get_address_table_size()
        {
            return 4 * (cmd_count + sits.Length);
        }

        public static int act_id2cmd(int act_id)
        {
            //unused, right now anyway
            //basically uses act IDs in range [0x6011,0x602e), but unsure if 0x601f or 0x6020 is unused
            if (act_id > 0x6010)
            {
                if (act_id < 0x6020)
                    return act_id - 0x6011;
                if ((act_id -= 0x6012) < cmd_count)
                    return act_id;
            }
            return 0;
        }

        public static List<string> checks = new List<string>()
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
