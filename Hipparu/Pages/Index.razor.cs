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
        protected enum GameState
        {
            MainMenu,
            HiraganaMode,
            KatakanaMode,
            MixedMode
        }
        protected GameState gameState = GameState.MainMenu;

        protected enum TextState
        {
            HiraganaMode,
            KatakanaMode
        }
        protected TextState textState = TextState.HiraganaMode;

        // Styles for the game and menu divs to decide visibilty
        protected string menuVisibility = "visible";
        protected string gameVisibility = "invisible";
        protected string mixedButtonVisibility = "invisible";

        // A list of answers to save us loading from json every time
        // Gets assigned to in OnInitializedAsync()
        protected static Answers masterAnswers;

        // Timer for the game and bool for whether it should be running
        protected TimeSpan gameTimer = new TimeSpan();
        protected bool isGameTimerRunning = false;

        // Best times the user has achieved
        protected TimeSpan bestMixedTime;
        protected TimeSpan bestHiraganaTime;
        protected TimeSpan bestKatakanaTime;


        // List for Answer Items
        protected List<AnswerItem> AnswerList = new List<AnswerItem>(){};

        // These variables are for displaying information on screen
        // These come with leading capital letters to show this
        protected AnswerItem LastDropped { get; set; }
        protected bool ActiveMaxItemWarning = false;
        protected bool PlacingItemBackWarning = false;
        protected bool HasWonGame = false;

        /// <summary>
        /// Run PrepareGame() to get the board ready, then switch case to decide which kana to use
        /// </summary>
        /// <param name="mode">Int provided by button</param>
        protected void SelectMode(int mode)
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
        protected void PrepareGame()
        {
            mixedButtonVisibility = "invisible";
            SwapBetweenGameAndMenu();
            AnswerList = BuildAnswerList();
            GameTimer();
        }
        /// <summary>
        /// Swap between menu being visible and game being hidden. This changes the strings of the class of the divs which the menu and games are located in. 
        /// </summary>
        protected void SwapBetweenGameAndMenu()
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
        protected static IList<AnswerItem> Shuffle<AnswerItem>(IList<AnswerItem> list)
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
        protected static List<AnswerItem> BuildAnswerList()
        {
            return (List<AnswerItem>)Shuffle(masterAnswers.Data);
        }
        /// <summary>
        /// Swap textState to Katakana if it's Hiragana, otherwise swap it to Hiragana.
        /// </summary>
        protected void SwapKana()
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
        protected void SuccessfulDrop()
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
        protected void ShowMaxItemWarning()
        {
            ClearCurrentWarnings();
            ActiveMaxItemWarning = true;
        }
        /// <summary>
        /// Clear active warnings. Show a warning for putting answers back in the item box.
        /// </summary>
        protected void ShowPlacingBackWarning()
        {
            ClearCurrentWarnings();
            PlacingItemBackWarning = true;
        }
        /// <summary>
        /// This resets all variables that may be currently displaying a warning
        /// </summary>
        protected void ClearCurrentWarnings()
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
        protected void ResetGame()
        {
            isGameTimerRunning = false;
            gameTimer = new TimeSpan();
            SwapBetweenGameAndMenu();
            EmptyGameLists();
        }
    /// <summary>
    /// Win the game for the player. 
    /// </summary>
    protected void WinGame()
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
        protected void SubmitScore(TimeSpan time, GameState gameState)
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
                        bestKatakanaTime = gameTimer;
                    }
                    break;
                case GameState.MixedMode:
                    if (bestMixedTime.TotalMilliseconds == 0 || gameTimer < bestMixedTime)
                    {
                        bestMixedTime = gameTimer;
                    }
                    break;
            }

        }

        // Lists for the game board and the method to empty it. These are placed at the bottom to save the readability of the rest of the document.
        // All places of the board need a position, and each position need to be uniquely labelled somehow.
        // So here are the unique lists for the dropzones. 
        #region yep this is happening
        protected List<AnswerItem> AList = new List<AnswerItem>() { };
        protected List<AnswerItem> KaList = new List<AnswerItem>() { };
        protected List<AnswerItem> GaList = new List<AnswerItem>() { };
        protected List<AnswerItem> SaList = new List<AnswerItem>() { };
        protected List<AnswerItem> ZaList = new List<AnswerItem>() { };
        protected List<AnswerItem> TaList = new List<AnswerItem>() { };
        protected List<AnswerItem> DaList = new List<AnswerItem>() { };
        protected List<AnswerItem> NaList = new List<AnswerItem>() { };
        protected List<AnswerItem> HaList = new List<AnswerItem>() { };
        protected List<AnswerItem> BaList = new List<AnswerItem>() { };
        protected List<AnswerItem> PaList = new List<AnswerItem>() { };
        protected List<AnswerItem> MaList = new List<AnswerItem>() { };
        protected List<AnswerItem> YaList = new List<AnswerItem>() { };
        protected List<AnswerItem> RaList = new List<AnswerItem>() { };
        protected List<AnswerItem> WaList = new List<AnswerItem>() { };
        protected List<AnswerItem> IList = new List<AnswerItem>() { };
        protected List<AnswerItem> KiList = new List<AnswerItem>() { };
        protected List<AnswerItem> GiList = new List<AnswerItem>() { };
        protected List<AnswerItem> ShiList = new List<AnswerItem>() { };
        protected List<AnswerItem> JiList = new List<AnswerItem>() { }; // shi with dakuten しじ
        protected List<AnswerItem> ChiList = new List<AnswerItem>() { };
        // protected List<AnswerItem> ChiJiList = new List<AnswerItem>() { }; // chi with dakuten ちぢ
        protected List<AnswerItem> NiList = new List<AnswerItem>() { };
        protected List<AnswerItem> HiList = new List<AnswerItem>() { };
        protected List<AnswerItem> BiList = new List<AnswerItem>() { };
        protected List<AnswerItem> PiList = new List<AnswerItem>() { };
        protected List<AnswerItem> MiList = new List<AnswerItem>() { };
        protected List<AnswerItem> RiList = new List<AnswerItem>() { };
        protected List<AnswerItem> UList = new List<AnswerItem>() { };
        protected List<AnswerItem> KuList = new List<AnswerItem>() { };
        protected List<AnswerItem> GuList = new List<AnswerItem>() { };
        protected List<AnswerItem> SuList = new List<AnswerItem>() { };
        protected List<AnswerItem> ZuList = new List<AnswerItem>() { };
        protected List<AnswerItem> TsuList = new List<AnswerItem>() { };
        // protected List<AnswerItem> DzuList = new List<AnswerItem>() { };
        protected List<AnswerItem> NuList = new List<AnswerItem>() { };
        protected List<AnswerItem> FuList = new List<AnswerItem>() { };
        protected List<AnswerItem> BuList = new List<AnswerItem>() { };
        protected List<AnswerItem> PuList = new List<AnswerItem>() { };
        protected List<AnswerItem> MuList = new List<AnswerItem>() { };
        protected List<AnswerItem> YuList = new List<AnswerItem>() { };
        protected List<AnswerItem> RuList = new List<AnswerItem>() { };
        protected List<AnswerItem> EList = new List<AnswerItem>() { };
        protected List<AnswerItem> KeList = new List<AnswerItem>() { };
        protected List<AnswerItem> GeList = new List<AnswerItem>() { };
        protected List<AnswerItem> SeList = new List<AnswerItem>() { };
        protected List<AnswerItem> ZeList = new List<AnswerItem>() { };
        protected List<AnswerItem> TeList = new List<AnswerItem>() { };
        protected List<AnswerItem> DeList = new List<AnswerItem>() { };
        protected List<AnswerItem> NeList = new List<AnswerItem>() { };
        protected List<AnswerItem> HeList = new List<AnswerItem>() { };
        protected List<AnswerItem> BeList = new List<AnswerItem>() { };
        protected List<AnswerItem> PeList = new List<AnswerItem>() { };
        protected List<AnswerItem> MeList = new List<AnswerItem>() { };
        protected List<AnswerItem> YeList = new List<AnswerItem>() { };
        protected List<AnswerItem> ReList = new List<AnswerItem>() { };
        protected List<AnswerItem> WeList = new List<AnswerItem>() { };
        protected List<AnswerItem> OList = new List<AnswerItem>() { };
        protected List<AnswerItem> KoList = new List<AnswerItem>() { };
        protected List<AnswerItem> GoList = new List<AnswerItem>() { };
        protected List<AnswerItem> SoList = new List<AnswerItem>() { };
        protected List<AnswerItem> ZoList = new List<AnswerItem>() { };
        protected List<AnswerItem> ToList = new List<AnswerItem>() { };
        protected List<AnswerItem> DoList = new List<AnswerItem>() { };
        protected List<AnswerItem> NoList = new List<AnswerItem>() { };
        protected List<AnswerItem> HoList = new List<AnswerItem>() { };
        protected List<AnswerItem> BoList = new List<AnswerItem>() { };
        protected List<AnswerItem> PoList = new List<AnswerItem>() { };
        protected List<AnswerItem> MoList = new List<AnswerItem>() { };
        protected List<AnswerItem> YoList = new List<AnswerItem>() { };
        protected List<AnswerItem> RoList = new List<AnswerItem>() { };
        protected List<AnswerItem> WoList = new List<AnswerItem>() { };
        protected List<AnswerItem> NList = new List<AnswerItem>() { };
        #endregion

        /// <summary>
        /// Empty all of the lists in the game to ensure the field is empty for a new round.
        /// </summary>
        protected void EmptyGameLists()
        {
            #region oh no not again
            AList = new List<AnswerItem>() { };
            KaList = new List<AnswerItem>() { };
            GaList = new List<AnswerItem>() { };
            SaList = new List<AnswerItem>() { };
            ZaList = new List<AnswerItem>() { };
            TaList = new List<AnswerItem>() { };
            DaList = new List<AnswerItem>() { };
            NaList = new List<AnswerItem>() { };
            HaList = new List<AnswerItem>() { };
            BaList = new List<AnswerItem>() { };
            PaList = new List<AnswerItem>() { };
            MaList = new List<AnswerItem>() { };
            YaList = new List<AnswerItem>() { };
            RaList = new List<AnswerItem>() { };
            WaList = new List<AnswerItem>() { };
            IList = new List<AnswerItem>() { };
            KiList = new List<AnswerItem>() { };
            GiList = new List<AnswerItem>() { };
            ShiList = new List<AnswerItem>() { };
            JiList = new List<AnswerItem>() { };
            ChiList = new List<AnswerItem>() { };
            // ChiJiList = new List<AnswerItem>() { };
            NiList = new List<AnswerItem>() { };
            HiList = new List<AnswerItem>() { };
            BiList = new List<AnswerItem>() { };
            PiList = new List<AnswerItem>() { };
            MiList = new List<AnswerItem>() { };
            RiList = new List<AnswerItem>() { };
            UList = new List<AnswerItem>() { };
            KuList = new List<AnswerItem>() { };
            GuList = new List<AnswerItem>() { };
            SuList = new List<AnswerItem>() { };
            ZuList = new List<AnswerItem>() { };
            TsuList = new List<AnswerItem>() { };
            // DzuList = new List<AnswerItem>() { };
            NuList = new List<AnswerItem>() { };
            FuList = new List<AnswerItem>() { };
            BuList = new List<AnswerItem>() { };
            PuList = new List<AnswerItem>() { };
            MuList = new List<AnswerItem>() { };
            YuList = new List<AnswerItem>() { };
            RuList = new List<AnswerItem>() { };
            EList = new List<AnswerItem>() { };
            KeList = new List<AnswerItem>() { };
            GeList = new List<AnswerItem>() { };
            SeList = new List<AnswerItem>() { };
            ZeList = new List<AnswerItem>() { };
            TeList = new List<AnswerItem>() { };
            DeList = new List<AnswerItem>() { };
            NeList = new List<AnswerItem>() { };
            HeList = new List<AnswerItem>() { };
            BeList = new List<AnswerItem>() { };
            PeList = new List<AnswerItem>() { };
            MeList = new List<AnswerItem>() { };
            YeList = new List<AnswerItem>() { };
            ReList = new List<AnswerItem>() { };
            WeList = new List<AnswerItem>() { };
            OList = new List<AnswerItem>() { };
            KoList = new List<AnswerItem>() { };
            GoList = new List<AnswerItem>() { };
            SoList = new List<AnswerItem>() { };
            ZoList = new List<AnswerItem>() { };
            ToList = new List<AnswerItem>() { };
            DoList = new List<AnswerItem>() { };
            NoList = new List<AnswerItem>() { };
            HoList = new List<AnswerItem>() { };
            BoList = new List<AnswerItem>() { };
            PoList = new List<AnswerItem>() { };
            MoList = new List<AnswerItem>() { };
            YoList = new List<AnswerItem>() { };
            RoList = new List<AnswerItem>() { };
            WoList = new List<AnswerItem>() { };
            NList = new List<AnswerItem>() { };
            #endregion
        }
    }
}


