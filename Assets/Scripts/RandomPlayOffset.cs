using UnityEngine;
using System.Collections;

public class RandomPlayOffset : MonoBehaviour 
{
    public float randMax = 1;
	// Use this for initialization
	IEnumerator Start () 
    {
        yield return new WaitForSeconds(Random.Range(0, randMax));
        GetComponent<Animator>().enabled = true;
	}
}
