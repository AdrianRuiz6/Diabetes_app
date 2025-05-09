using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayAnimation(GameObject prefab, Vector3 position, Vector3 scale, GameObject parent)
    {
        GameObject animatedObject = Instantiate(prefab, position, Quaternion.identity);
        animatedObject.transform.SetParent(parent.transform, false);
        animatedObject.transform.localScale = scale;
        StartCoroutine(DestroyAnimatedObject(animatedObject));
    }

    private IEnumerator DestroyAnimatedObject(GameObject animatedObject)
    {
        yield return new WaitForSeconds(3f);
        Destroy(animatedObject);
    }
}
