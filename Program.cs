using System.Xml;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TurnBasedGame
{
    using System;
    using System.Xml;

    class Program
    {
        static void Main()
        {
            XmlDocument config = new XmlDocument();
            config.Load("ConfigFile.xml");

            if (config.DocumentElement != null) // Ověříme, že XML bylo načteno správně
            {
                XmlNode maxXNode = config.DocumentElement.SelectSingleNode("WorldSize/maxX");
                XmlNode maxYNode = config.DocumentElement.SelectSingleNode("WorldSize/maxY");
                XmlNode difficultyNode = config.DocumentElement.SelectSingleNode("Difficulty");

                int x = (maxXNode != null) ? Convert.ToInt32(maxXNode.InnerText.Trim()) : 0;
                int y = (maxYNode != null) ? Convert.ToInt32(maxYNode.InnerText.Trim()) : 0;
                string difficulty = (difficultyNode != null) ? difficultyNode.InnerText.Trim() : "Unknown";

                Console.WriteLine($"World Size: {x}x{y}");
                Console.WriteLine($"Difficulty: {difficulty}");
            }
            else
            {
                Console.WriteLine("Error: Config file is empty or invalid.");
            }
        }
    }

}
