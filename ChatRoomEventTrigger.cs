using UnityEngine;
using UnityEngine.EventSystems;
using uTools;

public class ChatRoomEventTrigger : EventTrigger {

    bool isSwitch;
    float tempOriginX;
    Transform __PageFrame;
    Transform pageFrame
    {
        get
        {
            if (__PageFrame == null)
            {
                __PageFrame = transform.Find("Gobj_Pages/PageFrame");
            }
            return __PageFrame;
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        float currentPageFrameX = tempOriginX + ((eventData.position.x - eventData.pressPosition.x) / 5);
        if (!isSwitch)
        {
            if (currentPageFrameX < -260)
            {
                currentPageFrameX = -260;
            }
        }
        else
        {
            if (currentPageFrameX > -30)
            {
                currentPageFrameX = -30;
            }
        }
        pageFrame.localPosition = new Vector3(currentPageFrameX, 0, 0);
        

    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        tempOriginX = pageFrame.localPosition.x;
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        SwitchPage();
    }

    void SwitchPage()
    {
        uTweenPosition tween = transform.Find("Gobj_Pages/PageFrame").GetComponent<uTweenPosition>();
        
        tween.from = pageFrame.localPosition;
        
           
        if (!isSwitch)
        {
            if (pageFrame.transform.localPosition.x < -20)
            {

                tween.to = new Vector3(-290, 0, 0);
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0, 0, 180);
                isSwitch = true;
            }
            else
            {
                tween.to = new Vector3(0, 0, 0);
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (pageFrame.transform.localPosition.x > -270)
            {
                tween.to = new Vector3(0, 0, 0);
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0, 0, 0);
                isSwitch = false;
            }      
            else
            {
                tween.to = new Vector3(-290, 0, 0);
                transform.Find("Gobj_Pages/PageFrame/UIBtn_Arrow").localEulerAngles = new Vector3(0, 0, 180);

            }
        }
        tween.duration = Mathf.Abs(tween.to.x - tween.from.x) / 1000;
        tween.ResetToBeginning();
        tween.PlayForward();

    }

}
