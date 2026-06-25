using System.Collections;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    [SerializeField][Range(0.5f, 5)] float _waitTime;
    IEnumerator Start()
    {
       yield return new WaitForSeconds(_waitTime);
       gameObject.SetActive(false);
    }    
}
