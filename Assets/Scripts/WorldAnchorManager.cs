using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA.Sharing;

public class WorldAnchorManager : MonoBehaviour
{
    private static WorldAnchorManager instance;
    public static WorldAnchorManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<WorldAnchorManager>();
            return instance;
        }
    }

    WorldAnchorStore worldAnchorStore;

    public List<GameObject> objsToAnchor;
    Dictionary<GameObject, WorldAnchor> savedWorldAnchors;

    //Anchor Sharing
    byte[] importedAnchorStoreData;
    List<byte> exportedAnchorStoreData;

    //const string exportFilePath = "C:\\Firefight-WorldAnchors";
    string exportFilePath;

    public NavMeshAgent firefighterNavAgent;

    // Start is called before the first frame update
    void Start()
    {
        firefighterNavAgent.enabled = false;
        savedWorldAnchors = new Dictionary<GameObject, WorldAnchor>();
        HololensConfigController.Instance.logMessage("Attempting to load anchor store...");
        WorldAnchorStore.GetAsync(AnchorStoreLoaded);

        //exportedAnchorStoreData = new List<byte>();
        //exportFilePath = Path.Combine(Application.persistentDataPath, "Firefight-WorldAnchors.dat");


        /*if (File.Exists(exportFilePath))
        {
            Debug.Log("Importing exported data");
            HololensConfigController.Instance.logMessage("Importing exported data from: " + exportFilePath);
            importedAnchorStoreData = File.ReadAllBytes(exportFilePath);
            ImportWorldAnchor(importedAnchorStoreData); //Attempt to import WorldAnchorStore from exported bytes
        }
        else
        {
            Debug.Log("Exported data not found");
            HololensConfigController.Instance.logMessage("Exported data not found at path: " + exportFilePath);
        }*/
    }

    #region Application Reliant Anchors

    private void AnchorStoreLoaded(WorldAnchorStore store)
    {
        if (store != null)
        {
            worldAnchorStore = store;
            HololensConfigController.Instance.logMessage("AnchorStore count: " + worldAnchorStore.anchorCount);

            loadAnchors();
        }
        else
        {
            HololensConfigController.Instance.logMessage("Anchor store not found or empty");
        }

        /*else
        {
            if (File.Exists(exportFilePath))
            {
                Debug.Log("Application anchor found, importing external data");
                HololensConfigController.Instance.logMessage("Application anchor found, importing external data");
            }
            else
            {
                Debug.Log("Application anchor store is null, external data not found");
                HololensConfigController.Instance.logMessage("Application anchor store is null, external data not found");
            }
        }*/
    }

    private void loadAnchors()
    {
        foreach (string id in worldAnchorStore.GetAllIds())
        {
            GameObject obj = GameObject.Find(id);
            WorldAnchor savedAnchor = worldAnchorStore.Load(id, obj);

            HololensConfigController.Instance.logMessage(obj.name + " world anchor loaded");
            savedWorldAnchors.Add(obj, savedAnchor);

            /*if (!savedAnchor) //Obj doesn't have an anchor
            {
                Debug.Log("<color=red>" + obj.name + " doesn't have a world anchor saved</color>");
                HololensConfigController.Instance.logMessage(obj.name + " doesn't have a world anchor saved");
            }
            else //obj has a stored anchor
            {
                Debug.Log("<color=green>" + obj.name + " has a world anchor saved</color>");
                HololensConfigController.Instance.logMessage(obj.name + " world anchor loaded");
                savedWorldAnchors.Add(obj, savedAnchor);
            }*/
        }

        firefighterNavAgent.enabled = true;
    }

    public void saveAnchors()
    {        
        foreach (GameObject obj in objsToAnchor)
        {
            saveAnchor(obj);
        }
    }

    public void saveAnchor(GameObject obj)
    {
        WorldAnchor anchor = obj.GetComponent<WorldAnchor>();
        if (!anchor) //Does not already have a WorldAnchor
        {
            anchor = obj.AddComponent<WorldAnchor>();
        }

        if (worldAnchorStore.Save(obj.name, anchor)) //Anchor saved successfully
        {
            if (!savedWorldAnchors.ContainsKey(obj))
            {
                savedWorldAnchors.Add(obj, anchor);
            }
            
            HololensConfigController.Instance.logMessage(obj.name + " world anchor saved");
        }
        else //Anchor failed to save to the store
        {
            HololensConfigController.Instance.logMessage(obj.name + " world anchor NOT saved");
        }

        /*HololensConfigController.Instance.logMessage("Starting world anchor export 1...");
        ExportWorldAnchor(obj.name, anchor);*/
    }

    public void deleteAnchor(GameObject obj)
    {
        WorldAnchor anchorToRemove = obj.GetComponent<WorldAnchor>();
        if (anchorToRemove)
        {
            if (worldAnchorStore.Delete(obj.name))
            {
                HololensConfigController.Instance.logMessage(obj.name + " world anchor deleted");
                // remove any world anchor component from the game object so that it can be moved
                DestroyImmediate(anchorToRemove);
            }
            else
            {
                HololensConfigController.Instance.logMessage(obj.name + " world anchor could not be deleted");
            }
        }
    }

    public void clearAnchors()
    {
        foreach (KeyValuePair<GameObject, WorldAnchor> keyValue in savedWorldAnchors)
        {
            DestroyImmediate(keyValue.Value);
        }
        savedWorldAnchors.Clear();
        worldAnchorStore.Clear();
        //if (File.Exists(exportFilePath))
        //{
        //    File.Delete(exportFilePath);
        //}
    }

    #endregion Application Reliant Anchors

    #region Anchor Sharing

    private void ExportWorldAnchor(string id, WorldAnchor anchor)
    {
        HololensConfigController.Instance.logMessage("Starting world anchor export 2...");
        exportedAnchorStoreData.Clear();
        WorldAnchorTransferBatch transferBatch = new WorldAnchorTransferBatch();
        transferBatch.AddWorldAnchor(id, anchor);
        WorldAnchorTransferBatch.ExportAsync(transferBatch, OnExportDataAvailable, OnExportComplete);
    }

    private void OnExportComplete(SerializationCompletionReason completionReason)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            // If we have been transferring data and it failed, 
            // tell the client to discard the data
            //SendExportFailedToClient();
            Debug.Log("Export failed");
            HololensConfigController.Instance.logMessage("Export failed with reason: " + completionReason);
        }
        else
        {
            // Tell the client that serialization has succeeded.
            // The client can start importing once all the data is received.
            //SendExportSucceededToClient();

            File.WriteAllBytes(exportFilePath, exportedAnchorStoreData.ToArray());
            HololensConfigController.Instance.logMessage("Export complete, serialization completed, anchor data written");
        }
    }

    int dataCounter = 1;
    private void OnExportDataAvailable(byte[] data)
    {
        // Send the bytes to the client.  Data may also be buffered.
        //TransferDataToClient(data);

        exportedAnchorStoreData.AddRange(data);
        File.WriteAllBytes(exportFilePath, exportedAnchorStoreData.ToArray());
        
        HololensConfigController.Instance.logMessage("Anchor store data recieved " + dataCounter++);
    }

    private int retryCount = 10;
    private void ImportWorldAnchor(byte[] importedData)
    {
        HololensConfigController.Instance.logMessage("Import started");
        WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
    }
    
    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            Debug.Log("Failed to import: " + completionReason.ToString());
            HololensConfigController.Instance.logMessage("Failed to import: " + completionReason.ToString());
            if (retryCount > 0)
            {
                retryCount--;
                WorldAnchorTransferBatch.ImportAsync(importedAnchorStoreData, OnImportComplete);
            }
            return;
        }

        string[] ids = deserializedTransferBatch.GetAllIds();
        if (ids != null)
        {
            HololensConfigController.Instance.logMessage("Imported: " + ids.Length + " WorldAnchors, now applying...");
            foreach (string id in ids)
            {
                GameObject obj = GameObject.Find(id);
                if (obj != null)
                {
                    deserializedTransferBatch.LockObject(id, obj);
                    HololensConfigController.Instance.logMessage("Imported world anchor for: " + id);
                    //transferBatch.LockObject(id, gameObject);
                }
                else
                {
                    Debug.Log("Failed to find object for anchor id: " + id);
                    HololensConfigController.Instance.logMessage("Failed to find object for anchor id: " + id);
                }
            }
        }
        else
        {
            HololensConfigController.Instance.logMessage("Imported 0 world anchors");
        }
    }

#endregion Anchor Sharing
}
