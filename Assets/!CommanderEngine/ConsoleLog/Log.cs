using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;


namespace ConsoleLog
{
    public class Log : MonoBehaviour
    {
        private static Log carrier;
        /// <summary>
        /// Instance of the Log MonoBehaviour
        /// </summary>
        public static Log Carrier
        {
            get
            {
                if (carrier == null)
                {
                    carrier = new GameObject("ConsoleLogCarrier").AddComponent<Log>();
                    Write("Console Log is not present in scene. Creating default.", LogRecordType.Error);
                }
                return carrier;
            }
            private set
            {
                if (carrier == null)
                    carrier = value;
                else
                    carrier.WriteToLog("An instance of Log already exists with name: " + Carrier.gameObject.name, LogRecordType.Error);
            }
        }
        /// <summary>Sets the console on or off.</summary>
        public static bool ShowConsole { get { if (Carrier) return Carrier.showConsole; else return false; } set { if (Carrier) Carrier.showConsole = value; } }

        [Tooltip("How often update FPS to log.")] [SerializeField] private float updateFPSInterval = 5;
        [Tooltip("Unity UI Fps display.")] [SerializeField] private Text fpsText;

        [SerializeField] private bool showConsole = false;
        [Tooltip("Log device specifications in the beginning of the log file?")] [SerializeField] private bool logDeviceSpecs = true;
        [SerializeField] private KeyCode consoleToggleKey = KeyCode.K;
        [SerializeField] private int lineWidth = 19;
        [Tooltip("Whether mirror this log messages to Unity console. Auto true in build, change in Awake if you wish.")] [SerializeField] private bool mirrorToUnityDebug = true;

        [Tooltip("Set batch size, in which the log will be saved to file.")] [SerializeField] private int logBatch = 20;

        private int updateFPSFramesCounter = 0;
        private float updateFPSTimeCounter;
        private int maxLines;
        private int currentLine;
        private LogRecord[] consoleContent;
        private Queue<LogRecord> logsWriteToFileBatch = new Queue<LogRecord>();

        void Awake()
        {
            if (!Application.isEditor)
                mirrorToUnityDebug = true;
            Carrier = this;
            maxLines = Mathf.FloorToInt((Screen.height - 30) / lineWidth);
            consoleContent = new LogRecord[maxLines];
            WriteHeader();
            if (showConsole)
                WriteToLog("Press " + consoleToggleKey.ToString() + " to toggle Console.", LogRecordType.Engine, true);            
        }

        void Update()
        {
            if (Input.GetKeyUp(consoleToggleKey))
                showConsole = !showConsole;
            /*        if (Input.GetKeyUp(KeyCode.L) || Input.GetMouseButtonUp(2))
                        Application.CaptureScreenshot(logPath + "_" + Time.time.ToString("0") + ".png");*/

            if (fpsText)
                fpsText.text = "T: " + Time.time.ToString("0.0") + " fps: " + (1 / Time.deltaTime).ToString("0");

            updateFPSTimeCounter -= Time.deltaTime;
            updateFPSFramesCounter++;

            if (updateFPSTimeCounter < 0)
            {
                updateFPSTimeCounter = updateFPSInterval;
                WriteToLog("Frames in " + updateFPSInterval.ToString("0.0") + " seconds: " + updateFPSFramesCounter + " = " + (updateFPSFramesCounter / updateFPSInterval).ToString("0") + " FPS", LogRecordType.Engine, true);
                updateFPSFramesCounter = 0;
            }

        }

        void OnGUI()
        {
            if (showConsole)
            {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
                for (int i = 0; i < maxLines; i++)
                {
                    if (consoleContent[i] == null)
                        GUI.Label(new Rect(0, i * lineWidth + 30, Screen.width, lineWidth), "  ");
                    else
                    {
                        GUI.color = consoleContent[i].Color;
                        GUI.Label(new Rect(0, i * lineWidth + 30, Screen.width, lineWidth), consoleContent[i].ToString());
                    }
                }
            }
        }


        private void OnApplicationQuit()
        {
            WriteToFile();
        }

        /// <summary>
        /// Writes the log entry according to the setting of the Log MonoBehaviour instance.
        /// </summary>
        public static void Write(string message, LogRecordType type = LogRecordType.Message, bool disallowMirrorToConsole = false)
        {
            Carrier.WriteToLog(message, type, disallowMirrorToConsole);
        }

        private void WriteToLog(string message, LogRecordType type = LogRecordType.Message, bool disallowMirrorToConsole = false)
        {
            LogRecord lr = new LogRecord(message, type, Time.time);

            consoleContent[currentLine] = lr;
            if (++currentLine >= maxLines)
                currentLine = 0;
            consoleContent[currentLine] = null;
            logsWriteToFileBatch.Enqueue(lr);
            if (logsWriteToFileBatch.Count >= logBatch)
                WriteToFile();

            if (mirrorToUnityDebug && !disallowMirrorToConsole)
                MirrorToConsole(lr);
        }

        private static void MirrorToConsole(LogRecord lr)
        {
            if (lr.Type == LogRecordType.Error)
                Debug.LogError(lr.ToString());
            else if (lr.Type == LogRecordType.Warning)
                Debug.LogWarning(lr.ToString());
            else
                Debug.Log(lr.ToString());
        }

        private string _logPath = "";
        private string logPath
        {
            get
            {
                if (_logPath == "")
#if UNITY_EDITOR                    
                    _logPath = /*Application.dataPath + "\\Logs\\" +*/ "Logs\\Log_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
#else
                    _logPath = /*Application.dataPath + "\\..\\log_"*/ "Log_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
#endif

                return _logPath;
            }
        }

        private void WriteHeader()
        {
            string x = "  ";
            string specs = "PC: " + SystemInfo.deviceModel + x + SystemInfo.deviceName + x +
            SystemInfo.processorType + x + "RAM: " + SystemInfo.systemMemorySize + x +
            "GPU: " + SystemInfo.graphicsDeviceID + x + SystemInfo.graphicsDeviceName + x + SystemInfo.graphicsDeviceType + x + SystemInfo.graphicsDeviceVendor + x +
            "OS: " + SystemInfo.operatingSystem + x + SystemInfo.operatingSystemFamily;
            WriteToLog("Log from: " + System.DateTime.Now.ToString("yyyy MM dd HH:mm:ss"), LogRecordType.Message);
            if (logDeviceSpecs) WriteToLog(specs, LogRecordType.Message, true);
        }

        private void WriteToFile()
        {
            StreamWriter sw = new StreamWriter(logPath, true);
            while (logsWriteToFileBatch.Count > 0)
            {
                LogRecord lm = logsWriteToFileBatch.Dequeue();
                sw.WriteLine(lm.ToString());
            }
            sw.Close();
        }
    }
}