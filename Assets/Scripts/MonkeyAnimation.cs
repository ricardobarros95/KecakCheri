using UnityEngine;
using System.Collections;

public class MonkeyAnimation : MonoBehaviour {
	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(0.2f);
        GetComponent<Animator>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
