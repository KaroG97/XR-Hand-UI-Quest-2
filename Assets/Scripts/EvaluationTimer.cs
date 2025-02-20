﻿using System;
using System.IO;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class EvaluationTimer : MonoBehaviour
{
    public int userId = 0;
    public int iteration = 0;
    public int errorCount = 0;

    public FeaturesPanelKeyboard featurePanel;
    public Randomizer randomizer;
    public ArmUiHandler armUiHandler;
    public DateTime startTime;
    public DateTime endTime;
    public DateTime firstScrollTime;
    public DateTime targetReachedTime;    
    public TimeSpan duration;
    public string data;

    public bool isTimer = false;
    public bool isRec = false;
    public bool isAudio = false;
    public bool isHaptic = false;

    private string filename = "";
    private string rootPath = "";
    private string path = "";
    public string OutputFileName { get; } = "XR_HandUI_Taskperformance";


    private void Start()
    {
        startTime = DateTime.Now;
        iteration = 0;
        WriteData();
    }

    private void Update()
    {
        if (featurePanel.toggleHome.IsToggled)
        {
            userId = featurePanel.userId;
            isRec = featurePanel.isRec;
            isAudio = featurePanel.isAudio;
            isHaptic = featurePanel.isHaptic;
        }        


    }

    public void StartTimer()
    {
        if (!isTimer)
        {
            firstScrollTime = DateTime.Now;
            isTimer = true;
        }
        
             
    }

    public void StopTimer()
    {
        isTimer = false;
        targetReachedTime = DateTime.Now;
        duration = targetReachedTime - firstScrollTime;
        Debug.Log("isArmScrolling" + armUiHandler.isArmScrolling);
        WriteData();
        iteration++;

        //Debug.Log($"{filename}: {data}");
    }



    void OnApplicationQuit()
    {
        endTime = DateTime.Now;
        WriteData();
    }

    //private static string GetAndroidExternalFilesDir()
    //{
    //    using (AndroidJavaClass unityPlayer =
    //           new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //    {
    //        using (AndroidJavaObject context =
    //               unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
    //        {
    //            // Get all available external file directories (emulated and sdCards)
    //            AndroidJavaObject[] externalFilesDirectories =
    //                                context.Call<AndroidJavaObject[]>
    //                                ("getExternalFilesDirs", (object)null);
    //            AndroidJavaObject emulated = null;
    //            AndroidJavaObject sdCard = null;
    //            for (int i = 0; i < externalFilesDirectories.Length; i++)
    //            {
    //                AndroidJavaObject directory = externalFilesDirectories[i];
    //                using (AndroidJavaClass environment =
    //                       new AndroidJavaClass("android.os.Environment"))
    //                {
    //                    // Check which one is the emulated and which the sdCard.
    //                    bool isRemovable = environment.CallStatic<bool>
    //                                      ("isExternalStorageRemovable", directory);
    //                    bool isEmulated = environment.CallStatic<bool>
    //                                      ("isExternalStorageEmulated", directory);
    //                    if (isEmulated)
    //                        emulated = directory;
    //                    else if (isRemovable && isEmulated == false)
    //                        sdCard = directory;
    //                }
    //            }
    //            // Return the sdCard if available
    //            if (sdCard != null)
    //            {
    //                Debug.Log("sdCardHaptic:" + sdCard);
    //                return sdCard.Call<string>("getAbsolutePath");
    //            }
    //            else
    //            {
    //                Debug.Log("sdCardHaptic:" + emulated); 
    //                return emulated.Call<string>("getAbsolutePath");                    
    //            }

    //        }
    //    }
    //}


    public void WriteData()
    {

        //GetAndroidExternalFilesDir();

        data = String.Format("{0};{1};{2};{3};{4}:{5}:{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};", userId, iteration, firstScrollTime, targetReachedTime, duration.Hours.ToString(), duration.Minutes.ToString(), duration.Seconds.ToString(), isRec.ToString(), isAudio.ToString(), isHaptic.ToString(), errorCount, startTime, endTime, randomizer.currentTargetIndex, armUiHandler.isArmScrolling.ToString(), armUiHandler.draggingCount, armUiHandler.rightIsDominant.ToString());

        try
        {
            rootPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));

        }
        catch (Exception)
        {
            rootPath = Application.persistentDataPath;
        }




        filename = String.Format("{0}-{1}.csv", OutputFileName, userId);
        path = Path.Combine(Application.persistentDataPath, filename);


        //Debug.Log("path: " + path);

        //check if directory doesn't exit
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "")))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, ""));
        }
        else
        {
            TextWriter writer = new StreamWriter(path, true);
            writer.WriteLine(data);
            writer.Close();
        }
    }
}
