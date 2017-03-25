// Same behaviour as the Zoomer in Metroid NES

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : Monster {

    public bool ____public____;
    public bool clockwise;
    public float speed;

    public bool ____private____;
    public float bodySize;
    public Vector3 normalVector;
    public GameObject attachedTile;
    public Vector3 attachedPos;
    public Vector3 orientation;
    public Camera cam;

    public void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        bodySize = GetComponent<Renderer>().bounds.size.y/2f;
        normalVector = transform.rotation * Vector3.up;
        TryAttach();
        tileSize = attachedTile.GetComponent<Renderer>().bounds.size.y/2f;
        transform.position = attachedTile.transform.position+normalVector*(bodySize+tileSize);
        orientation = Quaternion.Euler(0, 0, clockwise?-90:90)*normalVector;
    }

    public void TryAttach() {
        float eps = 0.05f;
        attachedTile = Probe(-normalVector, bodySize+tileSize);
        if (attachedTile!=null) {
            attachedPos = attachedTile.transform.position;
        }
        else {
            RaycastHit hit;
            Physics.Raycast(transform.position-(bodySize+eps)*orientation, -normalVector, out hit, bodySize+tileSize);
            if (hit.transform!=null) {
                attachedTile = hit.transform.gameObject;
                attachedPos = attachedTile.transform.position;
            } else {
                Physics.Raycast(transform.position+(bodySize+eps)*orientation, -normalVector, out hit, bodySize+tileSize);
                if (hit.transform!=null) {
                    attachedTile = hit.transform.gameObject;
                    attachedPos = attachedTile.transform.position;
                } else {
                    print("Zoomer: No attachable tiles found!");
                    Die();
                }
            }
        }
    }

    public void Update() {
        float eps = 0.05f;
        if (attachedTile!=null) {
            if (attachedTile.transform.position != attachedPos)
                TryAttach();
            if (attachedTile == null) return;
            float delta = Vector3.Dot(orientation, transform.position-attachedTile.transform.position);
            Vector3 rotAngles = Vector3.zero;
            Vector3 adjustment = Vector3.zero;
            Quaternion rotation;
            GameObject groundTile, sideTile;
            if (delta>=bodySize+tileSize) {
                groundTile = Probe(-normalVector, bodySize+tileSize-eps);
                if (groundTile==null) {
                    rotAngles = 90*Vector3.Cross(normalVector, orientation);
                    adjustment = orientation*(delta-bodySize-tileSize);
                } else {
                    sideTile = Probe(orientation, bodySize);
                    if (sideTile == null) {
                        rotAngles = Vector3.zero;
                        attachedTile = groundTile;
                        attachedPos = groundTile.transform.position;
                    } else {
                        rotAngles = 90*Vector3.Cross(orientation, normalVector);
                        attachedTile = sideTile;
                        attachedPos = sideTile.transform.position;
                        adjustment = orientation*(delta-bodySize-tileSize);
                    }
                }
            } else {
                sideTile = Probe(orientation, bodySize);
                if (sideTile != null) {
                    rotAngles = 90*Vector3.Cross(orientation, normalVector);
                    attachedTile = sideTile;
                    attachedPos = sideTile.transform.position;
                    float tmpSize = Mathf.Abs(Vector3.Dot(orientation, sideTile.GetComponent<BoxCollider>().bounds.extents));
                    if (tmpSize >= 0.4f)
                        adjustment = orientation*(bodySize+tmpSize-Vector3.Dot(orientation, sideTile.transform.position-transform.position));
                }
            }
            if (rotAngles != Vector3.zero) {
                transform.position -= adjustment;
                rotation = Quaternion.Euler(rotAngles);
                transform.Rotate(rotAngles);
                orientation = rotation*orientation;
                normalVector = rotation*normalVector;
            }
        } else {
            TryAttach();
        }
    }

    public void FixedUpdate() {
        if ((int)Time.time % 2 == 0) return;
        if (!recovery)
            transform.position += orientation*speed;
    }    
}
