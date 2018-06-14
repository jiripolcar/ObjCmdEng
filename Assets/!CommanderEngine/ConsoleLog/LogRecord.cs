using UnityEngine;

namespace ConsoleLog
{
    public class LogRecord
    {
        private const string FloatToStringFormat = "000.00";
        private const string emptyRecord = "#&@EMPTY@&#";
        private const string delimiter = "\t";
        
        public string Message;
        public LogRecordType Type;
        public Color Color;
        public float Time;
        
        /// <summary>Record a log message.</summary>
        public LogRecord(string message, LogRecordType type = LogRecordType.Message, float time = 0)
        {
            this.Message = message;
            this.Type = type;
            this.Color = RecordTypeToColor(type);
            this.Time = time;
        }

        public override string ToString()
        {
            return Message == emptyRecord ? " " : Time.ToString(FloatToStringFormat) + delimiter + RecordTypeToString(Type) + delimiter + Message;
        }

        /// <summary>Converts a log message type to a corresponding color.</summary>        
        public static Color RecordTypeToColor(LogRecordType type)
        {
            switch (type)
            {
                case LogRecordType.Commander:
                    return Color.cyan;
                case LogRecordType.Engine:
                    return Color.green;
                case LogRecordType.Error:
                    return Color.red;
                case LogRecordType.Input:
                    return Color.yellow;
                case LogRecordType.Message:
                    return Color.white;
                case LogRecordType.Warning:
                    return Color.red;
                case LogRecordType.NetworkCommander:
                    return new Color(1, 0.5f, 0);
                default:
                    throw new System.Exception("RecordTypeToColor: " + type.ToString() + " is unexpected.");
            }
        }

        /// <summary>Gets name of the LogRecordType</summary>  
        public static string RecordTypeToString(LogRecordType type)
        {
            switch (type)
            {
                case LogRecordType.Engine:
                    return "Engine";
                case LogRecordType.Error:
                    return "Error";
                case LogRecordType.Input:
                    return "Input";
                case LogRecordType.Message:
                    return "Message";
                case LogRecordType.Warning:
                    return "Warning";
                case LogRecordType.Commander:
                    return "Commander";
                case LogRecordType.NetworkCommander:
                    return "NetComm";
                default:
                    throw new System.Exception("RecordTypeToString: " + type.ToString() + " is unexpected.");
            }
        }


        public static LogRecord Empty()
        {
            return new LogRecord(emptyRecord);            
        }
    }

    public enum LogRecordType
    {
        Commander,
        NetworkCommander,
        Engine,
        Error,
        Message,
        Input,
        Warning
    }
}