using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject _managerContainer; // Manager Sciptlerinin parent objecti
    [SerializeField] private string _sceneToLoad;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();   // Tum sahne install edildikten sonra calis

        DontDestroyOnLoad(_managerContainer); // Manager Sciptlerini tum sahnede calistirir
        SceneManager.LoadScene(_sceneToLoad);   // Açýlacak sahne
    }
}
