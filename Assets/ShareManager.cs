using UnityEngine;
using System.Collections;

public class ShareManager : MonoBehaviour {


    public static ShareManager instance;

    public string shareURL;

    void Awake()
    {
        instance = this;
    }


}
