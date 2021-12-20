using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public const string StepMetaFile = "SavingData.txt"; // okay

    private static List<GameManager.Step> steps;
    private static uint lastUsedIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    static public void GameStart()
    {
        LoadMetaData();
        Debug.Log("Data Manager Initialized");
    }

    static public void SaveMetaData()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + StepMetaFile);

        sw.WriteLine("1," + lastUsedIndex);

        sw.Close();
    }

    static public void LoadMetaData()
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + StepMetaFile;
        
        if (File.Exists(path))
        {
            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');
                

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == 1)
                    lastUsedIndex = uint.Parse(csv[1]);
            }
            sr.Close();
        }
    }

    static public void SaveSignleGame(List<GameManager.Step> steps)
    {
        lastUsedIndex++;
        SaveMetaData();
        
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + lastUsedIndex + ".txt");

        LinkedList<string> data = SerializationManager.SerializedSteps(steps);

        foreach (string line in data)
        {
            sw.WriteLine(line);
        }
        
        sw.Close();
        lastUsedIndex++;
    }

    static public void LoadSingleGame(int index)
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + index + ".txt";

        if (File.Exists(path))
        {
            steps.Clear();

            string line = "";
            StreamReader sr = new StreamReader(path);
            LinkedList<string> data = new LinkedList<string>();
            
            while ((line = sr.ReadLine()) != null) 
                data.AddLast(line);
            
            sr.Close();
            
            steps = SerializationManager.DeserializeSteps(data);
        }
    }
}

static public class SerializationManager
{
    static public LinkedList<string> SerializedSteps(List<GameManager.Step> steps)
    {
        LinkedList<string> data = new LinkedList<string>();
        foreach (GameManager.Step step in steps)
        {
            data.AddLast(step.Order + "," + ((int)step.State).ToString() + "," + step.Location);
        }

        return data;
    }

    static public List<GameManager.Step> DeserializeSteps(LinkedList<string> data)
    {
        List<GameManager.Step> steps = new List<GameManager.Step>();
        foreach (string line in data)
        {
            string[] csv = line.Split(',');

            steps.Add(new GameManager.Step(int.Parse(csv[0]),int.Parse(csv[1]),(BoxState)int.Parse(csv[2])));
            Debug.Log(int.Parse(csv[0]) + " " + int.Parse(csv[1]) + " " + (BoxState)int.Parse(csv[2]));
        }

        return steps;
    }
}
