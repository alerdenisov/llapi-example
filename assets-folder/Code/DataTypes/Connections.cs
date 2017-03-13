using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UniRx;

namespace LlapiExample
{
    public class ConnectionsRepository : IEnumerable<int>
    {
        private HashSet<int> connections;
        private Subject<int> newConnection;
        private Subject<int> newDisconnection;

        public ConnectionsRepository()
        {
            newConnection = new Subject<int>();
            newDisconnection = new Subject<int>();
            connections = new HashSet<int>();
        }

        public IObservable<int> Connections { get { return newConnection;  } }
        public IObservable<int> Disconnections { get { return newDisconnection; } }

        public int Count { get { return connections.Count; } }

        public bool Add(int connection)
        {
            if(connections.Add(connection))
            {
                newConnection.OnNext(connection);
                return true;
            }
            return false;
        }

        public bool Remove(int connection)
        {
            if (connections.Remove(connection))
            {
                newDisconnection.OnNext(connection);
                return true;
            }
            return false;
        }

        public int[] Clone()
        {
            return connections.ToArray();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return connections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return connections.GetEnumerator();
        }
    }
}