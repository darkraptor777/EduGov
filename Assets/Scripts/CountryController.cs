using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using toolbox;

public class CountryController : MonoBehaviour
{
    public string countryName;
    public int ID; //country's number ID
    public int size; //number of land hexes the country controls
    public int population; //number of citizens in the country
    public int votingAge; //minimum voting age 

    public Tilemap map; //border map of country
    public GameObject constituencyPrefab;
    public List<GameObject> constituencies;
    public int constituencyNo;
    Tilemap terrainMap;

    
    public executiveSystem Executive;
    public GameObject HeadOfState;

    
    public legislativeSystem Legislature;
    public List<GameObject> LegislativeChambers;
    public GameObject LegislativeChamberPrefab;

    
    public List<GameObject> PoliticalParties;
    public List<GameObject> BannedPoliticalParties;
    public GameObject partyPrefab;

    public EventController eventScript;

    public void setID(int id) { ID = id; }

    public bool isPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        constituencies = new List<GameObject>();
        isPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reviewProposal(string type, string prop)//the type of proposal and the specifics of the proposal
    {
        for (int x=0; x<PoliticalParties.Count;x++)
        {
            PoliticalParties[x].GetComponent<PartyController>().decidePolicyPosition(type, prop);
        }

        bool A = LegislativeChambers[0].GetComponent<LegislatureController>().reviewPoposal();
        bool B = true;

        if (Legislature == legislativeSystem.bicameral)
        {
            B = LegislativeChambers[1].GetComponent<LegislatureController>().reviewPoposal();
        }

        if ((Legislature == legislativeSystem.bicameral && A && B) || (Legislature == legislativeSystem.unicameral && A)) //if proposal was passed by all legislatures
        {
            enactProposal(type, prop);
        }
        else proposalRejected();
    }

    public void enactProposal(string type, string prop)
    {
        switch(type)
        {
            case "Electoral Method":
                switch(prop)
                {
                    case "First Past the Post":
                        foreach (GameObject L in LegislativeChambers)
                        {
                            L.GetComponent<LegislatureController>().electionType = electoralSystem.FPTP;
                        }
                        break;
                    case "Instant-runoff Voting":
                        foreach (GameObject L in LegislativeChambers)
                        {
                            L.GetComponent<LegislatureController>().electionType = electoralSystem.IRV;
                        }
                        break;
                }
                break;

            case "Voting Age":
                votingAge = int.Parse(prop);
                foreach (GameObject L in LegislativeChambers)
                {
                    L.GetComponent<LegislatureController>().minAge = votingAge;
                }
                break;

            case "Legislative System":
                switch (prop)
                {
                    case "Unicameral":
                        Legislature = legislativeSystem.unicameral;
                        LegislativeChambers.RemoveAt(1);
                        break;

                    case "Bicameral":
                        Legislature = legislativeSystem.bicameral;

                        LegislativeChambers.Add(Instantiate(LegislativeChamberPrefab, new Vector3(0, 0, 0), Quaternion.identity));
                        LegislativeChambers[1].transform.SetParent(gameObject.transform.GetChild(1));
                        LegislativeChambers[1].GetComponent<LegislatureController>().setup(chamber.Upper, constituencyNo, LegislativeChambers[0].GetComponent<LegislatureController>().electionTime);
                        LegislativeChambers[1].gameObject.name = "Upper Chamber";

                        int Count = constituencies.Count;
                        for (int x = 0; x < Count; x++)
                        {
                            constituencies[x].GetComponent<ConstituencyController>().setupCandidates(gameObject.transform.GetChild(2).gameObject, LegislativeChambers[1].GetComponent<LegislatureController>());
                        }

                        break;
                }
                break;

            case "Term Duration":
                foreach (GameObject L in LegislativeChambers)
                {
                    L.GetComponent<LegislatureController>().electionTime = int.Parse(prop);
                }
                break;

            case "Representation":
                switch (prop)
                {
                    case "Delegates":
                        foreach (GameObject L in LegislativeChambers)
                        {
                            L.GetComponent<LegislatureController>().Representative = representativeSystem.Delegate;
                        }
                        break;

                    case "Trustees":
                        foreach (GameObject L in LegislativeChambers)
                        {
                            L.GetComponent<LegislatureController>().Representative = representativeSystem.Trustee;
                        }
                        break;
                }
                break;

            case "Outlaw Party":
                GameObject oP = grabParty(prop);
                PoliticalParties.Remove(oP);
                BannedPoliticalParties.Add(oP);
                break;

            case "Legalise Party":
                GameObject lP = grabBannedParty(prop);
                BannedPoliticalParties.Remove(lP);
                PoliticalParties.Add(lP);
                break;

            default:
                Debug.Log("Unknown Proposal: " + type + ", " + prop);
                break;
        }

        eventScript.proposalEnacted();
    }

    
    public void proposalRejected()
    {
        eventScript.proposalRejected();
    }
    

    public void playerSelectCountry()
    {
        isPlayer = true;
    }

    public PartyController leadingParty()
    {
        GameObject p = null;
        float pP = 0.0f;
        for (int x = 0; x < PoliticalParties.Count; x++)
        {
            float xP = PoliticalParties[x].GetComponent<PartyController>().popularity;
            if ( xP > pP) { p = PoliticalParties[x]; pP = xP; }
        }

        return p.GetComponent<PartyController>();
    }

    public GameObject grabParty(string name)
    {
        for (int x = 0; x < PoliticalParties.Count; x++)
        {
            if (PoliticalParties[x].name == name) return PoliticalParties[x];
        }

        return null;
    }

    public GameObject grabBannedParty(string name)
    {
        for (int x = 0; x < BannedPoliticalParties.Count; x++)
        {
            if (BannedPoliticalParties[x].name == name) return BannedPoliticalParties[x];
        }

        return null;
    }

    public GameObject grabRandomPolitician()
    {
        LegislatureController l = LegislativeChambers[Random.Range(0, LegislativeChambers.Count)].GetComponent<LegislatureController>();
        return l.members[Random.Range(0, l.members.Count)];
    }

    public GameObject grabRandomParty()
    {
        return PoliticalParties[Random.Range(0, PoliticalParties.Count)];
    }

    public void selectName()
    {
        List<string> names = new List<string>() { "Ijallikstan", "Kreistad", "Swasia", "Bara", "Vulland", "Rima", "Salkong", "Sauiu", "Lumac", "Cosia", "Djibra", "Macoin", "Pato", "Cassica", "Machai", "Vakitts", "Saintre", "Emarkri", "Jibapit", "Ecobul", "Strani", "Fricege", "Fimati", "Rustra", "Reuland", "Tiatopa", "Guatenu", "Kingsi", "Giaria", "Gatu", "Galsriji", "Navabe", "Hamo", "Laloas", "Engstine", "Neaco", "Catiland", "Virle", "Reuvis", "Malconea", "Moako", "Vorates", "Denneso", "Gonorth", "Waithon", "Atherni", "Indastabul", "Tarkyr", "Tuland" };

        List<string> prefix = new List<string>() { "Kingdom of", "United Provinces of", "United States of", "Empire of", "Republic of", "Democratic Republic of", "Sultanate of" };


        string N = Random.Range(0,5) == 0 ? prefix[Random.Range(0, prefix.Count)] + " " + names[Random.Range(0, names.Count)] : names[Random.Range(0, names.Count)];

        countryName = N;
        gameObject.name = countryName;
    }

    public void establishLegislature()
    {
        int termT = Random.Range(1, 11);
        if (Legislature==legislativeSystem.unicameral) //if there's only one chamber
        {
            
            LegislativeChambers.Add(Instantiate(LegislativeChamberPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            LegislativeChambers[0].transform.SetParent(gameObject.transform.GetChild(1));
            LegislativeChambers[0].GetComponent<LegislatureController>().setup(chamber.Unified, constituencyNo, termT);
            LegislativeChambers[0].gameObject.name = "Unified Chamber";
        }
        else //if there's two chambers
        {
            LegislativeChambers.Add(Instantiate(LegislativeChamberPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            LegislativeChambers[0].transform.SetParent(gameObject.transform.GetChild(1));
            LegislativeChambers[0].GetComponent<LegislatureController>().setup(chamber.Lower, constituencyNo, termT);
            LegislativeChambers[0].gameObject.name = "Lower Chamber";

            LegislativeChambers.Add(Instantiate(LegislativeChamberPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            LegislativeChambers[1].transform.SetParent(gameObject.transform.GetChild(1));
            LegislativeChambers[1].GetComponent<LegislatureController>().setup(chamber.Upper, constituencyNo, termT);
            LegislativeChambers[1].gameObject.name = "Upper Chamber";
        }
    }

    public void establishParties()
    {
        string[] values = System.Enum.GetNames(typeof(platform));
        int count = values.Length;
        List<int> shuffled = new List<int>();
        for (int x = 0; x < count; x++)
        {
            shuffled.Add(x);
        }

        for (int x = 0; x < count; x++) //shuffle order of parties
        {
            int temp = shuffled[x];
            int r = Random.Range(0, count - 1);
            shuffled[x] = shuffled[r];
            shuffled[r] = temp;
        }

        for (int x = 0; x<count; x++)
        {
            PoliticalParties.Add(Instantiate(partyPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            PoliticalParties[x].transform.SetParent(gameObject.transform.GetChild(2));
            PoliticalParties[x].GetComponent<PartyController>().setup((platform)shuffled[x]);
        }
    }

    public void cataloguePartyMembers()
    {
        int count = PoliticalParties.Count;
        for (int x = 0; x < count; x++)
        {
            PoliticalParties[x].GetComponent<PartyController>().catalogueMembers();
        }
    }

    public void setupCandidates()
    {
        int Count = constituencies.Count;
        int LegisCount = LegislativeChambers.Count;
        for (int x = 0; x < Count; x++)
        {
            for (int y = 0; y < LegisCount; y++)
            {
                constituencies[x].GetComponent<ConstituencyController>().setupCandidates(gameObject.transform.GetChild(2).gameObject, LegislativeChambers[y].GetComponent<LegislatureController>());
            }
        }
    }

    public void calcPartyPopularities()
    {
        int count = PoliticalParties.Count;
        for (int x = 0; x < count; x++)
        {
            PoliticalParties[x].GetComponent<PartyController>().calcPopularity();
        }
    }

    public void holdElection(int X, int Y)
    {
        int LegisCount = LegislativeChambers.Count;
        terrainMap = GameObject.FindWithTag("Terrain").GetComponent<Tilemap>();
        int count = 0;
        for (int x = 0; x < X; x++)
        {
            for (int y = 0; y < Y; y++)
            {
                Vector3Int location = new Vector3Int(-x + X / 2, -y + Y / 2, 0);
                Tile tile = (Tile)map.GetTile(location);
                Tile tileT = (Tile)terrainMap.GetTile(location);
                if (tile != null)
                {
                    //size += 1;
                    if (tileT != null) //there are no constituencies in the sea
                    {
                        count += 1;

                        for (int l = 0; l < LegisCount; l++)
                        {
                            constituencies[count - 1].GetComponent<ConstituencyController>().elect(LegislativeChambers[l].GetComponent<LegislatureController>());
                        }

                        //constituencies[count - 1].GetComponent<ConstituencyController>().elect();
                    }
                }
            }
        }

        for (int l = 0; l < LegisCount; l++)
        {
            LegislativeChambers[l].GetComponent<LegislatureController>().updateSeats(PoliticalParties);
        }
    }

    public void catalogTiles(int X, int Y)
    {
        //int X = map.size.x;
        //int Y = map.size.y;
        //int c = 0;
        terrainMap = GameObject.FindWithTag("Terrain").GetComponent<Tilemap>();
        for(int x=0; x<X; x++)
        {
            for (int y = 0; y < Y; y++)
            {
                Vector3Int location = new Vector3Int(-x + X / 2, -y + Y / 2, 0);
                Tile tile = (Tile)map.GetTile(location);
                Tile tileT = (Tile)terrainMap.GetTile(location);
                if (tile != null)
                {
                    size += 1;
                    if (tileT != null) //there are no constituencies in the sea
                    {
                        constituencyNo += 1;
                        constituencies.Add(Instantiate(constituencyPrefab, new Vector3(0, 0, 0), Quaternion.identity));
                        constituencies[constituencyNo - 1].GetComponent<ConstituencyController>().setTile(location);
                        constituencies[constituencyNo - 1].GetComponent<ConstituencyController>().setName();
                        constituencies[constituencyNo - 1].GetComponent<ConstituencyController>().setPop();
                        population += constituencies[constituencyNo - 1].GetComponent<ConstituencyController>().getPop();
                        constituencies[constituencyNo - 1].transform.SetParent(gameObject.transform.GetChild(0));
                        //tiles.Add(location);
                    }
                }
            }
        }
    }
}
