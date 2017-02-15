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
    public Rigidbody HammerRigid, bodyRigid, RodRigid;
    SphereCollider feet;
    public Collider HammerCollider;
    public MeshRenderer HammerMesh;
    public GameObject propeller;
    //Material goldMat, redMat;


    public HammerLen len;
    public bool HammerLeft = true;
    public bool grounded = true;
    public bool prevGrounded = true;
    public bool isTakingOff = false;
    public bool needHammerCorrect = false;
    public bool needSwitch = false;
    public bool switched = false;
    public bool isHammer = true;
    public bool shouldClimb = false;
    //public Vector3 up;
    //public float dist;

    public int hitForce = 30;
    public int jumpForce = 50;
    public float hitRateS = 1f;
    public float hitRateL = 2f;
    public float jumpSpeed = 10f;
    public float speedX = 4f;
    public Vector3 speedH = new Vector3(4f, 0, 0);
    Vector3 groudCheckOffset;
    LayerMask groundPhysicsLayerMask;

    void Awake() {
        S = this;
    }

    void Start () {
        switchLenS();
        switchHammer();
        bodyRigid = GetComponent<Rigidbody>();

        Transform baseTrans = this.transform.Find("Feet");
        feet = baseTrans.GetComponent<SphereCollider>();
        groudCheckOffset = new Vector3(feet.radius * 0.4f, 0, 0);
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
		

    void FixedUpdate() {
        Vector3 checkLoc = feet.transform.position + Vector3.down * (feet.radius * 0.2f);
        prevGrounded = grounded;
        grounded = (Physics.Raycast(checkLoc + groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask))
                   || (Physics.Raycast(checkLoc - groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask));
        //Debug.DrawLine(checkLoc + groudCheckOffset, checkLoc + groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);
        //Debug.DrawLine(checkLoc - groudCheckOffset, checkLoc - groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);

        Vector3 vel = bodyRigid.velocity;

        //============================ Move ==========================
        if (getLeftJQ() == 1 || getLeftJQ() == 8) { vel.x = speedH.x; vel.z = speedH.z; }
        else if (getLeftJQ() == 4 || getLeftJQ() == 5) { vel.x = -speedH.x; vel.z = -speedH.z; }
        else { vel.x = 0; vel.z = 0; }

        if (Mathf.Abs(vel.y) < 0.001) vel.y = 0;

        //============================ Jump ==========================
        //Vector3 up = HammerRigid.gameObject.transform.up;
        //float dist = Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up);
        //print(dist);

        if (!needHammerCorrect && vel.y < 0 && !prevGrounded) needHammerCorrect = true;

        if (shouldClimb) {
            vel.y = jumpSpeed/1.2f;
            shouldClimb = false;
            print("climb!");
        }

        if (grounded && isTakingOff) { //give hero jump speed
            if (HammerLeft && hammerExactLeft()) vel.y = jumpSpeed;
            if (!HammerLeft && hammerExactRight()) vel.y = jumpSpeed;
        }
        else if (grounded && !isTakingOff && needHammerCorrect) {
            if (HammerRigid.gameObject.transform.localPosition.y > 0.001) { // hammer is above ground
                vel = Vector3.zero; //no bounce
                switchHammer();
                needHammerCorrect = false;
            }

            //put down hammer
            if (HammerLeft) {
                HammerRigid.AddForce(-Vector3.right * jumpForce, ForceMode.Impulse);
                HammerLeft = false;
                //print("left force" + Time.time);
            }
            else {
                HammerRigid.AddForce(Vector3.right * jumpForce, ForceMode.Impulse);
                HammerLeft = true;
                //print("right force" + Time.time);
            }
        }
        //else if (grounded && !isTakingOff && HammerRigid.gameObject.transform.localPosition.y > 0.1)        

        if (Mathf.Abs(vel.y) > 0.5f && hammerExactUp() && ClockwiseSeq.getRightJQ() == 0) { //0.5f means !grounded
            HammerRigid.gameObject.transform.up = Vector3.right;
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            //HammerRigid.gameObject.layer = LayerMask.NameToLayer("Hammer");//////
            isTakingOff = false;
        }
        else if (vel.y <= -0.5f) {
            isTakingOff = false;
        }

        bodyRigid.velocity = vel;

        //========== hammer no bounce ==============
        if (!isTakingOff && HammerLeft && hammerExactRight()) {
            //print("no bounce");
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = false;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        if (!isTakingOff && !HammerLeft && hammerExactLeft()) {
            //print("no bounce");
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = true;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }

        //========= switch hammer/propeller =========
        if (Input.GetAxis("Trigger") > 0.2 && !grounded &&
            HammerRigid.gameObject.transform.localPosition.y > 0) {
            switchHammer();
        }
        else if (Input.GetAxis("Trigger") <= 0 && !grounded) {
            switchPropeller();
        }
        /*
        if (needSwitch && HammerRigid.gameObject.transform.localPosition.y > 0) { // hammer is above ground
            if(HammerRigid.gameObject.layer == LayerMask.NameToLayer("Hammer")) {
                switchPropeller();
            }
            else if (HammerRigid.gameObject.layer == LayerMask.NameToLayer("HammerJump")) {
                switchHammer();
            }
            switched = true;
            needSwitch = false;

            */
        /*
        if (Input.GetAxis("Trigger") < -0.2) {
            switchPropeller();
        }
        else if (Input.GetAxis("Trigger") > 0.2) {
            switchHammer();
        }
    }*/

        //print(grounded);
        //print(vel.y);

    }
    /*
    void Update () {
        
        if (Input.GetKeyDown(KeyCode.S)) {
            StartCoroutine("HitBackForth");
        }
        else if (Input.GetKeyUp(KeyCode.S)) {
            StopCoroutine("HitBackForth");
        }
        
        //=================== Switch Hammer Length ===================
        if (Input.GetKeyDown(KeyCode.D)) {
            if (len == HammerLen.S)
                switchLenL();
            else 
                switchLenS();
        }
    }

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
    void switchLenS() {
        len = HammerLen.S;
        HammerRigid = HammerS.GetComponent<Rigidbody>();
        RodRigid = RodS.GetComponent<Rigidbody>();
        RodS.SetActive(true);
        HammerS.SetActive(true);
        HammerL.SetActive(false);
        RodL.SetActive(false);
    }

    void switchLenL() {
        len = HammerLen.L;
        HammerRigid = HammerL.GetComponent<Rigidbody>();
        RodRigid = RodL.GetComponent<Rigidbody>();
        RodL.SetActive(true);
        HammerL.SetActive(true);
        HammerS.SetActive(false);
        RodS.SetActive(false);
    }

    void switchPropeller() {
        if (Hammer.S.isColliding) return;
        propeller.SetActive(true);
        //HammerCollider.enabled = false;
        HammerMesh.enabled = false;
        isHammer = false;
    }

    void switchHammer() {
        propeller.SetActive(false);
        //HammerCollider.enabled = true;
        HammerMesh.enabled = true;
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("Hammer");
        isHammer = true;
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
        switchPropeller();
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");

        HammerRigid.AddForce(HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public void startJumpCounterClockwise() {
        if (isTakingOff) return;
        isTakingOff = true;
        switchPropeller();
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");

        HammerRigid.AddForce( - HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public bool hammerExactLeft() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.up) < 0.2f;
    }

    public bool hammerExactRight() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up) < 0.2f;
    }

    public bool hammerExactUp() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.right) < 0.2f;
    }

    int getLeftJQ() {
        float x = Input.GetAxis("LeftJoystickX");
        float y = Input.GetAxis("LeftJoystickY");
        return ClockwiseSeq.getQuadrant(x, y);
    }
}


