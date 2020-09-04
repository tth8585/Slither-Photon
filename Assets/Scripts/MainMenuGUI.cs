using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnRateButtonClick()
    {
        RateManager.instance.rateGame();
    }

    public void OnShareButtonClick()
    {
        Application.OpenURL(ShareManager.instance.shareURL);
    }

    public void OnRemoveAdsButtonClick()
    {
        AdRemover.instance.BuyNonConsumable();
    }

    public void OnRestoreAdsButtonClick()
    {
        AdRemover.instance.RestorePurchases();

    }


}
