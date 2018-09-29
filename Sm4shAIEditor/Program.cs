using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
                ConsoleMain(args);
        }

        private static void ConsoleMain(string[] args)
        {
            //project handling variables
            AITree tree = null;
            string[] fighters = new string[0];
            bool allFighters = true;
            AITree.AIType[] types = new AITree.AIType[0];
            bool allTypes = true;

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    opcode op = (opcode)Enum.Parse(typeof(opcode), args[i].TrimStart('-'));
                    string notOpenMsg = "ERROR: project must be open for opcode " + args[i];
                    string notClosedMsg = "ERROR: project must be closed for opcode " + args[i];
                    switch (op)
                    {
                        case opcode.h:
                            ConsoleHelpText();
                            break;
                        case opcode.wp:
                            if (tree != null) throw new Exception(notClosedMsg);
                            util.workDir = args[++i];
                            if (!Directory.Exists(util.workDir))
                                Directory.CreateDirectory(util.workDir);
                            break;
                        case opcode.cp:
                            util.compDir = args[++i];
                            if (!Directory.Exists(util.compDir))
                                Directory.CreateDirectory(util.compDir);
                            break;
                        case opcode.gp:
                            util.gameFtDir = args[++i];
                            break;
                        case opcode.fl:
                            i++;
                            if (args[i] == "all")
                            {
                                allFighters = true;
                                break;
                            }
                            allFighters = false;
                            fighters = args[i].Split(',');
                            break;
                        case opcode.tl:
                            {
                                i++;
                                if (args[i] == "all")
                                {
                                    allTypes = true;
                                    break;
                                }
                                allTypes = false;
                                string[] strTypes = args[i].Split(',');
                                List<AITree.AIType> typeList = new List<AITree.AIType>();
                                for (int j = 0; j < strTypes.Length; j++)
                                {
                                    AITree.AIType newType = AITree.StringToAIType[strTypes[j]];
                                    if (!typeList.Contains(newType))
                                        typeList.Add(newType);
                                }
                                types = typeList.ToArray();
                                break;
                            }
                        case opcode.d:
                            aism.DisassembleFile(args[++i], util.workDir);
                            break;
                        case opcode.a:
                            aism.AssembleFolder(args[++i], util.compDir);
                            break;
                        case opcode.po:
                            if (tree != null) throw new Exception(notClosedMsg);
                            tree = new AITree();
                            tree.InitOpenProject();
                            break;
                        case opcode.pn:
                            {
                                if (tree != null) throw new Exception(notClosedMsg);
                                tree = new AITree();
                                string[] loc_fighters = fighters;
                                AITree.AIType[] loc_types = types;
                                if (allFighters)
                                {
                                    List<string> fighterNames = new List<string>();
                                    foreach (string dir in Directory.EnumerateDirectories(util.gameFtDir))
                                        fighterNames.Add(util.GetFolderName(dir));
                                    loc_fighters = fighterNames.ToArray();
                                }
                                if (allTypes)
                                    loc_types = new AITree.AIType[]
                                    {
                                        AITree.AIType.attack_data, AITree.AIType.param, AITree.AIType.param_nfp, AITree.AIType.script
                                    };
                                tree.InitNewProject(loc_fighters, loc_types);
                                break;
                            }
                        case opcode.pac:
                            {
                                if (tree == null) throw new Exception(notOpenMsg);
                                string[] loc_fighters = fighters;
                                AITree.AIType[] loc_types = types;
                                if (allFighters)
                                {
                                    List<string> fighterNames = new List<string>();
                                    foreach (string dir in Directory.EnumerateDirectories(util.compDir))
                                        fighterNames.Add(util.GetFolderName(dir));
                                    loc_fighters = fighterNames.ToArray();
                                }
                                if (allTypes)
                                    loc_types = new AITree.AIType[]
                                    {
                                        AITree.AIType.attack_data, AITree.AIType.param, AITree.AIType.param_nfp, AITree.AIType.script
                                    };
                                tree.AddProjectFiles(loc_fighters, loc_types, AITree.AISource.compiled);
                                break;
                            }
                        case opcode.pag:
                            {
                                if (tree == null) throw new Exception(notOpenMsg);
                                string[] loc_fighters = fighters;
                                AITree.AIType[] loc_types = types;
                                if (allFighters)
                                {
                                    List<string> fighterNames = new List<string>();
                                    foreach (string dir in Directory.EnumerateDirectories(util.gameFtDir))
                                        fighterNames.Add(util.GetFolderName(dir));
                                    loc_fighters = fighterNames.ToArray();
                                }
                                if (allTypes)
                                    loc_types = new AITree.AIType[]
                                    {
                                        AITree.AIType.attack_data, AITree.AIType.param, AITree.AIType.param_nfp, AITree.AIType.script
                                    };
                                tree.AddProjectFiles(loc_fighters, loc_types, AITree.AISource.game_file);
                                break;
                            }
                        case opcode.pcm:
                            {
                                if (tree == null) throw new Exception(notOpenMsg);
                                string[] loc_fighters = fighters;
                                AITree.AIType[] loc_types = types;
                                if (allFighters)
                                {
                                    List<string> fighterNames = new List<string>();
                                    foreach (string dir in Directory.EnumerateDirectories(util.workDir))
                                        fighterNames.Add(util.GetFolderName(dir));
                                    loc_fighters = fighterNames.ToArray();
                                }
                                if (allTypes)
                                    loc_types = new AITree.AIType[]
                                    {
                                        AITree.AIType.attack_data, AITree.AIType.param, AITree.AIType.param_nfp, AITree.AIType.script
                                    };

                                foreach (var ft in loc_fighters)
                                {
                                    foreach (var type in loc_types)
                                    {
                                        string pathIn = AITree.GetFolderPath(ft, type, AITree.AISource.work);
                                        if (Directory.Exists(pathIn))
                                        {
                                            string pathOut = AITree.GetFolderPath(ft, type, AITree.AISource.compiled);
                                            aism.AssembleFolder(pathIn, pathOut);
                                        }
                                    }
                                }
                                break;
                            }
                        case opcode.pcl:
                            if (tree == null) throw new Exception(notOpenMsg);
                            tree = null;
                            break;
                        case opcode.pd:
                            Console.WriteLine("workspace: " + util.workDir);
                            Console.WriteLine("compile: " + util.compDir);
                            Console.WriteLine("game: " + util.gameFtDir);
                            break;
                        case opcode.pt:
                            {
                                if (tree == null) throw new Exception(notOpenMsg);
                                foreach (var ft in tree.fighters)
                                {
                                    Console.Write(ft.name + ": {");
                                    int count = ft.files.Count;
                                    for (int j = 0; j < count; j++)
                                    {
                                        Console.Write(AITree.AITypeToString[ft.files[j].type]);
                                        if (j < count - 1)
                                            Console.Write(", ");
                                    }
                                    Console.WriteLine("}");
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(helpReminder);
            }
        }

        private static void ConsoleHelpText()
        {
            Console.WriteLine("Assemble folder or disassemble file for Smash 4 AI. Usage:");
            Console.WriteLine("\t>no args: open the GUI");
            foreach (int opValue in Enum.GetValues(typeof(opcode)))
            {
                opcode op = (opcode)opValue;
                Console.Write("\t>");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Enum.GetName(typeof(opcode), opValue));
                Console.ResetColor();
                Console.WriteLine(": " + descriptions[op]);
            }
        }

        enum opcode { h, wp, cp, gp, fl, tl, d, a, po, pn, pac, pag, pcm, pcl, pd, pt };
        static Dictionary<opcode, string> descriptions = new Dictionary<opcode, string>()
            {
                { opcode.h, "this help text" },
                { opcode.wp, "set the workspace path: [full/local path]" },
                { opcode.cp, "set the compile path: [full/local path]" },
                { opcode.gp, "set the game fighter path: [full/local path]" },
                { opcode.fl, "set fighter list: [list]\n" +
                "\t  -'all' == all fighters, or separate names with ','\n" +
                "\t  -this list used for handling projects" },
                { opcode.tl, "set file type list: [list]\n" +
                "\t  -'all' == all types, or separate names with ','\n" +
                "\t  -available types are: attack_data, param, param_nfp, script\n" +
                "\t  -this list used for handling projects" },
                { opcode.d, "disassemble file to workspace: [input file]" },
                { opcode.a, "assemble folder to compile: [input folder]" },
                { opcode.po, "open project (uses current 'wp' value)" },
                { opcode.pn, "new project (uses current 'wp' and 'gp' values)" },
                { opcode.pac, "add files from compile directory to project" },
                { opcode.pag, "add files from game fighter directory to project" },
                { opcode.pcm, "compile project workspace data" },
                { opcode.pcl, "close project" },
                { opcode.pd, "print workspace/compile/game fighter directories" },
                { opcode.pt, "print project tree" }
            };
        static string helpReminder = string.Format("See {0} for help text", nameof(opcode.h));
    }
}
