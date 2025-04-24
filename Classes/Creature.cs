using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TurnBasedGame.Classes.Logging;
using TurnBasedGame.Classes.Objects;

namespace TurnBasedGame.Classes
{
    /// <summary>
    /// Base class for all creatures in the game world
    /// Implements Template Method pattern for creature behavior
    /// also inheriting from world object
    /// </summary>
    public abstract class Creature : WorldObject, ICombatant
    {
        protected readonly ILogger _logger;
        private int _lifePoints;
        private readonly List<IDamageModifier> _damageModifiers = new List<IDamageModifier>();
        private int maxHealth;


        public event Action<Creature> OnCreatureDied;
        public event Action<Creature, int> OnDamageTaken;

        /// <summary>
        /// Creature's name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Current health points
        /// </summary>
        public int LifePoints
        {
            get => _lifePoints;
            protected set
            {
                _lifePoints = Math.Max(0, value);
                
                if (_lifePoints == 0)
                {
                    Die();
                }
            }
        }

        /// <summary>
        /// Base damage without equipment
        /// </summary>
        public int BaseDamage { get; protected set; }

        /// <summary>
        /// Base defense without equipment
        /// </summary>
        public int BaseDefense { get; protected set; }

        /// <summary>
        /// Position in world coordinates
        /// </summary>
        public (int X, int Y) Position { get; set; }

        /// <summary>
        /// Current equipped weapon
        /// </summary>
        public AttackItem EquippedWeapon { get; private set; }

        /// <summary>
        /// Current equipped defense item
        /// </summary>
        public DefenceItem EquippedArmor { get; private set; }

        /// <summary>
        /// Creates an instance of the class Creature.
        /// </summary>
        /// <param name="position">X and Y position of the player.</param>
        /// <param name="name">The name of a player.</param>
        /// <param name="health">Amount of health points.</param>
        /// <param name="baseDamage">Base damage of players attack without any weapon.</param>
        /// <param name="baseDefense">Base defense of a player without any armor.</param>
        /// <param name="logger">Games logger.</param>
        protected Creature((int x, int y) position, string name,
                         int health, int baseDamage, int baseDefense,
                         ILogger logger)
            : base(position, name, false, false, logger)
        {
            Name = name;
            LifePoints = health;
            BaseDamage = baseDamage;
            BaseDefense = baseDefense;
            _logger = logger;
            maxHealth = health;
            
            //logger.Log(LogLevel.Debug, $"Created creature {name} at {position}");
        }

        /// <summary>
        /// Template method for attack sequence
        /// </summary>
        public virtual int Attack()
        {
            _logger.Log(LogLevel.Info, $"{Name} prepares to attack");

            int totalDamage = CalculateDamage();
            _logger.Log(LogLevel.Debug, $"{Name} attacks with {totalDamage} damage");

            return totalDamage;
        }

        /// <summary>
        /// Calculates total damage including modifiers
        /// </summary>
        protected virtual int CalculateDamage()
        {
            int damage = BaseDamage;

            if (EquippedWeapon != null)
            {
                damage += EquippedWeapon.CalculateDamage();
            }

            foreach (var modifier in _damageModifiers)
            {
                damage = modifier.ModifyDamage(damage, false);
            }

            return Math.Max(1, damage);
        }

        /// <summary>
        /// Receives damage with defense calculation
        /// </summary>
        public virtual void ReceiveHit(int hit)
        {
            int defense = BaseDefense + (EquippedArmor?.GetTotalDefense() ?? 0);
            int damageTaken = Math.Max(1, hit - defense);

            LifePoints -= damageTaken;
            _logger.Log(LogLevel.Info, $"{Name} took {damageTaken} damage (HP: {LifePoints})");

            OnDamageTaken?.Invoke(this, damageTaken);

            if (LifePoints <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Picks up a world object (Strategy pattern)
        /// </summary>
        public virtual void Pick(WorldObject obj)
        {
            if (obj == null) return;

            switch (obj)
            {
                case AttackItem weapon:
                    EquipWeapon(weapon);
                    //_logger.Log(LogLevel.Info, $"{Name} equiped {weapon.Name}");
                    break;

                case DefenceItem armor:
                    EquipArmor(armor);
                    //_logger.Log(LogLevel.Info, $"{Name} equiped {armor.Name}");
                    break;

                case HealthPotion potion:
                    
                    Consume(potion);
                    _logger.Log(LogLevel.Info, $"{Name} drank up {potion.Name}");
                    break;

                default:
                    _logger.Log(LogLevel.Warning, $"{Name} tried to pick up non-interactable object: {obj.Name}");
                    break;
            }
        }

        /// <summary>
        /// Equips a weapon
        /// </summary>
        public virtual void EquipWeapon(AttackItem weapon)
        {
            if (weapon == null) return;

            EquippedWeapon = weapon;
            _logger.Log(LogLevel.Info, $"{Name} equipped {weapon.Name}");
        }

        /// <summary>
        /// Equips a weapon
        /// </summary>
        public virtual void Consume(HealthPotion potion)
        {
            if (potion == null) return;
            if(LifePoints + potion.HealAmount <= maxHealth)
            {
                LifePoints += potion.HealAmount;
            }
            else
            {
                LifePoints = maxHealth;
            }
            
            _logger.Log(LogLevel.Info, $"{Name} healled for {potion.HealAmount}");
        }

        /// <summary>
        /// Equips armor
        /// </summary>
        public virtual void EquipArmor(DefenceItem armor)
        {
            if (armor == null) return;

            EquippedArmor = armor;
            _logger.Log(LogLevel.Info, $"{Name} equipped {armor.Name}");
        }

        /// <summary>
        /// Adds damage modifier (Decorator pattern)
        /// </summary>
        public void AddDamageModifier(IDamageModifier modifier)
        {
            _damageModifiers.Add(modifier);
            _logger.Log(LogLevel.Debug, $"{Name} gained {modifier.GetType().Name} modifier");
        }

        /// <summary>
        /// Invoke dying
        /// </summary>
        protected virtual void Die()
        {
            _logger.Log(LogLevel.Info, $"{Name} has died");
            OnCreatureDied?.Invoke(this);

        }

        protected override void ExecuteInteraction()
        {
            _logger.Log(LogLevel.Debug, $"{Name} interacts generically");
        }

        /// <summary>
        /// Moves creature to new position (Template method)
        /// </summary>
        public abstract void Move(int x, int y);

    }

    /// <summary>
    /// Interface for combat
    /// </summary>
    public interface ICombatant
    {
        int Attack();
        void ReceiveHit(int hit);
    }

}