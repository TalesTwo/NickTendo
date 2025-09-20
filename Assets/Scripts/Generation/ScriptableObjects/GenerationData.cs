using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RoomEntry
{
    public Types.RoomType roomType;
    public List<Room> roomPrefabs; // multiple prefabs for each type
}

[CreateAssetMenu(fileName = "RoomGenerationData", menuName = "ScriptableObjects/GenerationData", order = 1)]
public class GenerationData : ScriptableObject
{
    [SerializeField]
    private List<RoomEntry> roomEntries = new List<RoomEntry>();

    private Dictionary<Types.RoomType, List<Room>> _roomDict;

    public Dictionary<Types.RoomType, List<Room>> RoomDict
    {
        get
        {
            if (_roomDict == null)
            {
                _roomDict = new Dictionary<Types.RoomType, List<Room>>();
                foreach (var entry in roomEntries)
                {
                    if (!_roomDict.ContainsKey(entry.roomType))
                        _roomDict[entry.roomType] = new List<Room>();

                    // add all prefabs for this type
                    foreach (var room in entry.roomPrefabs)
                    {
                        if (room != null)
                            _roomDict[entry.roomType].Add(room);
                    }
                }
            }
            return _roomDict;
        }
    }
}