using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIHandler : MonoBehaviour {
    public GameObject leaderboardItemPrefab;

    SetLeaderboardInfo[] setLeaderboardInfos;

    private void Awake() {
        InitLeaderboard();
    }

    public void InitLeaderboard() {
        VerticalLayoutGroup leaderboardLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();

        CarLapCounter[] carLapCounters = FindObjectsOfType<CarLapCounter>();

        setLeaderboardInfos = new SetLeaderboardInfo[carLapCounters.Length];

        for (int i = 0; i < carLapCounters.Length; i++) {
            GameObject leaderboardInfoGameObject = Instantiate(leaderboardItemPrefab, leaderboardLayoutGroup.transform);
            setLeaderboardInfos[i] = leaderboardInfoGameObject.GetComponent<SetLeaderboardInfo>();

            setLeaderboardInfos[i].SetPositionText($"{i +1}.");
        }
    }

    public void UpdateList(List<CarLapCounter> lapCounters) {
        for (int i = 0; i < lapCounters.Count; i++) {
            setLeaderboardInfos[i].SetNameText(lapCounters[i].gameObject.name);
        }
    }
}
