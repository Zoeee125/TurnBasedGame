using System;
using System.Collections.Generic;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Objects
{
    /// <summary>
    /// Attack item with advanced logic of damaging
    /// Implements: Decorator (modifiers), Strategy (special attacks), Observer (events)
    /// </summary>
    public class AttackItem : WorldObject, IWeapon, IDurable
    {
        private readonly List<IDamageModifier> _modifiers = new List<IDamageModifier>();
        private readonly ILogger _logger;
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Base damage without modifiers
        /// </summary>
        public int BaseDamage { get; protected set; }

        /// <summary>
        /// Max range of attack
        /// </summary>
        public int Range { get; protected set; }

        /// <summary>
        /// Type of dammage
        /// </summary>
        public DamageType DamageType { get; }

        /// <summary>
        /// Chance for critical attack in percents
        /// </summary>
        public int CriticalChance { get; protected set; }

        /// <summary>
        /// Items durability (0 = broke)
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// Action when item has been used 
        /// </summary>
        public event Action<AttackItem> OnItemUsed;

        /// <summary>
        /// Action when durability has changed
        /// </summary>
        public event Action<int> OnDurabilityChanged;

        /// <summary>
        /// Creates an attack item
        /// <param name="position">X and Y position of the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="baseDamage">Base damage without modifiers.</param>
        /// <param name="logger">Games logger.</param>
        /// <param name="range">Range of the item</param>
        /// <param name="damageType">type of damage</param>
        /// <param name="criticalChance">Chance for critical attack in percents</param>
        /// <param name="durability">durability of an item</param>
        /// </summary>
        public AttackItem((int x, int y) position, string name, int baseDamage,
                         ILogger logger, int range = 1,
                         DamageType damageType = DamageType.Physical,
                         int criticalChance = 10, int durability = 100)
            : base(position, name, true, true, logger)
        {
            BaseDamage = baseDamage;
            Range = range;
            DamageType = damageType;
            CriticalChance = criticalChance;
            Durability = durability;
            _logger = logger;

            //_logger.Log(LogLevel.Debug, $"Created {name} at {position}");
        }

        /// <summary>
        /// Main item interaction
        /// </summary>
        public void Interact()
        {
            _logger.Log(LogLevel.Info, $"Interacting with {Name}");
            Console.WriteLine($"{Name} (Damage: {CalculateDamage()}, Range: {Range}, Type: {DamageType})");
            OnItemUsed?.Invoke(this);
        }

        /// <summary>
        /// Calculates whole damage modifiers included
        /// </summary>
        public int CalculateDamage()
        {
            int damage = BaseDamage;
            bool isCritical = _rng.Next(1, 101) <= CriticalChance;

            foreach (var modifier in _modifiers)
            {
                damage = modifier.ModifyDamage(damage, isCritical);
                _logger.Log(LogLevel.Debug, $"Applied {modifier.GetType().Name} modifier");
            }

            if (isCritical)
            {
                damage = (int)(damage * 1.5);
                _logger.Log(LogLevel.Info, "That was some critical hit!");
            }

            ReduceDurability(1);
            return Math.Max(1, damage);
        }

        /// <summary>
        /// Adds damage modifier (Decorator pattern)
        /// <param name="modifier">modiffier to add</param>
        /// </summary>
        public void AddModifier(IDamageModifier modifier)
        {
            _modifiers.Add(modifier);
            _logger.Log(LogLevel.Info, $"Added {modifier.GetType().Name} to {Name}");
        }

        /// <summary>
        /// Performs attack based on strategy
        /// <param name="strategy">Strategy for an attack</param>
        /// </summary>
        public void PerformSpecialAttack(IAttackStrategy strategy)
        {
            _logger.Log(LogLevel.Info, $"Performing special attack with {strategy.GetType().Name}");
            strategy.ExecuteAttack(this);
            ReduceDurability(5); 
        }

        /// <summary>
        /// Reduces durability of an item
        /// <param name="amount">Amount of durabilty to reduce</param>
        /// </summary>
        public void ReduceDurability(int amount)
        {
            Durability = Math.Max(0, Durability - amount);
            OnDurabilityChanged?.Invoke(Durability);

            _logger.Log(LogLevel.Debug, $"{Name} durability: {Durability}");

            if (Durability == 0)
            {
                _logger.Log(LogLevel.Warning, $"{Name} broke!");

                OnBroken?.Invoke(this);
            }
        }

        /// <summary>
        /// Action when item broke (durability reaches 0)
        /// </summary>
        public event Action<AttackItem> OnBroken;

        /// <summary>
        /// Repairs item by inputed amount
        /// <param name="amount">Amount of durability to repair</param>
        /// </summary>
        public void Repair(int amount)
        {
            Durability += amount;
            OnDurabilityChanged?.Invoke(Durability);
            _logger.Log(LogLevel.Info, $"Repaired {Name} by {amount}");
        }

        protected override void ExecuteInteraction()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interface for types of damage
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Piercing
    }

    /// <summary>
    /// Weapon interface
    /// </summary>
    public interface IWeapon
    {
        int CalculateDamage();
        void AddModifier(IDamageModifier modifier);
        void PerformSpecialAttack(IAttackStrategy strategy);
    }

    /// <summary>
    /// interface for managing durability
    /// </summary>
    public interface IDurable
    {
        int Durability { get; }
        void ReduceDurability(int amount);
        void Repair(int amount);
        event Action<int> OnDurabilityChanged;
    }

    /// <summary>
    /// interface for modifying
    /// </summary>
    public interface IDamageModifier
    {
        int ModifyDamage(int baseDamage, bool isCritical);
    }

    /// <summary>
    /// Strategy interface
    /// </summary>
    public interface IAttackStrategy
    {
        void ExecuteAttack(IWeapon weapon);
    }
}