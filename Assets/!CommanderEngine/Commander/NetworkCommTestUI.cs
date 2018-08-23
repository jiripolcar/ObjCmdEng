using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCommTestUI : MonoBehaviour
{

    private string who = "Joe";
    private string what = "sit";
    private string where = "Chair";
    private int num = 1;
    private string command;

    public Text sendButtonText;

    public string Who { get { return who; } set { who = value; UpdateCommand(); } }

    public string What { get { return what; } set { what = value; UpdateCommand(); } }

    public int Num { get { return num; } set { num = value; UpdateCommand(); } }

    public string Where { get { return where; } set { where = value; UpdateCommand(); } }

    //
    public void UpdateCommand()
    {
        command =
            "ow" + Command.valSep + who + Command.parDel +
            "cmd" + Command.valSep + what + Command.parDel +
            "tgt" + Command.valSep + where + num;

            //(what != "standup" ? "tgt" + Command.valSep + where + num : "");
        sendButtonText.text = "Send: " + command;
    }

    public void Do()
    {
        Commander.Do(command);
    }

    private void Start()
    {
        UpdateCommand();
    }
}
