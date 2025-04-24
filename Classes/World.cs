using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGame.Classes.Logging;
using TurnBasedGame.Classes.Objects;

namespace TurnBasedGame.Classes
{
    /// <summary>
    /// Represents the game world, including creatures and objects placed in a 2D grid.
    /// </summary>
    public class World
    {
        private readonly Dictionary<(int x, int y), List<WorldObject>> _spatialMap = new();
        private readonly ILogger _logger;

        /// <summary>
        /// Gets the maximum X boundary of the world.
        /// </summary>
        public int MaxX { get; }

        /// <summary>
        /// Gets the maximum Y boundary of the world.
        /// </summary>
        public int MaxY { get; }

        /// <summary>
        /// Gets the list of all creatures currently in the world.
        /// </summary>
        public List<Creature> Creatures { get; } = new List<Creature>();

        /// <summary>
        /// Gets the list of all world objects currently in the world.
        /// </summary>
        public List<WorldObject> WorldObjects { get; } = new List<WorldObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class using configuration settings.
        /// </summary>
        /// <param name="logger">Logger for recording world events and errors.</param>
        public World(ILogger logger)
        {
            _logger = logger;
            ConfigManager.LoadConfig();
            MaxX = ConfigManager.MaxX;
            MaxY = ConfigManager.MaxY;
        }

        /// <summary>
        /// Adds a creature to the world and places it on the spatial map.
        /// </summary>
        /// <param name="creature">The creature to add.</param>
        public void AddCreature(Creature creature)
        {
            if (creature == null)
            {
                _logger.Log(LogLevel.Warning, "Attempted to add null creature");
                return;
            }

            if (!IsPositionValid(creature.Position))
            {
                _logger.Log(LogLevel.Error, $"Invalid position {creature.Position} for creature {creature.Name}");
                return;
            }

            Creatures.Add(creature);
            AddToSpatialMap(creature); // Creature inherits from WorldObject

            _logger.Log(LogLevel.Info, $"Added creature {creature.Name} at {creature.Position}");
        }

        /// <summary>
        /// Adds a world object to the world and places it on the spatial map.
        /// </summary>
        /// <param name="worldObject">The world object to add.</param>
        public void AddWorldObject(WorldObject worldObject)
        {
            if (worldObject == null)
            {
                _logger.Log(LogLevel.Warning, "Attempted to add null world object");
                return;
            }

            if (!IsPositionValid(worldObject.Position))
            {
                _logger.Log(LogLevel.Error, $"Invalid position {worldObject.Position} for object {worldObject.Name}");
                return;
            }

            WorldObjects.Add(worldObject);
            AddToSpatialMap(worldObject);

            _logger.Log(LogLevel.Info, $"Added world object {worldObject.Name} at {worldObject.Position}");
        }

        /// <summary>
        /// Validates whether a position is within the boundaries of the world.
        /// </summary>
        /// <param name="position">The (x, y) position to validate.</param>
        /// <returns>True if the position is valid; otherwise, false.</returns>
        public bool IsPositionValid((int x, int y) position)
        {
            return position.x >= 0 && position.x < MaxX &&
                   position.y >= 0 && position.y < MaxY;
        }

        /// <summary>
        /// Gets all objects located at the specified position in the world.
        /// </summary>
        /// <param name="position">The (x, y) position to check.</param>
        /// <returns>An enumerable of world objects at the given position.</returns>
        public IEnumerable<WorldObject> GetObjectsAt((int x, int y) position)
        {
            return _spatialMap.TryGetValue(position, out var objects)
                ? objects
                : Enumerable.Empty<WorldObject>();
        }

        /// <summary>
        /// Adds a world object to the internal spatial map for fast lookup.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        private void AddToSpatialMap(WorldObject obj)
        {
            if (!_spatialMap.ContainsKey(obj.Position))
            {
                _spatialMap[obj.Position] = new List<WorldObject>();
            }
            _spatialMap[obj.Position].Add(obj);
        }
    }
}
