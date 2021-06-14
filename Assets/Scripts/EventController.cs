using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class EventController : MonoBehaviour
{
    public TileMapGenerator TMG;
    public GameController gameScript;
    public GameObject ChangePanel;
    public GameObject eventWindow;
    public TextMeshProUGUI eventWindowBody;
    public TextMeshProUGUI eventWindowTitle;
    //public GameController GC;
    private GameObject playerCountry;
    private CountryController playerCountryScript;

    public GameObject singleButton;
    public TextMeshProUGUI singleButtonText; //button corresponds to true
    public GameObject doubleButton;
    public TextMeshProUGUI doubleButtonText1; //button one corresponds to true
    public TextMeshProUGUI doubleButtonText2; //button two corresponds to false

    GameObject eventTarget;

    int currentEvent;

    public void Setup(GameObject c)
    {
        playerCountry = c;
        playerCountryScript = playerCountry.GetComponent<CountryController>();
    }

    /*
    //button function (some events have one button some have two)
    void buttons(List<string> options) //length of list corresponds to number of buttons
    {

    }
    */

    public void HoldElection(int termTime)
    {
        currentEvent = -1;
        playerCountryScript.holdElection(TMG.mapSize.x, TMG.mapSize.y);
        eventWindow.SetActive(true);
        eventWindowTitle.text = "Election Time!";
        eventWindowBody.text = "The people of "+ playerCountry.name+" have once again convened to decide who will lead them for the next "+ termTime.ToString()+" years. The "+ playerCountryScript.leadingParty().partyName +" has come out on top, securing the most seats in our legislative bodies.";
        singleButtonText.text = "OK";
        singleButton.SetActive(true);
    }

    public void HoldElection(bool option)
    {
        hideEventWindow();
        gameScript.advanceMonth();
    }

    public void proposalRejected()
    {
        currentEvent = -2;
        eventWindow.SetActive(true);
        eventWindowTitle.text = "Legislation Rejected";
        eventWindowBody.text = "All the politicians have convened in our nations legislative bodies to discuss the new legislation that has been put forward to them. Unfortunately the legislation was unable to achieve majority support and has been rejected by the electives of our country.";
        singleButtonText.text = "Disappointing";
        singleButton.SetActive(true);
    }

    public void proposalEnacted()
    {
        currentEvent = -2;
        eventWindow.SetActive(true);
        eventWindowTitle.text = "Legislation Enacted";
        eventWindowBody.text = "All the politicians have convened in our nations legislative bodies to discuss the new legislation that has been put forward to them. Today the electives of our country have voted in favour of new legislation which managed to achieve majority support. What implications this has for our countries future remains to be seen...";
        singleButtonText.text = "Excellent";
        singleButton.SetActive(true);
    }

    public void selectEvent()
    {
        eventWindow.SetActive(true);
        //randomly choose an event
        int eventCount = 6; //how many possible different events there are
        currentEvent = Random.Range(0, eventCount); //randomly select an event

        switch(currentEvent)
        {
            case 0:
                SlowNewsEvent();
                break;

            case 1:
                ChangesEvent();
                break;

            case 2:
                PoliticianScandalEvent();
                break;

            case 3:
                PoliticianBlunderEvent();
                break;

            case 4:
                PoliticianBoonEvent();
                break;

            case 5:
                PollEvent();
                break;

            default:
                defaultEvent();
                break;
        }
    }

    public void enactEvent(bool option)
    {
        switch (currentEvent)
        {
            case -1:
                HoldElection(option);
                break;

            case 0:
                SlowNewsEvent(option);
                break;

            case 1:
                ChangesEvent(option);
                break;

            case 2:
                PoliticianScandalEvent(option);
                break;

            case 3:
                PoliticianBlunderEvent(option);
                break;

            case 4:
                PoliticianBoonEvent(option);
                break;

            case 5:
                PollEvent(option);
                break;

            default:
                defaultEvent(option);
                break;
        }
    }

    void defaultEvent()
    {
        //title of event window
        eventWindowTitle.text = "Title"; 
        //main body of text in event window
        eventWindowBody.text = "Body text.";
        //
        singleButtonText.text = "Excellent";
        //set the "single button" setup to active
        singleButton.SetActive(true);

        //below is an example of a double button setup
        /*
        doubleButtonText1.text = "Yes";
        doubleButtonText2.text = "No";
        doubleButton.SetActive(true);
        */
    }

    void defaultEvent(bool option)
    {
        //insert event effects here
        //
        //
        //always include code to hide event window and advance the month
        hideEventWindow();
        gameScript.advanceMonth();
    }

    void SlowNewsEvent()
    {
        eventWindowTitle.text = "Slow News Day";
        eventWindowBody.text = "No significant news about the government has come out this month and ministers have managed to avoid any scandals. All the different publications are covering various minor stories. It would seem no news is good news.";
        singleButtonText.text = "Excellent";
        singleButton.SetActive(true);
    }

    void SlowNewsEvent(bool option)
    {
        hideEventWindow();
        gameScript.advanceMonth();
    }

    void ChangesEvent()
    {
        eventWindowTitle.text = "The Winds of Change";
        eventWindowBody.text = "The public's ire towards the government has been growing and many are calling for radical change. Perhaps this is an oppurtunity to reform and shift things in our favor, or perhaps it is better to stay the course and remain as we are.";
        doubleButtonText1.text = "Reform";
        doubleButtonText2.text = "Remain";
        doubleButton.SetActive(true);
    }

    void ChangesEvent(bool option)
    {
        hideEventWindow();
        if (option)
        {
            ChangePanel.SetActive(true);
            ChangePanel.GetComponent<changePanelController>().movePanel();
            //month advance done when change selected
            //gameScript.advanceMonth();
        }
        else
        {
            gameScript.advanceMonth();
        }
        
    }

    // void politician has scandal ()
    void PoliticianScandalEvent()
    {
        eventTarget = playerCountryScript.grabRandomPolitician();
        eventWindowTitle.text = "Political Scandal";
        eventWindowBody.text = eventTarget.name + " has been caught up in a scandal that has caused significant public outcry with several prominent figures calling for their resignation. It is doubtless that this will significantly damage their chances of re-election.";
        singleButtonText.text = "Oh dear";
        singleButton.SetActive(true);
    }

    void PoliticianScandalEvent(bool option)
    {
        eventTarget.GetComponent<PoliticianController>().adjustPopularity(-Random.Range(0.5f, 0.9f));
        hideEventWindow();
        gameScript.advanceMonth();
    }

    // void politician does thing to make themselves less popular ()
    void PoliticianBlunderEvent()
    {
        eventTarget = playerCountryScript.grabRandomPolitician();
        eventWindowTitle.text = "Politician Blunders";
        eventWindowBody.text = eventTarget.name + " caused public outcry today in what may be the worst political blunder of their career. No doubt this will hinder their popularity.";
        singleButtonText.text = "Oh dear";
        singleButton.SetActive(true);
    }

    void PoliticianBlunderEvent(bool option)
    {
        eventTarget.GetComponent<PoliticianController>().adjustPopularity(-Random.Range(0.1f, 0.5f));
        hideEventWindow();
        gameScript.advanceMonth();
    }


    // void politician does thing to make themselves more popular ()
    void PoliticianBoonEvent()
    {
        eventTarget = playerCountryScript.grabRandomPolitician();
        eventWindowTitle.text = "Politician Charms Public";
        eventWindowBody.text = "In a moment of humility " + eventTarget.name + " has endeared themselves to the members of the public and made themselves relatable to the common citizen. No doubt this will bolster their popularity.";
        singleButtonText.text = "OK";
        singleButton.SetActive(true);
    }

    void PoliticianBoonEvent(bool option)
    {
        eventTarget.GetComponent<PoliticianController>().adjustPopularity(Random.Range(0.1f, 0.9f));
        hideEventWindow();
        gameScript.advanceMonth();
    }


    // void poll ()
    void PollEvent()
    {
        eventTarget = playerCountryScript.grabRandomPolitician().GetComponent<PoliticianController>().Constituency_GO;
        GameObject c = eventTarget.GetComponent<ConstituencyController>().mostPopular();
        eventWindowTitle.text = "Poll Predictions";
        eventWindowBody.text = "Recent polls suggest that "+ c.name + "is favoured to win the " + eventTarget.name + " candidacy in any future election.";
        singleButtonText.text = "OK";
        singleButton.SetActive(true);
    }

    void PollEvent(bool option)
    {
        hideEventWindow();
        gameScript.advanceMonth();
    }

    // void party leader does thing to make their party more/less popular ()

    void hideEventWindow()
    {
        singleButton.SetActive(false);
        doubleButton.SetActive(false);
        eventWindow.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
