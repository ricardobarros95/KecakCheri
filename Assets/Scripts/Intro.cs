using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour 
{
    public AnimationClip clip;
    public Cinematic cin;
    public Camera camera;
    public GameObject logoCam;
    public GameObject[] borders;

	IEnumerator Start () 
    {
        yield return new WaitForSeconds(clip.length * 0.8f);
        cin.enabled = true;
        logoCam.SetActive(false);
        camera.enabled = true;
        GetComponent<Animator>().enabled = true;
        borders[0].SetActive(true);
        borders[1].SetActive(true);
	}
}
