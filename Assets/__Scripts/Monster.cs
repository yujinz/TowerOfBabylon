using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Monster : MonoBehaviour {

    public int lv = 1;
    public int health;
    public float resetTime = 0.2f;
    public float reviveTime = 10f;
    public int damage;
    public float itemProbability = 2.5f;
    public bool destroyOnInvisible = false;

    public bool ________private________;
    public bool destroy;
    public bool recovery = false;
    public bool markedDie = false;
    public float tileSize = 0.5f; // Default tile size.
    public Transform startTrans;
    public Vector3 startPos;

    public void Awake() {
        startTrans = transform;
        startPos = transform.localPosition;
    }

    public virtual void OnTriggerEnter(Collider other) {
        var go = other.gameObject;
        if (go.layer==LayerMask.NameToLayer("SamusBullet")) {
            HurtByBullet();
            if (health<=0) {
                //Instantiate(Effects.S.EnemyBulletExplosion, transform.position, transform.rotation);
                Die();
                return;
            }
            if (!recovery) {
                TurnDamagedColor();
                FreezeMotion();
                Invoke("Reset", resetTime);
                recovery = true;
            }
        } else if (go.layer == LayerMask.NameToLayer("SamusMissle")) {
            HurtByMissle();
            if (health<=0) {
                Die();
                return;
            }
            if (!recovery) {
                TurnDamagedColor();
                FreezeMotion();
                Invoke("Reset", resetTime);
                recovery = true;
            }
        }
    }

    public virtual void OnBecameInvisible() {
        //Sprint(name+" became invisible.");
        if (markedDie) Destroy(gameObject);
    }

    public virtual void OnTriggerExit(Collider collider) {
        GameObject other = collider.gameObject;
        if (transform.parent != null) {
            if (other == transform.parent.gameObject) {
                print(name+" leaves manager.");
                markedDie = true;
            }
        } else
            print("No EnemyManager found.");
    }

    public virtual void Die() {
        print("Enemy " + gameObject.name + " dies.");
            Vanish();
            Invoke("Revive", reviveTime);
        /*
        bool hasItem = HasItem();
        if (hasItem) {
            float factor = Samus.S.misslePack ? -1f : 0;
            if (Random.Range(factor, 1f)>=0)
                Instantiate(Items.S.kit, transform.position, transform.rotation);
            else
                Instantiate(Items.S.missle, transform.position, transform.rotation);
                
    }
        Destroy(gameObject);
        */
    }

    public void Vanish() {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.enabled = false;
        }

        //Instantiate(Effects.S.TileBreak, transform.position, transform.rotation);
        //destroy = false;
    }

    public void Revive() {
        //transform.position = startTrans.position;
        transform.localPosition = startPos;
        transform.eulerAngles = startTrans.eulerAngles;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.enabled = true;
        }
    }
    public bool HasItem() {
        return Random.Range(1-itemProbability, 1f)>=0;
    }

    public void Reset() {
        //GetComponent<SpriteRenderer>().material = Mats.S.Mat_Enemy;
        ContinueMotion();
        recovery = false;
    }

    public void TurnDamagedColor() {
        print("turn color");
        //GetComponent<SpriteRenderer>().material = Mats.S.Mat_Hurt;
    }

    public void FreezeMotion() {
        print("freeze!");
        GetComponent<Animator>().speed = 0;
    }
    public void ContinueMotion() {
        GetComponent<Animator>().speed = 1;
    }

    public GameObject Probe(Vector3 direction, float length) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, length, 1<<LayerMask.NameToLayer("Ground")))
            return hit.collider.gameObject;
        else
            return null;
    }

    public abstract void HurtByBullet();
    public abstract void HurtByMissle();

}
