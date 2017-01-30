using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
    class TShip
    {
        DataTable data;
        public TShip()
        {
            data = new DataTable ();
            DataColumn collumnID = new DataColumn ()
            {
                AllowDBNull = false,
                Caption = "ID"

            };
        }

        public DataTable Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }
    }
}
