using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    IUserAction userAction;
    public string gameMessage;
    public int time;
    private GUIStyle titleStyle, textStyle, timeStyle;
    private readonly string title = "Priests and Devils";
    // public Texture2D backgroundImage; // 背景图片
    // Start is called before the first frame update
    void Start()
    {
        time = 60;
        userAction = SSDirector.GetInstance().CurrentSceneController as IUserAction;

        // backgroundImage = Resources.Load<Texture2D>("Textures/sky");
        
        // 设置 Title 的样式
        titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontSize = 50;
        titleStyle.alignment = TextAnchor.MiddleCenter; // 设置文字居中对齐

        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.red;
        textStyle.fontSize = 30;
        textStyle.alignment = TextAnchor.MiddleCenter;

        timeStyle = new GUIStyle();
        timeStyle.normal.textColor = new Color(0.529f, 0.808f, 0.922f, 1.0f);
        timeStyle.fontSize = 30;
        timeStyle.alignment = TextAnchor.MiddleCenter;
    }

    // Update is called once per frame
    void OnGUI()
    {
        // if (backgroundImage != null)
        // {
        //     // 设置背景图片在屏幕上的大小，使其填满整个屏幕
        //     GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundImage, ScaleMode.StretchToFill);
        // }

        userAction.Check();

        // 在指定位置绘制Label
        GUI.Label(new Rect((Screen.width - 400f) / 2, 10f, 400f, 50f), title, titleStyle);
        GUI.Label(new Rect((Screen.width - 400f) / 2, 80f, 400f, 30f), gameMessage, textStyle);
        GUI.Label(new Rect(10, 0, 100, 50), "Time: " + time, timeStyle);
    }
}
