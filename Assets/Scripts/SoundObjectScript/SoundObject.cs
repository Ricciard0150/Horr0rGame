using System.Collections;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
   /* [SerializeField][Range(0.5f, 5)] float _waitTime;
    bool _isActive = true;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(_waitTime);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }*/
    /*void Update()
    {
        if(_isActive)
        {
            StartCoroutine(WaitForSeconds());
        }
    }

    IEnumerator WaitForSeconds()
    {
        _isActive = false;
        yield return new WaitForSeconds(_waitTime);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(_waitTime);
        gameObject.SetActive(true);
        _isActive = true;
        print("SoundObject: " + gameObject.name + " is now active again after " + _waitTime + " seconds.");
        yield return new WaitForSeconds(_waitTime);
    }*/
}
