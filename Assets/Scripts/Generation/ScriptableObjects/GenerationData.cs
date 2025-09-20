using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RoomEntry
{
    public Types.RoomType roomType;
    public List<SceneField> roomScenes; // multiple prefabs for each type
}

[CreateAssetMenu(fileName = "RoomGenerationData", menuName = "ScriptableObjects/GenerationData", order = 1)]
public class GenerationData : ScriptableObject
{
    [SerializeField]
    private List<RoomEntry> roomEntries = new List<RoomEntry>();

    // Dictionary now maps RoomType -> List of scenes instead of prefabs
    private Dictionary<Types.RoomType, List<SceneField>> _roomDict;

    public Dictionary<Types.RoomType, List<SceneField>> RoomDict
    {
        get
        {
            if (_roomDict == null)
            {
                _roomDict = new Dictionary<Types.RoomType, List<SceneField>>();
                foreach (var entry in roomEntries)
                {
                    if (!_roomDict.ContainsKey(entry.roomType))
                        _roomDict[entry.roomType] = new List<SceneField>();

                    foreach (var scene in entry.roomScenes)
                    {
                        if (scene != null)
                            _roomDict[entry.roomType].Add(scene);
                    }
                }
            }
            return _roomDict;
        }
    }
}
