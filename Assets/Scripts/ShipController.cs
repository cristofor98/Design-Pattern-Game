/*
 * Copyright (c) 2015 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

	public float moveSpeed = 10.0f;						        
	public float scatterShotTurretReloadTime = 2.0f;	
	public GameObject startWeapon;						        
	public List<GameObject> tripleShotTurrets;			 
	public List<GameObject> wideShotTurrets;			  
	public List<GameObject> scatterShotTurrets;			 
	public List<GameObject> activePlayerTurrets;		
	public GameObject explosion;						         
	public GameObject playerBullet;						       
  public ParticleSystem playerThrust;            
	private Rigidbody2D playerRigidbody;				     
	private Renderer playerRenderer;					       
	private CircleCollider2D playerCollider;			    

  public int upgradeState = 0;						         

  private GameObject leftBoundary;                   
  private GameObject rightBoundary;                  
  private GameObject topBoundary;                    
  private GameObject bottomBoundary;                 

	private AudioSource shootSoundFX;					        


	void Start () {
    leftBoundary = GameController.SharedInstance.leftBoundary;
    rightBoundary = GameController.SharedInstance.rightBoundary;
    topBoundary = GameController.SharedInstance.topBoundary;
    bottomBoundary = GameController.SharedInstance.bottomBoundary;

		playerCollider = gameObject.GetComponent<CircleCollider2D>();
		playerRenderer = gameObject.GetComponent<Renderer>();
		activePlayerTurrets = new List<GameObject>();
		activePlayerTurrets.Add(startWeapon);
		shootSoundFX = gameObject.GetComponent<AudioSource>();
		shootSoundFX.volume = GlobalVariables.SfxVolume;
		playerRigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (Input.GetKeyDown("space")) {
      Shoot();
		} else if(Input.GetKeyDown("escape")){
			SceneManager.LoadScene("Menu");
		}
		float xDir = Input.GetAxis("Horizontal");
		float yDir = Input.GetAxis("Vertical");
		playerRigidbody.velocity = new Vector2(xDir * moveSpeed, yDir * moveSpeed);
		playerRigidbody.position = new Vector2
			(
				Mathf.Clamp (playerRigidbody.position.x, leftBoundary.transform.position.x, rightBoundary.transform.position.x),
				Mathf.Clamp (playerRigidbody.position.y, bottomBoundary.transform.position.y, topBoundary.transform.position.y)
			);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Powerup") {
			CollectPowerup powerupScript = other.gameObject.GetComponent<CollectPowerup>();
			powerupScript.PowerupCollected();
			UpgradeWeapons();
		} 
    else if (other.gameObject.tag == "Enemy Ship 1" || other.gameObject.tag == "Enemy Ship 2" || other.gameObject.tag == "Enemy Laser") {
      GameController.SharedInstance.ShowGameOver();  
			playerRenderer.enabled = false;       
			playerCollider.enabled = false;      
      playerThrust.Stop();
			Instantiate(explosion, transform.position, transform.rotation);   
			for (int i = 0 ; i < 8; i++) {
				Vector3 randomOffset = new Vector3 (transform.position.x + Random.Range(-0.6f, 0.6f), transform.position.y + Random.Range(-0.6f, 0.6f), 0.0f); 
				Instantiate(explosion, randomOffset, transform.rotation);
			}
			Destroy(gameObject, 1.0f);
		}
	}

  void Shoot() {
    foreach(GameObject turret in activePlayerTurrets) {
      GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Player Bullet"); 
      if (bullet != null) {
        bullet.transform.position = turret.transform.position;
        bullet.transform.rotation = turret.transform.rotation;
        bullet.SetActive(true);
      }
    }
    shootSoundFX.Play();
  }

	void UpgradeWeapons() {     
    
		if (upgradeState == 0) {
			foreach(GameObject turret in tripleShotTurrets) {
				activePlayerTurrets.Add(turret);
			}
		} 
    else if (upgradeState == 1) {
			foreach(GameObject turret in wideShotTurrets) {
				activePlayerTurrets.Add(turret);
			}
		} 
    else if (upgradeState == 2) {
			StartCoroutine("ActivateScatterShotTurret");
    } 
    else {
      return;
    }
		upgradeState ++;
	}

    IEnumerator ActivateScatterShotTurret() {


		while (true) {
			foreach(GameObject turret in scatterShotTurrets) {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Player Bullet"); 
        if (bullet != null) {
          bullet.transform.position = turret.transform.position;
          bullet.transform.rotation = turret.transform.rotation;
          bullet.SetActive(true);
        }
			}
			shootSoundFX.Play();
			yield return new WaitForSeconds(scatterShotTurretReloadTime);
		}
	}
}
