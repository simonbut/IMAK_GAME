using UnityEngine;
using System.Runtime.InteropServices;

public class AudioPlay : MonoBehaviour {

    public GameObject NowOnCamera;
    public AudioSource Player1;

    // Shared Parameters
    public string SrcName;
    public int Len_in_10msec;

    public bool GetBGM(string Song_Name)
    {
        Player1.clip = Resources.Load(Song_Name) as AudioClip;
        if (Player1.clip != null)
        {   // Transfer the parameters
            Len_in_10msec = (int)(Player1.clip.length * 100.0f);
            SrcName = Song_Name;
            return true;
        }
        else
        {
            Debug.Log(string.Format("File Not Found -- {0}", Song_Name));
            return false;
        }
    }

    private AudioClip[] ListSE = new AudioClip[5];
    public void LoadSE()
    {
        ListSE[0] = Resources.Load("Click") as AudioClip;
        ListSE[1] = Resources.Load("Miss") as AudioClip;
        ListSE[2] = Resources.Load("Bad") as AudioClip;
        ListSE[3] = Resources.Load("Good") as AudioClip;
        ListSE[4] = Resources.Load("Perfect") as AudioClip;

    }

    private int[] SEQueue = new int[4]; // At most 4 simultaneously

    [DllImport("Game", EntryPoint = "ControlBGM")]
    extern static int ControlBGM();

    [DllImport("Game", EntryPoint = "ControlSE")]
    extern static void ControlSE(
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
        int[] SE);

    void Start () {
        NowOnCamera = GameObject.Find("Game_Test");
        if (NowOnCamera == null)
            Debug.Log("The Main Game Object was not found!");
        Player1 = NowOnCamera.AddComponent<AudioSource>();
        GetBGM("Yinhuacai");   // Load BGM here
        LoadSE();   // Load Sound Effects
    }


    public bool NowOnPlay = false;
    void Update () {
        if (ControlBGM() == 1 && !NowOnPlay)
        {
            Player1.Play();
            NowOnPlay = true;
        }
        else if(ControlBGM() == 2)
        {
            Player1.Pause();
            NowOnPlay = false;
        }
        else if(ControlBGM() == 0)
        {
            Player1.Stop();
            NowOnPlay = false;
        }

        ControlSE(SEQueue);
        foreach (int i in SEQueue){
            if (i > 0){
                Player1.PlayOneShot(ListSE[i - 1]); // i : 1 ~ 5
            }
        }
    }
}
