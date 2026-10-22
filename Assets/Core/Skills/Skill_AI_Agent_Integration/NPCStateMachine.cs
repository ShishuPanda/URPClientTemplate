using System;
using System.Collections.Generic;
using Core.Architecture;

namespace Core.Skills.AIAgent
{
    public interface INpcBehaviorState
    {
        void Enter(NPCStateMachine agent);
        void Update(NPCStateMachine agent, float deltaTime);
        void Exit(NPCStateMachine agent);
    }

    /// <summary>
    /// Event-driven, low-overhead state machine for NPCs driven by local logic or AI agent goals.
    /// </summary>
    public sealed class NPCStateMachine
    {
        private readonly Dictionary<Type, INpcBehaviorState> _states = new(8);
        public INpcBehaviorState CurrentState { get; private set; }

        public void AddState<T>(T state) where T : class, INpcBehaviorState
        {
            _states[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : class, INpcBehaviorState
        {
            if (_states.TryGetValue(typeof(T), out var nextState))
            {
                CurrentState?.Exit(this);
                CurrentState = nextState;
                CurrentState.Enter(this);
            }
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Update(this, deltaTime);
        }
    }
}
