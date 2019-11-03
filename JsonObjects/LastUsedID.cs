using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRichPresence.JsonObjects
{
    class LastUsedID
    {
        private String _ID;
        public String ID
        {
            get => _ID;
            set
            {
                _ID = value;
            }
        }

        public LastUsedID (String ID)
        {
            this.ID = ID;
        }
    }
}
