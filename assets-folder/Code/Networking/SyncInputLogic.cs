using Zenject;

namespace LlapiExample
{
    public class SyncInputLogic : INetworkLogic
    {
        [Inject] private OutgoingCommandsQueue outgoings;
        [Inject(Id = 0)] private CharacterStatus character;
        [Inject] private DiContainer container;

        public SyncInputLogic()
        {
        }

        public void Setup()
        {
        }

        public void Update()
        {
            if (!character.Controller)
                return;

            outgoings.Enqueue(container.Instantiate<CharacterLook>());
            outgoings.Enqueue(container.Instantiate<CharacterMove>());
        }
    }
}