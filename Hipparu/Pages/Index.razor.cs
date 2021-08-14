using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hipparu.Shared;
using Plk.Blazor.DragDrop;

namespace Hipparu.Pages
{
    public partial class Index
    {
        // Game states:
        // 0 - Main Menu
        // 1 - Hiragana Mode
        // 2 - Katakana Mode
        // 3 - Mixed Mode
        private int gameState = 0;

        // Text states
        // 0 - Hiragana
        // 1 - Katakana
        private int textState = 0;

        // Styles for the game and menu divs to decide visibilty
        private string menuVisibility = "menu-visible";
        private string gameVisibility = "game-invisible";

        // List for Answer Items with placeholder
        public List<AnswerItem> AnswerList = new List<AnswerItem>()
        {
                new AnswerItem(){Id = 1, RomajiScript = "n", HiraganaScript = "ん", KatakanaScript = "ン"},
        };

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
                    //set to display hiragana
                    break;

                case 2:
                    //set to display katakana
                    break;

                case 3:
                    //enable mixed game switch
                    break;
            }
        }
        /// <summary>
        /// Get board prepared for a new game
        /// </summary>
        private void PrepareGame()
        {
            SwapBetweenGameAndMenu();
            // Reset the game timer
            // Populate answer table
        }
        /// <summary>
        /// Swap between menu being visible and game being hidden. This changes the strings of the class of the divs which the menu and games are located in. 
        /// </summary>
        private void SwapBetweenGameAndMenu()
        {
            if (menuVisibility == "menu-invisible")
            {
                gameState = 0;
                menuVisibility = "menu-visible";
                gameVisibility = "game-invisible";
            }
            else
            {
                //gameState handled by SelectMode
                menuVisibility = "menu-invisible";
                gameVisibility = "game-visible";
            }
        }
    }
}


