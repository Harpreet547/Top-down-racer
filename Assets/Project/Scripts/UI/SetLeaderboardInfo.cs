using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLeaderboardInfo : MonoBehaviour {
    public Text driverPosition;
    public Text driverName;

    public void SetPositionText(string newPosition) {
        driverPosition.text = newPosition;
    }
    public void SetNameText(string newDriverName) {
        driverName.text = newDriverName;
    }
}
