using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sm4shAIEditor.Static
{
    static class script_data
    {
        public static List<string> CmdNames = new List<string>()
        {
            "End",
            "SetVar",//syntax varX = item
            "SetVec2D",//syntax vecX = item
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
            "CliffCheck",
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
            "CalcArrivePos_Sec",
            "Unk_2f",
            "Unk_30",
            "GetNearestCliffAbs",
            "ClearStick",
            "Unk_33",//new to Smash 4
            "Unk_34",
            "Unk_35",
            "Unk_36",//unused
            "Unk_37",
            "Unk_38",
            "Unk_39"
        };

        public static Dictionary<UInt32, string> script_value_special = new Dictionary<UInt32, string>()
        {
            //0x1000 = par_work_update value. Range of some sort?
            //0x1001 = next par_work_update value
            {0x1002, "ai_lr" },
            {0x1003, "get_lr_tgt" },
            {0x1004, "ai_pos_xy" },//vector
            {0x1005, "tgt_pos_xy" },//vector
            //0x1006 = distance from ledge in direction of opponent?
            {0x1007, "timer" },
            {0x1008, "ai_spd_xy" },//vector
            {0x1009, "zero" },
            {0x100a, "one" },
            {0x100b, "ai_pos_y" },
            {0x100c, "tgt_pos_y" },
            {0x100d, "ai_spd_y" },
            {0x100e, "randf"},
            //0x100f = {Ground = 2, Air = 1}
            //0x1010 = distance from front ledge?
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
