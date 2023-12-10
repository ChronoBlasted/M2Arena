using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEditor.Progress;

public enum Currency { coins, gems, trophies, hard }

public class NakamaUserAccount : MonoBehaviour
{
    IClient _client;
    ISession _session;

    Dictionary<string, int> _lastWalletData;

    public Dictionary<string, int> LastWalletData { get => _lastWalletData; set => _lastWalletData = value; }

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await GetPlayerData();
        await GetWalletData();
    }

    async Task GetPlayerData()
    {
        var account = await _client.GetAccountAsync(_session);
        var username = account.User.Username;
        var avatarUrl = account.User.AvatarUrl;
        var userId = account.User.Id;
    }

    public async Task GetWalletData()
    {
        var account = await _client.GetAccountAsync(_session);

        LastWalletData = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);

        foreach (var currency in LastWalletData.Keys)
        {
            if (currency == Currency.coins.ToString())
            {
                //.Instance.MenuView.TopBar.UpdateCoin(LastWalletData[currency]);
            }
            if (currency == Currency.gems.ToString())
            {
                //UIManager.Instance.MenuView.TopBar.UpdateGem(LastWalletData[currency]);
            }
            if (currency == Currency.trophies.ToString())
            {
                //UIManager.Instance.MenuView.TopBar.UpdateTrophy(LastWalletData[currency]);
            }
        }
    }
}