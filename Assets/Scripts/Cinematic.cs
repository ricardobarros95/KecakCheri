using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cinematic : MonoBehaviour {

    public AnimationClip ani;
    public GameObject[] blackBars;
    public GameObject gameHolder;
    public GameObject[] healthBars;
    public Transform[] monkeys;
    public float distance = 2;

    Vector3[] oldPositions = new Vector3[2];
    float timer = 0;
    public Animator canvasAnim;
    public GameObject[] activateElements;

	// Use this for initialization
	void Start () 
    {
        canvasAnim.enabled = true;
        oldPositions = new Vector3[2] { monkeys[0].position, monkeys[1].position };
        foreach (Transform monkey in monkeys)
            monkey.position -= monkey.forward * distance;
        StartCoroutine(StartGame());
	}
	
	// Update is called once per frame
	void Update () 
    {
        foreach (Transform monkey in monkeys)
            monkey.position += monkey.forward * distance / ani.length * Time.deltaTime;
        if((timer += Time.deltaTime) > ani.length)
            this.enabled = false;
	}
    
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(ani.length);
         for(int i = 0; i< blackBars.Length; i++)
             blackBars[i].SetActive(false);
         gameHolder.SetActive(true);
         for (int i = 0; i < healthBars.Length; i++)
             healthBars[i].SetActive(true);
         foreach (GameObject g in activateElements)
             g.SetActive(true);
    }
    
}
