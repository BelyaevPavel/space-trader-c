using Summary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XMLForm
{
    public partial class frmTemplates : Form
    {
        public frmTemplates()
        {
            InitializeComponent ();
        }

        private void frmTemplates_Load(object sender, EventArgs e)
        {
            lbComodity.DataSource = GlobalData.comodityTemplates.ComodityTemplatesList;
            lbComodity.ValueMember = "id";
            lbComodity.DisplayMember = "Title";
            lbComodity.Update ();
        }

        private void btnAddComodity_Click(object sender, EventArgs e)
        {
            GlobalData.comodityTemplates.ComodityTemplatesList.Add (new ComodityTemplate ()
            {
                Id = GlobalData.comodityTemplates.ComodityTemplatesList.Count+1,
                Title = tbComodityTitle.Text,
                Volume = Convert.ToInt32 (tbVolume.Text)
            });
            lbComodity.Update ();
        }

        private void btnSaveComodity_Click(object sender, EventArgs e)
        {

        }
    }
}
