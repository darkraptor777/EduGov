using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class CountryGenerator : MonoBehaviour
{
    public TileMapGenerator TMG;
    public GameObject countrySelectInfo;
    public GameObject countryPrefab;
    public GameObject countryMaps;
    public GameObject loading;
    public GameObject countrySelectionPanel;
    public List<GameObject> countries;
    public EventController eventScript;

    bool generationDone = false;
    bool startedGeneration = false;

    // Start is called before the first frame update
    void Start()
    {
        //countries = new List<GameObject>();
        countrySelectionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(TMG.generationDone&&!startedGeneration)
        {
            loading.SetActive(true);
            
            StartCoroutine(countrySetup());
            startedGeneration = true;
        }
    }

    public void setPanelActiveStatus(bool b = false)
    {
        countrySelectionPanel.SetActive(b);
    }

    public IEnumerator countrySetup()
    {
        while (!generationDone)
        {
            for (int c = 0; c < TMG.noAI; c++)
            {
                //create country
                //countries.Add(Instantiate(countryPrefab, new Vector3(0, 0, 0), Quaternion.identity));
                CountryController country = countries[c].GetComponent<CountryController>();
                //select country name
                country.selectName();
                country.setID(c);
                //do some country name validation

                country.map = countryMaps.transform.GetChild(c).GetComponent<Tilemap>();
                country.catalogTiles(TMG.mapSize.x,TMG.mapSize.y);
                country.establishLegislature();
                country.establishParties();
                country.setupCandidates();
                country.cataloguePartyMembers();
                country.calcPartyPopularities();
                country.holdElection(TMG.mapSize.x, TMG.mapSize.y);
                country.eventScript = eventScript;
                yield return new WaitForEndOfFrame();

                countrySelectInfo.transform.GetChild(c).GetChild(1).GetComponent<Text>().text = country.countryName;
                GameObject panel = countrySelectInfo.transform.GetChild(c).Find("Country Info Panel/Country Info").gameObject;
                
                //Debug.Log(panel);
                TextMeshProUGUI tmpText = panel.GetComponent<TextMeshProUGUI>();
                //Debug.Log(tmpText);
                tmpText.text="Size: " + country.size;
                tmpText.text += "\nPopulation: " + country.population;
                tmpText.text += "\nConstituencies: " + country.constituencyNo;
                tmpText.text += "\nLegislature: " + ((country.Legislature == legislativeSystem.unicameral) ? "Unicameral" : "Bicameral");
                tmpText.text += "\nParties: ";

                int parties = country.PoliticalParties.Count;
                for (int p=0; p<parties;p++)
                {
                    tmpText.text += "\n" + country.PoliticalParties[p].name + " - " + country.PoliticalParties[p].GetComponent<PartyController>().leader.name;
                }

                /*
                for (int county=0; county < country.constituencies.Count; county++)
                {
                    if(county== country.constituencies.Count-1)
                    {
                        tmpText.text += country.constituencies[county].GetComponent<ConstituencyController>().getName();
                    }
                    else
                    {
                        tmpText.text += country.constituencies[county].GetComponent<ConstituencyController>().getName() + ", ";
                    }
                }
                */


                countrySelectInfo.transform.GetChild(c).Find("Country Info Panel/Country Info Scrollbar").gameObject.GetComponent<Scrollbar>().value = 1;
                yield return new WaitForEndOfFrame();
            }


            generationDone = true;
            yield return new WaitForEndOfFrame();
        }
        loading.SetActive(false);
        countrySelectionPanel.SetActive(true);
    }
}
