using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace LlapiExample
{
    public class ServerLogic : INetworkLogic, IServerLogic
    {
        private StartGameView startGame;
        private OutgoingCommandsQueue outgoings;
        private ConnectionsRepository connections;
        private ConnectionStatus status;

        public void Update()
        {
        }

        public void Setup()
        {
        }

        public ServerLogic(
            StartGameView startGame, 
            ConnectionsRepository connections, 
            OutgoingCommandsQueue outgoings, 
            ConnectionStatus status)
        {
            this.startGame = startGame;
            this.connections = connections;
            this.outgoings = outgoings;
            this.status = status;
        }
    }
}