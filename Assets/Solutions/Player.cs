using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Solution {

  public class Player : MonoBehaviour {
    
    public Camera tangoCamera;
    public GameObject ballPrefab;
    public GameObject shieldPrefab;
    public TangoPointCloud tangoPointCloud;

    public int HP;

    public AudioClip throwAudio;
    public AudioClip hitAudio;
    public AudioClip loseAudio;
    public AudioClip deployShieldAudio;

    private float lastThrowTime_;

    // Use this for initialization
    void Start () {
      transform.parent = tangoCamera.transform;
      lastThrowTime_ = 0; // Add this line in existing Start function
    }

    // Update is called once per frame
    void Update () {
      bool shouldAttack = false;

      if (Input.touchCount == 1) {
        Touch t = Input.GetTouch (0);
        if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved) {
          shouldAttack = true;
        }
      }

      if (Input.touchCount == 2 && Input.GetTouch (0).phase == TouchPhase.Ended) {
        var touch1 = Input.GetTouch (0);
        var touch2 = Input.GetTouch (1);
        Vector3 planeCenter1, planeCenter2;
        Plane plane1, plane2;

        if (tangoPointCloud.FindPlane (tangoCamera, touch1.position, out planeCenter1, out plane1)
          && tangoPointCloud.FindPlane (tangoCamera, touch2.position, out planeCenter2, out plane2)) {

          if (Vector3.Dot (plane1.normal, plane2.normal) > 0.9f) { // let's say they are close enough
            DeployShield ((planeCenter1 + planeCenter2) / 2.0f, (plane1.normal + plane2.normal).normalized);
          }
        }
      }


      #if UNITY_EDITOR
      if (Input.GetButton("Jump")) shouldAttack = true;
      #endif

      if (shouldAttack) ThrowBall ();
    }

    void ThrowBall() {
      if (Time.time - lastThrowTime_ < 0.1f) // throw 10 times a second
        return;

      lastThrowTime_ = Time.time;

      // Throw a ball from player to, camera front.
      var playerBall = Instantiate(ballPrefab, transform.position + transform.forward * 0.6f, transform.rotation); // forward 0.6f, so it doesn’t collide with player capsule
      var ballRigidBody = playerBall.GetComponent<Rigidbody> ();
      ballRigidBody.velocity = transform.forward * 5; // Again the magic number, 5 for ball speed

      AudioSource.PlayClipAtPoint (throwAudio, transform.position);
      Destroy (playerBall, 10);
    }

    void DeployShield (Vector3 planeCenter, Vector3 normal) {
      Vector3 forward = (transform.forward - normal * Vector3.Dot (transform.forward, normal)).normalized;
      var shieldObj = Instantiate(shieldPrefab, planeCenter, Quaternion.LookRotation(forward, normal));

      AudioSource.PlayClipAtPoint (deployShieldAudio, planeCenter);
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
        if (HP <= 0) PlayerLose();
      }
    }

    void PlayerLose() {
      AndroidHelper.ShowAndroidToastMessage ("You lose.");
      AudioSource.PlayClipAtPoint (loseAudio, transform.position);
    }


  }

}