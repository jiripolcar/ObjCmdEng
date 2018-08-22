using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class Commander : MonoBehaviour
    {
        public static List<Command> ActiveCommands;
        public static List<Command> DisposedCommands;
        //public static Dictionary<string, Commander> RegisteredCommanders;

        public static Commander Find(string key)
        {
            ObjectIdentifier oi = ObjectIdentifier.Find(key);
            if (oi)
                return oi.commander;
            else
                return null;
        }

        public static Command Do(Command command)
        {
            if (NetworkCommander.IsClient)
                NetworkCommander.CollectCommandSync(command);
            else
                command.Owner.AddCommand(command);
            return command;
        }

        public static Command Do(string commandString)
        {
            return Do(Command.FromString(commandString));
        }

        private static void InitAndRegister(Commander subject)
        {
            if (ActiveCommands == null)
                ActiveCommands = new List<Command>();
            if (DisposedCommands == null)
                DisposedCommands = new List<Command>();
            /*if (RegisteredCommanders == null)
                RegisteredCommanders = new Dictionary<string, Commander>();*/
            /*if (RegisteredCommanders.ContainsKey(subject.name.ToLower()) || RegisteredCommanders.ContainsValue(subject))
                throw new System.Exception("Commander with the name '" + subject.name + "' is already registered!");
            else
                RegisteredCommanders.Add(subject.name.ToLower(), subject);*/

        }



    }
}