using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace Summary
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="TimeStep">Time step in milliseconds</param>
    public delegate void UpdateDelegate(double TimeStep);
    public static class LogAgregator
    {
        static List<LogEventArgs> _events = new List<LogEventArgs> ();

        public static List<LogEventArgs> Events
        {
            get
            {
                return _events;
            }

            private set
            {
                _events = value;
            }
        }

        public static void Log(LogEventArgs args)
        {
            _events.Add (args);
        }
    }
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string message, DateTime eventTime)
        {
            Message = message;
            EventTime = eventTime;
        }

        [JsonProperty]
        public string Message
        {
            get;
            private set;
        }

        [JsonProperty]
        public DateTime EventTime
        {
            get;
            private set;
        }
    }
    public class GlobalData
    {
        public static UpdateDelegate updateDelegate;

        public static Fleets fleets = new Fleets () { Title = "Fleets" };

        public static List<StationBase> stations = new List<StationBase> ();

        public static ComodityTamplates comodityTemplates = new ComodityTamplates ();

        public static ShipTamplates shipTemplates = new ShipTamplates ();

        public static World world = new World ();

        public static System.Timers.Timer timer;// = new Timer(TimeStep);

        public static DateTime CurrentTime = new DateTime (2409, 3, 5);

        public static double TimeStep = 100;

        public static bool ShipSelected = false;

        public static bool FleetSelected = false;

        public static int FleetIndex = -1;

        public static int ShipIndex = -1;
    }
    public static class Relations
    {
        /// <summary>
        /// Provides easy way to get comodity by id.
        /// </summary>
        public static Dictionary<int, ComodityTemplate> IdToComodity = new Dictionary<int, ComodityTemplate> ();

        public static void BuildRelations()
        {
            foreach (ComodityTemplate tamplate in GlobalData.comodityTemplates.ComodityTemplatesList)
            {
                IdToComodity.Add (tamplate.Id, tamplate);
            }
        }
    }
    public class Transform
    {
        //[XmlElement ("Position")]
        Vector3 _position;
        //[XmlElement ("Velocity")]
        Vector3 _velocity;
        //[XmlElement ("Direction")]
        Vector3 _direction;

        [JsonProperty]
        public Vector3 Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        [JsonProperty]
        public Vector3 Velocity
        {
            get
            {
                return _velocity;
            }

            set
            {
                _velocity = value;
            }
        }

        [JsonProperty]
        public Vector3 Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }

        public void Translate(double TimeStep)
        {
            _position += _velocity * TimeStep;
        }

        public Transform()
        {
            _position = new Vector3 ();
            _velocity = new Vector3 ();
            _direction = new Vector3 ();
        }
    }
    public class Trade
    {
        Dictionary<int, int> _sellOffers;
        Dictionary<int, int> _buyOffers;

        public Dictionary<int, int> SellOffers
        {
            get
            {
                return _sellOffers;
            }

            set
            {
                _sellOffers = value;
            }
        }

        public Dictionary<int, int> BuyOffers
        {
            get
            {
                return _buyOffers;
            }

            set
            {
                _buyOffers = value;
            }
        }
        
        public Trade()
        {
            _sellOffers = new Dictionary<int, int> ();
            _buyOffers = new Dictionary<int, int> ();
        }
    }
    public class CargoHold
    {
        #region Fields
        //[XmlElement ("CargoCapacity")]
        int _capacity;
        int _occupied;
        int _freeSpace;
        //[XmlArray ("StoredComodities"), XmlArrayItem ("Comodity")]
        Dictionary<int, int> _storedComodities;
        #endregion

        #region Properties
        /// <summary>
        /// Capacity of cargo hold.
        /// </summary>
        [JsonProperty]
        public int Capacity
        {
            get
            {
                return _capacity;
            }

            set
            {
                if (value > _occupied)
                {
                    _capacity = value;
                    FreeSpace = _capacity - _occupied;
                    if (Log != null)
                        Log (this, new LogEventArgs (string.Format ("Capacity changed to {0}", _capacity), GlobalData.CurrentTime));
                }
                else
                {
                    if (Log != null)
                        Log (this, new LogEventArgs (string.Format ("Capacity can not be changed to {0}. Current occupation amount({1}) is exceeding assigned value!", value, _occupied), GlobalData.CurrentTime));
                }
            }
        }

        public int Occupied
        {
            get
            {
                return _occupied;
            }

            private set
            {
                _occupied = value;
                FreeSpace = _capacity - _occupied;
            }
        }

        /// <summary>
        /// Stores relation between stored comodity id and it's quantity.
        /// </summary>
        [JsonProperty]
        public Dictionary<int, int> StoredComodities
        {
            get
            {
                return _storedComodities;
            }

            set
            {
                _storedComodities = value;
            }
        }

        public int FreeSpace
        {
            get
            {
                return _freeSpace;
            }

            private set
            {
                _freeSpace = value;
            }
        }

        public event EventHandler<LogEventArgs> Log;
        #endregion

        #region Constructors
        public CargoHold()
        {
            _storedComodities = new Dictionary<int, int> ();
        }

        public CargoHold(int Capacity)
            : this ()
        {
            this.Capacity = Capacity;
        }
        #endregion

        #region Methods
        public bool Store(int ComodityTamplateId, ref int Amount)
        {
            RecalculateOccupation ();
            if (CalculateOccupaiedVolume (ComodityTamplateId, Amount) < _capacity - _occupied)
            {
                if (_storedComodities.ContainsKey (ComodityTamplateId))
                {
                    _storedComodities [ComodityTamplateId] += Amount;
                }
                else
                {
                    _storedComodities.Add (ComodityTamplateId, Amount);
                }
                RecalculateOccupation ();
                if (Log != null && Amount != 0)
                    Log (this, new LogEventArgs (string.Format ("Stored {0} units of {1}. Cargo hold occupation changed to ({2}/{3}). [{4}]", Amount, Relations.IdToComodity [ComodityTamplateId].Title, _occupied, _capacity,
                Thread.CurrentThread.ManagedThreadId), GlobalData.CurrentTime));
                Amount = 0;
                return true;
            }
            else
            {
                int availableAmount = CalculateAmountToFillVolume (ComodityTamplateId, _capacity - _occupied);
                if (_capacity - _occupied > 0 && availableAmount > 0)
                {
                    if (_storedComodities.ContainsKey (ComodityTamplateId))
                    {

                        _storedComodities [ComodityTamplateId] += availableAmount;
                    }
                    else
                    {
                        _storedComodities.Add (ComodityTamplateId, availableAmount);
                    }
                    RecalculateOccupation ();
                    Amount -= availableAmount;
                    if (Log != null && Amount != 0)
                        Log (this, new LogEventArgs (string.Format ("Stored ({0}/{1}) units of {2}. Cargo hold occupation changed to ({3}/{4}). [{5}]", availableAmount, (Amount + availableAmount), Relations.IdToComodity [ComodityTamplateId].Title, _occupied, _capacity,
                Thread.CurrentThread.ManagedThreadId), GlobalData.CurrentTime));
                    return false;
                }
                else
                {
                    if (Log != null && Amount != 0)
                        Log (this, new LogEventArgs (string.Format ("Can't store {0} units of {1}. Too few space in cargo hold ({2}/{3}). [{4}]", Amount, Relations.IdToComodity [ComodityTamplateId].Title, _occupied, _capacity,
                Thread.CurrentThread.ManagedThreadId), GlobalData.CurrentTime));
                    return false;
                }
            }
        }

        /// <summary>
        /// Extracts available amount of requested comodity from stored comodities.
        /// </summary>
        /// <param name="ComodityTamplateId">Id of requested comodity.</param>
        /// <param name="Amount">Amount of requested comodity.</param>
        /// <returns>Returns <code>Dictionary</code> available amount of requested comodity.</returns>
        public Dictionary<int, int> Extract(int ComodityTamplateId, int Amount)
        {
            Dictionary<int, int> extractedComodity = new Dictionary<int, int> ();
            if (_storedComodities.ContainsKey (ComodityTamplateId))
            {
                if (_storedComodities [ComodityTamplateId] > Amount)
                {
                    extractedComodity.Add (ComodityTamplateId, Amount);
                    _storedComodities [ComodityTamplateId] -= Amount;
                }
                else
                {
                    extractedComodity.Add (ComodityTamplateId, _storedComodities [ComodityTamplateId]);
                    _storedComodities [ComodityTamplateId] = 0;
                }
            }
            else
                extractedComodity.Add (ComodityTamplateId, 0);
            RecalculateOccupation ();
            return extractedComodity;
        }

        internal Dictionary<int, int> Extract(int key)
        {
            throw new NotImplementedException ();
        }
        public void RecalculateOccupation()
        {
            int occupied = 0;
            foreach (int key in _storedComodities.Keys)
            {
                occupied += _storedComodities [key] * Relations.IdToComodity [key].Volume;
            }
            Occupied = occupied;
        }

        public bool HasComodity(int ComodityTamplateId)
        {
            return _storedComodities.ContainsKey (ComodityTamplateId);
        }

        public static int CalculateOccupaiedVolume(int ComodityTamplateId, int Amount)
        {
            return Relations.IdToComodity [ComodityTamplateId].Volume * Amount;
        }

        public static int CalculateAmountToFillVolume(int ComodityTamplateId, int Volume)
        {
            return Volume / Relations.IdToComodity [ComodityTamplateId].Volume;
        }
        #endregion
    }
    public class Production : IUpdatable, IProduction
    {
        #region Fields
        //[XmlArray ("ProductioLines"), XmlArrayItem ("ProductionLine")]
        List<ProductionLine> _productionLines;
        CargoHold _productionCargoHold;
        Dictionary<int, int> _productionDemand;
        UpdateDelegate updateDelegate;
        Dictionary<int, int> _productionOutput;
        Dictionary<int, int> _productionConsumption;
        Dictionary<int, int> _productionBalance;
        #endregion

        #region Properties
        [JsonProperty]
        public List<ProductionLine> ProductionLines
        {
            get
            {
                return _productionLines;
            }

            private set
            {
                _productionLines = value;
            }
        }

        [JsonProperty]
        public CargoHold ProductionCargoHold
        {
            get
            {
                return _productionCargoHold;
            }

            set
            {
                _productionCargoHold = value;
            }
        }

        public Dictionary<int, int> ProductionConsumption
        {
            get
            {
                return _productionConsumption;
            }

            private set
            {
                _productionConsumption = value;
            }
        }

        public Dictionary<int, int> ProductionOutput
        {
            get
            {
                return _productionOutput;
            }

            private set
            {
                _productionOutput = value;
            }
        }

        public Dictionary<int, int> ProductionBalance
        {
            get
            {
                return _productionBalance;
            }

            private set
            {
                _productionBalance = value;
            }
        }

        public event EventHandler<LogEventArgs> Log;
        #endregion

        #region Constructors
        public Production()
        {
            _productionDemand = new Dictionary<int, int> ();
            _productionOutput = new Dictionary<int, int> ();
            _productionConsumption = new Dictionary<int, int> ();
            ProductionCargoHold = new CargoHold ();
            ProductionLines = new List<ProductionLine> ();
            _productionCargoHold.Log += information_Log;
            updateDelegate += ProvideMaterials;
        }
        public Production(int CargoHoldCapacity)
            : this ()
        {
            ProductionCargoHold.Capacity = CargoHoldCapacity;
        }
        #endregion

        #region Methods
        private void ProvideMaterials(double TimeStep)
        {
            ProductionLines.OrderBy (line => line.Priority);
            foreach (ProductionLine line in ProductionLines)
            {
                Dictionary<int, int> requestedMaterials = line.RequestMaterials ();
                Dictionary<int, int> providedMaterials = new Dictionary<int, int> ();
                foreach (int key in requestedMaterials.Keys)
                {
                    if (requestedMaterials [key] > 0)
                        providedMaterials.Add (key, _productionCargoHold.Extract (key, requestedMaterials [key]) [key]);
                }

                Dictionary<int, int> rests = line.ResiveMaterials (providedMaterials);
                foreach (int key in rests.Keys)
                {
                    if (rests [key] > 0)
                    {
                        int amount = rests [key];
                        _productionCargoHold.Store (key, ref amount);
                    }
                }

                RequestMaterials ();
                //foreach (int key in line.ProductionDemand.Keys)
                //{
                //    if (ProductionCargoHold.StoredComodities.ContainsKey (key))
                //    {
                //        if (ProductionCargoHold.StoredComodities [key] - line.ProductionDemand [key] > 0)
                //        {

                //            ProductionCargoHold.StoredComodities [key] -= line.ProductionDemand [key];
                //            line.ProductionDemand [key] = 0;
                //        }
                //        else
                //        {
                //            line.ProductionDemand [key] -= ProductionCargoHold.StoredComodities [key];
                //            ProductionCargoHold.StoredComodities [key] = 0;
                //            if (_productionDemand.ContainsKey (key))
                //                _productionDemand [key] += line.ProductionDemand [key];
                //            else
                //                _productionDemand.Add (key, line.ProductionDemand [key]);
                //        }
                //    }
                //}
            }
        }

        public void AddProductionLine(ProductionLine NewProductionLine)
        {
            ProductionLines.Add (NewProductionLine);
            ProductionLines.Last ().Number = ProductionLines.Count;
            ProductionLines.Last ().Log += information_Log;
            updateDelegate += ProductionLines.Last ().Update;
            //AddToBalance (ProductionLines.Last ().ProducedComodityId);
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, int CargoHoldCapacity)
        {
            ProductionLine NewProductionLine = new ProductionLine (ComodityTamplateId, Priority, CargoHoldCapacity);
            ProductionLines.Add (NewProductionLine);
            ProductionLines.Last ().Number = ProductionLines.Count;
            ProductionLines.Last ().Log += information_Log;
            updateDelegate += ProductionLines.Last ().Update;
            //AddToBalance (ComodityTamplateId);
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, CargoHold StarterCargoHold)
        {
            ProductionLine NewProductionLine = new ProductionLine (ComodityTamplateId, Priority, StarterCargoHold);
            ProductionLines.Add (NewProductionLine);
            ProductionLines.Last ().Number = ProductionLines.Count;
            ProductionLines.Last ().Log += information_Log;
            updateDelegate += ProductionLines.Last ().Update;
            //AddToBalance (ComodityTamplateId);
        }

        public Dictionary<int, int> RequestMaterials()
        {
            foreach (ProductionLine line in ProductionLines)
            {
                Dictionary<int, int> requestedMaterials = line.RequestMaterials ();
                foreach (int key in requestedMaterials.Keys)
                {
                    if (_productionCargoHold.StoredComodities.ContainsKey (key))
                    {
                        AddDemand (requestedMaterials, key);
                    }
                    else
                    {
                        _productionCargoHold.StoredComodities.Add (key, 0);
                        AddDemand (requestedMaterials, key);
                    }
                    //if (_productionDemand.ContainsKey (key))
                    //{
                    //    _productionDemand [key] += _productionDemand [key] - (requestedMaterials [key] - _productionCargoHold.StoredComodities [key]);
                    //}
                    //else
                    //{
                    //    _productionDemand.Add (key, requestedMaterials [key] - _productionCargoHold.StoredComodities [key]);
                    //}
                }
            }
            return _productionDemand;
        }

        public Dictionary<int, int> ResiveMaterials(Dictionary<int, int> ProvidedMaterials)
        {
            Dictionary<int, int> rests = new Dictionary<int, int> ();
            foreach (int key in ProvidedMaterials.Keys)
            {
                int amount = ProvidedMaterials [key];
                _productionCargoHold.Store (key, ref amount);
                _productionDemand [key] -= ProvidedMaterials [key] - amount;
                rests.Add (key, amount);
            }
            return rests;
        }

        public void Update(double TimeStep)
        {
            //updateDelegate.Invoke ();
            ProvideMaterials (TimeStep);
            foreach (ProductionLine line in _productionLines)
            {
                line.Update (TimeStep);
            }
        }

        private void AddDemand(Dictionary<int, int> requestedMaterials, int key)
        {
            if (_productionDemand.ContainsKey (key))
            {
                _productionDemand [key] += requestedMaterials [key] - _productionCargoHold.StoredComodities [key] - _productionDemand [key];
            }
            else
            {
                _productionDemand.Add (key, requestedMaterials [key] - _productionCargoHold.StoredComodities [key]);
            }
        }

        private void AddToBalance(int ComodityTamplateId)
        {
            if (_productionOutput.ContainsKey (ComodityTamplateId))
            {
                _productionOutput [ComodityTamplateId] += Relations.IdToComodity [ComodityTamplateId].ProducedPerCycle;
            }
            else
            {
                _productionOutput.Add (ComodityTamplateId, Relations.IdToComodity [ComodityTamplateId].ProducedPerCycle);
            }
            foreach (int key in Relations.IdToComodity [ComodityTamplateId].RequiredMaterials.Keys)
            {
                if (_productionConsumption.ContainsKey (key))
                {
                    _productionConsumption [key] += Relations.IdToComodity [ComodityTamplateId].RequiredMaterials [key];
                }
                else
                {
                    _productionConsumption.Add (ComodityTamplateId, Relations.IdToComodity [ComodityTamplateId].RequiredMaterials [key]);
                }
            }
        }

        public Dictionary<int, int> RecalculateBalance()
        {
            Dictionary<int, int> productionBalance = new Dictionary<int, int> ();
            _productionOutput.Clear ();
            _productionConsumption.Clear ();
            foreach (ProductionLine line in _productionLines)
            {
                if (_productionOutput.ContainsKey (line.ProducedComodityId))
                {
                    _productionOutput [line.ProducedComodityId] += Relations.IdToComodity [line.ProducedComodityId].ProducedPerCycle;
                }
                else
                {
                    _productionOutput.Add (line.ProducedComodityId, Relations.IdToComodity [line.ProducedComodityId].ProducedPerCycle);
                }
                if (productionBalance.ContainsKey (line.ProducedComodityId))
                {
                    productionBalance [line.ProducedComodityId] += Relations.IdToComodity [line.ProducedComodityId].ProducedPerCycle;
                }
                else
                {
                    productionBalance.Add (line.ProducedComodityId, Relations.IdToComodity [line.ProducedComodityId].ProducedPerCycle);
                }
                foreach (int key in Relations.IdToComodity [line.ProducedComodityId].RequiredMaterials.Keys)
                {
                    if (_productionConsumption.ContainsKey (key))
                    {
                        _productionConsumption [key] += Relations.IdToComodity [line.ProducedComodityId].RequiredMaterials [key];
                    }
                    else
                    {
                        _productionConsumption.Add (key, Relations.IdToComodity [line.ProducedComodityId].RequiredMaterials [key]);
                    }
                    if (productionBalance.ContainsKey (key))
                    {
                        productionBalance [key] -= Relations.IdToComodity [line.ProducedComodityId].RequiredMaterials [key];
                    }
                    else
                    {
                        productionBalance.Add (key, -Relations.IdToComodity [line.ProducedComodityId].RequiredMaterials [key]);
                    }
                }
            }
            ProductionBalance = productionBalance;
            return productionBalance;
        }

        internal Dictionary<int, int> ExtractProduction(int CargoHoldFreeSpace)
        {
            int plantCargoFreeSpace = CargoHoldFreeSpace;
            Dictionary<int, int> extractedProduction = new Dictionary<int, int> ();
            foreach (ProductionLine line in _productionLines)
            {
                if (CargoHold.CalculateOccupaiedVolume (line.ProducedComodityId, 1) < _productionCargoHold.FreeSpace)
                {
                    int amount = line.ExtractProduction (_productionCargoHold.FreeSpace);
                    _productionCargoHold.Store (line.ProducedComodityId, ref amount);
                }
            }
            foreach (int key in _productionBalance.Keys)
            {
                if (_productionBalance [key] > 0)
                {
                    extractedProduction.Add (key, _productionCargoHold.Extract (key, plantCargoFreeSpace) [key]);
                    plantCargoFreeSpace -= CargoHold.CalculateOccupaiedVolume (key, extractedProduction [key]);
                }
            }
            return extractedProduction;
        }

        public Dictionary<int, int> GetProductionBalance()
        {
            return ProductionBalance;
        }
        #endregion

        #region Event handlers
        private void information_Log(object sender, LogEventArgs e)
        {
            if (Log != null)
                Log (this, new LogEventArgs ("Production - " + e.Message, e.EventTime));
        }
        #endregion
    }
    public class ProductionLine : IProductionLine
    {
        #region Fields
        int _number;
        int _priority;
        //[XmlElement ("ProducedComodityId")]
        int _producedComodityId;
        TimeSpan _productionTime;
        TimeSpan _estimatedProductionTime;
        //[XmlElement ("CargoHold")]
        CargoHold _productionLineCargoHold;
        bool _productionDemandSatisfied;
        bool _isIdle;
        Dictionary<int, int> _productionDemand;
        #endregion

        #region Properties
        [JsonProperty]
        public int Priority
        {
            get
            {
                return _priority;
            }

            set
            {
                _priority = value;
            }
        }

        [JsonProperty]
        public int ProducedComodityId
        {
            get
            {
                return _producedComodityId;
            }

            private set
            {
                _producedComodityId = value;
                if (Log != null)
                    Log (this, new LogEventArgs (string.Format ("Line {0} start producing {1}.", _number, Relations.IdToComodity [_producedComodityId]), GlobalData.CurrentTime));
                ProductionDemand.Clear ();
                CheckResourseSatisfaction ();
            }
        }

        [JsonProperty]
        public CargoHold ProductionLineCargoHold
        {
            get
            {
                return _productionLineCargoHold;
            }

            private set
            {
                _productionLineCargoHold = value;
            }
        }

        public bool ProductionDemandSatisfied
        {
            get
            {
                return _productionDemandSatisfied;
            }

            set
            {
                _productionDemandSatisfied = value;
                if (Log != null)
                {
                    if (_productionDemandSatisfied)
                        Log (this, new LogEventArgs ("Materials demand is satisfied.", GlobalData.CurrentTime));
                    else
                        Log (this, new LogEventArgs ("Materials demand is not satisfied.", GlobalData.CurrentTime));
                }
            }
        }

        public bool IsIdle
        {
            get
            {
                return _isIdle;
            }

            private set
            {
                _isIdle = value;
                if (Log != null)
                {
                    if (_isIdle)
                        Log (this, new LogEventArgs (string.Format ("Line {0} is idle.", _number), GlobalData.CurrentTime));
                    else
                        Log (this, new LogEventArgs (string.Format ("Line {0} is working.", _number), GlobalData.CurrentTime));
                }
            }
        }

        public Dictionary<int, int> ProductionDemand
        {
            get
            {
                return _productionDemand;
            }

            private set
            {
                _productionDemand = value;
            }
        }

        [JsonProperty]
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                _number = value;
            }
        }

        [JsonProperty]
        public TimeSpan EstimatedProductionTime
        {
            get
            {
                return _estimatedProductionTime;
            }

            private set
            {
                _estimatedProductionTime = value;
            }
        }

        [JsonProperty]
        public TimeSpan ProductionTime
        {
            get
            {
                return _productionTime;
            }

            private set
            {
                _productionTime = value;
            }
        }

        public event EventHandler<LogEventArgs> Log;

        public event EventHandler<LogEventArgs> DemandNotSatisfied;
        #endregion

        #region Methods
        public void SetProducibleComodity(int ComodityTamplateId)
        {
            ProducedComodityId = ComodityTamplateId;
            ProductionTime = Relations.IdToComodity [_producedComodityId].GetProductionTime ();
        }

        public bool CheckResourseSatisfaction()
        {
            Dictionary<int, int> requiredMaterials = Relations.IdToComodity [_producedComodityId].RequiredMaterials;
            _productionDemandSatisfied = true;
            foreach (int key in requiredMaterials.Keys)
            {
                if (_productionLineCargoHold.StoredComodities.ContainsKey (key))
                {
                    if (_productionLineCargoHold.StoredComodities [key] - requiredMaterials [key] < 0)
                    {
                        _productionDemandSatisfied = false;
                        AddDemand (requiredMaterials, key);
                    }
                }
                else
                {
                    _productionDemandSatisfied = false;
                    _productionLineCargoHold.StoredComodities.Add (key, 0);
                    AddDemand (requiredMaterials, key);
                }
            }
            if (!_productionDemandSatisfied)
            {
                if (Log != null)
                    Log (this, new LogEventArgs (string.Format ("Line {0} - production demand is not satisfied.", _number), GlobalData.CurrentTime));
            }
            return _productionDemandSatisfied;
        }

        private void AddDemand(Dictionary<int, int> requiredMaterials, int key)
        {
            if (_productionDemand.ContainsKey (key))
            {
                _productionDemand [key] += requiredMaterials [key] - _productionLineCargoHold.StoredComodities [key] - _productionDemand [key];
            }
            else
            {
                _productionDemand.Add (key, requiredMaterials [key] - _productionLineCargoHold.StoredComodities [key]);
            }
        }

        private void ProductionStep(double TimeStep)
        {
            if (_isIdle)
            {
                StartProduction ();
            }
            else
            {
                if ((_estimatedProductionTime - TimeSpan.FromMilliseconds (TimeStep)).Duration () > TimeSpan.FromMilliseconds (TimeStep))
                {
                    _estimatedProductionTime -= TimeSpan.FromMilliseconds (TimeStep);
                }
                else
                {
                    FinishProduction ();
                }
            }
        }

        private void FinishProduction()
        {
            int producedAmount = Relations.IdToComodity [_producedComodityId].ProducedPerCycle;
            if (_productionLineCargoHold.Store (_producedComodityId, ref producedAmount))
            {
                IsIdle = true;
                _estimatedProductionTime = TimeSpan.FromTicks (0);
                Log (this, new LogEventArgs (string.Format ("Line {0} - Finished production.", _number), GlobalData.CurrentTime));
            }
            else
                Log (this, new LogEventArgs (string.Format ("Line {0} - Too few place in cargo hold to finish production.", _number), GlobalData.CurrentTime));
        }

        private void StartProduction()
        {
            if (_productionDemandSatisfied)
            {
                _estimatedProductionTime = _productionTime;
                foreach (int key in Relations.IdToComodity [_producedComodityId].RequiredMaterials.Keys)
                {
                    _productionLineCargoHold.Extract (key, Relations.IdToComodity [_producedComodityId].RequiredMaterials [key]);
                }
                IsIdle = false;
            }
        }

        internal int ExtractProduction(int CargoHoldFreeSpace)
        {
            return _productionLineCargoHold.Extract (_producedComodityId, CargoHoldFreeSpace) [_producedComodityId];
        }
        internal Dictionary<int, int> RequestMaterials()
        {
            return _productionDemand;
        }

        internal Dictionary<int, int> ResiveMaterials(Dictionary<int, int> ProvidedMaterials)
        {
            Dictionary<int, int> rests = new Dictionary<int, int> ();
            foreach (int key in ProvidedMaterials.Keys)
            {
                if (ProvidedMaterials [key] > 0)
                {
                    int amount = ProvidedMaterials [key];
                    _productionLineCargoHold.Store (key, ref amount);
                    _productionDemand [key] -= ProvidedMaterials [key] - amount;
                    rests.Add (key, amount);
                }
            }
            return rests;
        }

        internal void Update(double TimeStep)
        {
            CheckResourseSatisfaction ();
            ProductionStep (TimeStep);
        }
        #endregion

        #region Constructors
        public ProductionLine()
        {
            _producedComodityId = 0;
            _estimatedProductionTime = new TimeSpan (0);
            _isIdle = true;
            ProductionLineCargoHold = new CargoHold ();
            ProductionLineCargoHold.Log += ProductionLineCargoHold_Log;
            _productionDemand = new Dictionary<int, int> ();
        }

        public ProductionLine(int ComodityTemplateId)
            : this ()
        {
            SetProducibleComodity (ComodityTemplateId);
            CheckResourseSatisfaction ();
        }

        public ProductionLine(int ComodityTemplateId, int Priority)
            : this (ComodityTemplateId)
        {
            _priority = Priority;
        }

        public ProductionLine(int ComodityTemplateId, int Priority, int CargoHoldCapacity)
            : this (ComodityTemplateId, Priority)
        {
            _productionLineCargoHold.Capacity = CargoHoldCapacity;
        }

        public ProductionLine(Comodity ComodityToProduce)
            : this (ComodityToProduce.TemplateId)
        {

        }

        public ProductionLine(ComodityTemplate ComodityToProduce)
            : this (ComodityToProduce.Id)
        {

        }

        public ProductionLine(int ComodityTemplateId, CargoHold LineCargoHold)
            : this (ComodityTemplateId)
        {
            _productionLineCargoHold = LineCargoHold;
        }

        public ProductionLine(int ComodityTemplateId, int Priority, CargoHold LineCargoHold)
            : this (ComodityTemplateId, Priority)
        {
            _productionLineCargoHold = LineCargoHold;
        }

        public ProductionLine(Comodity ComodityToProduce, CargoHold LineCargoHold)
            : this (ComodityToProduce)
        {
            _productionLineCargoHold = LineCargoHold;
        }

        #endregion

        #region Handlers
        private void ProductionLineCargoHold_Log(object sender, LogEventArgs e)
        {
            if (Log != null)
                Log (this, new LogEventArgs (string.Format ("Line №{0} - {1}", _number, e.Message), e.EventTime));
        }
        #endregion
    }
    public class ShipBase : IMovable, IProducible, IStorable, ITargetable, ITrader
    {
        #region Fields
        //[XmlElement ("ID")]
        int _id;
        //[XmlElement ("Title")]
        string _title;
        //[XmlElement ("Speed")]
        double _currentSpeed;
        //[XmlElement ("MaximalSpeed")]
        double _maxSpeed;
        //[XmlElement ("Acceleration")]
        double _acceleration;
        //[XmlElement ("ObjectTransform")]
        Transform _objTransform;
        //[XmlElement ("DestinationTransform")]
        Transform _destTransform;
        ITargetable _destination;
        //[XmlElement ("LoactionTitle")]
        string _locationTitle;
        //[XmlElement ("LoactionID")]
        int _locationID;
        //[XmlElement ("OwnerTitle")]
        string _ownerTitle;
        //[XmlElement ("OwnerID")]
        int _ownerID;
        //[XmlElement ("CargoHold")]
        CargoHold _shipCargoHold;
        #endregion

        #region Properties
        [JsonProperty]
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [JsonProperty]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        [JsonProperty]
        public double CurrentSpeed
        {
            get
            {
                return _currentSpeed;
            }

            set
            {
                _currentSpeed = value;
            }
        }

        [JsonProperty]
        public double MaxSpeed
        {
            get
            {
                return _maxSpeed;
            }

            set
            {
                _maxSpeed = value;
            }
        }

        [JsonProperty]
        public double Acceleration
        {
            get
            {
                return _acceleration;
            }

            set
            {
                _acceleration = value;
            }
        }

        [JsonProperty]
        public string LocationTitle
        {
            get
            {
                return _locationTitle;
            }

            set
            {
                _locationTitle = value;
            }
        }

        [JsonProperty]
        public int LocationID
        {
            get
            {
                return _locationID;
            }

            set
            {
                _locationID = value;
            }
        }

        [JsonProperty]
        public string OwnerTitle
        {
            get
            {
                return _ownerTitle;
            }

            set
            {
                _ownerTitle = value;
            }
        }

        [JsonProperty]
        public int OwnerID
        {
            get
            {
                return _ownerID;
            }

            set
            {
                _ownerID = value;
            }
        }

        [JsonProperty]
        public Transform ObjTransform
        {
            get
            {
                return _objTransform;
            }

            set
            {
                _objTransform = value;
            }
        }

        [JsonProperty]
        public Transform DestTransform
        {
            get
            {
                return _destTransform;
            }

            set
            {
                _destTransform = value;
            }
        }

        [JsonProperty]
        public CargoHold ShipCargoHold
        {
            get
            {
                return _shipCargoHold;
            }

            set
            {
                _shipCargoHold = value;
            }
        }

        [JsonProperty]
        internal ITargetable Destination
        {
            get
            {
                return _destination;
            }

            set
            {
                _destination = value;
            }
        }
        #endregion

        #region Methods
        public double GetSpeed()
        {
            return CurrentSpeed;
        }

        public Vector3 GetVelocity()
        {
            return ObjTransform.Velocity;
        }

        public Vector3 GetDirection()
        {
            return ObjTransform.Direction;
        }

        public Vector3 GetPosition()
        {
            return ObjTransform.Position;
        }

        public Vector3 GetDestPosition()
        {
            return DestTransform.Position;
        }

        public ITargetable GetDestination()
        {
            return _destination;
        }

        public double GetProductionCost()
        {
            throw new NotImplementedException ();
        }

        public TimeSpan GetProductionTime()
        {
            throw new NotImplementedException ();
        }

        public Dictionary<int, int> GetRequiredMaterials()
        {
            throw new NotImplementedException ();
        }

        public double GetVolume()
        {
            throw new NotImplementedException ();
        }

        public double GetMass()
        {
            throw new NotImplementedException ();
        }

        public void Move(double TimeStep)
        {
            if (ObjTransform.Velocity.Length < MaxSpeed)
            {
                ObjTransform.Velocity += ObjTransform.Direction * (Acceleration * TimeStep);
            }
            CurrentSpeed = ObjTransform.Velocity.Length;
            ObjTransform.Translate (TimeStep);
        }

        public void SetDirection()
        {
            ObjTransform.Direction = DestTransform.Position - ObjTransform.Position;
            ObjTransform.Velocity = ~ObjTransform.Direction;
        }

        public void SetDestination(Transform DestTransform)
        {
            _destTransform = DestTransform;
            SetDirection ();
        }

        public void FormTradeOffers()
        {
            throw new NotImplementedException ();
        }

        public Dictionary<int, int> GetBuyOffers()
        {
            throw new NotImplementedException ();
        }

        public Dictionary<int, int> GetSellOffers()
        {
            throw new NotImplementedException ();
        }        
        #endregion
    }
    public class StationBase : IUnmovable, IProducible, IUpdatable, ITrader, ITargetable
    {
        #region Fields
        //[XmlElement ("ID")]
        protected int _id;
        //[XmlElement ("Title")]
        protected String _title;
        //[XmlElement ("ObjectTransform")]
        protected Transform _objTransform;
        //[XmlElement ("LoactionTitle")]
        protected String _locationTitle;
        //[XmlElement ("LoactionID")]
        protected int _locationID;
        //[XmlElement ("OwnerTitle")]
        protected String _ownerTitle;
        //[XmlElement ("OwnerID")]
        protected int _ownerID;
        //[XmlElement ("CargoHold")]
        protected CargoHold _stationCargoHold;
        protected UpdateDelegate updateDelegate;
        private Trade _tradeOffers;
        #endregion

        #region Properties
        [JsonProperty]
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [JsonProperty]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        [JsonProperty]
        public Transform ObjTransform
        {
            get
            {
                return _objTransform;
            }

            set
            {
                _objTransform = value;
            }
        }

        [JsonProperty]
        public String LocationTitle
        {
            get
            {
                return _locationTitle;
            }

            set
            {
                _locationTitle = value;
            }
        }

        [JsonProperty]
        public int LocationID
        {
            get
            {
                return _locationID;
            }

            set
            {
                _locationID = value;
            }
        }

        [JsonProperty]
        public String OwnerTitle
        {
            get
            {
                return _ownerTitle;
            }

            set
            {
                _ownerTitle = value;
            }
        }

        [JsonProperty]
        public int OwnerID
        {
            get
            {
                return _ownerID;
            }

            set
            {
                _ownerID = value;
            }
        }

        [JsonProperty]
        public CargoHold StationCargoHold
        {
            get
            {
                return _stationCargoHold;
            }

            set
            {
                _stationCargoHold = value;
            }
        }

        [JsonProperty]
        protected Trade TradeOffers
        {
            get
            {
                return _tradeOffers;
            }

            private set
            {
                _tradeOffers = value;
            }
        }
        #endregion

        #region Methods
        public Vector3 GetPosition()
        {
            return ObjTransform.Position;
        }

        public double GetProductionCost()
        {
            throw new NotImplementedException ();
        }

        public TimeSpan GetProductionTime()
        {
            throw new NotImplementedException ();
        }

        public Dictionary<int, int> GetRequiredMaterials()
        {
            throw new NotImplementedException ();
        }

        public virtual void Update(double TimeStep)
        {
            throw new NotImplementedException ();
        }

        public virtual void FormTradeOffers()
        {
            throw new NotImplementedException ();
        }

        public virtual Dictionary<int, int> GetBuyOffers()
        {
            throw new NotImplementedException ();
        }

        public virtual Dictionary<int, int> GetSellOffers()
        {
            throw new NotImplementedException ();
        }        
        #endregion

        #region Constructors
        public StationBase()
        {
            _objTransform = new Transform ();
            _stationCargoHold = new CargoHold ();
            _tradeOffers = new Trade ();
        }
        #endregion
    }
    public class PlantStation : StationBase, IProduction
    {
        //[XmlElement ("Production")]
        Production _production;
        Dictionary<int, int> _productionBalance;

        [JsonProperty]
        Production Production
        {
            get
            {
                return _production;
            }

            set
            {
                _production = value;
            }
        }

        public override void Update(double TimeStep)
        {
            lock (this)
            {
                //updateDelegate.Invoke (TimeStep);
                ExtractProduction ();
                _production.Update (TimeStep);
                ProvideMaterials (TimeStep);
                FormTradeOffers ();
            }
        }

        public void AddProductionLine(ProductionLine NewProductionLine)
        {
            _production.AddProductionLine (NewProductionLine);
            _productionBalance = _production.RecalculateBalance ();
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, int CargoHoldCapacity)
        {
            _production.AddProductionLine (Priority, ComodityTamplateId, CargoHoldCapacity);
            _productionBalance = _production.RecalculateBalance ();
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, CargoHold StarterCargoHold)
        {
            _production.AddProductionLine (Priority, ComodityTamplateId, StarterCargoHold);
            _productionBalance = _production.RecalculateBalance ();
        }

        private void ExtractProduction()
        {
            Dictionary<int, int> extractedProduction = _production.ExtractProduction (_stationCargoHold.FreeSpace);
            foreach (int key in extractedProduction.Keys)
            {
                int amount = extractedProduction [key];
                _stationCargoHold.Store (key, ref amount);
            }
        }

        public PlantStation()
        {
            _production = new Production ();
            _productionBalance = new Dictionary<int, int> ();
            _production.Log += _production_Log;
            updateDelegate += _production.Update;
            updateDelegate += ProvideMaterials;
            Production.ProductionCargoHold.Capacity += 500;
        }

        private void _production_Log(object sender, LogEventArgs e)
        {
            LogAgregator.Log (new LogEventArgs (_title + " - " + e.Message, e.EventTime));
        }

        private void ProvideMaterials(double TimeStep)
        {
            Dictionary<int, int> requestedMaterials = _production.RequestMaterials ();
            Dictionary<int, int> providedMaterials = new Dictionary<int, int> ();
            foreach (int key in requestedMaterials.Keys)
            {
                if (requestedMaterials [key] > 0)
                    providedMaterials.Add (key, _stationCargoHold.Extract (key, requestedMaterials [key]) [key]);
            }

            Dictionary<int, int> rests = _production.ResiveMaterials (providedMaterials);
            foreach (int key in rests.Keys)
            {
                if (rests [key] > 0)
                {
                    int amount = rests [key];
                    _stationCargoHold.Store (key, ref amount);
                }
            }
        }

        public Dictionary<int, int> GetProductionBalance()
        {
            return ((IProduction) Production).GetProductionBalance ();
        }

        public override void FormTradeOffers()
        {
            int totalNegativeBalanceUnits = 0;
            foreach (int key in _productionBalance.Keys)
                if (_productionBalance [key] < 0)
                    totalNegativeBalanceUnits -= _productionBalance [key];
            foreach (int key in _productionBalance.Keys)
            {
                if (_productionBalance [key] >= 0)
                {
                    if (TradeOffers.SellOffers.ContainsKey (key))
                    {
                        TradeOffers.SellOffers [key] = StationCargoHold.StoredComodities [key];
                    }
                    else
                    {
                        TradeOffers.SellOffers.Add (key, StationCargoHold.StoredComodities [key]);
                    }
                }
                else
                {
                    if (_productionBalance.ContainsKey (key))
                    {
                        if (TradeOffers.BuyOffers.ContainsKey (key))
                        {
                            TradeOffers.BuyOffers [key] = CargoHold.CalculateAmountToFillVolume (key, -(int) ((double) (_productionBalance [key]) / totalNegativeBalanceUnits * 0.5*StationCargoHold.FreeSpace));
                        }
                        else
                        {
                            TradeOffers.BuyOffers.Add (key, CargoHold.CalculateAmountToFillVolume (key, -(int) ((double) (_productionBalance [key]) / totalNegativeBalanceUnits * 0.5 * StationCargoHold.FreeSpace)));
                        }
                    }
                }
            }
        }

        public override Dictionary<int, int> GetBuyOffers()
        {
            return TradeOffers.BuyOffers;
        }

        public override Dictionary<int, int> GetSellOffers()
        {
            return TradeOffers.SellOffers;
        }
    }
    public class Fleet
    {
        //[XmlElement ("Title")]
        [JsonProperty]
        public String Title
        {
            get;
            set;
        }
        //[XmlArray ("Ships"), XmlArrayItem ("Ship")]
        [JsonProperty]
        public List<ShipBase> shipList = new List<ShipBase> ();
    }
    public class Fleets
    {
        //[XmlElement ("Title")]
        [JsonProperty]
        public String Title
        {
            get;
            set;
        }
        /// <summary>
        /// Containts all fleet description.
        /// </summary>
        //[XmlArray ("FleetsList"), XmlArrayItem ("Fleet")]
        [JsonProperty]
        public List<Fleet> fleetList = new List<Fleet> ();
    }
    public class Comodity : ComodityTemplate
    {
        //[XmlElement ("TemplateID")]
        int _templateId;
        //[XmlElement ("Count")]
        int _count;
        //[XmlElement ("StorageTitle")]
        int _storageTitle;
        //[XmlElement ("StorageID")]
        int _storageID;

        [JsonProperty]
        public int TemplateId
        {
            get
            {
                return _templateId;
            }

            set
            {
                _templateId = value;
            }
        }

        [JsonProperty]
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
            }
        }

        [JsonProperty]
        public int StorageTitle
        {
            get
            {
                return _storageTitle;
            }

            set
            {
                _storageTitle = value;
            }
        }

        [JsonProperty]
        public int StorageID
        {
            get
            {
                return _storageID;
            }

            set
            {
                _storageID = value;
            }
        }

        public Comodity()
        {
        }
    }
    public class ComodityTemplate : IProducible, IStorable
    {
        //[XmlElement ("ID")]
        int _id;
        //[XmlElement ("Title")]
        String _title;
        //[XmlElement ("Volume")]
        int _volume;
        int _producedPerCycle;
        //[XmlElement ("Mass")]
        double _mass;
        //[XmlArray ("RequiredMaterials"), XmlArrayItem ("Comodity")]
        Dictionary<int, int> _requiredMaterials;
        TimeSpan _productionTime;

        [JsonProperty]
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [JsonProperty]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        [JsonProperty]
        public int Volume
        {
            get
            {
                return _volume;
            }

            set
            {
                _volume = value;
            }
        }

        [JsonProperty]
        public double Mass
        {
            get
            {
                return _mass;
            }

            set
            {
                _mass = value;
            }
        }

        [JsonProperty]
        public Dictionary<int, int> RequiredMaterials
        {
            get
            {
                return _requiredMaterials;
            }

            set
            {
                _requiredMaterials = value;
            }
        }

        [JsonProperty]
        public TimeSpan ProductionTime
        {
            get
            {
                return _productionTime;
            }

            set
            {
                _productionTime = value;
            }
        }

        [JsonProperty]
        public int ProducedPerCycle
        {
            get
            {
                return _producedPerCycle;
            }

            set
            {
                _producedPerCycle = value;
            }
        }

        public double GetProductionCost()
        {
            throw new NotImplementedException ();
        }

        public TimeSpan GetProductionTime()
        {
            return ProductionTime;
        }

        public Dictionary<int, int> GetRequiredMaterials()
        {
            return RequiredMaterials;
        }

        public double GetVolume()
        {
            return _volume;
        }

        public double GetMass()
        {
            return _mass;
        }

        public ComodityTemplate()
        {
            _requiredMaterials = new Dictionary<int, int> ();
        }
    }
    public class ComodityTamplates
    {
        [JsonProperty]
        int _lastId;
        //[XmlArray ("ComodityTemplates"), XmlArrayItem ("ComodityTemplate")]
        List<ComodityTemplate> _comodityTemplatesList = new List<ComodityTemplate> ();

        [JsonProperty]
        public List<ComodityTemplate> ComodityTemplatesList
        {
            get
            {
                return _comodityTemplatesList;
            }
        }

        public ComodityTamplates()
        {
            _lastId = 0;
            _comodityTemplatesList = new List<ComodityTemplate> ();
        }

        public void AddTamplate(ComodityTemplate tamplate)
        {
            tamplate.Id = ++_lastId;
            _comodityTemplatesList.Add (tamplate);
        }
    }
    public class ShipTamplates
    {
        //[XmlArray ("ShipTemplates"), XmlArrayItem ("ShipTemplate")]
        [JsonProperty]
        public List<Comodity> ShipTemplatesList = new List<Comodity> ();
    }
    public class Location
    {

        //[XmlElement ("ID")]
        int _id;
        //[XmlElement ("Title")]
        String _title;
        /// <summary>
        /// Containts all fleet description.
        /// </summary>
        //[XmlArray ("ShipList"), XmlArrayItem ("Ship")]
        List<Fleet> _shipList = new List<Fleet> ();
        //[XmlArray ("Linked Locations List"), XmlArrayItem ("Location")]
        List<Fleet> _linkedLocationsList = new List<Fleet> ();

        [JsonProperty]
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [JsonProperty]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        [JsonProperty]
        public List<Fleet> ShipList
        {
            get
            {
                return _shipList;
            }

            set
            {
                _shipList = value;
            }
        }

        [JsonProperty]
        public List<Fleet> LinkedLocationsList
        {
            get
            {
                return _linkedLocationsList;
            }

            set
            {
                _linkedLocationsList = value;
            }
        }
    }
    public class Owner
    {

        //[XmlElement ("ID")]
        [JsonProperty]
        public int id
        {
            get;
            set;
        }
        //[XmlElement ("Title")]
        [JsonProperty]
        public String Title
        {
            get;
            set;
        }
        //[XmlArray ("Owned Ships"), XmlArrayItem ("Ship")]
        [JsonProperty]
        public List<Fleet> ownedShips = new List<Fleet> ();

    }
    public class World
    {
        List<Location> _locations;

        [JsonProperty]
        public List<Location> Locations
        {
            get
            {
                return _locations;
            }

            set
            {
                _locations = value;
            }
        }

        public World()
        {
            _locations = new List<Location> ();
        }
    }
}
