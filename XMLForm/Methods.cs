using Summary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Timers;
using System.Windows.Forms;

namespace XMLForm
{
    public class AuxMethods
    {
        static int n = 0;
        public static void LoadData()
        {
            try
            {
                GlobalData.comodityTemplates = JsonHelper.FromJsonFile<ComodityTamplates> ("ComodityTamplates.xml");
                Relations.BuildRelations ();
                PlantStation plant = PlantStationCreator ();//JsonHelper.FromJsonFile<PlantStation> ("FirstStation.xml");
                GlobalData.stations.Add (plant);
                ShipBase ship = JsonHelper.FromJsonFile<ShipBase> ("FirstShip.xml");
                GlobalData.fleets = JsonHelper.FromJsonFile<Fleets> ("Fleets.xml");
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                //if (GlobalData.fleets.fleetList.Count == 0)
                //{
                //    Fleet fleet = new Fleet () { Title = "1st fleet" };
                //    fleet.shipList.Add (new ShipBase ()
                //    {
                //        Title = "Rahanas",
                //        ShipCargoHold = new CargoHold () { Capacity = 720 }
                //    });
                //    fleet.shipList.Add (new ShipBase ()
                //    {
                //        Title = "Sanahar",
                //        ShipCargoHold = new CargoHold () { Capacity = 2000 }
                //    });
                //    GlobalData.fleets.fleetList.Add (fleet);
                //}
            }
        }

        public static void SaveData()
        {
            JsonHelper.ToJsonFile (GlobalData.fleets, "Fleets.xml");
            JsonHelper.ToJsonFile (GlobalData.comodityTemplates, "ComodityTamplates.xml");
            JsonHelper.ToJsonFile (GlobalData.stations [0], "FirstPlant.xml");

            //XmlSerializer Serializer = new XmlSerializer (typeof (Fleets));
            //using (StreamWriter streamWriter = new StreamWriter ("Fleets.xml"))
            //{
            //    Serializer.Serialize (streamWriter, GlobalData.fleets);
            //}
            //throw new NotImplementedException ();
        }

        public static void Update(DateTime dateTime)
        {
            GlobalData.updateDelegate.Invoke (GlobalData.TimeStep);
            if (n++ == 300)
            {
                GlobalData.timer.Enabled = false;
                GlobalData.stations.GetHashCode ();
                MessageBox.Show (string.Format ("Update {0}. Logs {1}. Last: {2}", dateTime, LogAgregator.Events.Count, LogAgregator.Events.Last ().Message));
                JsonHelper.ToJsonFile (LogAgregator.Events, "Log.xml");
            }
            GlobalData.CurrentTime.AddMilliseconds (GlobalData.TimeStep);
            //try
            //{
            //    foreach (StationBase station in GlobalData.stations)
            //    {
            //        station.Update (GlobalData.TimeStep);
            //    }
            //    GlobalData.CurrentTime.AddMilliseconds (GlobalData.TimeStep);
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show (e.Message);
            //}
            //MessageBox.Show (string.Format ("Update {0}", dateTime));
        }

        public static void FormUpdateDelegate()
        {
            foreach (StationBase station in GlobalData.stations)
            {
                GlobalData.updateDelegate += station.Update;
            }
        }

        private static PlantStation PlantStationCreator()
        {
            PlantStation plant = new PlantStation ()
            {
                LocationID = 1,
                LocationTitle = "Nowhere",
                ObjTransform = new Transform ()
                {
                    Position = new Vector3 (-42, 21, 6)
                },
                StationCargoHold = new CargoHold (10000),
                OwnerID = 1,
                OwnerTitle = "Overlord",
                Title = "First Plant"
            };
            int amount = 2000;
            plant.StationCargoHold.Store (1, ref amount);
            plant.AddProductionLine (1, 5, 1000);
            plant.AddProductionLine (1, 1, 1000);
            plant.AddProductionLine (1, 1, 1000);
            return plant;
        }

        private static ComodityTamplates ComodityTamplatesCreation()
        {
            ComodityTamplates comodityTamplates = new ComodityTamplates ();
            comodityTamplates.AddTamplate (new ComodityTemplate ()
            {
                Mass = 1,
                Volume = 1,
                Title = "Silicon",
                ProductionTime = new TimeSpan (0, 0, 1),
                ProducedPerCycle=1
            });
            comodityTamplates.AddTamplate (new ComodityTemplate ()
            {
                Mass = 1,
                Volume = 1,
                Title = "Hydrogen",
                ProductionTime = new TimeSpan (0, 0, 1),
                ProducedPerCycle = 1
            });
            comodityTamplates.AddTamplate (new ComodityTemplate ()
            {
                Mass = 1,
                Volume = 1,
                Title = "Helium",
                ProductionTime = new TimeSpan (0, 0, 1),
                ProducedPerCycle = 1
            });
            return comodityTamplates;
        }
    }
}
