using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution {

  public class Monster : MonoBehaviour {

    public GameObject ballPrefab;
    public GameObject target;
    public GameObject explosionPrefab;

    public int HP;
    public int splitLevel = 2;

    public AudioClip throwAudio;
    public AudioClip explorationAudio;
    public AudioClip hitAudio;
    public AudioClip winAudio;

    private Transform ballStart;
    private static int totalMonster = 1;
    private GameObject child1;
    private GameObject child2;


    // Use this for initialization
    void Start () {
      StartCoroutine (Throw());
      ballStart = transform.Find ("BallStart");

      if (splitLevel > 0) CreateSplitChildren ();
    }

    // Update is called once per frame
    void Update () {
      this.transform.LookAt (target.transform.position);
    }

    IEnumerator Throw() {
      while (true) {
        yield return new WaitForSeconds (1);
        ThrowAt (new Vector3(-1, 0, 0) + target.transform.position);
        yield return new WaitForSeconds (1);
        ThrowAt (Vector3.zero + target.transform.position);
        yield return new WaitForSeconds (1);
        ThrowAt (new Vector3(1, 0, 0) + target.transform.position);
        yield return new WaitForSeconds (3);
        ThrowAt (new Vector3(-1, 0, 0) + target.transform.position);
        yield return new WaitForSeconds (0.2f);
        ThrowAt (Vector3.zero + target.transform.position);
        yield return new WaitForSeconds (0.2f);
        ThrowAt (new Vector3(1, 0, 0) + target.transform.position);

        yield return new WaitForSeconds (2);
      }
    }

    void ThrowAt(Vector3 target) {
      var dir = (target - ballStart.position).normalized;
      var newBall = Object.Instantiate<GameObject> (ballPrefab, ballStart.position, Quaternion.identity);
      var ballRigidbody = newBall.GetComponent<Rigidbody>();
      ballRigidbody.velocity = dir * 5; // 5 seems to be a magic number that as a human I can dodge the ball.

      AudioSource.PlayClipAtPoint (throwAudio, transform.position);
      Destroy (newBall, 10);
    }

    void OnCollisionEnter(Collision col){
      if (col.gameObject.name.Contains ("Ball")) {
        OnHit ();
        Destroy (col.gameObject);
      }
    }

    void OnHit() {
      if (HP > 0) {
        HP -= 1;
        AudioSource.PlayClipAtPoint (hitAudio, transform.position);
        if (HP <= 0) MonsterLose ();
      }
    }

    void MonsterLose() {
      var explosionObj = Instantiate (explosionPrefab, transform.position, transform.rotation);
      Destroy (explosionObj, 1.0f); // Destroy the explosion object after 1 seconds
      AudioSource.PlayClipAtPoint (explorationAudio,  target.transform.position); // play at player location, to make it sounds louder.
      Destroy (gameObject);

      if (splitLevel > 0) Split();
      totalMonster -= 1;

      if (totalMonster == 0) {
        AndroidHelper.ShowAndroidToastMessage ("Congratulations, you win!");
        AudioSource.PlayClipAtPoint (winAudio, target.transform.position);
      }
    }

    void CreateSplitChildren() {
      child1 = Instantiate (gameObject, transform.position + new Vector3 (Random.Range(-1,1f), Random.Range(-1,1f), 0), Quaternion.AngleAxis(180, new Vector3(0, 1, 0)));
      child1.GetComponent<Monster> ().splitLevel = splitLevel - 1;
      child1.transform.localScale = transform.localScale / 2.0f;

      child2 = Instantiate (gameObject, transform.position + new Vector3 (Random.Range(-1,1f), Random.Range(-1,1f), 0), Quaternion.AngleAxis(180, new Vector3(0, 1, 0)));
      child2.GetComponent<Monster> ().splitLevel = splitLevel - 1;
      child2.transform.localScale = transform.localScale / 2.0f;

      child1.SetActive (false);
      child2.SetActive (false);
    }

    void Split() {
      child1.SetActive (true);
      child2.SetActive (true);

      totalMonster += 2;
    }

  }

}