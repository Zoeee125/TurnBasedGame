using System;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Objects
{
    /// <summary>
    /// Defensive item with support for modifiers and durability.
    /// Implements the Decorator pattern for modifiers and uses Observer pattern for durability changes.
    /// </summary>
    public class DefenceItem : WorldObject, IDefense, IDurable
    {
        private readonly List<IDefenseModifier> _modifiers = new List<IDefenseModifier>();
        private readonly ILogger _logger;

        /// <summary>
        /// Base defense without any modifiers.
        /// </summary>
        public int BaseDefense { get; protected set; }

        /// <summary>
        /// Type of defense the item provides.
        /// </summary>
        public DefenseType DefenseType { get; }

        /// <summary>
        /// Current durability of the item.
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// Event triggered whenever the item's durability changes.
        /// </summary>
        public event Action<int> OnDurabilityChanged;

        /// <summary>
        /// Creates a new defensive item with the given properties.
        /// </summary>
        /// <param name="position">X and Y position on the game field.</param>
        /// <param name="name">Name of the item.</param>
        /// <param name="baseDefense">Base defense value without modifiers.</param>
        /// <param name="logger">Game logger instance.</param>
        /// <param name="defenseType">Type of defense this item offers.</param>
        /// <param name="durability">Initial durability of the item.</param>
        public DefenceItem((int x, int y) position, string name, int baseDefense,
                         ILogger logger, DefenseType defenseType = DefenseType.Physical,
                         int durability = 100)
            : base(position, name, true, true, logger)
        {
            BaseDefense = baseDefense;
            DefenseType = defenseType;
            Durability = durability;
            _logger = logger;

            //_logger.Log(LogLevel.Debug, $"Created {name} at {position}");
        }

        /// <summary>
        /// Displays item details including total defense and type.
        /// </summary>
        public void Interact()
        {
            _logger.Log(LogLevel.Info, $"Interacting with {Name}");
            Console.WriteLine($"{Name} (Defence: {GetTotalDefense()}, Type: {DefenseType})");
        }

        /// <summary>
        /// Calculates total defense including all modifiers.
        /// </summary>
        /// <returns>Total defense value after applying all modifiers.</returns>
        public int GetTotalDefense()
        {
            int defense = BaseDefense;
            foreach (var modifier in _modifiers)
            {
                defense = modifier.ModifyDefense(defense);
                _logger.Log(LogLevel.Debug, $"Applied {modifier.GetType().Name} modifier");
            }
            return Math.Max(0, defense);
        }

        /// <summary>
        /// Adds a new modifier to this defense item.
        /// </summary>
        /// <param name="modifier">Defense modifier to apply.</param>
        public void AddModifier(IDefenseModifier modifier)
        {
            _modifiers.Add(modifier);
            _logger.Log(LogLevel.Info, $"Added {modifier.GetType().Name} to {Name}");
        }

        /// <summary>
        /// Reduces the item's durability by a given amount.
        /// </summary>
        /// <param name="amount">Amount of durability to reduce.</param>
        public void ReduceDurability(int amount)
        {
            Durability = Math.Max(0, Durability - amount);
            OnDurabilityChanged?.Invoke(Durability);

            _logger.Log(LogLevel.Debug, $"{Name} durability: {Durability}");

            if (Durability <= 0)
            {
                _logger.Log(LogLevel.Warning, $"{Name} se rozbil!");
            }
        }

        /// <summary>
        /// Increases the item's durability by a given amount.
        /// </summary>
        /// <param name="amount">Amount of durability to restore.</param>
        public void Repair(int amount)
        {
            Durability += amount;
            OnDurabilityChanged?.Invoke(Durability);
            _logger.Log(LogLevel.Info, $"Repaired {Name} by {amount}");
        }

        /// <summary>
        /// Placeholder for subclass-specific interaction logic.
        /// </summary>
        protected override void ExecuteInteraction()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Different types of defense an item can provide against various sources of damage.
    /// </summary>
    public enum DefenseType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Universal
    }

    /// <summary>
    /// Defines the behavior of objects that provide defensive capabilities and support for modifiers.
    /// </summary>
    public interface IDefense
    {
        /// <summary>
        /// Calculates total defense with modifiers.
        /// </summary>
        int GetTotalDefense();

        /// <summary>
        /// Adds a defense modifier to the object.
        /// </summary>
        void AddModifier(IDefenseModifier modifier);
    }

    /// <summary>
    /// Represents a modifier that alters the base defense value.
    /// </summary>
    public interface IDefenseModifier
    {
        /// <summary>
        /// Modifies the given base defense and returns a new value.
        /// </summary>
        /// <param name="baseDefense">Original base defense value.</param>
        /// <returns>Modified defense value.</returns>
        int ModifyDefense(int baseDefense);
    }
}
