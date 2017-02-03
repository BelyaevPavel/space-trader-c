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
        public static void LoadData(GlobalData globalData)
        {
            try
            {
                Owner basicOwner = new Owner ()
                {
                    Balance = 10000000,
                    id = 1,
                    Title = "United Space State"
                };
                Owner SolePropriator = new Owner ()
                {
                    Balance = 1000,
                    id = 2,
                    Title = "Jack Dowson"
                };
                //globalData = JsonHelper.FromJsonFile<GlobalData> ("BasicGlobalData.xml");
                GlobalData.owners.Add (basicOwner);
                GlobalData.comodityTemplates = JsonHelper.FromJsonFile<ComodityTamplates> ("ComodityTamplates.xml");
                //globalData.stations = JsonHelper.FromJsonFile<List<StationBase>> ("Stations.xml");
                Relations.BuildRelations ();
                PlantStation plant = PlantStationCreator ();
                //JsonHelper.FromJsonFile<PlantStation> ("FirstStation.xml");
                PlantStation plant1 = PlantStationCreator1 ();
                PlantStation plant2 = PlantStationCreator2 ();
                GlobalData.stations.Add (plant);
                GlobalData.stations.Add (plant1);
                GlobalData.stations.Add (plant2);
                //ShipBase ship = JsonHelper.FromJsonFile<ShipBase> ("FirstShip.xml");
                ShipBase ship = new ShipBase (new Transform (), new CargoHold (10000), basicOwner)
                {
                    Acceleration=0.5,
                    MaxSpeed=1,
                    Id=1,
                    Title="Navuhodonosor"
                };
                ShipBase ship1 = new ShipBase (new Transform (), new CargoHold (1000), SolePropriator)
                {
                    Acceleration = 0.75,
                    MaxSpeed = 1.25,
                    Id = 2,
                    Title = "Ephrat"
                };
                GlobalData.ships.Add (ship);
                GlobalData.ships.Add (ship1);
                GlobalData.fleets = JsonHelper.FromJsonFile<Fleets> ("Fleets.xml");
                JsonHelper.ToJsonFile (globalData, "BasicGlobalData.xml");
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

        public static void SaveData(GlobalData globalData)
        {
            JsonHelper.ToJsonFile (globalData, "GlobalData.xml");
            //JsonHelper.ToJsonFile (globalData.fleets, "Fleets.xml");
            //JsonHelper.ToJsonFile (globalData.comodityTemplates, "ComodityTamplates.xml");
            //JsonHelper.ToJsonFile (globalData.stations [0], "FirstPlant.xml");
            //JsonHelper.ToJsonFile (globalData.stations, "Stations.xml");
            //JsonHelper.ToJsonFile (globalData.buyOffers, "buyOffers.xml");
            //JsonHelper.ToJsonFile (globalData.sellOffers, "sellOffers.xml");

            //XmlSerializer Serializer = new XmlSerializer (typeof (Fleets));
            //using (StreamWriter streamWriter = new StreamWriter ("Fleets.xml"))
            //{
            //    Serializer.Serialize (streamWriter, GlobalData.fleets);
            //}
            //throw new NotImplementedException ();
        }

        public static void Update(DateTime dateTime, GlobalData globalData)
        {
            GlobalData.updateDelegate.Invoke (GlobalData.TimeStep);
            if (n++ == 40000)
            {
                GlobalData.timer.Enabled = false;
                GlobalData.stations.GetHashCode ();
                MessageBox.Show (string.Format ("Update {0}. Logs {1}. Last: {2}", dateTime, LogAgregator.Events.Count, LogAgregator.Events.Last ().Message));
                LogAgregator.ToFile (@"LogOutput.txt");
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
            foreach (ShipBase ship in GlobalData.ships)
            {
                GlobalData.updateDelegate += ship.Update;
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
                StationOwner = GlobalData.owners.Last (),
                Title = "Electronics Plant"
            };
            int amount = 500;
            plant.StationCargoHold.Store (1, ref amount);
            plant.AddProductionLine (1, 5, 1000);
            return plant;
        }

        private static PlantStation PlantStationCreator1()
        {
            PlantStation plant = new PlantStation ()
            {
                LocationID = 1,
                LocationTitle = "Nowhere",
                ObjTransform = new Transform ()
                {
                    Position = new Vector3 (42, -21, -6)
                },
                StationCargoHold = new CargoHold (10000),
                StationOwner = GlobalData.owners.Last (),
                Title = "Silicon Plant"
            };
            plant.AddProductionLine (1, 1, 1000);
            plant.AddProductionLine (1, 1, 1000);
            return plant;
        }

        private static PlantStation PlantStationCreator2()
        {
            PlantStation plant = new PlantStation ()
            {
                LocationID = 1,
                LocationTitle = "Nowhere",
                ObjTransform = new Transform ()
                {
                    Position = new Vector3 (0, -21, 20)
                },
                StationCargoHold = new CargoHold (10000),
                StationOwner = GlobalData.owners.Last (),
                Title = "VLSI Plant"
            };
            plant.AddProductionLine (1, 6, 1000);
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
                ProducedPerCycle = 1
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
