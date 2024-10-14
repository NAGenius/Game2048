using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基于 MVC 模式的 2048 小游戏
/// </summary>
public class Game2048 : MonoBehaviour {

    // Model 部分
    private int[,] grid = new int[4, 4]; // 4x4 游戏网格
    private bool gameWon = false; // 游戏状态
    private bool gameOver = false;

    private GUIStyle tileStyle, labelStyle, boxStyle, buttonStyle; // UI 样式
    private Texture2D backgroundTexture, tileTexture, buttonTexture; // 纹理对象

    /// <summary>
    /// 控件、纹理、UI样式初始化, 注意和 Awake() 的区别
    /// </summary>
    void Start() {
        // 自定义纹理, 也可以自行加载纹理图片
        backgroundTexture = CreateGradientTexture(new Color(0.9f, 0.9f, 0.9f), new Color(0.7f, 0.7f, 0.7f)); // 背景纹理
        tileTexture = CreateSolidColorTexture(new Color(0.8f, 0.7f, 0.6f)); // 方块纹理
        buttonTexture = CreateButtonTexture(new Color(0.2f, 0.6f, 0.8f)); // 按钮纹理

        Init();
    }

    /// <summary>
    /// View 部分, 游戏的 UI 渲染 (视图)
    /// </summary>
    void OnGUI() {
        InitGUIStyles();

        GUI.DrawTexture(new Rect(100, 25, 400, 400), backgroundTexture);

        if (GUI.Button(new Rect(250, 450, 100, 40), "Restart", buttonStyle)) {
            Init();
        }

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                DrawTile(i, j);
            }
        }

        if (gameWon) {
            GUI.Box(new Rect(150, 200, 300, 100), "You Win!", labelStyle);
        }
        else if (gameOver) {
            GUI.Box(new Rect(150, 200, 300, 100), "Game Over!", labelStyle);
        }
    }

    
    void InitGUIStyles() {
        if (tileStyle == null) {
            tileStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter
            };

            labelStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = buttonTexture;

            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = backgroundTexture;
        }
    }

    void DrawTile(int row, int col) {
        int value = grid[row, col];
        string label = value == 0 ? "" : value.ToString();

        tileStyle.normal.background = CreateTileTexture(value);
        GUI.Button(new Rect(120 + col * 90, 45 + row * 90, 80, 80), label, tileStyle);
    }

    /// <summary>
    /// Controller 部分, 游戏逻辑控制 (游戏中需要改变 Model 的地方)
    /// 初始化游戏状态
    /// </summary>
    void Init() {
        grid = new int[4, 4];
        AddRandomTile();
        AddRandomTile();
        gameWon = false;
        gameOver = false;
    }

    /// <summary>
    /// 在空白位置添加一个随机的 2 或 4
    /// </summary>
    void AddRandomTile() {
        List<Vector2> emptyTiles = new List<Vector2>();

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (grid[i, j] == 0) {
                    emptyTiles.Add(new Vector2(i, j));
                }
            }
        }

        if (emptyTiles.Count > 0) {
            Vector2 randomPos = emptyTiles[Random.Range(0, emptyTiles.Count)];
            grid[(int)randomPos.x, (int)randomPos.y] = Random.value < 0.9f ? 2 : 4;
        }
    }

    Texture2D CreateSolidColorTexture(Color color) {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    Texture2D CreateGradientTexture(Color topColor, Color bottomColor) {
        Texture2D texture = new Texture2D(1, 2);
        texture.SetPixel(0, 0, bottomColor);
        texture.SetPixel(0, 1, topColor);
        texture.Apply();
        return texture;
    }

    Texture2D CreateButtonTexture(Color buttonColor) {
        Texture2D texture = new Texture2D(128, 128);
        Color shadowColor = new Color(buttonColor.r * 0.8f, buttonColor.g * 0.8f, buttonColor.b * 0.8f);

        for (int y = 0; y < 128; y++) {
            for (int x = 0; x < 128; x++) {
                if (x > 10 && y > 10) {
                    texture.SetPixel(x, y, shadowColor); 
                } else {
                    texture.SetPixel(x, y, buttonColor);
                }
            }
        }
        texture.Apply();
        return texture;
    }

    Texture2D CreateTileTexture(int value) {
        float intensity = Mathf.Log(value + 1, 2) / 11.0f;
        Color tileColor = new Color(0.9f - intensity, 0.8f - intensity, 0.7f - intensity);
        return CreateSolidColorTexture(tileColor);
    }

    /// <summary>
    /// 处理游戏输入, 每一帧调用
    /// </summary>
    void Update() {
        if (gameWon || gameOver) return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp();
        else if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
    }

    /// <summary>
    /// 上移
    /// </summary>
    void MoveUp() {
        bool moved = SlideUp();
        if (moved) {
            AddRandomTile();
            CheckGameOver();
        }
    }

    /// <summary>
    /// 下移
    /// </summary>
    void MoveDown() {
        RotateGrid(180);
        bool moved = SlideUp();
        RotateGrid(180);
        if (moved) {
            AddRandomTile();
            CheckGameOver();
        }
    }

    /// <summary>
    /// 左移
    /// </summary>
    void MoveLeft() {
        RotateGrid(90);
        bool moved = SlideUp();
        RotateGrid(270);
        if (moved) {
            AddRandomTile();
            CheckGameOver();
        }
    }

    /// <summary>
    /// 右移
    /// </summary>
    void MoveRight() {
        RotateGrid(270);
        bool moved = SlideUp();
        RotateGrid(90);
        if (moved) {
            AddRandomTile();
            CheckGameOver();
        }
    }

    /// <summary>
    /// 向上滑动并合并相同的数字
    /// </summary>
    /// <returns></returns>
    bool SlideUp() {
        bool moved = false;

        for (int j = 0; j < 4; j++) {
            int[] column = new int[4];
            int pos = 0;

            // 计算非零数字
            for (int i = 0; i < 4; i++) {
                if (grid[i, j] != 0) {
                    column[pos] = grid[i, j];
                    pos++;
                }
            }

            // 合并
            for (int i = 0; i < 3; i++) {
                if (column[i] != 0 && column[i] == column[i + 1]) {
                    column[i] *= 2;
                    column[i + 1] = 0;
                    if (column[i] == 2048) {
                        gameWon = true;
                    }
                    moved = true;
                }
            }

            // 移动
            int[] newColumn = new int[4];
            pos = 0;
            for (int i = 0; i < 4; i++) {
                if (column[i] != 0) {
                    newColumn[pos] = column[i];
                    pos++;
                }
            }

            // 更新网格
            for (int i = 0; i < 4; i++) {
                if (grid[i, j] != newColumn[i]) {
                    grid[i, j] = newColumn[i];
                    moved = true;
                }
            }
        }

        return moved;
    }

    /// <summary>
    /// 旋转网格
    /// </summary>
    /// <param name="angle"></param>
    void RotateGrid(int angle) {
        int[,] newGrid = new int[4, 4];
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (angle == 90) {
                    newGrid[j, 3 - i] = grid[i, j];
                }
                else if (angle == 180) {
                    newGrid[3 - i, 3 - j] = grid[i, j];
                }
                else if (angle == 270) {
                    newGrid[3 - j, i] = grid[i, j];
                }
            }
        }
        grid = newGrid;
    }

    /// <summary>
    /// 检查游戏是否结束
    /// </summary>
    void CheckGameOver() {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (grid[i, j] == 0) return; // Still empty spaces
                if (i < 3 && grid[i, j] == grid[i + 1, j]) return; // Same in row
                if (j < 3 && grid[i, j] == grid[i, j + 1]) return; // Same in column
            }
        }
        gameOver = true;
    }
}