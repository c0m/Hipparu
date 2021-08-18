using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hipparu.Shared;
using Microsoft.AspNetCore.Components;
using Plk.Blazor.DragDrop;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Diagnostics;

namespace Hipparu.Pages
{
    public partial class Index : ComponentBase
    {
        enum GameState
        {
            MainMenu,
            HiraganaMode,
            KatakanaMode,
            MixedMode
        }
        private GameState gameState = GameState.MainMenu;

        enum TextState
        {
            HiraganaMode,
            KatakanaMode
        }
        private TextState textState = TextState.HiraganaMode;

        // Styles for the game and menu divs to decide visibilty
        private string menuVisibility = "visible";
        private string gameVisibility = "invisible";
        private string mixedButtonVisibility = "invisible";

        // A list of answers to save us loading from json every time
        // Gets assigned to in OnInitializedAsync()
        private static Answers masterAnswers;

        // Timer for the game and bool for whether it should be running
        private TimeSpan gameTimer = new TimeSpan();
        private bool isGameTimerRunning = false;

        // Best times the user has achieved
        private TimeSpan bestMixedTime;
        private TimeSpan bestHiraganaTime;
        private TimeSpan bestKatakanaTime;


        // List for Answer Items
        public List<AnswerItem> AnswerList = new List<AnswerItem>(){};

        // Lists for the game board
        public List<AnswerItem> AList = new List<AnswerItem>() { };
        public List<AnswerItem> IList = new List<AnswerItem>() { };
        public List<AnswerItem> YaList = new List<AnswerItem>() { };
        public List<AnswerItem> HiList = new List<AnswerItem>() { };
        public List<AnswerItem> HaList = new List<AnswerItem>() { };
        public List<AnswerItem> BiList = new List<AnswerItem>() { };
        public List<AnswerItem> BaList = new List<AnswerItem>() { };

        // These variables let us know when to tell the player when they're going wrong
        private AnswerItem LastDropped { get; set; }
        private bool ActiveMaxItemWarning = false;
        private bool PlacingItemBackWarning = false;
        private bool HasWonGame = false;

        /// <summary>
        /// Run PrepareGame() to get the board ready, then switch case to decide which kana to use
        /// </summary>
        /// <param name="mode">Int provided by button</param>
        private void SelectMode(int mode)
        {
            PrepareGame();

            switch (mode)
            {
                case 1:
                    gameState = GameState.HiraganaMode;
                    textState = TextState.HiraganaMode;
                    break;

                case 2:
                    gameState = GameState.KatakanaMode;
                    textState = TextState.KatakanaMode;
                    break;

                case 3:
                    gameState = GameState.MixedMode;
                    textState = TextState.HiraganaMode;
                    mixedButtonVisibility = "visible";
                    break;
            }
        }
        /// <summary>
        /// Get board prepared for a new game
        /// </summary>
        private void PrepareGame()
        {
            mixedButtonVisibility = "invisible";
            SwapBetweenGameAndMenu();
            AnswerList = BuildAnswerList();
            GameTimer();
        }
        /// <summary>
        /// Swap between menu being visible and game being hidden. This changes the strings of the class of the divs which the menu and games are located in. 
        /// </summary>
        private void SwapBetweenGameAndMenu()
        {
            if (menuVisibility == "invisible")
            {
                menuVisibility = "visible";
                gameVisibility = "invisible";
            }
            else
            {
                menuVisibility = "invisible";
                gameVisibility = "visible";
            }
        }
        /// <summary>
        /// Take a list of Answer Items (usually masterAnswers) and spit out a shuffled version of the list
        /// </summary>
        /// <typeparam name="AnswerItem"></typeparam>
        /// <param name="list">Answers, usually masterAnswers</param>
        /// <returns></returns>
        public static IList<AnswerItem> Shuffle<AnswerItem>(IList<AnswerItem> list)
        {
            Random rng = new Random();
            var source = list.ToList();
            int n = source.Count;
            var shuffled = new List<AnswerItem>(n);
            shuffled.AddRange(source);
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                AnswerItem value = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = value;
            }
            return shuffled;
        }
        /// <summary>
        /// Return a shuffled list of masterAnswers
        /// </summary>
        /// <returns></returns>
        private static List<AnswerItem> BuildAnswerList()
        {
            // Temporary answer list for testing in the limited layout
            List<AnswerItem> tinyList = new List<AnswerItem>() {    new AnswerItem() { Id = 1, HiraganaScript = "あ", KatakanaScript = "ア", RomajiScript = "a" },
                                                                    new AnswerItem() { Id = 2, HiraganaScript = "や", KatakanaScript = "ヤ", RomajiScript = "ya" },
                                                                    new AnswerItem() { Id = 3, HiraganaScript = "は", KatakanaScript = "ハ", RomajiScript = "ha" },
                                                                    new AnswerItem() { Id = 4, HiraganaScript = "ば", KatakanaScript = "バ", RomajiScript = "ba" },
                                                                    new AnswerItem() { Id = 5, HiraganaScript = "い", KatakanaScript = "イ", RomajiScript = "i" },
                                                                    new AnswerItem() { Id = 6, HiraganaScript = "ひ", KatakanaScript = "ヒ", RomajiScript = "hi" },
                                                                    new AnswerItem() { Id = 7, HiraganaScript = "び", KatakanaScript = "ビ", RomajiScript = "bi" }
                                                                    };
            return tinyList; 
            //return (List<AnswerItem>)Shuffle(masterAnswers.Data);
        }
        /// <summary>
        /// Swap textState to Katakana if it's Hiragana, otherwise swap it to Hiragana.
        /// </summary>
        private void SwapKana()
        {
            if (textState == TextState.HiraganaMode)
            {
                textState = TextState.KatakanaMode;
                return;
            }
            textState = TextState.HiraganaMode;
        }
        /// <summary>
        /// Clear active warnings. Check if the player has won the game, then win it.
        /// </summary>
        private void SuccessfulDrop()
        {
            ClearCurrentWarnings();
            if (AnswerList.Count == 0)
            {
                WinGame();
            }
        }
        /// <summary>
        /// Clear active warnings. Show a warning for attempting to place more items in an already answered square. 
        /// </summary>
        private void ShowMaxItemWarning()
        {
            ClearCurrentWarnings();
            ActiveMaxItemWarning = true;
        }
        /// <summary>
        /// Clear active warnings. Show a warning for putting answers back in the item box.
        /// </summary>
        private void ShowPlacingBackWarning()
        {
            ClearCurrentWarnings();
            PlacingItemBackWarning = true;
        }
        /// <summary>
        /// This resets all variables that may be currently displaying a warning
        /// </summary>
        private void ClearCurrentWarnings()
        {
            LastDropped = null;
            ActiveMaxItemWarning = false;
            PlacingItemBackWarning = false;
            HasWonGame = false;
        }
        /// <summary>
        /// Handle the game timer during the game. Set isGameTimerRunning to true and add a second to the timespan every second then update the state.
        /// </summary>
        /// <returns></returns>
        async Task GameTimer()
        {
            isGameTimerRunning = true;
            while (isGameTimerRunning)
            {
                await Task.Delay(1000);
                if (isGameTimerRunning)
                {
                    gameTimer = gameTimer.Add(new TimeSpan(0, 0, 1));
                    StateHasChanged();
                }
            }
        }
        /// <summary>
        /// Get the game ready for a fresh round by turning off the timer, refreshing it, swapping back to the menu, and emptying the game lists.
        /// </summary>
        private void ResetGame()
        {
            isGameTimerRunning = false;
            gameTimer = new TimeSpan();
            SwapBetweenGameAndMenu();
            EmptyGameLists();
        }
        /// <summary>
        /// Empty all of the lists in the game to ensure the field is empty for a new round.
        /// </summary>
        private void EmptyGameLists()
        {
            AnswerList = new List<AnswerItem>() { };
            AList = new List<AnswerItem>() { };
            IList = new List<AnswerItem>() { };
            YaList = new List<AnswerItem>() { };
            HiList = new List<AnswerItem>() { };
            HaList = new List<AnswerItem>() { };
            BiList = new List<AnswerItem>() { };
            BaList = new List<AnswerItem>() { };
        }
        /// <summary>
        /// Win the game for the player. 
        /// </summary>
        private void WinGame()
        {
            isGameTimerRunning = false;
            HasWonGame = true;
            SubmitScore(gameTimer, gameState);
        }
        /// <summary>
        /// Check if we have a new record and update the current records if it is.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="gameState"></param>
        private void SubmitScore(TimeSpan time, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.HiraganaMode:
                    if (bestHiraganaTime.TotalMilliseconds == 0 || gameTimer < bestHiraganaTime)
                    {
                        bestHiraganaTime = gameTimer;
                    }
                    break;
                case GameState.KatakanaMode:
                    if (bestKatakanaTime.TotalMilliseconds == 0 || gameTimer < bestKatakanaTime)
                    {
                        bestHiraganaTime = gameTimer;
                    }
                    break;
                case GameState.MixedMode:
                    if (bestMixedTime.TotalMilliseconds == 0 || gameTimer < bestMixedTime)
                    {
                        bestHiraganaTime = gameTimer;
                    }
                    break;
            }

        }
    }
}


