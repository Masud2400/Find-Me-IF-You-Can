using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LogData
{
    [System.Serializable]
    public class ArrowEntry
    {
        public string keyObject;
        //public List<Vector3> valueObjects = new List<Vector3>();
    }

    [System.Serializable]
    public class BlockEntry
    {
        public Vector3 keyVector;
        public List<int> values = new List<int>();
    }

    [System.Serializable]
    public class ConnectionEntry
    {
        public string groupName;
        public List<string> connections = new List<string>();
    }

    // For Dictionary<int, Dictionary<int, Vector3>>
    [System.Serializable]
    public class LocationEntry
    {
        public int x;
        public int y;
        public Vector3 position;
    }

    [System.Serializable]
    public class OccupiedPositionEntry
    {
        public Vector3 position;
    }

    [System.Serializable]
    public class SaveDataWrapper
    {
        public List<ArrowEntry> arrows = new List<ArrowEntry>();
        //public List<BlockEntry> blocks = new List<BlockEntry>();
        //public List<ConnectionEntry> allConnections = new List<ConnectionEntry>();

        // Used by SaveToJsonTwo
        //public List<LocationEntry> locations = new List<LocationEntry>();
        //public List<OccupiedPositionEntry> occupiedPositions = new List<OccupiedPositionEntry>();
    }

    public static void SaveToJson(
        Dictionary<string, List<BlockData>> arrowDict
        //Dictionary<Vector3, List<int>> firstArrowBlock,
        //Dictionary<string, HashSet<string>> arrowConnections
		)
    {
        SaveDataWrapper wrapper = new SaveDataWrapper();

        foreach (var kvp in arrowDict)
        {
            ArrowEntry entry = new ArrowEntry
            {
                keyObject = kvp.Key
            };

            /*foreach (BlockData go in kvp.Value)
            {
                entry.valueObjects.Add(go.position);
            }*/

            wrapper.arrows.Add(entry);
        }
		
		/*
        foreach (var kvp in firstArrowBlock)
        {
            wrapper.blocks.Add(new BlockEntry
            {
                keyVector = kvp.Key,
                values = kvp.Value
            });
        }

        foreach (var kvp in arrowConnections)
        {
            ConnectionEntry entry = new ConnectionEntry
            {
                groupName = kvp.Key
            };

            foreach (var go in kvp.Value)
            {
                entry.connections.Add(go != null ? go : null);
            }

            wrapper.allConnections.Add(entry);
        }*/

        string json = JsonUtility.ToJson(wrapper, true);

        string documentsPath =
            System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.MyDocuments);

        string filePath = Path.Combine(documentsPath, "dataTwo.json");

        File.WriteAllText(filePath, json);
    }
	
	/*
    public static void SaveToJsonTwo(
        Dictionary<int, Dictionary<int, Vector3>> locations,
        HashSet<Vector3> occupiedPositions)
    {
        SaveDataWrapper wrapper = new SaveDataWrapper();

        // Save locations
        foreach (var outerEntry in locations)
        {
            int x = outerEntry.Key;

            foreach (var innerEntry in outerEntry.Value)
            {
                int y = innerEntry.Key;
                Vector3 position = innerEntry.Value;

                wrapper.locations.Add(new LocationEntry
                {
                    x = x,
                    y = y,
                    position = position
                });
            }
        }

        // Save occupied positions
        foreach (Vector3 position in occupiedPositions)
        {
            wrapper.occupiedPositions.Add(new OccupiedPositionEntry
            {
                position = position
            });
        }

        // Convert everything to JSON
        string json = JsonUtility.ToJson(wrapper, true);

        string documentsPath =
            System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.MyDocuments);

        string filePath = Path.Combine(documentsPath, "dataThree.json");

        File.WriteAllText(filePath, json);
    }*/
}
