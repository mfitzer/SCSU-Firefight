using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

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

    // Start is called before the first frame update
    void Start()
    {
        objsToAnchor = new List<GameObject>();
        savedWorldAnchors = new Dictionary<GameObject, WorldAnchor>();
        WorldAnchorStore.GetAsync(AnchorStoreLoaded);
    }

    private void AnchorStoreLoaded(WorldAnchorStore store)
    {
        if (store != null)
        {
            worldAnchorStore = store;
            Debug.Log("AnchorStore retrieved");
            Debug.Log("AnchorStore count: " + worldAnchorStore.anchorCount);
            HololensConfigController.Instance.logMessage("AnchorStore count: " + worldAnchorStore.anchorCount);
            
            loadAnchors();
        }
        else
        {
            Debug.Log("Anchor store is null");
        }
    }

    private void loadAnchors()
    {
        foreach (GameObject obj in objsToAnchor)
        {
            WorldAnchor savedAnchor = worldAnchorStore.Load(obj.name, obj);
            if (!savedAnchor) //Obj doesn't anchor
            {
                Debug.Log("<color=red>" + obj.name + " doesn't have a world anchor saved</color>");
                HololensConfigController.Instance.logMessage(obj.name + " doesn't have a world anchor saved");
            }
            else //obj has a stored anchor
            {
                Debug.Log("<color=green>" + obj.name + " has a world anchor saved</color>");
                HololensConfigController.Instance.logMessage(obj.name + " world anchor loaded");
                savedWorldAnchors.Add(obj, savedAnchor);
            }
        }
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

        bool saved = worldAnchorStore.Save(obj.name, anchor);
        if (saved) //Anchor saved successfully
        {
            if (!savedWorldAnchors.ContainsKey(obj))
            {
                savedWorldAnchors.Add(obj, anchor);
            }
            Debug.Log("<color=green>" + obj.name + " world anchor saved</color>");
            HololensConfigController.Instance.logMessage(obj.name + " world anchor saved");
        }
        else //Anchor failed to save to the store
        {
            Debug.Log("<color=red>" + obj.name + " world anchor not saved</color>");
            HololensConfigController.Instance.logMessage(obj.name + " world anchor NOT saved");
        }
    }

    public void deleteAnchor(GameObject obj)
    {
        WorldAnchor anchorToRemove = obj.GetComponent<WorldAnchor>();
        if (anchorToRemove)
        {
            if (worldAnchorStore.Delete(obj.name))
            {
                HololensConfigController.Instance.logMessage(obj.name + " world anchor deleted");
            }
            // remove any world anchor component from the game object so that it can be moved
            DestroyImmediate(anchorToRemove);
        }
    }

    public void clearAnchors()
    {
        worldAnchorStore.Clear();
    }
}
