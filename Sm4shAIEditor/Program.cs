using System;
using Sm4shAIEditor.Static;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    static class Program
    {
        static string[] keys = { "-a", "-d", "-h" };
        static string helpReminder = string.Format("See {0} for help text", keys[2]);
        static string usage = string.Format("usage: assemble folder or disassemble file for Smash 4 AI\n" +
            "assembly: {0} [input folder] [output file]\n" +
            "disassembly: {1} [input file] [output folder]\n" +
            "this text: {2}", keys[0], keys[1], keys[2]);

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
            try
            {
                string op = (string)args[0];
                if (op == keys[0])
                {
                    aism.AssembleFolder((string)args[1], (string)args[2]);
                }
                else if (op == keys[1])
                {
                    aism.DisassembleFile((string)args[1], (string)args[2]);
                }
                else if (op == keys[2])
                    Console.WriteLine(usage);
                else
                    throw new Exception(string.Format("Invalid operation key {0}", op));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(helpReminder);
            }
        }
    }
}
