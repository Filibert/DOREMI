using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateTrack {
    [MenuItem("Assets/Create/Track")]
    public static TrackScriptable Create()
    {
        TrackScriptable asset = ScriptableObject.CreateInstance<TrackScriptable>();

        AssetDatabase.CreateAsset(asset, "Assets/Tracks/Track.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
