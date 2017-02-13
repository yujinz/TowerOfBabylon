using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HammerLen {
    L,
    S
}

public enum Facing {
    L,
    R
}

public class Hero : MonoBehaviour {
    static public Hero S;

    public GameObject HammerS, HammerL, RodS, RodL;
    Rigidbody HammerRigid, bodyRigid, RodRigid;
    SphereCollider feet;

    public HammerLen len;
    public bool HammerLeft = true;
    public bool grounded = true;
    public bool isTakingOff = false;
    //public Vector3 up;
    //public float dist;

    public int hitForce = 30;
    public int jumpForce = 50;
    public float hitRateS = 1f;
    public float hitRateL = 2f;
    public float jumpSpeed = 10f;
    public float speedX = 4f;
    Vector3 groudCheckOffset;
    LayerMask groundPhysicsLayerMask;

    void Awake() {
        S = this;
    }

    void Start () {
        SwitchLenS();
        bodyRigid = GetComponent<Rigidbody>();

        Transform baseTrans = this.transform.Find("Feet");
        feet = baseTrans.GetComponent<SphereCollider>();
        groudCheckOffset = new Vector3(feet.radius * 0.4f, 0, 0);
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKeyDown(KeyCode.S)) {
            StartCoroutine("HitBackForth");
        }
        else if (Input.GetKeyUp(KeyCode.S)) {
            StopCoroutine("HitBackForth");
        }
        */
        //=================== Switch Hammer Length ===================
        if (Input.GetKeyDown(KeyCode.D)) {
            if (len == HammerLen.S)
                SwitchLenL();
            else 
                SwitchLenS();
        }
    }

    void FixedUpdate() {
        Vector3 checkLoc = feet.transform.position + Vector3.down * (feet.radius * 0.2f);
        grounded = (Physics.Raycast(checkLoc + groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask))
                   || (Physics.Raycast(checkLoc - groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask));

        Debug.DrawLine(checkLoc + groudCheckOffset, checkLoc + groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);
        Debug.DrawLine(checkLoc - groudCheckOffset, checkLoc - groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);


        Vector3 vel = bodyRigid.velocity;

        //============================ Move ==========================
        /*
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))  vel.x = -speedX;
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))   vel.x = speedX;
        else  vel.x = 0;
        */
        if (getLeftJQ() == 1 || getLeftJQ() == 8) { vel.x = speedX; }
        else if (getLeftJQ() == 4 || getLeftJQ() == 5) { vel.x = -speedX; }
        else { vel.x = 0; }

        //============================ Jump ==========================
        //Vector3 up = HammerRigid.gameObject.transform.up;
        //float dist = Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up);
        //print(dist);

        if (grounded && isTakingOff) { //give hero jump speed
            if (HammerLeft && hammerExactLeft()) vel.y = jumpSpeed;
            if (!HammerLeft && hammerExactRight()) vel.y = jumpSpeed;
        }
        else if (grounded && !isTakingOff && HammerRigid.gameObject.transform.localPosition.y > -0.1 &&// is landing off
            HammerRigid.gameObject.layer == LayerMask.NameToLayer("HammerJump")) {
            vel = Vector3.zero; //no bounce
            HammerRigid.gameObject.layer = LayerMask.NameToLayer("Hammer");
            if (!HammerLeft)  HammerRigid.AddForce(-Vector3.right * hitForce, ForceMode.Impulse);
            else              HammerRigid.AddForce(Vector3.right * hitForce, ForceMode.Impulse);
        }        

        if (Mathf.Abs(vel.y) > 0.5f && hammerExactUp()) { //0.5f means !grounded
            HammerRigid.gameObject.transform.up = Vector3.right;
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            isTakingOff = false;
        }
        else if (vel.y <= -0.5f) {
            isTakingOff = false;
        }

        bodyRigid.velocity = vel;

        //========== hammer no bounce ==============
        if (!isTakingOff && HammerLeft && hammerExactRight()) {
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = false;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        if (!isTakingOff && !HammerLeft && hammerExactLeft()) {
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = true;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }

    }
    /*
    IEnumerator HitBackForth() {
        while (true) {
            if (HammerLeft) {
                HammerLeft = false;
            }
            else {
                HammerLeft = true;
            }

            HammerRigid.AddForce(transform.up * hitForce, ForceMode.Impulse);
            float period = (len == HammerLen.S ? hitRateS : hitRateL);
            yield return new WaitForSeconds(period);
        }
    }
    */
    void SwitchLenS() {
        len = HammerLen.S;
        HammerRigid = HammerS.GetComponent<Rigidbody>();
        RodRigid = RodS.GetComponent<Rigidbody>();
        RodS.SetActive(true);
        HammerS.SetActive(true);
        HammerL.SetActive(false);
        RodL.SetActive(false);
    }

    void SwitchLenL() {
        len = HammerLen.L;
        HammerRigid = HammerL.GetComponent<Rigidbody>();
        RodRigid = RodL.GetComponent<Rigidbody>();
        RodL.SetActive(true);
        HammerL.SetActive(true);
        HammerS.SetActive(false);
        RodS.SetActive(false);
    }

    public void hitClockwise() {
        HammerRigid.AddForce(HammerRigid.gameObject.transform.up * hitForce, ForceMode.Impulse);
    }

    public void hitCounterClockwise() {
        HammerRigid.AddForce(-HammerRigid.gameObject.transform.up * hitForce, ForceMode.Impulse);
    }

    public void startJumpClockwise() {
        if (isTakingOff) return;
        isTakingOff = true;
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");

        HammerRigid.AddForce(HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public void startJumpCounterClockwise() {
        if (isTakingOff) return;
        isTakingOff = true;
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");

        HammerRigid.AddForce( - HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public bool hammerExactLeft() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.up) < 0.15f;
    }

    public bool hammerExactRight() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up) < 0.15f;
    }

    public bool hammerExactUp() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.right) < 0.15f;
    }

    int getLeftJQ() {
        float x = Input.GetAxis("LeftJoystickX");
        float y = Input.GetAxis("LeftJoystickY");
        return clockwiseSeq.getQuadrant(x, y);
    }
}


