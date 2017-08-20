using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.SceneManagement;

namespace IMAK_Game
{
    
    public class CSMain : MonoBehaviour
    {
        /* ===================================================== //
                          Interface Parameters
        // ===================================================== */
        // Please Transfer these PARAMETERS
        internal string SongName;
        internal int SongLength;

        internal static int Game_Score;
        internal static int Game_Combos;
        internal static int Full_Combos;
        internal static int SpeedControl; // Suggest 1 - 10

        internal static int[] Hit_Results = new int[5];

        /* ===================================================== //
                           Interface Functions
        // ===================================================== */
        // List of Textures
        List<Texture2D> IMAK_Objects        = new List<Texture2D>(0);
        List<bool>      Dynamic_Ptr         = new List<bool>(0); // Dynamic or not

        public Camera cam;

        public int Now_Score;
        public int[] Combos_ = new int[2]; // Now | Global

        // SCREEN param
        public float RATIO_X;
        public float RATIO_Y;
        public int Total_Num_Balls;

        public byte[] LoadTexture(string FileName)
        {
            FileStream fStr = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            fStr.Seek(0, SeekOrigin.Begin);

            byte[] BT = new byte[fStr.Length];
            fStr.Read(BT, 0, (int)fStr.Length);
            fStr.Close();
            fStr.Dispose();
            fStr = null;

            return BT;
        }
        public bool AddObj(string FileName, bool isDynamic = false)
        {
            Texture2D Tex1 = Resources.Load(FileName) as Texture2D;
            if (Tex1 != null)
            {
                IMAK_Objects.Add(Tex1);
                //Debug.Log("Good!");
                if (isDynamic)
                {
                    Dynamic_Ptr.Add(true);
                }
                else
                    Dynamic_Ptr.Add(false);

                
                return true;
            }
            else
            {
                Debug.Log(string.Format("File Not Found -- {0}", FileName));
                return false;
            }
            
        }

        private void InitializeFromDLL()
        {

        }

        // After Rendering
        
        public Color[] Brush;
        private void PaintFromInterface(Interface_Graph Info, bool _isDynamic = false)
        {
            // ChangeAlpha: Defaulted as unused
            // paint out
            if (_isDynamic)
            {
                float Clip_W = 1.0f / (float)Info.ClipMapX;
                float Clip_H = 1.0f / (float)Info.ClipMapY;
                float Clip_X = Clip_W * Info.OffsetX;
                float Clip_Y = Info.ClipPtrY * Clip_H;

                Graphics.DrawTexture(new Rect(Info.x, Screen.height - Info.y, Info.width, -Info.height), IMAK_Objects[Info.Src_ID],
                                 new Rect(Clip_X, Clip_Y, Clip_W, Clip_H),
                                 0, 0, 0, 0,
                                 new Color(0.5f, 0.5f, 0.5f, Info.alpha / 2));
            }
            else
                Graphics.DrawTexture(new Rect(Info.x, Screen.height - Info.y, Info.width, -Info.height), IMAK_Objects[Info.Src_ID]);

            
        }

        public void DrawText(string text, int x, int y, int width, int height,
                             GUIStyle Sty, TextAnchor Alignment = TextAnchor.MiddleLeft)
        {
            int X = (int)((float)x * RATIO_X);
            int Y = (int)((float)y * RATIO_Y);
            int W = (int)((float)width * RATIO_X);
            int H = (int)((float)height * RATIO_Y);
            Sty.alignment = Alignment;
            GUI.Label(new Rect(X, Y, W, H), text, Sty);
        }


        /* ===================================================== //
                           Imported Functions
        // ===================================================== */
        public struct Interface_Graph
        {
            public int x;
            public int y;
            public int width;
            public int height;
            public float alpha;
            public int Src_ID;

            public int ClipMapX;
            public int ClipMapY;
            public int ClipPtrX;
            public int ClipPtrY;

            public int OffsetX;

            public Interface_Graph(int X, int Y, int Width, int Height, int Alpha, int Src,
                                   int _ClipMapX = 1, int _ClipMapY = 1,
                                   int _ClipPtrX = 0, int _ClipPtrY = 0,
                                   int _State = 0)
            {
                x = X;
                y = Y;
                width = Width;
                height = Height;
                alpha = (float)Alpha / 255.0f;
                Src_ID = Src;

                ClipMapX = _ClipMapX;
                ClipMapY = _ClipMapY;
                ClipPtrX = _ClipPtrX;
                ClipPtrY = _ClipPtrY;

                OffsetX = _State;
            }
        }

        public struct Interface_Text
        {
            public int x;
            public int y;
            public int Size;
            public float alpha;
            public char[] Text;

            public Interface_Text(char[] text, int X, int Y, int size, int Alpha)
            {
                x = X;
                y = Y;
                Size = size;
                alpha = (float)Alpha / 255.0f;
                Text = text;
            }
        };

        [DllImport("Game", EntryPoint = "DllAnime")]
        extern static void DllAnime
            ([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 64)]
             Interface_Graph[] DataSet,
             [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 64)]
             Interface_Text[] Text);

        [DllImport("Game", EntryPoint = "Initialize")]
        extern static void Initialize(int W, int H, int Speed); // 1 - 10


        [DllImport("Game", EntryPoint = "UpdateTime")]
        extern static float UpdateTime(float T);

        [DllImport("Game", EntryPoint = "LoopUpdate")]
        extern static void LoopUpdate
            ([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
             int[] DataSet);

        [DllImport("Game", EntryPoint = "Deliver_Result")]
        extern static int Deliver_Result
            ([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            int[] Combos, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
            int[] Results);

        [DllImport("Game", EntryPoint = "UpdateStage")]
        extern static int UpdateStage(int S, bool ChangeState);


        private int TimeTurn(int min, int sec, int msec)
        {
            return (min*6000 + sec*100 + msec/10);
        }

        /* ===================================================== //
                              GUI Functions
        // ===================================================== */
        public int NowOnStage;
        void Start()
        {
            IMAK_Objects.Clear();
            Dynamic_Ptr.Clear();
            Now_Score = 0;
            Combos_[0] = 0;
            Combos_[1] = 0;
            // Get the Song's Info
            GameObject Inter = GameObject.Find("Game_Test");
            AudioPlay TempScript = Inter.GetComponent<AudioPlay>();
            SongName = TempScript.SrcName;
            SongLength = TempScript.Len_in_10msec;

            RATIO_X = (float)Screen.width / 1920.0f;
            RATIO_Y = (float)Screen.height / 1080.0f;

            // Nothing to change here, Everything is done in DllAnime
            UpdateTime(Time.time);  // Unity -> C++ plugin

            // Read the beatmap
            Inter = GameObject.Find("Game_Test");
            GetBGM_Map TempScript2 = Inter.GetComponent<GetBGM_Map>();

            
            TempScript2.ReadMap("Yinhuacai_map");
            Total_Num_Balls = GetBGM_Map.Total_Balls;
            Initialize(Screen.width, Screen.height, SpeedControl);

            //Debug.Log(Screen.width);
            //Debug.Log(Screen.height);
            for (int i = 0; i < 5; ++i)
                Hit_Results[i] = 0;

            // Add Src
            AddObj("Red",true);
            AddObj("Blue", true);
            AddObj("Got", true);
            AddObj("Gradual", true);
            AddObj("Light", true);
            AddObj("Line");
            AddObj("Middle");
            AddObj("Got");
            AddObj("PAUSE", true);
            AddObj("Progress");
            AddObj("Ready", true);
            AddObj("Axis");
            AddObj("Back");
            AddObj("EndBack",true);
            AddObj("TimeButton");

            // Load Text Font
            cam = GetComponent<Camera>();

            // Initialize Text
            myFont = Resources.Load("Asia") as Font;
            My_Text_Name   = string.Format("<color=#151316>{0}</color>", SongName);

            Font_S1.normal.background = null;
            Font_S1.fontSize = (int)(48.0f * RATIO_Y);
            Font_S1.font = myFont;

            Font_S2.normal.background = null;
            Font_S2.fontSize = (int)(52.0f * RATIO_Y);
            Font_S2.font = myFont;

            TS = Inter.GetComponent<TouchControl>();
            NowOnStage = 0;
        }
        private TouchControl TS;
        private int Old_Stage;

        void Update()
        {

            // Nothing to change here, Everything is done in DllAnime


            // Update Results
            Now_Score = Deliver_Result(Combos_, Hit_Results);
            // Deliver Hits Statistics
            Button_Test.Stat_Hits[0] = Hit_Results[1] > 0 ? Hit_Results[1] : Button_Test.Stat_Hits[0];
            Button_Test.Stat_Hits[1] = Hit_Results[2] > 0 ? Hit_Results[2] : Button_Test.Stat_Hits[1];
            Button_Test.Stat_Hits[2] = Hit_Results[3] > 0 ? Hit_Results[3] : Button_Test.Stat_Hits[2];
            if (Hit_Results[0] == Total_Num_Balls)
                Button_Test.if_Finished = true;
            My_Text_SCORE = string.Format("<color=#151316>Score:{0}</color>", Now_Score);
            My_Text_Combos = string.Format("<color=#ffea5e>Combos x{0}</color>", Combos_[0]);
            TS.MoblieInput();

            // Stage Controler
            if (NowOnStage != Old_Stage)
            {
                NowOnStage = UpdateStage(NowOnStage, true);
                Old_Stage = NowOnStage;
            }
            else
                NowOnStage = UpdateStage(NowOnStage, false);
            if (NowOnStage == 2)
            {
                NowOnStage = 0;
                SceneManager.LoadScene(0);  // Go To another one
            }

            // Deliver Static Values
            if (Now_Score > Game_Score)     Game_Score = Now_Score;
            if (Combos_[0] > Game_Combos)   Game_Combos = Combos_[0];
        }

        // Text to display
        private string My_Text_Name;
        private string My_Text_SCORE;
        private string My_Text_Combos;

        private GUIStyle Font_S1 = new GUIStyle();
        private GUIStyle Font_S2 = new GUIStyle();

        private Font myFont;


        void OnGUI()
        {
            
            int[] Size_Info = new int[2];
            LoopUpdate(Size_Info);

            // Rendering Work
            Interface_Graph[] paintData = new Interface_Graph[Size_Info[0]];
            Interface_Text[] TextData   = new Interface_Text[Size_Info[1]];
            UpdateTime(Time.time);  // Unity -> C++ plugin
            DllAnime(paintData, TextData);    // C++ plugin -> Unity

            // Foreach : Paint Textures
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            foreach (Interface_Graph D in paintData)
                PaintFromInterface(D, Dynamic_Ptr[D.Src_ID]);

            GL.PopMatrix();

            // Paint Text
            DrawText(SongName, 560, 12, 800, 48, Font_S1, TextAnchor.MiddleCenter);
            DrawText(My_Text_SCORE, 1496, 12, 384, 48, Font_S1, TextAnchor.MiddleRight);
            if (Combos_[0] > 4)
                DrawText(My_Text_Combos, 560, 860, 800, 52, Font_S2, TextAnchor.MiddleCenter);
                
        }

    }
}
