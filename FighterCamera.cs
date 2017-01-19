
// To the extent possible under law, 
// Tim Glasser (tim_glasser@hotmail.com)     https://www.facebook.com/tim.glasser.75 
// has waived all copyright and related or neighboring rights and responsibilties to
// FighterCamera C# Classes. This work is published from California.

// As indicated by the Creative Commons, the text on this page may be copied, 
// modified and adapted for your use, without any other permission from the author.

// Please do not remove this notice
using UnityEngine;

// FighterCamera emulates the camera control in classic 3D fighting games such as Tekken and Virtua Fighter. 
// Fighter Camera solves the position and lookat angle for a Unity Camera to keep two fighting figures in optimal view.
// A vector called interdist is calculated between each fighter for each frame.
// The mid point of the interdist vector is the midpoint betweeen the fighters.
// Camera looks at this point. The camera position is calculated by determining the optimum distance the camera must keep 
//  to maintain both fighters (plus the edges) in view. This distance is multiplied by the camera direction vector 
// (The cross product of the interfighter vector and the up vector) to position the camera relative to the mid point of the fighters
//  This script component must be attached to a Unity Camera object

[RequireComponent(typeof(Camera))]
public class FighterCamera : MonoBehaviour {
	
	public Transform fighter1;
	public Transform fighter2;
	public float camHeight = 1.5f;
	public float camSpeed = 20.0f;
	public float edge = 2.0f;
    public float near = 5.0f;
    public float far = 10000.0f;

    private Vector3 pos1;
	private Vector3 pos2 ;
	private Vector3 midPos, newPos ;
	private Vector3 interLine ;
	private Vector3 camDir ;
	private Vector3 campos;

	private float w, d, fov;

	// Use this for initialization
	void Start () {
        // Early out if we don't have a target
        if (!fighter1)
            return;
        if (!fighter2)
            return;
        calcFrustum(fighter1, fighter2); // updates w and d
	}

    // Calculates the private screen width (w) and screen depth (d) distances 
    private void calcFrustum(Transform p1, Transform p2){

        fov = Mathf.Tan(Camera.main.fieldOfView * Mathf.PI / 180.0f); // convert to rads
        w = Vector3.Magnitude(p1.position - p2.position ) + (2.0f*edge);
        d = (w / 2.0f) / fov;                                        
    }

	void Update(){
        // Early out if we don't have a target
        if (!fighter1)
            return;
        if (!fighter2)
            return;
        calcFrustum(fighter1, fighter2); // updates w and d
	}

	void OnDrawGizmos ()
	{
        // Early out if we don't have a target
        if (!fighter1)
            return;
        if (!fighter2)
            return;
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        Vector3 lh1 = new Vector3(fighter1.position.x, camHeight, fighter1.position.z);
        Vector3 lh2 = new Vector3(fighter2.position.x, camHeight, fighter2.position.z);
        Gizmos.DrawLine(lh1,lh2);
		Gizmos.DrawWireSphere(midPos,0.2f);
     
    }
	
	// Update is called once per frame
	void LateUpdate () {
		// Early out if we don't have a target
		if (!fighter1)
			return;
		if (!fighter2)
			return;

		pos1 = fighter1.position;
		pos2 = fighter2.position;
		interLine = pos1-pos2;
		midPos = pos1-(interLine / 2.0f);
		midPos.y = camHeight; // set height of lookat target

		interLine = pos1-pos2;
        float interDist = interLine.magnitude;
		camDir = Vector3.Cross(interLine,Vector3.up); //left hand rule

        float camTargetdist; 
        camTargetdist = Mathf.Clamp(d, near, far); // not too close or too far away
        newPos = (camDir.normalized * camTargetdist) + midPos; // update the distance in calcScreen
        campos = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * camSpeed);
		campos.y = camHeight;

        // Always look at the midpoint between the fighters
        transform.position = campos;
        transform.LookAt (midPos);
	}
}
