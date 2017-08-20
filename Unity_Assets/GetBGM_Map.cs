using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;

public class GetBGM_Map : MonoBehaviour
{
    public static int Total_Balls;

    [DllImport("Game", EntryPoint = "GetBeatIn")]
    extern static void GetBeatIn(int msecx10, int _x, int _type, int Start, int End);

    [DllImport("Game", EntryPoint = "ResetMapList")]
    extern static void ResetMapList();

    private TextAsset DataText;
    public void ReadMap(string FileName)
    {
        ResetMapList();
        Total_Balls = -1;   // Skip the redundant head
        // File Path
        DataText = Resources.Load(FileName) as TextAsset;
        string RawContent = DataText.text;
        Regex ReturnLine = new Regex("\n");

        if (RawContent != null)
        {
            Regex Delimiter = new Regex(",");

            string[] MyContent = ReturnLine.Split(RawContent);
            // ------------------------------------------------- //
            int Start = 0;
            int End = 0;
            string Song_Name;

            int Ind_Line = 0;

            string Linehere = MyContent[Ind_Line];

            while (!Linehere.Contains("#Header_Info"))
            {
                Ind_Line++;
                Linehere = MyContent[Ind_Line];
            }


            // Read the header
            {
                {
                    Ind_Line++;
                    Linehere = MyContent[Ind_Line];
                }
                string[] X = Delimiter.Split(Linehere);
                Song_Name = X[0];
                Start = Convert.ToInt32(X[1]);
                End = Convert.ToInt32(X[2]);
            }

            do
            {
                Ind_Line++;
                Linehere = MyContent[Ind_Line];
            }
            while (!Linehere.Contains("#Start"));

            {
                Ind_Line++;
                Linehere = MyContent[Ind_Line];
            }
            // ------------------------------------------------- //
            // ------------------------------------------------- //
            while (!Linehere.Contains("#End"))
            {
                {
                    Ind_Line++;
                    Linehere = MyContent[Ind_Line];
                }
                if (Linehere.Contains("#End")) break;
                string[] X = Delimiter.Split(Linehere);
                int[] datas = new int[3];
                int Ind = 0;
                foreach (string s in X)
                {
                    datas[Ind] = Convert.ToInt32(s);
                    Ind++;
                }

                // Time | PosX | Type //
                GetBeatIn(datas[0], datas[1], datas[2], Start, End);
                Total_Balls++;
            }
            // ------------------------------------------------- //
        }
        else
        {
            Debug.Log(FileName + ".imap was not found");
        }
    }
}

/*
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;

public class GetBGM_Map : MonoBehaviour
{
    [DllImport("Game", EntryPoint = "GetBeatIn")]
    extern static void GetBeatIn(int msecx10, float _x, int _type, int Start, int End);


    string RawContent;
    IEnumerator GetContent(string FileName)
    {
        string path = path = Application.streamingAssetsPath + '/' + FileName + ".imap";
        
        //StreamReader DataReader = new StreamReader(path);
        WWW DataReader = new WWW(path);

        yield return DataReader;
        
        RawContent = DataReader.text;
        Debug.Log(RawContent);

        /*if (DataReader.error != null)
        {
            yield return null;
        }
    }

    public void ReadMap(string FileName)
    {
        // File Path
        Regex ReturnLine = new Regex("\n");

        if (GetContent(FileName) != null)
        {
            Regex Delimiter = new Regex(",");

            string[] MyContent = ReturnLine.Split(RawContent);
            // ------------------------------------------------- //
            int Start = 0;
            int End = 0;
            string Song_Name;

            int Ind_Line = 0;

            string Linehere = MyContent[Ind_Line];

            while (!Linehere.Contains("#Header_Info"))
            {
                Ind_Line++;
                Linehere = MyContent[Ind_Line];
            }
                

            // Read the header
            {
                {
                    Ind_Line++;
                    Linehere = MyContent[Ind_Line];
                }
                string[] X = Delimiter.Split(Linehere);
                Song_Name = X[0];
                Start = Convert.ToInt32(X[1]);
                End   = Convert.ToInt32(X[2]);
                Debug.Log(Start);
                Debug.Log(End);
            }

            do
            {
                Ind_Line++;
                Linehere = MyContent[Ind_Line];
            }
            while (!Linehere.Contains("#Start"));
            // ------------------------------------------------- //
            // ------------------------------------------------- //
            while (!Linehere.Contains("#End"))
            {
                {
                    Ind_Line++;
                    Linehere = MyContent[Ind_Line];
                }
                if (Linehere.Contains("#End")) break;

                string[] X = Delimiter.Split(Linehere);
                int[] datas = new int[3];
                int Ind = 0;
                foreach(string s in X)
                {
                    datas[Ind] = Convert.ToInt32(s);
                    Ind++;
                }
                
                // Time | PosX | Type //
                //GetBeatIn(datas[0], datas[1], datas[2], Start, End);
            }
            // ------------------------------------------------- //
        }
        else
        {
            Debug.Log(FileName + ".imap was not found");
        }
    }
}
*/