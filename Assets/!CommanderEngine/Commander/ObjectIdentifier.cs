using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public class ObjectIdentifier : MonoBehaviour
    {
        public static Dictionary<string, ObjectIdentifier> objectIdentifiers { get; set; }

        [SerializeField] public SeatControl seatControl;


        [Tooltip("If null or zero length, will use gameObject.name")]
        [SerializeField] public string identifyAs = "";

        private void Reset()
        {
            if (!seatControl)
                seatControl = GetComponent<SeatControl>();
        }

        void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            Deregister();
        }

        public void ChangeRegistration(string newId)
        {
            Deregister();
            identifyAs = newId.ToLower();
            Register();
        }

        private void Register()
        {
            identifyAs = (identifyAs.Length > 0 ? identifyAs : name).ToLower();

            if (objectIdentifiers == null)
                objectIdentifiers = new Dictionary<string, ObjectIdentifier>();
            if (objectIdentifiers.ContainsKey(identifyAs))
                ConsoleLog.Log.Write("RegString already exists! " + identifyAs, ConsoleLog.LogRecordType.Error);
            objectIdentifiers.Add(identifyAs, this);
        }

        private bool Deregister()
        {
            string regString = identifyAs.Length > 0 ? identifyAs : name;
            return objectIdentifiers.Remove(regString);
        }

        public static ObjectIdentifier Find(string key)
        {
            ObjectIdentifier oi;
            if (objectIdentifiers.TryGetValue(key.ToLower(), out oi))
                return oi;
            else
                return null;
        }

        public static ObjectIdentifier RegisterGameObject(GameObject target, string identifier = "")
        {
            ObjectIdentifier oi = target.AddComponent<ObjectIdentifier>();
            oi.identifyAs = identifier;
            return oi;
        }



    }
}