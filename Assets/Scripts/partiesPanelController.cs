using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class partiesPanelController : MonoBehaviour
{
    public Text buttonText;
    public bool isExpanded;

    public Vector3 hidden;
    public Vector3 expanded;

    public GameObject playerCountry;
    public GameObject countries;

    public GameObject partyScollboxPrefab_Bicameral;
    public GameObject partyScollboxPrefab_Unicameral;

    public GameObject politicianTextPrefab;

    bool setup = false;

    public void movePanel()
    {
        if(!isExpanded)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = expanded;
            buttonText.text = "<<";
            isExpanded = true;
        }
        else if (isExpanded)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = hidden;
            buttonText.text = ">>";
            isExpanded = false;
        }
    }

    public void getPlayerCountry()
    {
        foreach (Transform child in countries.transform)
        {
            if(child.gameObject.GetComponent<CountryController>().isPlayer)
            {
                playerCountry = child.gameObject;
                break;
            }
        }
    }

    public void setupPanel()
    {
        CountryController pCountryScript = playerCountry.GetComponent<CountryController>();
        GameObject differentPartiesContent = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        foreach (GameObject party in pCountryScript.PoliticalParties)
        {
            PartyController partyScript = party.GetComponent<PartyController>();
            GameObject partyScrollbox=null;
            if(pCountryScript.Legislature == legislativeSystem.unicameral)
            {
                partyScrollbox = Instantiate(partyScollboxPrefab_Unicameral, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (pCountryScript.Legislature == legislativeSystem.bicameral)
            {
                partyScrollbox = Instantiate(partyScollboxPrefab_Unicameral, new Vector3(0, 0, 0), Quaternion.identity);
            }
            partyScrollbox.transform.SetParent(differentPartiesContent.transform);
            GameObject partyContent = partyScrollbox.transform.GetChild(0).GetChild(0).gameObject;
            partyContent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = partyScript.partyName + " -\n" + (partyScript.popularity*100.0f).ToString("n2") + "% popularity";
            partyContent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Party Leader - " + partyScript.leader.GetComponent<PoliticianController>().Name;

            if (pCountryScript.Legislature == legislativeSystem.unicameral)
            {
                //get seat amount 
                partyContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Unified Legislature-\n" + partyScript.tallyLower + " Seats";
                GameObject politicianList = partyContent.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
                foreach (GameObject politician in partyScript.members)
                {
                    PoliticianController poli = politician.GetComponent<PoliticianController>();
                    if (poli.isElected) //only display elected officials
                    {
                        GameObject politicianText = Instantiate(politicianTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        politicianText.transform.SetParent(politicianList.transform);
                        politicianText.GetComponent<TextMeshProUGUI>().text = poli.Name + "- " + poli.Constituency;
                    }
                }
            }
            else if (pCountryScript.Legislature == legislativeSystem.bicameral)
            {
                //get seat amount 
                partyContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Upper Legislature-\n" + partyScript.tallyUpper + " Seats";
                partyContent.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Lower Legislature-\n" + partyScript.tallyLower + " Seats";
                GameObject politicianListUpper = partyContent.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
                GameObject politicianListLower = partyContent.transform.GetChild(5).GetChild(0).GetChild(0).gameObject;
                foreach (GameObject politician in partyScript.members)
                {
                    PoliticianController poli = politician.GetComponent<PoliticianController>();
                    if (poli.isElected) //only display elected officials
                    {
                        GameObject politicianText = Instantiate(politicianTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        if(poli.legislature==chamber.Upper) politicianText.transform.SetParent(politicianListUpper.transform);
                        else if (poli.legislature == chamber.Lower) politicianText.transform.SetParent(politicianListLower.transform);
                        politicianText.GetComponent<TextMeshProUGUI>().text = poli.Name + "- " + poli.Constituency;
                    }
                }
            }
        }


        setup = true;
    }
    
    void Awake()
    {
        getPlayerCountry();
        if(!setup) setupPanel();
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
