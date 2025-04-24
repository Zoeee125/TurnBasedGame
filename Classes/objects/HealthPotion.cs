using System;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Objects
{
    /// <summary>
    /// Healing item that restores health points when consumed.
    /// </summary>
    public class HealthPotion : WorldObject, IConsumable
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Amount of health points restored by the potion.
        /// </summary>
        public int HealAmount { get; }

        /// <summary>
        /// Initializes a new instance of the HealthPotion class.
        /// </summary>
        /// <param name="position">X and Y position on the game field.</param>
        /// <param name="name">Name of the potion.</param>
        /// <param name="healAmount">Amount of health points restored when used.</param>
        /// <param name="logger">Logger used for game events.</param>
        public HealthPotion((int x, int y) position, string name, int healAmount, ILogger logger)
            : base(position, name, true, true, logger)
        {
            HealAmount = healAmount;
            _logger = logger;

            //_logger.Log(LogLevel.Debug, $"Created {name} at {position}");
        }

        /// <summary>
        /// Interacts with the potion, triggering its consumption.
        /// </summary>
        public void Interact()
        {
            _logger.Log(LogLevel.Info, $"Using {Name} (+{HealAmount} HP)");
            ExecuteInteraction();
        }

        /// <summary>
        /// Consumes the potion.
        /// </summary>
        public void Consume()
        {
            _logger.Log(LogLevel.Debug, $"Consumed {Name}");
            
        }

        /// <summary>
        /// Executes the potion's interaction logic (consumption).
        /// </summary>
        protected override void ExecuteInteraction()
        {
            Consume();
        }
    }

    /// <summary>
    /// Interface for consumable items that can be used up.
    /// </summary>
    public interface IConsumable
    {
        /// <summary>
        /// Triggers the consumption logic of the item.
        /// </summary>
        void Consume();
    }
}
