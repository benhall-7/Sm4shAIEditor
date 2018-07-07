using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Sm4shAIEditor.Static
{
    static class ft_xml_data
    {
        static string xmlFile = util.workDirectory + "game_fighters.xml";
        static string ftElement = "fighter";
        static string fileElement = "file";
        static Dictionary<AITree.AIFighter.AIFile.Type, string> filetypeEnumToString = new Dictionary<AITree.AIFighter.AIFile.Type, string>()
        {
            { AITree.AIFighter.AIFile.Type.attack_data, "atkd" },
            { AITree.AIFighter.AIFile.Type.param, "param" },
            { AITree.AIFighter.AIFile.Type.param_nfp, "param_nfp" },
            { AITree.AIFighter.AIFile.Type.script, "script" }
        };
        static Dictionary<string, AITree.AIFighter.AIFile.Type> filetypeStringToEnum = new Dictionary<string, AITree.AIFighter.AIFile.Type>()
        {
            {"atkd", AITree.AIFighter.AIFile.Type.attack_data },
            {"param", AITree.AIFighter.AIFile.Type.param },
            {"param_nfp", AITree.AIFighter.AIFile.Type.param_nfp },
            {"script", AITree.AIFighter.AIFile.Type.script }
        };

        static void WriteGameFileXML(AITree tree)
        {
            tree.Sort();
            var fighterData = tree.fighters;

            XmlWriter writer = XmlWriter.Create(xmlFile);
            writer.WriteStartDocument();
            foreach(var ft in fighterData)
            {
                writer.WriteStartElement(ftElement);
                writer.WriteString(ft.name);
                foreach(var file in ft.files)
                {
                    if (file.source == AITree.AIFighter.AIFile.Source.game_file)
                    {
                        writer.WriteStartElement(fileElement);
                        writer.WriteString(filetypeEnumToString[file.type]);
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteEndDocument();
            writer.Close();
        }
    }
}
