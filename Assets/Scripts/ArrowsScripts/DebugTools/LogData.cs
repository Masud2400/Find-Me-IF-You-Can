using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LogData
{
    // 1. Create serializable wrappers because JsonUtility doesn't support Dictionaries
    [System.Serializable]
    public class ArrowEntry
    {
        public string keyObject;
        public List<Vector3> valueObjects = new List<Vector3>();
    }

    [System.Serializable]
    public class BlockEntry
    {
        public Vector3 keyVector;
        public List<int> values = new List<int>();
    }

    [System.Serializable]
    public class SaveDataWrapper
    {
        public List<ArrowEntry> arrows = new List<ArrowEntry>();
        public List<BlockEntry> blocks = new List<BlockEntry>();
    }

    public static void SaveToJson(
		Dictionary<GameObject, List<GameObject>> arrowDict, 
        Dictionary<Vector3, List<int>> firstArrowBlock)
    {
        SaveDataWrapper wrapper = new SaveDataWrapper();

        // 3. Convert GameObject Dict (saving object names instead of the GameObjects themselves)
        foreach (var kvp in arrowDict)
        {
            ArrowEntry entry = new ArrowEntry { keyObject = kvp.Key.name };
            foreach (var go in kvp.Value)
            {
                entry.valueObjects.Add(go.transform.localPosition);
            }
            wrapper.arrows.Add(entry);
        }

        // 4. Convert Vector3 Dict
        foreach (var kvp in firstArrowBlock)
        {
            wrapper.blocks.Add(new BlockEntry { keyVector = kvp.Key, values = kvp.Value });
        }

        // 5. Serialize and write to the specified path
        string json = JsonUtility.ToJson(wrapper, true);
        
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(documentsPath, "dataTwo.json");
        
        File.WriteAllText(filePath, json);
        Debug.Log("Data successfully saved to: " + filePath);
    }
}
