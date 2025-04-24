using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGame.Classes.Logging;
using TurnBasedGame.Classes.Objects;

namespace TurnBasedGame.Classes
{
    /// <summary>
    /// Manages turn order and handles turn-based logic in the game.
    /// </summary>
    public class TurnManager
    {
        private readonly List<Creature> _creatures;
        private readonly ILogger _logger;
        private int _currentTurnIndex;
        private int _roundCount;

        /// <summary>
        /// Gets the currently active creature whose turn it is.
        /// </summary>
        public Creature CurrentCreature => _creatures[_currentTurnIndex];

        /// <summary>
        /// Gets the current round number. Starts from 1.
        /// </summary>
        public int RoundNumber => _roundCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnManager"/> class.
        /// </summary>
        /// <param name="creatures">Initial list of creatures participating in the turn order.</param>
        /// <param name="logger">Logger used for game event logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when creatures or logger is null.</exception>
        public TurnManager(List<Creature> creatures, ILogger logger)
        {
            _creatures = creatures ?? throw new ArgumentNullException(nameof(creatures));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentTurnIndex = 0;
            _roundCount = 1;

            _logger.Log(LogLevel.Info, $"TurnManager initialized with {_creatures.Count} creatures");
        }

        /// <summary>
        /// Advances to the next creature's turn. Increments the round counter if a full cycle is completed.
        /// </summary>
        public void NextTurn()
        {
            _currentTurnIndex = (_currentTurnIndex + 1) % _creatures.Count;

            if (_currentTurnIndex == 0)
            {
                _roundCount++;
                _logger.Log(LogLevel.Info, $"Starting round {_roundCount}");
            }

            _logger.Log(LogLevel.Debug, $"{CurrentCreature.Name}'s turn started");
        }

        /// <summary>
        /// Returns the current turn order starting from the active creature.
        /// </summary>
        /// <returns>An enumerable of creatures in current turn order.</returns>
        public IEnumerable<Creature> GetTurnOrder()
        {
            return _creatures.Skip(_currentTurnIndex)
                             .Concat(_creatures.Take(_currentTurnIndex));
        }

        /// <summary>
        /// Sorts the creatures based on initiative, currently using BaseDamage as a proxy for speed.
        /// </summary>
        public void SortByInitiative()
        {
            _creatures.Sort((a, b) => b.BaseDamage.CompareTo(a.BaseDamage));
            _currentTurnIndex = 0;

            _logger.Log(LogLevel.Info, "Reordered turn initiative:");
            foreach (var creature in _creatures)
            {
                _logger.Log(LogLevel.Info, $"- {creature.Name} (Speed: {creature.BaseDamage})");
            }
        }

        /// <summary>
        /// Adds a new creature to the list of creatures in the turn order.
        /// </summary>
        /// <param name="creature">Creature to be added.</param>
        public void AddCreature(Creature creature)
        {
            if (creature == null) return;

            _creatures.Add(creature);
            _logger.Log(LogLevel.Info, $"Added {creature.Name} to turn order");
        }

        /// <summary>
        /// Removes a creature from the turn order. Adjusts turn index if necessary.
        /// </summary>
        /// <param name="creature">Creature to be removed.</param>
        public void RemoveCreature(Creature creature)
        {
            if (creature == null) return;

            int index = _creatures.IndexOf(creature);
            if (index >= 0)
            {
                _creatures.RemoveAt(index);
                if (_currentTurnIndex >= index && _currentTurnIndex > 0)
                {
                    _currentTurnIndex--;
                }
                _logger.Log(LogLevel.Info, $"Removed {creature.Name} from turn order");
            }
        }
    }
}
