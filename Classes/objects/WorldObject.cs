using System;
using System.Collections.Generic;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Objects
{
    /// <summary>
    /// Base class for all objects in the game world.
    /// Implements the Observer and Template Method design patterns.
    /// </summary>
    public abstract class WorldObject : IInteractable, IIdentifiable
    {
        private readonly List<IInteractionEffect> _effects = new List<IInteractionEffect>();
        protected readonly ILogger _logger;

        /// <summary>
        /// Position in world coordinates.
        /// </summary>
        public (int X, int Y) Position { get; set; }

        /// <summary>
        /// Display name of the object.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Indicates whether the object can be picked up by creatures.
        /// </summary>
        public bool Lootable { get; protected set; }

        /// <summary>
        /// Indicates whether the object can be removed from the game world.
        /// </summary>
        public bool Removable { get; protected set; }

        /// <summary>
        /// Unique identifier of the object.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Event triggered when the object is interacted with.
        /// </summary>
        public event Action<WorldObject> OnInteracted;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObject"/> class.
        /// </summary>
        /// <param name="position">Initial X and Y coordinates.</param>
        /// <param name="name">Display name of the object.</param>
        /// <param name="lootable">Whether the object can be looted.</param>
        /// <param name="removable">Whether the object can be removed from the world.</param>
        /// <param name="logger">Logger for debugging and events.</param>
        /// <exception cref="ArgumentNullException">Thrown if name or logger is null.</exception>
        protected WorldObject((int x, int y) position, string name, bool lootable, bool removable, ILogger logger)
        {
            Position = position;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Lootable = lootable;
            Removable = removable;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.Log(LogLevel.Debug, $"Created {GetType().Name} '{Name}' at {Position}");
        }

        /// <summary>
        /// Template method for interaction sequence.
        /// </summary>
        public void Interact()
        {
            PreInteraction();
            ExecuteInteraction(); // Overridden in subclasses
            PostInteraction();
        }

        /// <summary>
        /// Optional logic before interaction occurs.
        /// </summary>
        protected virtual void PreInteraction()
        {
            _logger.Log(LogLevel.Debug, $"Preparing interaction with {Name}");
        }

        /// <summary>
        /// Core interaction logic to be implemented by derived classes.
        /// </summary>
        protected abstract void ExecuteInteraction();

        /// <summary>
        /// Applies all registered effects to the object.
        /// </summary>
        protected virtual void ApplyEffects()
        {
            foreach (var effect in _effects)
            {
                effect.Apply(this);
                _logger.Log(LogLevel.Debug, $"Applied {effect.GetType().Name} to {Name}");
            }
        }

        /// <summary>
        /// Optional logic after interaction is completed.
        /// </summary>
        protected virtual void PostInteraction()
        {
            _logger.Log(LogLevel.Debug, $"Finished interaction with {Name}");
            OnInteracted?.Invoke(this);
        }

        /// <summary>
        /// Adds an interaction effect to the object (Decorator pattern).
        /// </summary>
        /// <param name="effect">Effect to apply during interaction.</param>
        public void AddEffect(IInteractionEffect effect)
        {
            _effects.Add(effect);
            _logger.Log(LogLevel.Debug, $"Added effect {effect.GetType().Name} to {Name}");
        }

        /// <summary>
        /// Moves the object to a new position.
        /// </summary>
        /// <param name="newPosition">New X and Y coordinates.</param>
        public virtual void MoveTo((int x, int y) newPosition)
        {
            Position = newPosition;
            _logger.Log(LogLevel.Info, $"{Name} moved to {Position}");
        }
    }

    /// <summary>
    /// Interface for objects that can be interacted with.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Triggers an interaction with the object.
        /// </summary>
        void Interact();

        /// <summary>
        /// Adds an interaction effect to the object.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        void AddEffect(IInteractionEffect effect);
    }

    /// <summary>
    /// Interface for objects with a unique identifier.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid Id { get; }
    }

    /// <summary>
    /// Interface for effects that can be applied to world objects.
    /// </summary>
    public interface IInteractionEffect
    {
        /// <summary>
        /// Applies the effect to the specified world object.
        /// </summary>
        /// <param name="target">Target object of the effect.</param>
        void Apply(WorldObject target);
    }
}
