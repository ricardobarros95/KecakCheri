using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum BtnType { A, B, X, Y }

public class Game : MonoBehaviour
{
    //parameters
    public int damage = 20;
    public int heal = 15;
    public float speedMod = 0.1f;
    float pauseDelay = 0.5f;

    //player healths
    public int maxHealth = 100;

    //global speed modifier
    [HideInInspector] public float speed = 1;
    float internalTimer = 5;

    //contains the transforms of the buytton traps for players 1-2
    List<Transform>[] btnTraps = new List<Transform>[2];

    [HideInInspector] public int[] currentSelectedAction = new int[2];

    List<int>[] combos = new List<int>[2];
    List<Transform>[] fallers = new List<Transform>[2];
    PoopAction[] currentActions = new PoopAction[2];
    bool[] ready = new bool[2];
    bool[] success = new bool[2];
    int[] prevState = new int[2];
    float[] fallSpeeds = new float[2];
    float spawnHeight = 1;

    public RectTransform p1HealthRect;
    public RectTransform p2HealthRect;

    Player[] players = new Player[2];


    public GameObject p1Action;
    public GameObject p2Action;
    public Sprite[] actions;

    public Animator[] animators;
    public Sprite[] normalButtonSprites;
    public Sprite[] litButtonsSprites;
    public Sprite failedButtonSprite;
    public GameObject[] p1buttonObjects;
    public GameObject[] p2buttonObjects;
    public int[] score = new int[2];
    public Text scoreText;
    public GameObject winningPanel;
    public GameObject poopPrefab;
    public GameObject poopPrefab2;
    public AnimationClip poopAnimation;
    public GameObject[] buffs;
    public Sprite[] winSprites;
    public AudioClip[] clips;
    public Image[] currentActionImage;
    public AudioClip[] successSounds;
    public AudioClip failSound;

    int[] presses = new int[2];

    void Start()
    {
        for(int player = 0; player < 2; player++)
        {
            //constructing everything
            btnTraps[player] = new List<Transform>();
            fallers[player] = new List<Transform>();
            players[player] = new Player(maxHealth);

            for (int btn = 0; btn < 4; btn++)
            {
                GameObject obj = GameObject.Find(((BtnType)btn).ToString() + " - " + player);
                btnTraps[player].Add(obj.transform);
            }
        }
        StartCoroutine(SpawnFallers(0, pauseDelay));
        StartCoroutine(SpawnFallers(1, pauseDelay));
    }

    void Update()
    {
        buffs[0].SetActive(players[0].IsInvulnerable());
        buffs[1].SetActive(players[1].IsInvulnerable());

        if(ready[0] && ready[1] && fallers[0].Count == 0 && fallers[1].Count == 0)
        {
            Debug.Log("================================================== ");
            StartCoroutine(SpawnFallers(0, pauseDelay));
            StartCoroutine(SpawnFallers(1, pauseDelay));
            animators[0].SetInteger("Action", -1);
            animators[1].SetInteger("Action", -1);
            presses[0] = presses[1] = 0;
        }

        int p1Change = (int)Input.GetAxis("SwitchP1");
        int p2Change = (int)Input.GetAxis("SwitchP2");
        if(p1Change != prevState[0])
        {
            if (p1Change != 0)
            {
                currentSelectedAction[0] = (currentSelectedAction[0] + p1Change + 3) % 3;
                p1Action.GetComponent<Image>().sprite = actions[currentSelectedAction[0]];
                Debug.Log("P1 SUCESS");
            }
            prevState[0] = p1Change;
        }
        if(p2Change != prevState[1])
        {
            if(p2Change != 0)
            {
                currentSelectedAction[1] = (currentSelectedAction[1] + p2Change + 3) % 3;
                p2Action.GetComponent<Image>().sprite = actions[currentSelectedAction[1]];
            }
            prevState[1] = p2Change;
        }
        

        for (int player = 0; player < 2; player++)
            for (int btn = 0; btn < 4; btn++)
                if (Input.GetKeyDown("joystick " + (player + 1) + " button " + btn))
                    ClickCheck(player, btn);

        UpdateFallers();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetHealth() >= 100) players[i].SetHealth(100);
            else if (players[i].GetHealth() <= 0) players[i].SetHealth(0);
        }
        p1HealthRect.transform.localScale = new Vector3((float)players[0].GetHealth() / maxHealth, 1, 1);
        p2HealthRect.transform.localScale = new Vector3((float)players[1].GetHealth() / maxHealth, 1, 1);
        CheckWinningCondition();
    }

    void CheckWinningCondition()
    {
        if(players[0].GetHealth() <= 0)
        {
            score[1]++;
            players[0].SetHealth(100);
            players[1].SetHealth(100);
            speed = 1;
            scoreText.text = score[0].ToString() + " - " + score[1].ToString();
            animators[0].SetInteger("Action", 4);
            animators[1].SetInteger("Action", 3);
        }
        else if (players[1].GetHealth() <= 0)
        {
            score[0]++;
            players[0].SetHealth(100);
            players[1].SetHealth(100);
            speed = 1;
            scoreText.text = score[0].ToString() + " - " + score[1].ToString();
            animators[1].SetInteger("Action", 4);
            animators[0].SetInteger("Action", 3);
        }
        if(score[0] == 2 || score[1] == 2)
        {
            if (score[0] > score[1])
            {
                winningPanel.GetComponent<Image>().sprite = winSprites[0];
                winningPanel.SetActive(true);
                this.enabled = false;
                animators[1].SetInteger("Action", 4);
                animators[0].SetInteger("Action", 3);
            }
            else
            {
                winningPanel.GetComponent<Image>().sprite = winSprites[1];
                winningPanel.SetActive(true);
                this.enabled = false;
                animators[0].SetInteger("Action", 4);
                animators[1].SetInteger("Action", 3);
            }
            
        }
    }

    public void RestartButton()
    {
        Application.LoadLevel(0);
    }

    List<int> GenerateCombo(int btnCount)
    {
        List<int> btns = new List<int>(btnCount);
        for (int i = 0; i < btnCount; i++)
            btns.Add(Random.Range(0, 4));
        return btns;
    }

    IEnumerator SpawnFallers(int player, float delay)
    {
        ready[player] = false;
        animators[player].SetInteger("Action", -1);
        yield return new WaitForSeconds(delay);
        int btnCount = 0;
        switch(currentSelectedAction[player])
        {
            case 0:
                currentActions[player] = new PoopAction("Damage", this);
                btnCount = 6;
                Debug.Log("Next Action: Damage");
                break;
            case 1:
                currentActions[player] = new PoopAction("Heal", this);
                btnCount = 5;
                Debug.Log("Next Action: Heal");
                break;
            case 2:
                currentActions[player] = new PoopAction("Dodge", this);
                btnCount = 4;
                Debug.Log("Next Action: Dodge");
                break;
        }
        currentActions[player].damage = damage;
        currentActions[player].heal = heal;
        currentActions[player].globalSpeedFactor = speedMod;
        currentActionImage[player].sprite = actions[currentSelectedAction[player]];
        combos[player] = GenerateCombo(btnCount);
        float deltaStep = internalTimer * speed / btnCount;
        success[player] = true;
        //if (player == 1)
        //    Debug.Log("Player " + player + " reset success: " + success[player]);
        fallSpeeds[player] = spawnHeight / deltaStep;
        while (combos[player].Count > 0)
        {
            //wait until it's time to send out a new combo piece
            yield return new WaitForSeconds(deltaStep);

            //spawn the mofo-piece
            string name = btnTraps[player][combos[player][0]].name.Substring(0, 1);
            GameObject comboPiecePrefab = (GameObject)Resources.Load(name);
            int trapInd = combos[player][0];
            Vector3 catcherPos = btnTraps[player][trapInd].position;
            GameObject comboPiece = (GameObject)Instantiate(comboPiecePrefab, catcherPos + new Vector3(0, spawnHeight, 0.001f), Quaternion.Euler(0, 0, 180));
            comboPiece.name = comboPiece.name.Substring(0, 1);
            fallers[player].Add(comboPiece.transform);
            //piece spawned, remove from combo
            combos[player].RemoveAt(0);
            
        }
        ready[player] = true;
    }

    void ClickCheck(int player, int btnClicked)
    {
        if (fallers[player].Count == 0)
            return;

        int btn = -1;
        switch (fallers[player][0].name[0])
        {
            case 'Y':
                btn = 3;
                break;
            case 'X':
                btn = 2;
                break;
            case 'B':
                btn = 1;
                break;
            case 'A':
                btn = 0;
                break;
        }
        Debug.Log(fallers[player][0].name + " vs " + (BtnType)btnClicked + " player: " + player + " state: " + success[player]);
        //if the player pressed one of the buttons to capture
        if (btnClicked == btn)
        {
            //if it's next and in the boundaries - perform the action
            if (!CheckBoundaries(btn, player))
            {
                success[player] = false;
                StartCoroutine(changeWrongSprite(btn, player));
            }
            else
            {
                StartCoroutine(ChangeRightSprite(btn, player));
                if(success[player])
                    GetComponent<AudioSource>().PlayOneShot(clips[presses[player]++]);
            }
            //destroying the falling combo piece
            Destroy(fallers[player][0].gameObject);
            //removing the current combo piece
            fallers[player].RemoveAt(0);

            //check if was the last one - then perform the action
            if (fallers[player].Count == 0 && combos[player].Count == 0 && success[player])
            {
                Debug.Log("Player " + player + " performing an action!");
                currentActions[player].Use(players[player], players[player == 0 ? 1 : 0]);
                animators[player].SetInteger("Action", currentActions[player].GetActionId());
                GetComponent<AudioSource>().PlayOneShot(successSounds[currentActions[player].GetActionId()]);
                if (currentActions[player].GetActionId() == 0)
                    StartCoroutine(DestroyPoop(player));
            }
        }
        else
        {
            if(success[player])
                GetComponent<AudioSource>().PlayOneShot(failSound);
            success[player] = false;
            StartCoroutine(changeWrongSprite(btnClicked, player));
            Debug.Log("Wrong button, expected " + (BtnType)btn + " got " + (BtnType)btnClicked + " for player " + player);
            GetComponent<AudioSource>().PlayOneShot(successSounds[currentActions[player].GetActionId()]);
        }
    }

    IEnumerator DestroyPoop(int player)
    {
        if(player == 0)
        {
            GameObject poop = Instantiate(poopPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            yield return new WaitForSeconds(poopAnimation.length);
            Destroy(poop);
        }
        else
        {
            GameObject poop = Instantiate(poopPrefab2, new Vector3(2.8f, -1f, -0.9f), Quaternion.identity) as GameObject;
            yield return new WaitForSeconds(poopAnimation.length);
            Destroy(poop);
        }
    }

    IEnumerator ChangeRightSprite(int btn, int player)
    {
        if (player == 0)
            p1buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = litButtonsSprites[btn].texture;
        else
            p2buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = litButtonsSprites[btn].texture;

        yield return new WaitForSeconds(0.2f);
        if (player == 0)
            p1buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = normalButtonSprites[btn].texture;
        else
            p2buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = normalButtonSprites[btn].texture;
    }

    IEnumerator changeWrongSprite(int btn, int player)
    {
        if (player == 0)
            p1buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = failedButtonSprite.texture;
        else
            p2buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = failedButtonSprite.texture;

        yield return new WaitForSeconds(0.2f);
        if (player == 0)
            p1buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = normalButtonSprites[btn].texture;
        else
            p2buttonObjects[btn].GetComponent<MeshRenderer>().material.mainTexture = normalButtonSprites[btn].texture;
    }

    bool CheckBoundaries(int btn, int player)
    {
        Transform faller = fallers[player][0];
        float height = faller.position.y;
        float targetHeight = btnTraps[player][btn].position.y;
        bool result = Mathf.Abs(targetHeight - height) < 0.2;
        if(!result)
            Debug.Log("Player " + player + " failed boundary check");
        return result;
    }

    void UpdateFallers()
    {
        for(int player = 0; player < 2; player++)
        {
            foreach (Transform faller in fallers[player])
            {
                Vector3 pos = faller.position;
                pos.y -= fallSpeeds[player] * Time.deltaTime;
                faller.position = pos;
            }

            if(fallers[player].Count > 0)
            {
                if (fallers[player][0].position.y - btnTraps[0][0].position.y < -1) //they're all on the same height
                {
                    Destroy(fallers[player][0].gameObject);
                    fallers[player].RemoveAt(0);
                    success[player] = false;
                    //if(player == 1)
                    //    Debug.Log("Player " + player + " fell through");
                }
            }
        }
    }
}
