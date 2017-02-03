using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Timers;
using Summary;

namespace XMLForm
{
    public partial class frmMain : Form
    {
        public GlobalData globalData;
        public frmMain()
        {
            InitializeComponent ();

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AuxMethods.SaveData (globalData);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add ();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveItem ();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //ComodityTamplates comodityTamplates = ComoditiTamplatesCreation ();
            //JsonHelper.ToJsonFile (comodityTamplates, "ComodityTamplates.xml");
            //PlantStation g = new PlantStation ()
            //{

            //};
            //ShipBase ship = new ShipBase ()
            //{
            //    Id = 1,
            //    ShipCargoHold = new CargoHold ()
            //    {
            //        Capacity = 5135
            //    },
            //    LocationID = 1,
            //    LocationTitle = "Nowhere",
            //    OwnerID = 1,
            //    OwnerTitle = "Overlord",
            //    ObjTransform = new Transform () { Position = new Vector3 (1, 60.2, 42) },
            //    Title = "First Ship"
            //};
            //ship.ShipCargoHold.StoredComodities.Add (1, 524);
            //ship.ShipCargoHold.StoredComodities.Add (6, 5);
            //XmlHelper.ToXmlFile (ship, "FirstShip.xml");
            //JsonHelper.ToJsonFile (ship, "FirstShip.xml");
            globalData = new GlobalData ();
            InitialPraparation ();
            BuildTree ();
            //for (int i = 0; i < 150; i++)
            //{
            //    AuxMethods.Update (DateTime.Now);
            //}
            //JsonHelper.ToJsonFile (LogAgregator.Events, "Log.xml");
            //AuxMethods.Update (DateTime.Now);
        }

        private void InitialPraparation()
        {
            try
            {
                AuxMethods.LoadData (globalData);
            }
            catch (Exception e)
            {
                MessageBox.Show (e.Message);
                throw;
            }
            finally
            {
                AuxMethods.FormUpdateDelegate ();
                GlobalData.timer = new System.Timers.Timer (GlobalData.TimeStep/100);
                GlobalData.timer.Enabled = true;
                //AuxMethods.handler = AuxMethods.Update;
                //ElapsedEventHandler
                GlobalData.timer.Elapsed += Timer_Elapsed;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AuxMethods.Update (e.SignalTime, globalData);
        }

        private void BuildTree()
        {
            treeView1.Nodes.Clear ();
            treeView1.BeginUpdate ();
            foreach (Fleet fleet in GlobalData.fleets.fleetList)
            {
                TreeNode fleetNode = new TreeNode (fleet.Title);
                foreach (ShipBase ship in fleet.shipList)
                {
                    TreeNode shipNode = new TreeNode (ship.Title);
                    fleetNode.Nodes.Add (shipNode);
                }
                treeView1.Nodes.Add (fleetNode);
            }
            treeView1.EndUpdate ();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //if (e.Node.Parent != null)
            //{
            //    //MessageBox.Show (e.Node.Parent.Text + " " + e.Node.Parent.Index + " " + e.Node.Text + " " + e.Node.Index);
            //    tbTitle.Text = GlobalData.fleets.fleetList [e.Node.Parent.Index].shipList [e.Node.Index].Title;
            //    tbCapacity.Text = String.Format ("{0}", GlobalData.fleets.fleetList [e.Node.Parent.Index].shipList [e.Node.Index].ShipCargoHold.Capacity);
            //    GlobalData.ShipSelected = true;
            //    GlobalData.FleetSelected = false;
            //    GlobalData.FleetIndex = e.Node.Parent.Index;
            //    GlobalData.ShipIndex = e.Node.Index;
            //    btnAdd.Enabled = false;
            //    btnSave.Enabled = true;
            //    toolStripStatusLabel1.Text = String.Format ("Статус: выбран {0}", GlobalData.fleets.fleetList [GlobalData.FleetIndex].shipList [GlobalData.ShipIndex].Title);
            //}
            //else
            //{
            //    //MessageBox.Show (e.Node.Text + " " + e.Node.Index);
            //    GlobalData.ShipSelected = false;
            //    GlobalData.FleetSelected = true;
            //    btnAdd.Enabled = true;
            //    btnSave.Enabled = false;
            //    GlobalData.FleetIndex = e.Node.Index;
            //    GlobalData.ShipIndex = -1;
            //    toolStripStatusLabel1.Text = String.Format ("Статус: выбран {0}", GlobalData.fleets.fleetList [GlobalData.FleetIndex].Title);
            //}

        }

        private void Add()
        {
            //if (GlobalData.FleetSelected)
            //{
            //    GlobalData.fleets.fleetList [GlobalData.FleetIndex].shipList.Add (new ShipBase ()
            //    {
            //        Title = tbTitle.Text,
            //        ShipCargoHold = new CargoHold () { Capacity = Convert.ToInt32 (tbCapacity.Text) }
            //    });
            //    BuildTree ();
            //    toolStripStatusLabel1.Text = String.Format ("Статус: в флот {0} добавлен корабль.", GlobalData.fleets.fleetList [GlobalData.FleetIndex].Title);
            //}
            //else
            //    MessageBox.Show ("Select fleet!");
            //throw new NotImplementedException ();
        }

        private void SaveItem()
        {
            //if (GlobalData.FleetSelected && GlobalData.ShipSelected)
            //{
            //    GlobalData.fleets.fleetList [GlobalData.FleetIndex].shipList [GlobalData.ShipIndex].Title = tbTitle.Text;
            //    GlobalData.fleets.fleetList [GlobalData.FleetIndex].shipList [GlobalData.ShipIndex].ShipCargoHold.Capacity = Convert.ToInt32 (tbCapacity.Text);
            //    toolStripStatusLabel1.Text = String.Format ("Статус: корабль {0} из флота {1} отредактирован.", GlobalData.fleets.fleetList [GlobalData.FleetIndex].shipList [GlobalData.ShipIndex].Title, GlobalData.fleets.fleetList [GlobalData.FleetIndex].Title);
            //}
            //else
            //    MessageBox.Show ("Select ship!");
            //throw new NotImplementedException ();
        }

        private void tsmiTemplateEditor_Click(object sender, EventArgs e)
        {
            frmTemplates TemplateEditorForm = new XMLForm.frmTemplates ();
            TemplateEditorForm.Show ();
        }
    }
}
