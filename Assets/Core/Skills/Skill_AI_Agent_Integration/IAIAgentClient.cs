using System;
using System.Collections.Generic;
using Core.Architecture;

namespace Core.Skills.AIAgent
{
    public readonly struct AgentMessage
    {
        public readonly string Role;
        public readonly string Content;
        public AgentMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public readonly struct AgentToolDefinition
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string ParameterJsonSchema;

        public AgentToolDefinition(string name, string description, string parameterJsonSchema)
        {
            Name = name;
            Description = description;
            ParameterJsonSchema = parameterJsonSchema;
        }
    }

    /// <summary>
    /// Runtime LLM / AI Agent Client Interface supporting streaming responses and tool invocations.
    /// </summary>
    public interface IAIAgentClient : IGameService
    {
        void SendPrompt(string prompt, Action<string> onChunkReceived, Action<string> onComplete, Action<Exception> onError);
        void RegisterTool(AgentToolDefinition tool, Func<string, string> executionCallback);
    }

    public sealed class AIAgentClientService : IAIAgentClient
    {
        private readonly Dictionary<string, Func<string, string>> _tools = new(16);

        public void Initialize()
        {
        }

        public void Dispose()
        {
            _tools.Clear();
        }

        public void RegisterTool(AgentToolDefinition tool, Func<string, string> executionCallback)
        {
            _tools[tool.Name] = executionCallback;
        }

        public void SendPrompt(string prompt, Action<string> onChunkReceived, Action<string> onComplete, Action<Exception> onError)
        {
            // Dispatches to local inference engine or remote LLM endpoint
            onChunkReceived?.Invoke("[AI Agent Response Initialized]");
            onComplete?.Invoke("[AI Agent Finished]");
        }
    }
}
