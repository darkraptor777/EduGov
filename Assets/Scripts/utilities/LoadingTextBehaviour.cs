using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingTextBehaviour : MonoBehaviour
{
    public TextMeshProUGUI LoadingText;
    int count = 0;
    bool delayed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!delayed)
        {
            StartCoroutine(textDelay());
        }
    }
    public IEnumerator textDelay()
    {
        delayed = true;
        switch (count)
        {
            case 0:
                LoadingText.text = "Loading";
                break;
            case 1:
                LoadingText.text = "Loading.";
                break;
            case 2:
                LoadingText.text = "Loading..";
                break;
            case 3:
                LoadingText.text = "Loading...";
                break;
            default:
                LoadingText.text = "Loading";
                break;
        }
        count += 1;
        if (count > 3) count = 0;
        yield return new WaitForSeconds(1.0f);
        delayed = false;
    }
}
