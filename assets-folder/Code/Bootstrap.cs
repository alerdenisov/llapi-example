using UnityEngine;
using System.Collections;
using Zenject;
using System;
using UniRx;

namespace LlapiExample
{
    public class Bootstrap : MonoInstaller
    {
        public StatusView Status;
        public SelectRoleView Role;
        public ServerConfigView ServerConfig;
        public ClientConfigView ClientConfig;
        public StartGameView StartGame;
        public RespawnView RespawnView;

        public Firerer CharacterPrefab;
        public Crate CratePrefab;
        public FirererPlayerInput InputPrefab;
        public GameplayCamera CameraPrefab;
        public Bullet BulletPrefab;

        public override void InstallBindings()
        {
            BindState();
            BindCommands();
            BindLogics();
            BindViews();
            BindHandlers();
            BindPrefabs();

            Container.Bind<CommanderStatus>().WithId(0).To<CommanderStatus>().AsCached();
        }

        private void BindPrefabs()
        {
            Container.Bind<Firerer>().FromInstance(CharacterPrefab).AsSingle();
            Container.Bind<Crate>().FromInstance(CratePrefab).AsSingle();
            Container.Bind<Bullet>().FromInstance(BulletPrefab).AsSingle();

            Container.Bind<FirererPlayerInput>().FromComponentInNewPrefab(InputPrefab).AsSingle();
            Container.Bind<GameplayCamera>().FromComponentInNewPrefab(CameraPrefab).AsSingle();
        }

        private void BindState()
        {
            Container.Bind<ConnectionStatus>().FromInstance(new ConnectionStatus()).AsSingle();
            Container.Bind<PlayStatus>().FromInstance(new PlayStatus()).AsSingle();

            Container.Bind<ConnectionsRepository>().FromInstance(new ConnectionsRepository()).AsSingle();
            Container.Bind<IncomingCommandsQueue>().FromInstance(new IncomingCommandsQueue()).AsSingle();
            Container.Bind<OutgoingCommandsQueue>().FromInstance(new OutgoingCommandsQueue()).AsSingle();
        }

        public override void Start()
        {
            base.Start();
            DontDestroyOnLoad(gameObject);
            Container.InstantiateComponent<NetworkController>(gameObject);
        }

        private void BindHandlers()
        {
        }

        private void BindViews()
        {
            Container.Bind<SelectRoleView>().FromInstance(Role).AsSingle();
            Container.Bind<ServerConfigView>().FromInstance(ServerConfig).AsSingle();
            Container.Bind<ClientConfigView>().FromInstance(ClientConfig).AsSingle();
            Container.Bind<StartGameView>().FromInstance(StartGame).AsSingle();
            Container.Bind<StatusView>().FromInstance(Status).AsSingle();
            Container.Bind<RespawnView>().FromInstance(RespawnView).AsSingle();
        }

        private void BindLogics()
        {
            BindLogic<ClientStartGameLogic>("client");
            BindLogic<ServerLogic>("server");
            BindLogic<ServerStartGameLogic>("server");

            BindLogic<RepositoriesForConnectionLogic>("both");
            BindLogic<InitializeGameObjectsLogic>("both");
            BindLogic<UpdateStatusLogic>("both");
            BindLogic<CharacterSpawnLogic>("both");

            BindLogic<SyncInputLogic>("both");

            // commands handlers
            // BindLogic<CharacterSpawnHadler>("both");
            BindLogic<CharacterLookHandler>("both");
            BindLogic<CharacterMoveHandler>("both");
            BindLogic<CharacterShootHandler>("both");
            // BindLogic<CharacterBulletHandler>("both");
            // BindLogic<CharacterBuiltHandler>("both");

            BindLogic<EntityCreateHandler>("both");
            BindLogic<EntityDamageHandler>("both");
            BindLogic<EntityRemoveHandler>("both");
        }

        private void BindLogic<T>(string id) where T : INetworkLogic
        {
            Container.Bind<INetworkLogic>().WithId(id).To<T>().AsSingle();
        }

        private void BindCommands()
        {
            BindCommand<StartGame>(CommandIds.Game_Start);

            BindCommand<CharacterSpawn>(CommandIds.Character_Spawn);
            BindCommand<CharacterDie>(CommandIds.Character_Die);
            BindCommand<CharacterShoot>(CommandIds.Character_Shoot);
            BindCommand<CharacterLook>(CommandIds.Character_Look);
            BindCommand<CharacterMove>(CommandIds.Character_Move);
            BindCommand<CharacterBuilt>(CommandIds.Character_Built);
            BindCommand<CharacterBullet>(CommandIds.Character_Bullet);

            BindCommand<EntityCreate>(CommandIds.Entity_Create);
            BindCommand<EntityDamage>(CommandIds.Entity_Damage);
            BindCommand<EntityRemove>(CommandIds.Entity_Remove);
        }

        private void BindCommand<T>(byte id) where T : ICommand
        {
            Container.Bind<ICommand>().WithId(id).To<T>().AsCached();
        }
    }
}