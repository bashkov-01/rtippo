using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class GameScript : MonoBehaviour
{
    /// <summary>
    /// Cообщение, которое отображает текущую руку и текущий счет денег игроков.
    /// </summary>
    public TMP_Text[] playersText;
    /// <summary>
    /// Отображение имен игроков (по умолчанию игрок 1 и так далее).
    /// </summary>
    public TMP_Text[] playersNameText;
    /// <summary>
    /// Кнопка для добора карты.
    /// </summary>
    public Button hitButton;
    /// <summary>
    /// Кнопка для раздачи карт.
    /// </summary>
    public Button dealButton;
    /// <summary>
    /// Кнопка для пропуска хода.
    /// </summary>
    public Button standButton;
    /// <summary>
    /// Кнопка с определенной ставкой.
    /// </summary>
    public Button betFirstButton;
    /// <summary>
    /// Кнопка с определенной ставкой.
    /// </summary>
    public Button betSecondButton;
    /// <summary>
    /// Кнопка с определенной ставкой.
    /// </summary>
    public Button betThirdButton;
    /// <summary>
    /// Кнопка для подсчета очков игроков и оплаты выигрыша.
    /// </summary>
    public Button calculateButton;
    /// <summary>
    /// Кнопка для удвоения ставки игрока.
    /// </summary>
    public Button doubleButton;
    /// <summary>
    /// Кнопка для начала следующего раунда.
    /// </summary>
    public Button nextRoundButton;
    /// <summary>
    /// Кнопка для сдачи карт игрока.
    /// </summary>
    public Button surrenderButton;
    /// <summary>
    /// Объект класса Deck. В нем хранятся массивы картинок каждой карты и значения каждой карты.
    /// </summary>
    public DeckScript deck = new DeckScript();
    /// <summary>
    /// Массив картинок карт игроков. В данный массив значений происходит присвоение конкретной рандомной карты.
    /// </summary>
    public Image[] playerCardImages;
    /// <summary>
    /// Массив игроков. Данный класс содержит много методов, которые используются для правильных ходов в игре.
    /// Можно посмотреть описание каждого метода в классе PlayerScript.
    /// </summary>
    public PlayerScript[] players;
    /// <summary>
    /// Словарь, в коротом первый элемент - индекс игрока, второй элемент - список индексов карт которые уже разданы.
    /// При доборе карт происходит проверка текущего индекса игрока и индекса последней разданной карты и новая карта становится на lastindex + 1. 
    /// </summary>
    private Dictionary<int, List<int>> valuesCards;
    /// <summary>
    /// Изображение скрытой карты дилера.
    /// </summary>
    private Sprite hideCard = null;
    public Image imageHideCard;
    /// <summary>
    /// Значение скрытой карты дилера.
    /// </summary>
    private int openCard = 0;
    /// <summary>
    /// Индекс дилера. То есть это последний индекс в массиве PlayerScript. Player.Length - 1.
    /// </summary>
    public int dilerIndex;
    /// <summary>
    /// Индекс текущего игрока
    /// </summary>
    public int currentPlayerIndex = 0;


    /// <summary>
    /// <b>При вызове метода Surrender игрок сдается от дальнейшей игры. Возвращается 1/2 его ставки.</b>
    /// </summary>
    private void Surrender()
    {
        players[currentPlayerIndex].SetCheckPay(true);
        int pay = players[currentPlayerIndex].GetBetAmount() / 2;
        players[currentPlayerIndex].SetMoney(pay);
        playersText[currentPlayerIndex].text = $"Игрок {currentPlayerIndex + 1} сдался. Сумма: {players[currentPlayerIndex].GetMoney()}";
        currentPlayerIndex += 1;
    }

    /// <summary>
    /// <b>В методе Update меняем цвет имени текущего игрока для того, чтобы понимать кто текущий игрок и кто должен делать ход.</b>
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < playersNameText.Length; i++)
        {
            playersNameText[i].color = i == currentPlayerIndex ? Color.gray : Color.white;
        }
        if(currentPlayerIndex == dilerIndex)
        {
            VisibleButton(doubleButton, false);
            VisibleButton(surrenderButton, false);
        }
    }

    /// <summary>
    /// <b>В методе Start происходят начальные действия для начала игры.</b><br></br>
    /// (получаем количество игроков, передаем в массив players, кнопки деактивируем, присваиваем имена игрокам)
    /// </summary>
    public void Start()
    {
        int selectedOption = DropdownScript.SelectedOption;
        selectedOption += 1;
        players = new PlayerScript[selectedOption];
        valuesCards = new Dictionary<int, List<int>>();
        dilerIndex = players.Length - 1;

        for (int i = 0; i < players.Length; i++)
        {
            playersText[i].text = "";
            players[i] = new PlayerScript();
            if (i == dilerIndex)
                playersNameText[i].text = "Дилер";
            else
                playersNameText[i].text = $"Игрок {i + 1}";
        }

        for (int i = 0; i < playerCardImages.Length; i++)
            playerCardImages[i].gameObject.SetActive(false);

        VisibleButton(hitButton, false);
        VisibleButton(standButton, false);
        VisibleButton(dealButton, false);
        VisibleButton(calculateButton, false);
        VisibleButton(nextRoundButton, false);
        VisibleButton(doubleButton, false);
        VisibleButton(surrenderButton, false);

        dealButton.onClick.AddListener(() => DealCards());
        hitButton.onClick.AddListener(() => HitCards());
        standButton.onClick.AddListener(() => StandCards());
        betFirstButton.onClick.AddListener(() => Bet(20));
        betSecondButton.onClick.AddListener(() => Bet(50));
        betThirdButton.onClick.AddListener(() => Bet(100));
        calculateButton.onClick.AddListener(() => CalculatePoints());
        nextRoundButton.onClick.AddListener(() => NextRound());
        doubleButton.onClick.AddListener(() => DoubleBet());
        surrenderButton.onClick.AddListener(() => Surrender());
        //playerCardImages[dilerIndex * 6].sprite = playerCardImages[dilerIndex * 6].sprite;
        Debug.Log("Длина массива " + players.Length + "\nOption " + selectedOption + "\nТекущий игрок " + currentPlayerIndex + "\nLast Index" + dilerIndex);
    }

    /// <summary>
    /// <b>В методе DoubleBet происходит удвоение ставки игрока.</b>
    /// </summary>
    private void DoubleBet()
    {
        int bet = players[currentPlayerIndex].GetBetAmount();
        players[currentPlayerIndex].BetMoney(bet);
        int index = currentPlayerIndex;
        HitCards();
        if (index == currentPlayerIndex) currentPlayerIndex += 1;
    }

    /// <summary>
    /// <b>В методе StandCards происходит пропуск хода игрока.</b>
    /// </summary>
    private void StandCards()
    {
        currentPlayerIndex += 1;
        VisibleButton(surrenderButton, true);
        VisibleButton(doubleButton, true);
        if (currentPlayerIndex == players.Length)
        {
            VisibleButton(calculateButton, true);
            VisibleButton(standButton, false);
            VisibleButton(hitButton, false);
        }

        else if (currentPlayerIndex == dilerIndex)
        {
            playerCardImages[currentPlayerIndex * 6].sprite = hideCard;
            PrintCurrentText(currentPlayerIndex);
        }
    }

    /// <summary>
    /// <b>В методе NextRound происходит начало нового раунда.</b>
    /// </summary>
    private void NextRound()
    {
        currentPlayerIndex = 0;
        foreach (var entry in valuesCards)
        {
            foreach (int cardIndex in entry.Value)
            {
                playerCardImages[cardIndex].gameObject.SetActive(false);
            }
            players[entry.Key].SetHand(0);
        }
        valuesCards.Clear();
        for (int i = 0; i < players.Length; i++) players[i].SetCheckPay(false);
        for (int i = 0; i < players.Length; i++) playersText[i].ClearMesh();
        hideCard = null;
        playerCardImages[dilerIndex * 6].sprite = imageHideCard.sprite;

        VisibleButton(betFirstButton, true);
        VisibleButton(betSecondButton, true);
        VisibleButton(betThirdButton, true);
        VisibleButton(calculateButton, false);
        VisibleButton(nextRoundButton, false);
    }

    /// <summary>
    /// <b>В методе DealCards происходит раздача двух карт каждому игроку.</b>
    /// </summary>
    private void DealCards()
    {
        currentPlayerIndex = 0;
        for (int i = 0; i < players.Length; i++)
        {
            List<int> cardIndices = new List<int>();
            for (int j = 0; j < 2; j++)
            {
                int randomIndex = deck.GetUniqueCard();
                if (i == dilerIndex && j == 0)
                {
                    playerCardImages[i * 6 + j].gameObject.SetActive(true);
                    hideCard = deck.Cards[randomIndex];
                }
                else
                {
                    playerCardImages[i * 6 + j].gameObject.SetActive(true);
                    playerCardImages[i * 6 + j].sprite = deck.Cards[randomIndex];
                    openCard = deck.ValuesOfCards[randomIndex];
                }

                players[i].AceCheck(deck.ValuesOfCards[randomIndex]);
                cardIndices.Add(i * 6 + j);
            }
            if (!valuesCards.ContainsKey(i))
                valuesCards.Add(i, cardIndices);
            else
                valuesCards[i] = cardIndices;
            PrintCurrentText(i);
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetHand() != 21)
            {
                currentPlayerIndex = i;
                break;
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            CalculatePointsBlackJack(i);
        }

        if (openCard != 11) VisibleButton(surrenderButton, true);

        VisibleButton(dealButton, false);
        VisibleButton(hitButton, true);
        VisibleButton(standButton, true);
        VisibleButton(surrenderButton, true);

        if (currentPlayerIndex != dilerIndex)
        {
            VisibleButton(doubleButton, true);
        }
    }

    /// <summary>
    /// <b>В методе PrintCurrentText происходит вывод текущей информаиции об игроке.</b>
    /// </summary>
    /// <param name="index">Текущий игрок</param>
    /// <returns>Сообщение о текущем состоянии игрока</returns>
    private string PrintCurrentText(int index)
    {
        if (index != dilerIndex)
            return playersText[index].text = $"Игрок {index + 1}: {players[index].GetHand()}\nСумма {players[index].GetMoney()}\n";
        else
        {
            if (playerCardImages[index * 6].sprite == hideCard) return playersText[index].text = $"Дилер. {players[index].GetHand()}\n";
            else return playersText[index].text = "";
        }
    }


    /// <summary>
    /// <b>В методе VisibleButton происходит активация и деактивация кнопок.</b>
    /// </summary>
    /// <param name="button">Название кнопки</param>
    /// <param name="flag">Состояние кнопки. true - активная, false - неактивная.</param>
    private void VisibleButton(Button button, bool flag)
    {
        button.gameObject.SetActive(flag);
    }

    /// <summary>
    /// <b>В методе CalculatePointsBlackJack происходит проверка на блекджек и выплата выигрыша игроку в случае блекджека</b>
    /// </summary>
    /// <param name="index">Индекс текущего игрока</param>
    private void CalculatePointsBlackJack(int index)
    {
        if (index != dilerIndex)
        {
            if (players[index].GetHand() == 21 && openCard < 10)
            {
                players[index].SetCheckPay(true);
                int pay = players[index].GetBetAmount() * 3 / 2 + players[index].GetBetAmount();
                players[index].SetMoney(pay);
                PrintCurrentText(index);
            }
        }
    }

    /// <summary>
    /// <b>В методе CalculatePoints происходит подсчет очков всех игроков и определение победителей.</b>
    /// </summary>
    private void CalculatePoints()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (i != dilerIndex)
            {
                if (!players[i].GetCheckPay() == true)
                {
                    if (players[i].GetHand() > 21 || (players[i].GetHand() < players[dilerIndex].GetHand() && players[dilerIndex].GetHand() < 22))
                    {
                        players[i].SetCheckPay(true);
                        playersText[i].text = $"Игрок {i + 1} - проиграл. Рука: {players[i].GetHand()}. Сумма: {players[i].GetMoney()}. ";
                    }
                    else if ((players[i].GetHand() > players[dilerIndex].GetHand() && players[dilerIndex].GetHand() < 22) || (players[dilerIndex].GetHand() > 21 && players[i].GetHand() < 22))
                    {
                        int pay = players[i].GetBetAmount() + players[i].GetBetAmount();
                        players[i].SetMoney(pay);
                        players[i].SetCheckPay(true);
                        playersText[i].text = $"Игрок {i + 1} - выиграл. Рука: {players[i].GetHand()}. Сумма: {players[i].GetMoney()}. ";
                    }
                    else if (players[i].GetHand() == players[dilerIndex].GetHand())
                    {
                        int pay = players[i].GetBetAmount();
                        players[i].SetMoney(pay);
                        players[i].SetCheckPay(true);
                        playersText[i].text = $"Игрок {i + 1} - сыграл в ничью. Рука: {players[i].GetHand()}. Сумма: {players[i].GetMoney()}. ";
                    }
                }
            }
            //else if(i == dilerIndex) playersText[i].text = $"Дилер. Рука: {players[i].GetHand()}. ";
        }
        VisibleButton(nextRoundButton, true);
        VisibleButton(calculateButton, false);
        VisibleButton(doubleButton, false);
    }

    /// <summary>
    /// <b>В методе HitCards происходит добор карты текущего игрока.</b>
    /// </summary>
    private void HitCards()
    {
        VisibleButton(doubleButton, false);
        int randomIndex = deck.GetUniqueCard();
        int lastCardIndex = valuesCards[currentPlayerIndex][valuesCards[currentPlayerIndex].Count - 1];
        valuesCards[currentPlayerIndex].Add(lastCardIndex + 1);
        playerCardImages[lastCardIndex + 1].gameObject.SetActive(true);
        playerCardImages[lastCardIndex + 1].sprite = deck.Cards[randomIndex];
        players[currentPlayerIndex].AceCheck(deck.ValuesOfCards[randomIndex]);
        PrintCurrentText(currentPlayerIndex);
        if (players[currentPlayerIndex].GetHand() >= 21)
        {
            if (currentPlayerIndex != dilerIndex)
            {
                currentPlayerIndex += 1;
                VisibleButton(surrenderButton, true);
                VisibleButton(doubleButton, true);
                VisibleButton(doubleButton, true);
                if (currentPlayerIndex == dilerIndex)
                {
                    playerCardImages[currentPlayerIndex * 6].sprite = hideCard;
                    PrintCurrentText(currentPlayerIndex);
                }
            }

            else if (currentPlayerIndex == dilerIndex)
            {
                VisibleButton(calculateButton, true);
                VisibleButton(hitButton, false);
                VisibleButton(standButton, false);
                VisibleButton(surrenderButton, false);
            }
        }
    }

    /// <summary>
    /// <b>В методе Bet происходит ставка игрока.</b>
    /// </summary>
    /// <param name="bet">Ставка</param>
    public void Bet(int bet)
    {
        players[currentPlayerIndex].BetMoney(bet);
        PrintCurrentText(currentPlayerIndex);
        currentPlayerIndex += 1;
        if (currentPlayerIndex == dilerIndex)
        {
            VisibleButton(dealButton, true);
            VisibleButton(betFirstButton, false);
            VisibleButton(betSecondButton, false);
            VisibleButton(betThirdButton, false);
        }
    }
}