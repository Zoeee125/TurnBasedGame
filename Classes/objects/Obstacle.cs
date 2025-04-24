using System;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Objects
{
    /// <summary>
    /// Base abstract class for all obstacle types in the game world.
    /// Implements the Template Method pattern for defining the sequence of obstacle interactions.
    /// </summary>
    public abstract class Obstacle : WorldObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Obstacle"/> class.
        /// </summary>
        /// <param name="position">X and Y position of the obstacle on the game field.</param>
        /// <param name="name">Name of the obstacle.</param>
        /// <param name="lootable">Indicates whether the obstacle can be looted.</param>
        /// <param name="removable">Indicates whether the obstacle can be removed from the game field.</param>
        /// <param name="logger">Logger used for game events.</param>
        protected Obstacle((int x, int y) position, string name, bool lootable, bool removable, ILogger logger)
            : base(position, name, lootable, removable, logger)
        {
        }

        /// <summary>
        /// Gets or sets the amount of damage dealt when a player interacts with this obstacle.
        /// </summary>
        public int ContactDamage { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the obstacle can be destroyed.
        /// </summary>
        public bool IsDestructible { get; protected set; }

        /// <summary>
        /// Gets or sets the current durability (health points) of the obstacle.
        /// </summary>
        public int Durability { get; protected set; }

        /// <summary>
        /// Template method that defines the sequence of interaction with the obstacle.
        /// This method calls virtual methods that can be overridden in subclasses.
        /// </summary>
        public void Interact()
        {
            PlayInteractionSound();
            ApplyContactDamage();
            HandleDestruction();
        }

        /// <summary>
        /// Plays a sound when the obstacle is interacted with.
        /// Can be overridden to provide custom sounds.
        /// </summary>
        protected virtual void PlayInteractionSound()
        {
            Console.WriteLine($"*{Name} makes a sound*");
        }

        /// <summary>
        /// Applies contact damage to the player or entity interacting with the obstacle.
        /// </summary>
        protected virtual void ApplyContactDamage()
        {
            if (ContactDamage > 0)
            {
                Console.WriteLine($"{Name} deals {ContactDamage} damage on contact!");
            }
        }

        /// <summary>
        /// Handles the destruction logic of the obstacle if it is destructible.
        /// Reduces durability and outputs destruction messages.
        /// </summary>
        protected virtual void HandleDestruction()
        {
            if (!IsDestructible) return;

            Durability -= 10;
            Console.WriteLine($"{Name} durability: {Durability}");

            if (Durability <= 0)
            {
                Console.WriteLine($"{Name} was destroyed!");
                // TODO: Mark for removal from game world
            }
        }
    }
}
