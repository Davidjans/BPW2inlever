using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager m_ResourceManager;

    public int m_GoldCount;

    public delegate void GoldAmountChange();
    public GoldAmountChange m_GoldAmountChange;

    private void Awake()
    {
        m_ResourceManager = this;
        m_GoldCount = PlayerPrefs.GetInt("GoldPieces");

        m_GoldAmountChange += SetPlayerPref;
    }

    public void RemoveGold(int _amount)
    {
        m_GoldCount -= _amount;
        m_GoldAmountChange();
    }

    public void AddGold(int _amount)
    {
        m_GoldCount += _amount;
        m_GoldAmountChange();
    }

    private void SetPlayerPref()
    {
        PlayerPrefs.SetInt("GoldPieces", m_GoldCount);
    }
}
