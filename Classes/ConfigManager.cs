using System;
using System.Xml;

namespace TurnBasedGame.Classes
{
    /// <summary>
    /// Manages game configuration loaded from an XML file.
    /// </summary>
    public static class ConfigManager
    {
        private static GameDifficulty _difficulty;

        /// <summary>
        /// Gets the maximum X dimension of the game world.
        /// </summary>
        public static int MaxX { get; private set; }

        /// <summary>
        /// Gets the maximum Y dimension of the game world.
        /// </summary>
        public static int MaxY { get; private set; }

        /// <summary>
        /// Gets the difficulty level of the game.
        /// </summary>
        public static GameDifficulty Difficulty
        {
            get => _difficulty;
            private set
            {
                _difficulty = value;
                Console.WriteLine($"Difficulty set to: {_difficulty}");
            }
        }

        /// <summary>
        /// Loads game configuration from an XML file.
        /// </summary>
        /// <param name="filePath">Path to the XML configuration file. Defaults to "config.xml".</param>
        public static void LoadConfig(string filePath = "config.xml")
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);

                MaxX = int.Parse(doc.SelectSingleNode("//WorldSize/maxX")?.InnerText ?? "10");
                MaxY = int.Parse(doc.SelectSingleNode("//WorldSize/maxY")?.InnerText ?? "10");

                var difficultyText = doc.SelectSingleNode("//Difficulty")?.InnerText ?? "Beginner";
                Difficulty = ParseDifficulty(difficultyText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                SetDefaults();
            }
        }

        /// <summary>
        /// Parses a string into a <see cref="GameDifficulty"/> enum.
        /// </summary>
        /// <param name="text">Difficulty level as string.</param>
        /// <returns>Corresponding <see cref="GameDifficulty"/> value.</returns>
        private static GameDifficulty ParseDifficulty(string text)
        {
            return text.ToLower() switch
            {
                "intermediate" => GameDifficulty.Intermediate,
                "pro" => GameDifficulty.Pro,
                _ => GameDifficulty.Beginner
            };
        }

        /// <summary>
        /// Sets default configuration values in case of error.
        /// </summary>
        private static void SetDefaults()
        {
            MaxX = 10;
            MaxY = 10;
            Difficulty = GameDifficulty.Beginner;
        }
    }
}
