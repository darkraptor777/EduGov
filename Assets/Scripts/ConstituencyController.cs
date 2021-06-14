using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toolbox;

public class ConstituencyController : MonoBehaviour
{
    int population;
    string Name;
    public Vector3Int tile;
    //constituency type;
    enum constituencyTypes { };
    constituencyTypes type;

    public GameObject politicianPrefab;
    public List<GameObject> candidates = new List<GameObject>();
    public List<float> candidatesPopularity = new List<float>();

    public GameObject representative;

    public void setTile(Vector3Int t) { tile = t; }
    public void setPop() 
    {
        population = (int)System.Math.Round((double)Random.Range(1, 10000) / 1000d, 0) * 1000; //population can range from 1,000 to 10,000,000 to the nearest thousand
    }
    public int getPop() { return population; }
    public string getName() { return Name; }

    public GameObject mostPopular()
    {
        float p = 0.0f;
        int i = 0;
        for(int x=0; x< candidatesPopularity.Count; x++)
        {
            if (candidatesPopularity[x] > p) { p = candidatesPopularity[x]; i = x; }
        }

        return candidates[i];
    }

    public void setupCandidates(GameObject parties, LegislatureController legislature)
    {
        float remaining = 1.0f;
        for(int p = 0; p < parties.transform.childCount; p++)
        {
            candidates.Add(Instantiate(politicianPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            candidates[p].transform.SetParent(parties.transform.GetChild(p));//parent to it's party
            candidates[p].GetComponent<PoliticianController>().setup(Random.Range(legislature.minAge, legislature.maxAge), (gender)Random.Range(0, 2), gameObject, legislature.Chamber);


            //favour candidates at the start of the list be eh it's a functional enough abstraction
            if (remaining > 0.0f)
            {
                float popularity = Random.Range(0.0f, remaining);
                candidatesPopularity.Add(popularity);
                remaining -= popularity;
            }
            else
            {
                candidatesPopularity.Add(0.0f);
            }
        }

        elect(legislature);
    }

    public void elect(LegislatureController legislature)
    {
        //roughly 25% world population is aged 0-14 and roughly 50% is below 30
        //voting age ranges from 14-30
        //(50%-25%=25%), (30-14=16)
        //% per year is 0.2525/16= 0.015625
        float populationAgeModifier = (0.015625f * (float)legislature.minAge) + 0.25f;

        

        //do checks for electoral type
        int winner = 0;
        switch (legislature.electionType)
        {
            case electoralSystem.FPTP:
                winner = FPTP(populationAgeModifier);
                break;
            case electoralSystem.IRV:
                winner = IRV(populationAgeModifier);
                break;
        }
        

        candidates[winner].GetComponent<PoliticianController>().isElected=true;
        representative = candidates[winner];
    }

    int FPTP(float populationAgeModifier)
    {
        List<int> votes = new List<int>();
        //calculate votes
        for (int x = 0; x < candidates.Count; x++)
        {
            candidates[x].GetComponent<PoliticianController>().isElected = false;
            votes.Add((int)((population * populationAgeModifier) * candidatesPopularity[x]));
        }

        //work out winner (this method is FPTP)
        int highest = 0;
        int winner = 0; //index of winner
        for (int x = 0; x < votes.Count; x++)
        {
            if (votes[x] > highest)
            {
                highest = votes[x];
                winner = x;
            }
        }

        return winner;
    }

    int IRV(float populationAgeModifier)
    {
        List<int> votes = new List<int>();
        int totalVotes = 0;
        //calculate votes
        for (int x = 0; x < candidates.Count; x++)
        {
            candidates[x].GetComponent<PoliticianController>().isElected = false;
            int v = (int)((population * populationAgeModifier) * candidatesPopularity[x]);
            votes.Add(v);
            totalVotes += v;
        }

        bool foundWinner = false;
        int winner = 0; //index of winner
        int threshold = (int)(totalVotes / 2); //required number of votes
        int lowest = 0; //lowest vote count
        int loser = 0; //index of candidate next up to be removed from runnings
        while (!foundWinner)
        {
            
            for (int x = 0; x < votes.Count; x++)
            {
                if (votes[x] == 0)
                {
                    continue;
                }

                else if(votes[x] >= threshold)
                {
                    winner = x;
                    foundWinner = true;
                    break;
                }

                else if (votes[x] < lowest)
                {
                    lowest = votes[x];
                    loser = x;
                }
            }

            if(!foundWinner) //if no winner, eliminate loser
            {

                int remaining = lowest;
                votes[loser] = 0;
                for (int x = 0; x < votes.Count; x++)
                {
                    if (votes[x] != 0) //if not an already eliminated candidate
                    {
                        if (remaining > 0.0f)
                        {
                            int runoff = Random.Range(0, remaining+1); //int Random.Range is not inclusive so +1 needed
                            votes[x]+=runoff;
                            remaining -= runoff;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }


        return winner;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setName()
    {
        List<string> prefix = new List<string>() { "Little", "Great", "Upper", "Lower", "Market", "Low", "Dry", "Old", "Water", "St", "New", "North", "South", "East", "West", "Good" };
        List<string> suffix = new List<string>() { "Heath", "Head", "Green", "on Sea", "Cross", "Waters", "on the Hill", "in the Willows", "Scroop", "Leap", "Glen", "End", "Grange", "Hyde", "Hatch", "Tye", "Hill", "Row" };

        List<string> start = new List<string>() { "Oul", "Ports", "North", "South", "East", "West", "Ox", "Birch", "Har", "Wool", "Otter", "White", "Lox", "Kir", "Oak", "New", "Har", "Over", "Keld", "Mars", "Bar", "Fox", "Moor", "Kin", "Chelms", "Mal", "Black", "Don", "Thorn", "Nor", "Red", "Dan", "Middle", "Guild", "Hop", "Hart", "Brain", "Lin", "Yar", "Over", "Clack", "King", "Hazel", "Lang", "Dod", "Nettle", "Rise", "Carl", "Bur", "Boult", "Ap", "Potter", "Noc", "Reep", "Buck", "Hyke", "Hadd", "Swinder", "Scar", "Au", "Bass", "Naven", "Staple", "Noc", "Well", "Lead", "Leaden", "Sut", "Gel", "Clay", "Fos", "Bel", "Sys", "Goner", "All", "Bark", "Kel", "Hey", "Sedge", "Cast", "Grant", "Barrow", "Rops", "Somer", "Harlax", "Hum", "Pon", "Sapper", "Brace", "Bur", "Cogg", "Sprox", "Sway", "Spring", "Birk", "Swin", "Salt", "San", "Gun", "Coun", "Sew", "Aun", "Greet", "Pick", "Cottes", "Essen", "Barns", "Hamble", "Ticken", "Tin", "Stam", "Stan", "Uff", "Luffen", "Witter", "Waker", "Colly", "Nass", "Blather", "Glap", "Bene", "Cotter", "Tan", "Warm", "Den", "Mor", "Folks", "Calde", "Thurn", "Copp", "Win", "Wigs", "Clop", "Clap", "Hamer", "Titch", "Broms", "Spald", "By", "Cat", "Har", "Boln", "Chad", "Perten", "Dill", "Stone", "Tose", "Beg", "Colm", "Root", "Roth", "Chaw", "Abbots", "Hard", "Eynes", "Eat", "Tet", "Ravens", "Ren", "Ickle", "Pir", "Letch", "Hex", "Pegs", "Off", "Charl", "Wymond", "Aps", "Cloth", "Man", "Bend", "Rad", "More", "Bad", "New", "Willin", "Hous", "Bore", "Boy", "Pur", "Mun", "Mount", "Brent", "Battles", "Hull", "Raw", "Rams", "Ash", "Hatters", "In", "Tilling", "Hawk", "Shot", "Ray", "Fobb", "Ply", "Ror", "Wolver", "Tel", "Bide", "Mine", "Taun", "Swan", "Yeo", "Ex", "Dor", "Here", "Wey", "Dere"};
        List<string> end = new List<string>() { "brook", "worth", "hall", "don", "wood", "wike", "beck", "marsh", "den", "mere", "field", "bern", "holm", "hampton", "mouth", "fold", "hurst", "thorpe", "by", "lea", "ington", "ford", "caster", "burn", "bury", "bry", "haven", "thwaite", "dale", "wick", "tree", "coln", "ton", "burgh", "end", "pool", "ham", "ly", "ley", "lay", "le", "hanworth", "ney", "han", "nall", "bourn", "ingore", "ston", "pole", "strop", "nook", "nell", "les", "minster", "stern", "dine", "cote", "weston", "cott", "ing", "wycke", "inghay", "stock", "sor", "wincle", "wold", "thorn", "grave", "land", "wary", "hold", "sands", "sey", "age", "ish", "wickbury", "thornbury", "lands", "gate", "gale", "dow", "nessing", "bridge", "cay", "reth", "leigh", "quay", "sea", "stowe", "chester", "hampton", "ditch", "combe", "staple", "vil", "halgh", "ness" };

        switch (Random.Range(0,10))
        {
            case 0:
                Name = prefix[Random.Range(0, prefix.Count)] + " " + start[Random.Range(0, start.Count)] + end[Random.Range(0, end.Count)];
                break;
            case 1:
                Name = start[Random.Range(0, start.Count)] + end[Random.Range(0, end.Count)] + " " + suffix[Random.Range(0, suffix.Count)];
                break;
            case 2:
                Name = start[Random.Range(0, start.Count)] + end[Random.Range(0, end.Count)] + " " + start[Random.Range(0, start.Count)] + end[Random.Range(0, end.Count)];
                break;
            default:
                Name = start[Random.Range(0, start.Count)] + end[Random.Range(0, end.Count)];
                break;
        }
        gameObject.name = Name;
    }

}
