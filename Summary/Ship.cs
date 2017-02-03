using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{

    public class ShipBase : IMovable, IProducible, IStorable, ITargetable, ITrader, IUpdatable
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
        Owner _shipOwner;
        //[XmlElement ("CargoHold")]
        CargoHold _shipCargoHold;
        Order _shipOrder;
        bool _isIdle;
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

        [JsonProperty]
        public Owner ShipOwner
        {
            get
            {
                return _shipOwner;
            }

            set
            {
                _shipOwner = value;
            }
        }

        [JsonProperty]
        public Order ShipOrder
        {
            get
            {
                return _shipOrder;
            }

            private set
            {
                _shipOrder = value;
            }
        }

        [JsonProperty]
        public bool IsIdle
        {
            get
            {
                return _isIdle;
            }

            private set
            {
                _isIdle = value;
                //if (_isIdle)
                //    Log.Invoke (this, new LogEventArgs (string.Format ("Ship {0}-{1} is idle now.", _title, _id), GlobalData.CurrentTime));
                //else
                //    Log.Invoke (this, new LogEventArgs (string.Format ("Ship {0}-{1} is idle now.", _title, _id), GlobalData.CurrentTime));
            }
        }

        public event EventHandler<LogEventArgs> Log;
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

        public Transform GetTransform()
        {
            return _objTransform;
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
                ObjTransform.Velocity += ObjTransform.Direction * (Acceleration * TimeStep*0.001);
            }
            if (ObjTransform.Velocity.Length > (DestTransform.Position - ObjTransform.Position).Length)
                ObjTransform.Velocity = (DestTransform.Position - ObjTransform.Position);
            CurrentSpeed = ObjTransform.Velocity.Length;
            //if (ObjTransform.Position.Length > DestTransform.Position.Length)
            //{
            //    ObjTransform.Velocity = new Vector3 ();
            //    SetDirection ();
            //    if (Log != null)
            //        Log (this, new LogEventArgs ("Movement failed", GlobalData.CurrentTime));
            //}
            ObjTransform.Translate (TimeStep);
        }

        public void SetDirection()
        {
            ObjTransform.Direction = ~(DestTransform.Position - ObjTransform.Position);
            ObjTransform.Velocity = ObjTransform.Direction * (Acceleration * (GlobalData.TimeStep * 0.001));
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

        public ITargetable GetTarget()
        {
            return this;
        }

        public Dictionary<int, int> Sell(TradeOffer Offer, ref int Payment, int AmountToSell)
        {
            // TODO: Add sell logic.
            Dictionary<int, int> extacted = new Dictionary<int, int> ();
            if (Offer != null)
            {
                int amountToExtract = Payment / Offer.Price;
                extacted = _shipCargoHold.Extract (Offer.ComodityId, amountToExtract);
                ShipOwner.Balance += amountToExtract * Offer.Price;
                Payment -= amountToExtract * Offer.Price;
            }
            else
            {
                extacted.Add (Offer.ComodityId, 0);
            }
            return extacted;
        }

        public bool Buy(TradeOffer Offer, ITrader Seller, int AmountToBuy)
        {
            // TODO: Add buy logic.
            Dictionary<int, int> extacted = new Dictionary<int, int> ();
            if (Offer != null)
            {
                int amountToBuy = 0;

                // TODO: Check amount calculation logic.
                if (Offer.Amount < AmountToBuy)
                {
                    if (CargoHold.CalculateOccupaiedVolume (Offer.ComodityId, Offer.Amount) <= _shipCargoHold.FreeSpace)
                    {
                        amountToBuy = Offer.Amount;//?
                    }
                    else
                    {
                        amountToBuy = CargoHold.CalculateAmountToFillVolume (Offer.ComodityId, _shipCargoHold.FreeSpace);//?
                    }
                }
                else
                {
                    if (CargoHold.CalculateOccupaiedVolume (Offer.ComodityId, AmountToBuy) <= _shipCargoHold.FreeSpace)
                    {
                        amountToBuy = AmountToBuy;//?
                    }
                    else
                    {
                        amountToBuy = CargoHold.CalculateAmountToFillVolume (Offer.ComodityId, _shipCargoHold.FreeSpace);//?
                    }
                }
                if (amountToBuy > 0)
                {
                    int Payment = amountToBuy * Offer.Price < _shipOwner.Balance ? amountToBuy * Offer.Price : _shipOwner.Balance;
                    _shipOwner.Balance -= Payment;
                    extacted = Seller.Sell (Offer, ref Payment, amountToBuy);
                    int extractedAmout = extacted [Offer.ComodityId];
                    _shipOwner.Balance += Payment;
                    _shipCargoHold.Store (Offer.ComodityId, ref extractedAmout);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public void Update(double TimeStep)
        {
            lock (this)
            {
                try
                {
                    if (IsIdle)
                        FreeTrade ();
                    else
                    {
                        if (ShipOrder.Act ())
                        {
                            IsIdle = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private void FreeTrade()
        {
            List<TradeRunOrder> findedTradeRuns = new List<TradeRunOrder> ();
            foreach (int key in Relations.IdToComodity.Keys)
            {
                TradeRunOrder findedTradeRun = FindTradeRun (key);
                if (findedTradeRun != null)
                    findedTradeRuns.Add (findedTradeRun);
            }
            findedTradeRuns.Sort (delegate (TradeRunOrder x, TradeRunOrder y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;
                else
                    return x.CompareTo (y);
            });
            if (findedTradeRuns.Count > 0)
            {
                ShipOrder = new TradeRunOrder (findedTradeRuns.Last ());
                ShipOrder.InProgress = true;
                IsIdle = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComodityId"></param>
        /// <returns></returns>
        private TradeRunOrder FindTradeRun(int ComodityId)
        {
            int bestBuyIndex = -1;
            int bestSellIndex = -1;
            int bestProfit = 0;
            int bestAmountToTrade = 0;
            List<TradeOffer> buyOffers = GlobalData.buyOffers.FindAll (x => x.ComodityId == ComodityId);
            buyOffers.Sort (delegate (TradeOffer x, TradeOffer y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;
                else
                    return x.CompareTo (y);
            });
            List<TradeOffer> sellOffers = GlobalData.sellOffers.FindAll (x => x.ComodityId == ComodityId);
            sellOffers.Sort (delegate (TradeOffer x, TradeOffer y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;
                else
                    return x.CompareTo (y);
            });
            for (int i = 0; i < sellOffers.Count; i++)
            {
                for (int j = 0; j < buyOffers.Count; j++)
                {
                    int amountToTrade = 0;
                    if (buyOffers [j].Price < sellOffers [i].Price)
                        continue;
                    int currentDealProfit = CalculateProfit (sellOffers [i], buyOffers [j], ref amountToTrade);
                    if (bestProfit < currentDealProfit)
                    {
                        bestProfit = currentDealProfit;
                        bestBuyIndex = j;
                        bestSellIndex = i;
                        bestAmountToTrade = amountToTrade;
                    }
                }
            }
            if (bestBuyIndex > -1 && bestSellIndex > -1 && bestProfit > 0 && bestAmountToTrade > 0)
                return new TradeRunOrder (bestProfit, bestAmountToTrade, this, sellOffers [bestSellIndex], buyOffers [bestBuyIndex]);
            else
                return null;
        }

        private int CalculateProfit(TradeOffer SellOffer, TradeOffer BuyOffer, ref int AmountToTrade)
        {
            AmountToTrade = _shipOwner.Balance / SellOffer.Price > SellOffer.Amount ? SellOffer.Amount : _shipOwner.Balance / SellOffer.Price;
            return BuyOffer.Amount < AmountToTrade ? BuyOffer.Amount * (BuyOffer.Price - SellOffer.Price) : AmountToTrade * (BuyOffer.Price - SellOffer.Price);
        }
        #endregion

        #region Constructors
        public ShipBase()
        {
            _isIdle = true;
            _objTransform = new Transform ();
            _shipCargoHold = new CargoHold ();
            //_objTransform.Log += _objTransform_Log;
        }

        public ShipBase(Transform ShipTransform)
            : this ()
        {
            _objTransform = ShipTransform;
            //_objTransform.Log += _objTransform_Log;
        }

        public ShipBase(Transform ShipTransform, CargoHold ShipCargoHold)
            : this (ShipTransform)
        {
            _shipCargoHold = ShipCargoHold;
        }

        public ShipBase(Transform ShipTransform, CargoHold ShipCargoHold, Owner ShipOwner)
            : this (ShipTransform, ShipCargoHold)
        {
            _shipOwner = ShipOwner;
        }
        #endregion

        private void _objTransform_Log(object sender, LogEventArgs e)
        {
            LogAgregator.Log (new LogEventArgs (string.Format ("Ship {0}-{1} - {3}", _title, _id, e.Message), e.EventTime));
        }

    }

}
