using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    /// <summary>
    /// значение суммы игрока по умолчанию
    /// </summary>
    private int money = 500;
    /// <summary>
    /// значение руки игрока по умолчанию
    /// </summary>
    private int hand = 0;
    /// <summary>
    /// значение ставки игрока по умолчанию
    /// </summary>
    private int betAmount = 0;
    bool flag;

    /// <summary>
    /// Метод AceCheck проверяет выпадающую карту на значение, так как туз может быть либо 11 либо 1 в зависимости от ситуации
    /// </summary>
    /// <param name="number">значение карты</param>
    public void AceCheck(int number)
    {
        if (number == 11 && hand + 11 > 21)
        {
            hand += 1;
        }

        else if (number == 11 && hand + 11 < 22)
        {
            hand += 11;
        }

        else hand += number;
    }

    /// <summary>
    /// Метод BetMoney, отвечающий за ставку игрока
    /// </summary>
    /// <param name="amount">ставка игрока</param>
    /// <returns>текущая сумма денег игрока</returns>
    public int BetMoney(int amount)
    {
        betAmount = amount; 
        return money -= amount;
    }

    /// <summary>
    /// Метод GetBetAmount возвращает ставку игрока
    /// </summary>
    /// <returns>ставка игрока</returns>
    public int GetBetAmount()
    {
        return betAmount;
    }

    /// <summary>
    /// Метод SetCheckPay изменяет состояние оплаты игрока. Если человек оплатил, то flag = true
    /// </summary>
    /// <param name="flag">Значение оплаты bool</param>
    /// <returns>Возвращает изменение состояния оплаты</returns>
    public bool SetCheckPay(bool flag)
    {
        return this.flag = flag;
    }

    /// <summary>
    /// Метод GetCheckPay возвращает состояние оплаты игрока
    /// </summary>
    /// <returns>Возвращает bool значение состояние оплаты игрока</returns>
    public bool GetCheckPay()
    {
        return flag;
    }

    /// <summary>
    /// Метод GetHand возвращает текущую руку игрока
    /// </summary>
    /// <returns>рука игрока</returns>
    public int GetHand()
    {
        return hand;
    }

    /// <summary>
    /// Обнуляет руку игрока
    /// </summary>
    /// <param name="newhand">Значение руки</param>
    /// <returns>значение руки игрока</returns>
    public int SetHand(int newhand)
    {
        return hand = newhand;
    }

    /// <summary>
    /// Метод GetMoney возвращает сумму денег игрока
    /// </summary>
    /// <returns>значение суммы денег игрока</returns>
    public int GetMoney()
    {
        return money;
    }

    /// <summary>
    /// Обновляет сумму денег игрока
    /// </summary>
    /// <param name="pay">платеж, который выиграл игрок</param>
    /// <returns>обновленное значение суммы денег игрока</returns>
    public int SetMoney(int pay)
    {
        return money += pay;
    }
}