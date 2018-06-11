using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;


namespace ConsoleLog
{
    public class Log : MonoBehaviour
    {
        /// <summary>
        /// Instance of the Log MonoBehaviour
        /// </summary>
        public static Log Carrier { get; set; }
        /// <summary>
        /// Sets the console on or off.
        /// </summary>
        public static bool ShowConsole { get { if (Carrier) return Carrier.showConsole; else return false; } set { if (Carrier) Carrier.showConsole = value; } }


        [Tooltip("How often update FPS to log.")] [SerializeField] private float updateFPSInterval = 5;
        [Tooltip("Unity UI Fps display.")] [SerializeField] private Text fpsText;
        private int updateFPSFramesCounter = 0;
        private float updateFPSTimeCounter;

        [SerializeField] private bool showConsole = false;
        [SerializeField] private KeyCode consoleToggleKey = KeyCode.K;
        [SerializeField] private int lineWidth = 19;
        [SerializeField] private bool mirrorToUnityDebug = true;
        private int maxLines;
        private int currentLine;
        private LogRecord[] consoleContent;
        private Queue<LogRecord> logsWriteToFileBatch = new Queue<LogRecord>();
        [Tooltip("Set batch size, in which the log will be saved to file.")] [SerializeField] private int logBatch = 20;

        void Awake()
        {
            if (Carrier == null)
                Carrier = this;
            else
                WriteToLog("An instance of Log already exists with name: " + Carrier.gameObject.name, LogRecordType.Error);
            maxLines = Mathf.FloorToInt((Screen.height - 30) / lineWidth);
            consoleContent = new LogRecord[maxLines];

            WriteToLog("Press " + consoleToggleKey.ToString() + " to toggle Console.", LogRecordType.Engine);
            
        }

        private void Start()
        {
            WriteHeader();
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
                WriteToLog("Frames in " + updateFPSInterval.ToString("0.0") + " seconds: " + updateFPSFramesCounter + " = " + (updateFPSFramesCounter / updateFPSInterval).ToString("0") + " FPS", LogRecordType.Engine);
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
        public static void Write(string message, LogRecordType type = LogRecordType.Message)
        {
            Carrier.WriteToLog(message, type);
        }

        private void WriteToLog(string message, LogRecordType type = LogRecordType.Message)
        {
            LogRecord lr = new LogRecord(message, type, Time.time);

            consoleContent[currentLine] = lr;
            if (++currentLine >= maxLines)
                currentLine = 0;
            consoleContent[currentLine] = null;
            logsWriteToFileBatch.Enqueue(lr);
            if (logsWriteToFileBatch.Count >= logBatch)
                WriteToFile();

            if (mirrorToUnityDebug)
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
                    _logPath = Application.dataPath + "\\Logs\\" + System.DateTime.Now.ToFileTime() + ".log";
#else
                    _logPath = Application.dataPath + "\\..\\log_" + System.DateTime.Now.ToFileTime() + ".txt";
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
            WriteToLog("Log " + System.DateTime.Now.ToShortDateString(), LogRecordType.Message);
            WriteToLog(specs, LogRecordType.Message);
            WriteToFile();
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