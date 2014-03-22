using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    class UndoRedo : IReceiver, IUndoRedo
    {
        /// <summary>
        /// Liste des messages que l'on peut réémettre pour réaliser à nouveau le changement
        /// </summary>
        private List<Message> ListDo;

        /// <summary>
        /// Liste des messages que l'on peut réémettre pour défaire le changement
        /// </summary>
        private List<Message> ListUndo;

        /// <summary>
        /// Constructeur
        /// </summary>
        public UndoRedo()
        {
            ListDo = new List<Message>();
            ListUndo = new List<Message>();
        }

        /// <summary>
        /// Réception du message
        /// </summary>
        /// <param name="message">message réceptionné</param>
        public void Receive(Message message)
        {
            Message reverse;
            switch (message.Type)
            {
                case ChangeType.AddControl:
                    reverse = new Message(message.Sender, ChangeType.RemoveControl, message.Arguments);
                    break;

                case ChangeType.ChangeProperty:
                    reverse = new Message(message.Sender, ChangeType.ChangeProperty, message.Arguments);
                    break;

                case ChangeType.RemoveControl:
                    reverse = new Message(message.Sender, ChangeType.AddControl, message.Arguments);
                    break;
                default:
                    return;
            }

            switch (message.Action)
            {
                case ActionType.Do:
                    ListUndo.Insert(0, reverse);
                    break;

                case ActionType.Undo:
                    ListDo.Insert(0, reverse);
                    break;
            }
        }

        public void Do()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
