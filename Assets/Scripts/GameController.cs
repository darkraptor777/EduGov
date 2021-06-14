using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class GameController : MonoBehaviour
{
    private int month = 1;
    private int year = 0;
    public TextMeshProUGUI monthText;

    private int timeSinceLastElection;

    public EventController eventScript;

    public changePanelController changeScript;
    private GameObject playerCountry;
    private CountryController playerCountryScript;

    private int termTime;

    // Start is called before the first frame update
    void Awake()
    {
        grabPlayerCountry();
        eventScript.Setup(playerCountry);
        eventScript.selectEvent();
        termTime = playerCountryScript.LegislativeChambers[0].GetComponent<LegislatureController>().electionTime;
        updateMonthText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMonthText()
    {
        monthText.text = "Year: " + year.ToString() + " Month: " + month.ToString();
    }

    public void advanceMonth()
    {
        month += 1;
        if(month>12) { year += 1; month = 1; timeSinceLastElection += 1; }
        updateMonthText();

        if (timeSinceLastElection >= termTime) { eventScript.HoldElection(termTime); timeSinceLastElection = 0; }

        else
        {
            eventScript.selectEvent();
        }
        
        
    }

    void grabPlayerCountry()
    {
        playerCountry = changeScript.playerCountry;
        playerCountryScript = playerCountry.GetComponent<CountryController>();
    }
}
