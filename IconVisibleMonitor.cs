using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconVisibleMonitor : MonoBehaviour {

    Camera mainCamera;
    RectTransform rectTransform;
    public Image icon;
    public int iconIndex;
    public string fbId;
    bool needInit;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        if (rectTransform.IsVisibleFrom(mainCamera))
        {
            if (needInit)
            {
                needInit = false;
                Kfc.Generic.IconFetcher.SetIcon(icon, iconIndex, fbId);
            }

            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
	}

    public void SetFbIdAndIconIndex(int _iconIndex,string _fbId)
    {
        iconIndex = _iconIndex;
        fbId = _fbId;
        needInit = true;
    }
}
