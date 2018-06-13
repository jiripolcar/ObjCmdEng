using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class Commander : MonoBehaviour
    {
        public static List<Command> ActiveCommands;
        public static List<Command> DisposedCommands;
        public static Dictionary<string, Commander> RegisteredCommanders;

        public static Commander Find(string key)
        {
            Commander c;
            if (RegisteredCommanders.TryGetValue(key, out c))
                return c;
            else
                return null;
        }

        public static Command Do(Command command)
        {
            if (command.Owner == null)
                Debug.Log("je to nulla");
            command.Owner.AddCommand(command);
            return command;
        }

        private static void InitAndRegister(Commander subject)
        {
            if (ActiveCommands == null)
                ActiveCommands = new List<Command>();
            if (DisposedCommands == null)
                DisposedCommands = new List<Command>();
            if (RegisteredCommanders == null)
                RegisteredCommanders = new Dictionary<string, Commander>();
            if (RegisteredCommanders.ContainsKey(subject.name.ToLower()) || RegisteredCommanders.ContainsValue(subject))
                throw new System.Exception("Commander with the name '" + subject.name + "' is already registered!");
            else
                RegisteredCommanders.Add(subject.name.ToLower(), subject);
        }



    }
}