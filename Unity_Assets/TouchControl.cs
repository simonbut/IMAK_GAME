using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using IMAK_Game;

public class TouchControl : MonoBehaviour
{
    private float ratio_X;
    private float ratio_Y;
    CSMain TempScript;

    [DllImport("Game", EntryPoint = "GetTouch")]
    extern static void GetTouch(float x, int slot, float y = -128.0f);


    void Start()
    {
        Input.multiTouchEnabled = true;
        GameObject Inter = GameObject.Find("Game_Test");
        TempScript = Inter.GetComponent<CSMain>();
        ratio_X = TempScript.RATIO_X;
        ratio_Y = TempScript.RATIO_Y;
    }

    // Update is called once per frame  
    void Update()
    {
    }


    public int Count;
    public void MoblieInput()
    {
        Count = Input.touchCount > 4 ? 4 : Input.touchCount;
        if (Count > 0)
        {
            float mx = -128;
            float my = -196;

            for (int i = 0; i < Count; ++i){
                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    mx = Input.touches[i].position.x;
                    my = Input.touches[0].position.y;
                    GetTouch(mx, i, Screen.height - my);
                }
            }
            // For Test
        }

        // For Test in PC
        if (Input.GetMouseButtonDown(0))
        {
            GetTouch(Input.mousePosition.x, 0, Screen.height - Input.mousePosition.y);
        }
    }
}