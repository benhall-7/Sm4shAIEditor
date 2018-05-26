using System;
using System.Collections.Generic;

namespace Sm4shAIEditor.Static
{
    static class script_data
    {
        public static List<string> cmds = new List<string>()
        {
            "End",
            "SetVar",//syntax varX = item
            "SetVec",//syntax vecX = item
            "Label",
            "Return",
            "SearchLabel",
            "If",
            "IfNot",//special syntax, the "Not" portion is in the params
            "Else",
            "EndIf",//syntax }
            "SetStickRel",
            "SetButton",
            "VarAdd",//syntax varX +=
            "VarSub",//syntax varX -=
            "VarMul",//syntax varX *=
            "VarDiv",//syntax varX /=
            "VecAdd",//syntax vecX +=
            "VecSub",//syntax vecX -=
            "VecMul",//syntax vecX *=
            "VecDiv",//syntax vecX /=
            "GoToCurrentLabel",
            "SetVarRandf",
            "Or",//syntax ||
            "OrNot",//syntax || !
            "And",//syntax &&
            "AndNot",//syntax && !
            "SetFrame",
            "SetAct",
            "GoToLabel",
            "GetNearestCliffRel",
            "VarAbs",
            "SetStickAbs",
            "Unk_20",
            "Unk_21",
            "SetWait",
            "Unk_23",//something with position and cliff
            "CalcArriveFrameX",
            "CalcArriveFrameY",
            "GetShieldHP",
            "StagePtRand",
            "CalcArrivePosX",
            "CalcArrivePosY",
            "AtkdDiceRoll",
            "Unk_2b",
            "Norm",
            "Dot",
            "CalcArrivePosSec",
            "Unk_2f",
            "Unk_30",
            "GetNearestCliffAbs",
            "ClearStick",
            "Unk_33",//new to Smash 4
            "Unk_34",
            "Unk_35",
            "Unk_36",//this one is unused
            "Unk_37",
            "Unk_38",
            "Unk_39"
        };

        //schema: for each possible overflow of each command passed through this, the outline of each int[] is as follows
        //CmdID, arg0, arg1, arg2, etc. For each arg:
        //0 = raw value
        //1 = variable set
        //2 = vector set
        //3 = get_script_value
        //if a command or its args aren't represented in the list it will go through a switch for special cases/default behavior
        public static List<byte[]> cmd_args = new List<byte[]>()
        {
            new byte[] {0x03, 0},//label
            new byte[] {0x05, 0},//search label
            new byte[] {0x0a, 3, 3},//set stick rel
            new byte[] {0x15, 1, 3, 3, 3, 3},//set var with randf
            new byte[] {0x1a, 3},//set act timer/set frame
            new byte[] {0x1b, 0},//set act
            new byte[] {0x1c, 0},//go to label
            new byte[] {0x1d, 2},//vector (cliff position)
            new byte[] {0x1f, 3, 3},//set stick abs
            new byte[] {0x22, 3},//set wait
            new byte[] {0x24, 1, 3},//calcArriveFrameX
            new byte[] {0x25, 1, 3},//calcArriveFrameY
            new byte[] {0x26, 1},//gets shield hp
            new byte[] {0x27, 2},//vector (random stage point)
            new byte[] {0x28, 1, 3},//calcArrivePosX
            new byte[] {0x29, 1, 3},//calcArrivePosY
            new byte[] {0x2c, 1, 3, 3},//Norm
            new byte[] {0x2e, 1, 1, 3},//CalcArrivePosSec
            new byte[] {0x31, 2},//vector (cliff position)
        };

        public static Dictionary<UInt32, string> script_value_special = new Dictionary<UInt32, string>()
        {
            //0x1000 = par_work_update value. Represents distance of some sort?
            //0x1001 = next par_work_update value
            {0x1002, "ai_lr" },
            {0x1003, "lr_tgt" },
            {0x1004, "ai_pos" },//vector
            {0x1005, "tgt_pos" },//vector
            {0x1006, "edge_dist_toward_tgt" },
            {0x1007, "timer" },
            {0x1008, "ai_spd" },//vector
            {0x1009, "zero" },
            {0x100a, "one" },
            {0x100b, "ai_pos_y" },
            {0x100c, "tgt_pos_y" },
            {0x100d, "ai_spd_y" },
            {0x100e, "randf"},
            //0x100f = {Ground = 2, Air = 1}? Based on usage this may have changed in Smash 4
            {0x1010, "edge_dist_front" },
            {0x1011, "ai_rank" },
            //{0x1012, "" }, some byte
            //{0x1013, "" }, next byte
            {0x1014, "ctrl_weapon_pos" },
            {0x1015, "ctrl_weapon_pos_y" },
            {0x1016, "tgt_spd" },
            {0x1017, "tgt_spd_y" },
            {0x1018, "ctrl_weapon_spd" },
            {0x1019, "ctrl_weapon_spd_y" },
            {0x101a, "ai_hipn_pos" },
            {0x101b, "ai_hipn_pos_y" },
            {0x101c, "tgt_hipn_pos" },
            {0x101d, "tgt_hipn_pos" },
            {0x101e, "ai_dmg" },
            {0x101f, "tgt_dmg" },
            //0x1020 is some ai param
            {0x1021, "jump_height" },
            {0x1022, "jump_length" },
            {0x1023, "air_jump_height" },
            {0x1024, "air_jump_length" },
            {0x1025, "tgt_lr" },
            {0x1026, "tgt_jumps_remain" },
            {0x1027, "ecb_height" },//not 100% sure but is definitely related
            {0x1028, "tgt_ecb_height" },//not 100% sure but is definitely related
            {0x1029, "edge_dist_back" },
            {0x102a, "blastzone_bottom" },
            {0x102b, "blastzone_top" },
            {0x102c, "blastzone_left" },
            {0x102d, "blastzone_right" },
            {0x102e, "stage_length" },
        };//maximum value = 0x103E

        public static List<string> buttons = new List<string>
        {
            "attack", "special", "shield", "jump"
        };

        public static Dictionary<UInt32, string> if_chks = new Dictionary<UInt32, string>()
        {
            {0x1002, "timer_passed" },
            {0x1005, "ai_aerial" },
            {0x1007, "greater" },
            {0x1008, "less" },
            {0x1009, "geq" },
            {0x100a, "leq" },
            {0x100d, "ai_off_stage" },
            {0x100f, "ai_action" },
            {0x101b, "tgt_aerial" },
            {0x101e, "ai_char" },
            {0x101f, "tgt_char" },
            {0x1024, "ai_subaction" }
        };

        //Key = ID, Value = type of arguments:
        //0 = get_script_value
        //1 = fighter name
        //If an ID is not represented it is either treated as a special case or uses raw values
        //A special case could be a check that uses multiple types of arguments but I haven't found one yet
        public static Dictionary<UInt32, byte> if_chk_args = new Dictionary<uint, byte>()
        {
            {0x1000, 0},
            {0x1001, 0},
            {0x1002, 0},
            {0x1007, 0},
            {0x1008, 0},
            {0x1009, 0},
            {0x100a, 0},
            {0x1010, 0},
            {0x101e, 1},
            {0x101f, 1},
            {0x1021, 0},
            {0x1022, 0},
            {0x102a, 0},
        };

        public static List<string> fighters = new List<string>()
        {
            "miifighter",
            "miiswordsman",
            "miigunner",
            "mario",
            "donkey",
            "link",
            "samus",
            "yoshi",
            "kirby",
            "fox",
            "pikachu",
            "luigi",
            "captain",
            "ness",
            "peach",
            "koopa",
            "zelda",
            "sheik",
            "marth",
            "gamewatch",
            "ganon",
            "falco",
            "wario",
            "metaknight",
            "pit",
            "szerosuit",
            "pikmin",
            "diddy",
            "dedede",
            "ike",
            "lucario",
            "robot",
            "toonlink",
            "lizardon",
            "sonic",
            "purin",
            "mariod",
            "lucina",
            "pitb",
            "rosetta",
            "wiifit",
            "littlemac",
            "murabito",
            "palutena",
            "reflet",
            "duckhunt",
            "koopajr",
            "shulk",
            "gekkouga",
            "pacman",
            "rockman",
            "mewtwo",
            "ryu",
            "lucas",
            "roy",
            "cloud",
            "bayonetta",
            "kamui",
            "koopag",
            "warioman",
            "littlemacg",
            "lucariom",
            "miienemyf",
            "miienemys",
            "miienemyg"
        };
    }
}
