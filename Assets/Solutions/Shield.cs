using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution {

  public class Shield : MonoBehaviour {

    public AudioClip[] hitAudioList;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    void OnCollisionEnter(Collision col){
      if (col.gameObject.name.Contains ("Ball")) {
        AudioClip hitAudio = hitAudioList [Random.Range (0, hitAudioList.Length - 1)];
        AudioSource.PlayClipAtPoint (hitAudio, col.transform.position);
        Destroy (col.gameObject);
      }
    }

  }

}