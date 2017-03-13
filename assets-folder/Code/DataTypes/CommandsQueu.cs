using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace LlapiExample
{
    public class IncomingCommandsQueue : CommandsQueue { }
    public class OutgoingCommandsQueue : CommandsQueue { }

    public class CommandsQueue : IEnumerable<ICommand>
    {
        private Queue<ICommand> commands;
        private Subject<ICommand> commandsStream;
        public IObservable<ICommand> Observable { get { return commandsStream; } }

        public CommandsQueue()
        {
            commands = new Queue<ICommand>();
            commandsStream = new Subject<ICommand>();
        }

        public void Enqueue(ICommand cmd)
        {
            commands.Enqueue(cmd);
            commandsStream.OnNext(cmd);
        }

        public void Flush()
        {
            commands.Clear();
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return commands.GetEnumerator();
        }
    }
}