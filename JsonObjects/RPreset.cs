using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRichPresence.JsonObjects
{
    class RPreset
    {
        public String ActivityName;
        public String Description;
        public Boolean InLobby;
        public int LobbyCount;
        public int LobbyMax;
        public Boolean Thumbnails;
        public String LargeImageKeyword;
        public String LargeImageText;
        public String SmallImageKeyword;
        public String SmallImageText;
        public Boolean TimeElapsedCheckbox;
        public Boolean TimeRemainingCheckbox;
        public DateTime TimeElapsed;
        public DateTime TimeRemaining;

        public RPreset()
        {

        }

    }
}
