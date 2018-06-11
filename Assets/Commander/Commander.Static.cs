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

        /*        public static Command FindCommandByID(uint commandID)
                {
                    ActiveCommands.Find((c) => { return commandID == c.commandID; });
                    return null;
                }

                public static Command FindAnyCommandByID(uint commandID)
                {
                    Command found = FindCommandByID(commandID);
                    if (found == null)
                        DisposedCommands.Find((c) => { return commandID == c.commandID; });
                    return null;
                }*/

        public static Command Do(Command c)
        {
            c.Owner.AddCommand(c);
            return c;
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