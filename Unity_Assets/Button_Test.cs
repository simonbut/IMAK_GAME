using UnityEngine;
using UnityEngine.SceneManagement;
using IMAK_Game;

public class Button_Test : MonoBehaviour
{

    public int Game_Scores;
    public int Game_Combos;
    public int Total_Combos;


    public static bool if_Finished;
    public static int[] Stat_Hits = new int[4];


    void Start()
    {
        //Bridge_Scene = SceneManager.GetSceneAt(1);
        Game_Scores = 0;
        Game_Combos = 0;
        Total_Combos = 0;
    }

    void Update()
    {
        Game_Scores = CSMain.Game_Score;
        Game_Combos = CSMain.Game_Combos;
        Total_Combos = GetBGM_Map.Total_Balls;
        // Keep Conservation
        Stat_Hits[3] = Total_Combos - Stat_Hits[0]- Stat_Hits[1]- Stat_Hits[2];

        CSMain.SpeedControl = 2;    // Change speed here
    }

    public void Click()
    {
        Game_Scores = 0;
        Game_Combos = 0;
        Stat_Hits[0] = 0;
        Stat_Hits[1] = 0;
        Stat_Hits[2] = 0;
        Stat_Hits[3] = 0;
        if_Finished = false;
        SceneManager.LoadScene(1);  // Go To another one
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(50, 100, 200, 100), string.Format("Scores: {0:}", Game_Scores));
        GUI.Label(new Rect(50, 150, 200, 100), "Status: " + (if_Finished ? "Finished" : "Unfinished"));
        GUI.Label(new Rect(50, 200, 200, 100), 
            string.Format("Max Combos: {0:}", Game_Combos)+
            string.Format(" / {0:}", Total_Combos));
        GUI.Label(new Rect(50, 250, 200, 400),
            string.Format("Perfect: {0:}\n", Stat_Hits[0]) +
            string.Format("   Good: {0:}\n", Stat_Hits[1]) +
            string.Format("    Bad: {0:}\n", Stat_Hits[2]) +
            string.Format("   Miss: {0:}",   Stat_Hits[3]));
    }
}