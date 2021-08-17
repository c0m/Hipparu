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

        // List for Answer Items
        public List<AnswerItem> AnswerList = new List<AnswerItem>(){};

        // Lists for the game board
        public List<AnswerItem> WaList = new List<AnswerItem>() { };
        public List<AnswerItem> RaList = new List<AnswerItem>() { new AnswerItem() { Id = 777, HiraganaScript = "　", KatakanaScript = "　", RomajiScript = "　" } };
        public List<AnswerItem> YaList = new List<AnswerItem>() { };
        public List<AnswerItem> MaList = new List<AnswerItem>() { };
        public List<AnswerItem> HaList = new List<AnswerItem>() { };
        public List<AnswerItem> NaList = new List<AnswerItem>() { };
        public List<AnswerItem> TaList = new List<AnswerItem>() { };
        public List<AnswerItem> SaList = new List<AnswerItem>() { };
        public List<AnswerItem> KaList = new List<AnswerItem>() { };
        public List<AnswerItem> AList = new List<AnswerItem>() { };
        public List<AnswerItem> KiList = new List<AnswerItem>() { };

        // LastDropped lets us tell the player when they're going wrong
        private AnswerItem LastDropped { get; set; }
        

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
                    textState = TextState.HiraganaMode;
                    break;

                case 2:
                    textState = TextState.KatakanaMode;
                    break;

                case 3:
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
            // Reset the game timer
        }
        /// <summary>
        /// Swap between menu being visible and game being hidden. This changes the strings of the class of the divs which the menu and games are located in. 
        /// </summary>
        private void SwapBetweenGameAndMenu()
        {
            if (menuVisibility == "invisible")
            {
                gameState = GameState.MainMenu;
                menuVisibility = "visible";
                gameVisibility = "invisible";
            }
            else
            {
                //gameState handled by SelectMode
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
            return (List<AnswerItem>)Shuffle(masterAnswers.Data);
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
        /// Clear LastDropped. Check if the player has won the game, then win it.
        /// </summary>
        private void SuccessfulDrop()
        {
            LastDropped = null;
            if (AnswerList.Count == 0)
            {
                //win game
            }
        }
    }
}


