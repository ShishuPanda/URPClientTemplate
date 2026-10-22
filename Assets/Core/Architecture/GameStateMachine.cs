using System;
using System.Collections.Generic;

namespace Core.Architecture
{
    /// <summary>
    /// Contract for discrete game states handled by the finite state machine.
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Update(float deltaTime);
        void Exit();
    }

    /// <summary>
    /// Deterministic, zero-allocation Game State Machine orchestrating enterprise lifecycle states
    /// (e.g. Boot, Authentication, MainMenu, Matchmaking, InGameLoop, Paused).
    /// </summary>
    public sealed class GameStateMachine : IGameService
    {
        private readonly Dictionary<Type, IGameState> _states = new(16);
        public IGameState CurrentState { get; private set; }
        public Type CurrentStateType => CurrentState?.GetType();

        public void Initialize()
        {
        }

        public void Dispose()
        {
            CurrentState?.Exit();
            CurrentState = null;
            _states.Clear();
        }

        /// <summary>
        /// Registers a state instance with the state machine.
        /// </summary>
        public GameStateMachine RegisterState<T>(T state) where T : class, IGameState
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            _states[typeof(T)] = state;
            return this;
        }

        /// <summary>
        /// Transitions to the target state.
        /// </summary>
        public void TransitionTo<T>() where T : class, IGameState
        {
            var targetType = typeof(T);
            if (!_states.TryGetValue(targetType, out var nextState))
            {
                throw new InvalidOperationException($"[GameStateMachine] State {targetType.Name} is not registered.");
            }

            if (CurrentState == nextState) return;

            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

        /// <summary>
        /// Updates the current active state. Call inside the main game loop.
        /// </summary>
        public void Tick(float deltaTime)
        {
            CurrentState?.Update(deltaTime);
        }
    }
}
