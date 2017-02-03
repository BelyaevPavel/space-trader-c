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

    public enum TradeOfferType
    {
        Sell = 0,
        Buy = 1
    }

    public class GlobalData
    {
        public static UpdateDelegate updateDelegate;

        [JsonProperty]
        public static Fleets fleets;

        [JsonProperty]
        public static List<StationBase> stations;

        [JsonProperty]
        public static List<ShipBase> ships;

        [JsonProperty]
        public static List<TradeOffer> sellOffers;

        [JsonProperty]
        public static List<TradeOffer> buyOffers;

        [JsonProperty]
        public static ComodityTamplates comodityTemplates;

        [JsonProperty]
        public static ShipTamplates shipTemplates;

        [JsonProperty]
        public static World world;

        [JsonProperty]
        public static List<Owner> owners;

        public static System.Timers.Timer timer;// = new Timer(TimeStep);

        [JsonProperty]
        public static DateTime CurrentTime = new DateTime (2409, 3, 5);

        public static double TimeStep = 100;

        //[JsonProperty]
        //public Fleets Fleets
        //{
        //    get
        //    {
        //        return fleets;
        //    }

        //    set
        //    {
        //        fleets = value;
        //    }
        //}

        //[JsonProperty]
        //public List<StationBase> Stations
        //{
        //    get
        //    {
        //        return stations;
        //    }

        //    set
        //    {
        //        stations = value;
        //    }
        //}

        //[JsonProperty]
        //public List<TradeOffer> SellOffers
        //{
        //    get
        //    {
        //        return sellOffers;
        //    }

        //    set
        //    {
        //        sellOffers = value;
        //    }
        //}

        //[JsonProperty]
        //public List<TradeOffer> BuyOffers
        //{
        //    get
        //    {
        //        return buyOffers;
        //    }

        //    set
        //    {
        //        buyOffers = value;
        //    }
        //}

        //[JsonProperty]
        //public ComodityTamplates ComodityTemplates
        //{
        //    get
        //    {
        //        return comodityTemplates;
        //    }

        //    set
        //    {
        //        comodityTemplates = value;
        //    }
        //}

        //[JsonProperty]
        //public ShipTamplates ShipTemplates
        //{
        //    get
        //    {
        //        return shipTemplates;
        //    }

        //    set
        //    {
        //        shipTemplates = value;
        //    }
        //}

        //[JsonProperty]
        //public World World
        //{
        //    get
        //    {
        //        return world;
        //    }

        //    set
        //    {
        //        world = value;
        //    }
        //}

        public GlobalData()
        {
            fleets = new Fleets () { Title = "Fleets" };
            stations = new List<StationBase> ();
            sellOffers = new List<TradeOffer> ();
            buyOffers = new List<TradeOffer> ();
            comodityTemplates = new ComodityTamplates ();
            shipTemplates = new ShipTamplates ();
            world = new World ();
            owners = new List<Owner> ();
            ships = new List<Summary.ShipBase> ();
        }
    }
    public static class Relations
    {
        /// <summary>
        /// Provides easy way to get comodity by id.
        /// </summary>
        public static Dictionary<int, ComodityTemplate> IdToComodity = new Dictionary<int, ComodityTemplate> ();

        public static Dictionary<int, Owner> IdToOwner = new Dictionary<int, Owner> ();

        public static void BuildRelations()
        {
            foreach (ComodityTemplate tamplate in GlobalData.comodityTemplates.ComodityTemplatesList)
            {
                IdToComodity.Add (tamplate.Id, tamplate);
            }
            IdToOwner.Add (1, new Owner (1000000));
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
                //Log.Invoke (this, new LogEventArgs (string.Format ("Moved to ({0},{1},{2})", _position.X, _position.Y, _position.Z), GlobalData.CurrentTime));
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
                //Log.Invoke (this, new LogEventArgs (string.Format ("Velocity changed to {0},{1},{2}", _velocity.Length), GlobalData.CurrentTime));
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

        public event EventHandler<LogEventArgs> Log;
        public void Translate(double TimeStep)
        {
            // 0.001 coeff because of TimeStep in milleseconds.
            _position += _velocity * (0.001*TimeStep);
        }

        public Transform()
        {
            _position = new Vector3 ();
            _velocity = new Vector3 ();
            _direction = new Vector3 ();
        }

        public Transform(Vector3 Position, Vector3 Velocity, Vector3 Direction)
        {
            _position = Position;
            _velocity = Velocity;
            _direction = Direction;
        }

        public Transform(Transform transform)
            : this (transform.Position, transform.Velocity, transform.Direction)
        {

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
    public class TradeOffer : IComparable<TradeOffer>, IEquatable<TradeOffer>
    {
        protected int comodityId;
        protected int amount;
        protected int price;
        protected ITrader offerer;
        protected TradeOfferType offerType;

        public int ComodityId
        {
            get
            {
                return comodityId;
            }

            set
            {
                comodityId = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        public int Price
        {
            get
            {
                return price;
            }

            set
            {
                price = value;
            }
        }

        public ITrader Offerer
        {
            get
            {
                return offerer;
            }

            set
            {
                offerer = value;
            }
        }

        public TradeOfferType OfferType
        {
            get
            {
                return offerType;
            }

            set
            {
                offerType = value;
            }
        }

        public TradeOffer(int comodityId, int amount, int price, TradeOfferType offerType, ITrader offerer)
        {
            ComodityId = comodityId;
            Amount = amount;
            Price = price;
            Offerer = offerer;
            OfferType = offerType;
        }

        public int CompareTo(TradeOffer other)
        {
            if (other == null)
                return 1;

            else
                return this.price.CompareTo (other.price);
        }

        public bool Equals(TradeOffer other)
        {
            if (other == null)
                return false;
            return (this.price.Equals (other.price));
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

        /// <summary>
        /// Calculates volume that will be occupied by <paramref name="Amount"/> of comodity with id equal to <paramref name="ComodityTamplateId"/>.
        /// </summary>
        /// <param name="ComodityTamplateId">Sets comodity tamplate id.</param>
        /// <param name="Amount">Sets amount of comodity.</param>
        /// <returns>Calculated volume.</returns>
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
        bool _delegatesInitialised;
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
                SignUpLines ();
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
            _delegatesInitialised = false;
            _productionDemand = new Dictionary<int, int> ();
            _productionOutput = new Dictionary<int, int> ();
            _productionConsumption = new Dictionary<int, int> ();
            _productionBalance = new Dictionary<int, int> ();
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
            SignUpLine (ProductionLines.Last ());
            //AddToBalance (ProductionLines.Last ().ProducedComodityId);
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, int CargoHoldCapacity)
        {
            ProductionLine NewProductionLine = new ProductionLine (ComodityTamplateId, Priority, CargoHoldCapacity);
            ProductionLines.Add (NewProductionLine);
            ProductionLines.Last ().Number = ProductionLines.Count;
            SignUpLine (ProductionLines.Last ());
            //AddToBalance (ComodityTamplateId);
        }

        public void AddProductionLine(int Priority, int ComodityTamplateId, CargoHold StarterCargoHold)
        {
            ProductionLine NewProductionLine = new ProductionLine (ComodityTamplateId, Priority, StarterCargoHold);
            ProductionLines.Add (NewProductionLine);
            ProductionLines.Last ().Number = ProductionLines.Count;
            SignUpLine (ProductionLines.Last ());
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
            if (!_delegatesInitialised)
            {
                SignUpLines ();
                _delegatesInitialised = true;
            }
            //updateDelegate.Invoke ();
            ProvideMaterials (TimeStep);
            //foreach (ProductionLine line in _productionLines)
            //{
            //    line.Update (TimeStep);
            //}
            updateDelegate.Invoke (TimeStep);
        }

        private void SignUpLine(ProductionLine line)
        {
            line.Log -= information_Log;
            line.Log += information_Log;
            updateDelegate -= line.Update;
            updateDelegate += line.Update;
        }

        private void SignUpLines()
        {
            foreach (ProductionLine line in ProductionLines)
            {
                SignUpLine (line);
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
            if (Relations.IdToComodity.ContainsKey (_producedComodityId))
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
            else
                return false;
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
        int _baseCost;
        int _minCost;
        int _maxCost;
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

        [JsonProperty]
        public int BaseCost
        {
            get
            {
                return _baseCost;
            }

            set
            {
                int oldValue = _baseCost;
                _baseCost = value;
                if (OnCostChange != null && oldValue != _baseCost)
                    OnCostChange (this, new LogEventArgs (string.Format ("Base cost of {0} is changed to {1}", _title, _baseCost), GlobalData.CurrentTime));
            }
        }

        [JsonProperty]
        public int MinCost
        {
            get
            {
                return _minCost;
            }

            set
            {
                int oldValue = _minCost;
                _minCost = value;
                if (OnCostChange != null && oldValue != _minCost)
                    OnCostChange (this, new LogEventArgs (string.Format ("Minimal cost of {0} is changed to {1}", _title, _minCost), GlobalData.CurrentTime));
            }
        }

        [JsonProperty]
        public int MaxCost
        {
            get
            {
                return _maxCost;
            }

            set
            {
                int oldValue = _minCost;
                _maxCost = value;
                if (OnCostChange != null && oldValue != _maxCost)
                    OnCostChange (this, new LogEventArgs (string.Format ("Maximal cost of {0} is changed to {1}", _title, _maxCost), GlobalData.CurrentTime));
            }
        }

        public event EventHandler<LogEventArgs> OnCostChange;

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

        public void RecalculateCosts()
        {
            int newMinCost = 0;
            if (_requiredMaterials.Count > 0)
            {
                foreach (int key in _requiredMaterials.Keys)
                    newMinCost += Relations.IdToComodity [key]._baseCost;
                MinCost = newMinCost;
                if (MinCost * 1.5 > _baseCost)
                    BaseCost = MinCost * 2;
                else
                {
                    if (MinCost * 2.5 < _baseCost)
                    {
                        BaseCost = MinCost * 2;
                    }
                }
                MaxCost = _baseCost + Math.Abs (_baseCost - _minCost);
            }
            else
            {
                MinCost = _baseCost / 2;
                MaxCost = _baseCost * 2;
            }
        }

        public ComodityTemplate()
        {
            _requiredMaterials = new Dictionary<int, int> ();
            RecalculateCosts ();
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
        int _balance;
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

        public int Balance
        {
            get
            {
                return _balance;
            }

            set
            {
                _balance = value;
            }
        }

        //[XmlArray ("Owned Ships"), XmlArrayItem ("Ship")]
        [JsonProperty]
        public List<Fleet> ownedShips;
        public Owner()
        {
            ownedShips = new List<Summary.Fleet> ();
        }

        public Owner(int balance)
            : this ()
        {
            Balance = balance;
        }
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
