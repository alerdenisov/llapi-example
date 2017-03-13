using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace LlapiExample
{

    public enum Team : byte
    {
        Unknown,
        TeamA,
        TeamB
    }

    public class CommanderStatus
    {
        private Team team;
        private int id;
        private BehaviorSubject<Firerer> characterInstance;
        private Dictionary<Guid, NetworkEntity> entities;
        private DiContainer container;
        private Firerer firererPrefab;
        private Crate cratePrefab;
        private Bullet bulletPrefab;
        private OutgoingCommandsQueue outgoings;

        public Firerer Character
        {
            get { return characterInstance.Value; }
            set { characterInstance.OnNext(value); }
        }

        public Team Team { get { return team; } }

        public IObservable<Firerer> ObservableCharacter { get { return characterInstance; } }

        private static int charStatusCount = 0;

        public CommanderStatus(DiContainer container, Firerer firererPrefab, Crate crate, Bullet bullet, OutgoingCommandsQueue outgoings)
        {
            this.outgoings = outgoings;
            this.firererPrefab = firererPrefab;
            this.cratePrefab = crate;
            this.bulletPrefab = bullet;
            this.container = container;

            characterInstance = new BehaviorSubject<Firerer>(null);

            entities = new Dictionary<Guid, NetworkEntity>();
        }

        internal void SetTeam(Team team)
        {
            this.team = team;
        }

        private NetworkEntity Spawn(NetworkEntity prefab, Vector3 position, Vector3 forward, Guid id)
        {
            var rotation = Quaternion.LookRotation(forward, Vector3.up);

            var instance = container.InstantiatePrefab(prefab.gameObject);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.localScale = Vector3.one;

            var network = instance.GetComponent<NetworkEntity>();

            network.IsMine = this.id == 0;
            network.CommanderId = this.id;
            network.Commander = this;
            network.Id = id;

            entities.Add(network.Id, network);

            return network;
        }

        public NetworkEntity SpawnPrefab(PrefabType type, Vector3 position, Vector3 forward, Guid id)
        {
            var rotation = Quaternion.LookRotation(forward, Vector3.up);

            NetworkEntity prefab = null;

            switch(type) {
                case PrefabType.Character:
                    prefab = firererPrefab;
                    break;
                case PrefabType.Crate:
                    prefab = cratePrefab;
                    break;
                case PrefabType.Bullet:
                    prefab = bulletPrefab;
                    break;
            }

            var entity = Spawn(prefab, position, forward, id);
            if (type == PrefabType.Character)
            {
                Character = (entity as Firerer);
            }

            return entity;
        }

        public NetworkEntity SpawnCharacter(Vector3? position = null, Vector3? forward = null, Guid? id = null)
        {
            if (!position.HasValue)
                position = Vector3.zero;

            if (!forward.HasValue)
                forward = Vector3.forward;

            if (!id.HasValue)
                id = Guid.NewGuid();

            return SpawnPrefab(PrefabType.Character, position.Value, forward.Value, id.Value);
        }

        public NetworkEntity SpawnCrate(Vector3? position = null, Vector3? forward = null, Guid? id = null)
        {
            if (!position.HasValue)
                position = Vector3.zero;

            if (!forward.HasValue)
                forward = Vector3.forward;

            if (!id.HasValue)
                id = Guid.NewGuid();
            return SpawnPrefab(PrefabType.Crate, position.Value, forward.Value, id.Value);
        }

        public NetworkEntity SpawnBullet(Vector3? position = null, Vector3? forward = null, Guid? id = null)
        {
            if (!position.HasValue)
                position = Vector3.zero;

            if (!forward.HasValue)
                forward = Vector3.forward;

            if (!id.HasValue)
                id = Guid.NewGuid();

            return SpawnPrefab(PrefabType.Bullet, position.Value, forward.Value, id.Value);
        }

        public void Die()
        {
            Character.Die();
            characterInstance.OnNext(null);
        }

    }
}