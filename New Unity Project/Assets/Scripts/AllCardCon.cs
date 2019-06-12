using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AllCardCon : MonoBehaviour
{
    public static int mId;
    public Transform[] players;
    public Transform table;

    public float cardDisplayWidth;

    private Transform[] cardsTrans;
    private CardControl[] cardCons;

    public static int dragingCard = -1;

    public GameObject askUI;
    public Text askUIText;

    public bool distributeAni;
        

    void Awake()
    {
        int children = transform.childCount;
        cardsTrans = new Transform[children];
        cardCons = new CardControl[children];
        for (int i = 0; i < children; ++i)
        {
            cardsTrans[i] = transform.GetChild(i);
            cardCons[i] = cardsTrans[i].gameObject.AddComponent<CardControl>();
            cardCons[i].cardId = i;
        }
        
    }

    void Start() {
        askUI.SetActive(false);
        int []card = new int[52];
        Random rand = new Random();
        int iTarget = 0, iCardTemp = 0;
        for (int i = 0; i < 52; i++)
            card[i] = i;
        for (int i = 0; i < 52; i++) {
            iTarget = UnityEngine.Random.Range(0, 52);
            iCardTemp = card[i];
            card[i] = card[iTarget];
            card[iTarget] = iCardTemp;
        }
        for (int i = 0; i < Data.PlayerNumber; i++)
        {
            for(int j=0; j< Data.PlayerCardNumber; j++) {
                Give(i, new int[] { card[j+i*Data.PlayerCardNumber] });
                Debug.Log("person:"+i+" "+card[j + i * Data.PlayerCardNumber]);
            }
        }
    }

    void Update()
    {
        if (mId == nextPlayerCon.player)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (string.Equals(hit.transform.tag, "Card"))
                    {
                        dragingCard = hit.collider.gameObject.GetComponent<CardControl>().cardId;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragingCard > -1)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        string hitTag = hit.transform.tag;
                        //click the card(take it)
                        if (string.Equals(hitTag, "Card") && Data.NeedDrawCard == true)
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            if (hit.collider.GetComponent<CardControl>().cardId == dragingCard)
                            {
                                Give(0, new int[] { dragingCard });
                                CardPosition(oldParent);
                            }
                        }
                        //give card away
                        if (string.Equals(hitTag, "Player"))
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            Give(hit.collider.GetComponent<OtherPlayer>().playerId, new int[] { dragingCard });
                            CardPosition(oldParent);
                        }
                        //put on table
                        if (string.Equals(hitTag, "Table"))
                        {
                            int oldParent = cardCons[dragingCard].getParent();
                            PutOnTable(hit.point, dragingCard);
                            CardPosition(oldParent);
                        }
                    }
                    dragingCard = -1;
                }
            }
        }
    }

    public void Give(int player, int[] cards)
    {
        foreach (int i in cards)
        {
            cardsTrans[i].SetParent(players[player]);
            cardsTrans[i].localPosition = Vector3.zero;
            cardsTrans[i].localRotation = Quaternion.Euler(Vector3.zero);
        }
        CardPosition(player);
    }

    public void CardPosition(int player)
    {
        if (player < 4)
        {
            Transform playerTrans = players[player];
            int children = playerTrans.childCount;

            float gap = cardDisplayWidth / (children + 1);
            float xPos = - cardDisplayWidth / 2 + gap;
            for (int i = 0; i < children; ++i)
            {
                playerTrans.GetChild(i).localPosition = new Vector3(xPos, 0, i * -0.000001f);
                xPos += gap;
            }
        }
    }

    public void PutOnTable(Vector3 point, int cardId) {
        Transform card = cardsTrans[cardId];
        card.SetParent(table);
        card.localPosition = new Vector3(point.x, 0, point.z);
        card.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));    
    }

    public void Ask(int player){
        askUIText.text = "玩家" + player.ToString() + "正要取走你一張牌";
        askUI.SetActive(true);
    }

    public void AskOK()
    {
        askUI.SetActive(false);
    }

    public void AskCancel() {
        askUI.SetActive(false);
    }

    public void aniBtnClick(Text text) {
        if (mId == 0)
        {
            distributeAni = !distributeAni;
            text.text = (distributeAni) ? "發牌過程on" : "發牌過程off";
        } 
    }

    public void reStart() {
        if(mId == 0)
            SceneManager.LoadScene(2);
    }

    public void exit() {
        SceneManager.LoadScene(0);
    }
}