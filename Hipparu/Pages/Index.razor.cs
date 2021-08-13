using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hipparu.Pages
{
    public partial class Index
    {
        public int gameMode = 0;

        public void IncrementCount(int mode)
        {
            if (mode == 0)
            {
                gameMode++;
            }
            else
            {
                gameMode = 0;
            }
        }
    }
}
