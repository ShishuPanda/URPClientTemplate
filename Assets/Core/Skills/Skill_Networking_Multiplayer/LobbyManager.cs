using System;
using System.Collections.Generic;
using Core.Architecture;

namespace Core.Skills.Networking
{
    public readonly struct LobbyInfo
    {
        public readonly string LobbyId;
        public readonly string Name;
        public readonly int MaxPlayers;
        public readonly int CurrentPlayers;

        public LobbyInfo(string id, string name, int max, int current)
        {
            LobbyId = id;
            Name = name;
            MaxPlayers = max;
            CurrentPlayers = current;
        }
    }

    /// <summary>
    /// Decoupled Lobby and Matchmaking service interface.
    /// </summary>
    public interface ILobbyService : IGameService
    {
        void CreateLobby(string lobbyName, int maxPlayers, Action<string> onCreated);
        void JoinLobby(string lobbyId, Action<bool> onJoined);
        void LeaveLobby();
        IReadOnlyList<LobbyInfo> GetCachedLobbies();
    }

    public sealed class LobbyManager : ILobbyService
    {
        private readonly List<LobbyInfo> _lobbies = new(16);

        public void Initialize()
        {
        }

        public void Dispose()
        {
            _lobbies.Clear();
        }

        public void CreateLobby(string lobbyName, int maxPlayers, Action<string> onCreated)
        {
            var id = Guid.NewGuid().ToString("N")[..8];
            _lobbies.Add(new LobbyInfo(id, lobbyName, maxPlayers, 1));
            onCreated?.Invoke(id);
        }

        public void JoinLobby(string lobbyId, Action<bool> onJoined)
        {
            onJoined?.Invoke(true);
        }

        public void LeaveLobby()
        {
        }

        public IReadOnlyList<LobbyInfo> GetCachedLobbies() => _lobbies;
    }
}
