using UniRx;
using Zenject;

namespace LlapiExample
{
    public class CharacterStatus
    {
        private BehaviorSubject<FirererController> controllerInstance;
        private BehaviorSubject<Firerer> characterInstance;
        private DiContainer container;
        private Firerer firererPrefab;

        public FirererController Controller
        {
            get { return controllerInstance.Value; }
            set { controllerInstance.OnNext(value); }
        }

        public Firerer Character
        {
            get { return characterInstance.Value; }
            set { characterInstance.OnNext(value); }
        }

        public IObservable<FirererController> ObservableController { get { return controllerInstance; } }
        public IObservable<Firerer> ObservableCharacter { get { return characterInstance; } }

        private static int charStatusCount = 0;

        public CharacterStatus(DiContainer container, Firerer firererPrefab)
        {
            this.firererPrefab = firererPrefab;
            this.container = container;
            controllerInstance = new BehaviorSubject<FirererController>(null);
            characterInstance = new BehaviorSubject<Firerer>(null);
        }

        public void Spawn()
        {
            var characterGo = container.InstantiatePrefab(firererPrefab);
            Character = characterGo.GetComponent<Firerer>();
            Controller = Character.GetComponent<FirererController>();
        }

        public void Die()
        {
            Controller.Die();
            controllerInstance.OnNext(null);
            characterInstance.OnNext(null);
        }

    }
}