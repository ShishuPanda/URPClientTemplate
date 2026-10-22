using System;
using Core.Architecture;

namespace Core.Skills.Networking
{
    public enum NetworkConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting
    }

    public readonly struct NetworkStateChangedEvent : IGameEvent
    {
        public readonly NetworkConnectionState State;
        public NetworkStateChangedEvent(NetworkConnectionState state) => State = state;
    }

    /// <summary>
    /// Network Topology abstraction layer compatible with Netcode for GameObjects, Photon Fusion, or Mirror.
    /// Eliminates direct engine-level network coupling from gameplay systems.
    /// </summary>
    public interface INetworkService : IGameService
    {
        NetworkConnectionState State { get; }
        int PingMilliseconds { get; }
        bool IsServer { get; }
        bool IsClient { get; }

        void StartHost(ushort port);
        void StartClient(string address, ushort port);
        void Disconnect();
        void SendRpc(string rpcName, byte[] payload, bool reliable);
    }

    public sealed class NetworkServiceMock : INetworkService
    {
        public NetworkConnectionState State { get; private set; } = NetworkConnectionState.Disconnected;
        public int PingMilliseconds { get; private set; } = 20;
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }

        public void Initialize()
        {
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void StartHost(ushort port)
        {
            IsServer = true;
            IsClient = true;
            State = NetworkConnectionState.Connected;
            EventBus.Publish(new NetworkStateChangedEvent(State));
        }

        public void StartClient(string address, ushort port)
        {
            IsServer = false;
            IsClient = true;
            State = NetworkConnectionState.Connecting;
            EventBus.Publish(new NetworkStateChangedEvent(State));
        }

        public void Disconnect()
        {
            State = NetworkConnectionState.Disconnected;
            IsServer = false;
            IsClient = false;
            EventBus.Publish(new NetworkStateChangedEvent(State));
        }

        public void SendRpc(string rpcName, byte[] payload, bool reliable)
        {
            // Abstraction layer forwards payload to active transport layer
        }
    }
}
