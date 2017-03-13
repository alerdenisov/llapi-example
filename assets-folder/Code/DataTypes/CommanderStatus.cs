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
        private OwnTable table;

        public void SetOwner(int connectionId)
        {
            id = connectionId;
        }

        public Firerer Character
        {
            get { return characterInstance.Value; }
            set { characterInstance.OnNext(value); }
        }

        public Team Team { get { return team; } }

        public IObservable<Firerer> ObservableCharacter { get { return characterInstance; } }

        private static int charStatusCount = 0;

        public CommanderStatus(DiContainer container, Firerer firererPrefab, Crate crate, Bullet bullet, OutgoingCommandsQueue outgoings, OwnTable table)
        {
            this.table = table;
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

        public void Destroy(NetworkEntity entity)
        {
            Destroy(entity.Id);
        }

        public void Destroy(Guid id)
        {
            if(!entities.ContainsKey(id))
            {
                Debug.LogError("Unknown id: " + id);
                return;
            }

            if (entities[id].gameObject)
                GameObject.Destroy(entities[id].gameObject, 0.1f);

            entities.Remove(id);
            table.Release(id);
        }

        public void Damage(Guid id, float amount, Vector3? point = null, Vector3? force = null)
        {
            if(!entities.ContainsKey(id))
            {
                Debug.LogError("Unknown id: " + id);
                return;
            }

            var entity = entities[id] as VitalEntity;

            if(!entity)
            {
                Debug.LogError("Non vital entity: " + entities[id]);
                return;
            }

            if(!point.HasValue)
            {
                point = entity.transform.position;
            }

            if(!force.HasValue)
            {
                force = Vector3.forward;
            }

            entity.DamageReceive(amount, point.Value, force.Value);
        }

        private NetworkEntity Spawn(NetworkEntity prefab, Vector3 position, Vector3 forward, Guid guid)
        {
            var rotation = Quaternion.LookRotation(forward, Vector3.up);

            Debug.LogFormat("Instantiate from container {0}, on commander {1}", prefab.name, this.id);
            var instance = container.InstantiatePrefab(prefab.gameObject);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.localScale = Vector3.one;

            var network = instance.GetComponent<NetworkEntity>();

            network.IsMine = id == 0;
            network.CommanderId = id;
            network.Commander = this;
            network.Id = guid;

            entities.Add(network.Id, network);
            table.Obtain(guid, this.id);

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