using UnityEngine;
using System.Collections;
using UniRx;

namespace LlapiExample
{
    public enum ConnectionState : byte
    {
        Disconnected = 1 << 7,           // 1000 0000
        Connecting = 1,                  // 0000 0001
        Connected = 1 << 1,              // 0000 0010
        Server = 1 << 2,                 // 0000 0100
        Client = 1 << 3,                 // 0000 1000
        InGame = 1 << 4,                 // 0001 0000
        Paused = 1 << 5,
    }

    public class ConnectionStatus
    {
        private BehaviorSubject<ConnectionState> state;

        public ConnectionStatus(ConnectionState initial = ConnectionState.Disconnected)
        {
            state = new BehaviorSubject<ConnectionState>(initial);
        }

        public IObservable<ConnectionState> Observable
        {
            get { return state; }
        }

        public ConnectionState Current
        {
            get { return state.Value; }
        }

        public bool IsConnecting
        {
            get { return (Current & ConnectionState.Connecting) == ConnectionState.Connecting; }
        }

        public bool IsConnected { get { return (Current & ConnectionState.Connected) == ConnectionState.Connected; } }
        public bool IsServer { get { return (Current & ConnectionState.Server) == ConnectionState.Server; } }
        public bool IsClient { get { return (Current & ConnectionState.Client) == ConnectionState.Client; } }
        public bool InGame { get { return (Current & ConnectionState.InGame) == ConnectionState.InGame; } }
        public bool IsPaused { get { return (Current & ConnectionState.Paused) == ConnectionState.Paused; } }

        public bool IsReady { get { return IsConnected && (IsServer || IsClient); } }
        public bool IsDefinedRole { get { return IsServer || IsClient; } }

        public void Set(ConnectionState next)
        {
            state.OnNext(next);
        }

        public void Append(ConnectionState flag)
        {
            state.OnNext(Current | flag);
        }

        public void Remove(ConnectionState flag)
        {
            state.OnNext(Current & ~flag);
        }
    }
}