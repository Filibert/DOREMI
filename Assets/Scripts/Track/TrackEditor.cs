using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrackEditor : EditorWindow {

    public TrackScriptable trackScriptable;
    int viewIndex = 1;

    [MenuItem("Window/Track Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(TrackEditor));
    }

    void OnEnable()
    {
        if(EditorPrefs.HasKey("ObjectPath")) 
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            trackScriptable = AssetDatabase.LoadAssetAtPath (objectPath, typeof(TrackScriptable)) as TrackScriptable;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Track Editor", EditorStyles.boldLabel);

        if (trackScriptable != null)
        {
            if (GUILayout.Button("Show chorus list"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = trackScriptable;
            }
        }

        if (GUILayout.Button("Open Track"))
        {
            OpenTrack();
        }
        if (GUILayout.Button("New Track"))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = trackScriptable;
        }
        GUILayout.EndHorizontal();

        if (trackScriptable == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Track", GUILayout.ExpandWidth(false)))
            {
                CreateNewTrack();
            }
            if (GUILayout.Button("Open Existing Track", GUILayout.ExpandWidth(false)))
            {
                OpenTrack();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (trackScriptable != null)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex > 1)
                    viewIndex--;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex < trackScriptable.chorusList.Count)
                {
                    viewIndex++;
                }
            }

            GUILayout.Space(60);

            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
            {
                DeleteItem(viewIndex - 1);
            }

            GUILayout.EndHorizontal();
            if (trackScriptable.chorusList == null)
                Debug.Log("wtf");
            if (trackScriptable.chorusList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1, trackScriptable.chorusList.Count);
                //Mathf.Clamp (viewIndex, 1, trackScriptable.chorusList.Count);
                EditorGUILayout.LabelField("of   " + trackScriptable.chorusList.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                trackScriptable.chorusList[viewIndex - 1].instrumentName = EditorGUILayout.TextField("Instrument Name", trackScriptable.chorusList[viewIndex - 1].instrumentName as string);
                trackScriptable.chorusList[viewIndex - 1].instrumentImage = EditorGUILayout.ObjectField("Instrument Icon", trackScriptable.chorusList[viewIndex - 1].instrumentImage, typeof(Sprite), false) as Sprite;

                GUILayout.Space(10);

                if (GUILayout.Button("Select sound path"))
                {
                    trackScriptable.chorusList[viewIndex - 1].path = EditorUtility.OpenFilePanel("Sound path", ".", "wav,mp3");
                    if (trackScriptable.chorusList[viewIndex - 1].path.StartsWith(Application.dataPath))
                    {
                        trackScriptable.chorusList[viewIndex - 1].path = "Assets" + trackScriptable.chorusList[viewIndex - 1].path.Substring(Application.dataPath.Length);
                    }
                }
                trackScriptable.chorusList[viewIndex - 1].path = EditorGUILayout.TextField("Sound path", trackScriptable.chorusList[viewIndex - 1].path as string);

                GUILayout.Space(10);

                //trackScriptable.chorusList[viewIndex - 1].audioSource = EditorGUILayout.ObjectField("Audio Source", trackScriptable.chorusList[viewIndex - 1].audioSource, typeof(CustomAudioSource), false) as CustomAudioSource;


            }
            else
            {
                GUILayout.Label("This track is empty.");
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(trackScriptable);
        }
    }

    void CreateNewTrack()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 1;
        trackScriptable = CreateTrack.Create();
        if (trackScriptable)
        {
            trackScriptable.chorusList = new List<ChorusScriptable>();
            string relPath = AssetDatabase.GetAssetPath(trackScriptable);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenTrack()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Track", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            trackScriptable = AssetDatabase.LoadAssetAtPath(relPath, typeof(TrackScriptable)) as TrackScriptable;
            if (trackScriptable.chorusList == null)
                trackScriptable.chorusList = new List<ChorusScriptable>();
            if (trackScriptable)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddItem()
    {
        ChorusScriptable newChorus = new ChorusScriptable();
        newChorus.instrumentName = "New instrument";
        trackScriptable.chorusList.Add(newChorus);
        viewIndex = trackScriptable.chorusList.Count;
    }

    void DeleteItem(int index)
    {
        trackScriptable.chorusList.RemoveAt(index);
    }

}
