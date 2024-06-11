using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerController _player;
    public TMP_Text money;
    //private int Price(int level) => 10 + 5 * (level - 1);

    private void Start()
    {
        

        _player = PlayerController.Instance;
        money.text = _player.Statistics.Money.ToString();
        UIController.Instance._healthButtonText.text = $"{Price(_player.Statistics.StartUpdateHealth, _player.Statistics.HealthLevel)} coins";
        UIController.Instance._speedButtonText.text = $"{Price(_player.Statistics.StartUpdateSpeed, _player.Statistics.SpeedLevel)} coins";
        UIController.Instance._damageButtonText.text = $"{Price(_player.Statistics.StartUpdateDamage, _player.Statistics.DamageLevel)} coins";
        UIController.Instance._atkSpeedButtonText.text = $"{Price(_player.Statistics.StartUpdateAtkSpeed, _player.Statistics.AtkSpeedLevel)} coins";
        UIController.Instance._moneyButtonText.text = $"{Price(_player.Statistics.StartUpdateMoney, _player.Statistics.MoneyLevel)} coins";


    }

    public int Price(int minCoin, int level)
    {
        return minCoin + (5 * (level - 1));
    }

    public void BuyUpgradeHealth()
    {
        if (_player.Statistics.Money < Price(_player.Statistics.StartUpdateHealth, _player.Statistics.HealthLevel)) return;

        _player.Statistics.Money -= Price(_player.Statistics.StartUpdateHealth, _player.Statistics.HealthLevel);
        _player.Statistics.UpgradeHealth();

        UIController.Instance.UpdateCoinText(_player.Statistics.Money);
        UIController.Instance.ChangeHealthStat(Price(_player.Statistics.StartUpdateHealth, _player.Statistics.HealthLevel), _player.Statistics.MaxHealth);
        money.text = _player.Statistics.Money.ToString();
    }

    public void BuyUpgradeDamage()
    {
        if (_player.Statistics.Money < Price(_player.Statistics.StartUpdateDamage,_player.Statistics.DamageLevel)) return;

        _player.Statistics.Money -= Price(_player.Statistics.StartUpdateDamage, _player.Statistics.DamageLevel);
        _player.Statistics.UpgradeDamage();

        UIController.Instance.UpdateCoinText(_player.Statistics.Money);
        UIController.Instance.ChangeDamageStat(Price(_player.Statistics.StartUpdateDamage, _player.Statistics.DamageLevel), _player.Statistics.AdditionalDamage);
        money.text = _player.Statistics.Money.ToString();
    }

    public void BuyUpgradeSpeed()
    {
        if (_player.Statistics.Money < Price(_player.Statistics.StartUpdateSpeed,_player.Statistics.SpeedLevel)) return;

        _player.Statistics.Money -= Price(_player.Statistics.StartUpdateSpeed,_player.Statistics.SpeedLevel);
        _player.Statistics.UpgradeSpeed();

        UIController.Instance.UpdateCoinText(_player.Statistics.Money);
        UIController.Instance.ChangeSpeedStat(Price(_player.Statistics.StartUpdateSpeed, _player.Statistics.SpeedLevel), _player.Statistics.Speed);
        money.text = _player.Statistics.Money.ToString();
    }
    public void BuyUpgradeAtkSpeed()
    {
        if (_player.Statistics.Money < Price(_player.Statistics.StartUpdateAtkSpeed,_player.Statistics.AtkSpeedLevel)) return;

        _player.Statistics.Money -= Price(_player.Statistics.StartUpdateAtkSpeed, _player.Statistics.AtkSpeedLevel);
        _player.Statistics.UpgradeAttackSpeed();

        UIController.Instance.UpdateCoinText(_player.Statistics.Money);
        UIController.Instance.ChangeAtkSpeedStat(Price(_player.Statistics.StartUpdateAtkSpeed, _player.Statistics.AtkSpeedLevel), _player.Statistics.AttackDelay);
        money.text = _player.Statistics.Money.ToString();
    }

    public void BuyUpgradeMoney()
    {
        if (_player.Statistics.Money < Price(_player.Statistics.StartUpdateMoney,_player.Statistics.MoneyLevel)) return;

        _player.Statistics.Money -= Price(_player.Statistics.StartUpdateMoney,_player.Statistics.MoneyLevel);
        _player.Statistics.UpgradeMoneyMultiplier();

        UIController.Instance.UpdateCoinText(_player.Statistics.Money);
        UIController.Instance.ChangeMoneyStat(Price(_player.Statistics.StartUpdateMoney, _player.Statistics.MoneyLevel), _player.Statistics.MoneyMultipler);
        money.text = _player.Statistics.Money.ToString();
    }

    
}
