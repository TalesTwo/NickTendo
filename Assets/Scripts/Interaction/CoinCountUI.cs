using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCountUI : MonoBehaviour
{
    private TextMeshProUGUI coinCount;
    // Start is called before the first frame update
    void Start()
    {
        coinCount = gameObject.GetComponent<TextMeshProUGUI>();
        EventBroadcaster.PlayerStatsChanged += OnChangedStats;
        SetCoins();
    }
    private void SetCoins()
    {
        coinCount.text = PlayerStats.Instance.GetCoins().ToString();
    }

    void OnChangedStats(PlayerStatsEnum BufFType, float BuffValue)
    {
        if (BufFType == PlayerStatsEnum.Coins)
        {
            SetCoins();
        }
    }
}
