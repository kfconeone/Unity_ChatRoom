using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kfc.ChatRoom
{
    public class PlayerBean : MonoBehaviour
    {

        public string account;
        public int vipLevel;
        public string nickName;
        public string saySomething;
        public int iconIndex;
        public string fbId;

        public Text txt_nickName;
        public Text txt_saySomething;

        public void Init(string _account, string _nickName, string _fbId, int _iconIndex, int _vipLevel, string _saySomething)
        {
            account = _account;
            nickName = _nickName;
            fbId = _fbId;
            iconIndex = _iconIndex;
            vipLevel = _vipLevel;
            saySomething = _saySomething;
        }

        public void Cat(string _account, string _nickName, string _fbId, int _iconIndex, int _vipLevel, string _saySomething)
        {
            Debug.Log("zzz");
            Debug.Log(_account);
        }
    }
}
