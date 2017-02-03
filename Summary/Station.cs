using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
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
        //protected String _ownerTitle;
        //[XmlElement ("OwnerID")]
        //protected int _ownerID;
        Owner _stationOwner;
        //[XmlElement ("CargoHold")]
        protected CargoHold _stationCargoHold;
        protected UpdateDelegate updateDelegate;
        //private Trade _tradeOffers;
        protected List<TradeOffer> _buyOffers;
        protected List<TradeOffer> _sellOffers;
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

        //[JsonProperty]
        //public String OwnerTitle
        //{
        //    get
        //    {
        //        return _ownerTitle;
        //    }

        //    set
        //    {
        //        _ownerTitle = value;
        //    }
        //}

        //[JsonProperty]
        //public int OwnerID
        //{
        //    get
        //    {
        //        return _ownerID;
        //    }

        //    set
        //    {
        //        _ownerID = value;
        //    }
        //}

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
        public List<TradeOffer> BuyOffers
        {
            get
            {
                return _buyOffers;
            }

            private set
            {
                _buyOffers = value;
            }
        }

        [JsonProperty]
        public List<TradeOffer> SellOffers
        {
            get
            {
                return _sellOffers;
            }

            private set
            {
                _sellOffers = value;
            }
        }

        [JsonProperty]
        public Owner StationOwner
        {
            get
            {
                return _stationOwner;
            }

            set
            {
                _stationOwner = value;
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

        public ITargetable GetTarget()
        {
            return this;
        }

        public Dictionary<int, int> Sell(TradeOffer Offer, ref int Payment, int AmountToSell)
        {
            // TODO: Polish sell logic.
            TradeOffer currentOffer = _sellOffers.Find (x => x.ComodityId == Offer.ComodityId);
            Dictionary<int, int> extacted = new Dictionary<int, int> ();
            if (currentOffer != null)
            {
                int amountToExtract = Payment / currentOffer.Price;
                extacted = _stationCargoHold.Extract (Offer.ComodityId, amountToExtract);
                _stationOwner.Balance += amountToExtract * currentOffer.Price;
                Payment -= amountToExtract * currentOffer.Price;
            }
            else
            {
                extacted.Add (Offer.ComodityId, 0);
            }
            return extacted;
        }

        public bool Buy(TradeOffer Offer, ITrader Seller, int AmountToBuy)
        {
            // TODO: Polish sell logic.
            TradeOffer currentOffer = _buyOffers.Find (x => x.ComodityId == Offer.ComodityId);
            Dictionary<int, int> extacted = new Dictionary<int, int> ();
            if (currentOffer != null)
            {
                int amountToBuy = 0;
                if (Offer.Amount < AmountToBuy)
                {
                    if (CargoHold.CalculateOccupaiedVolume (Offer.ComodityId, Offer.Amount) <= _stationCargoHold.FreeSpace)
                    {
                        amountToBuy = Offer.Amount;//?
                    }
                    else
                    {
                        amountToBuy = CargoHold.CalculateAmountToFillVolume (Offer.ComodityId, _stationCargoHold.FreeSpace);//?
                    }
                }
                else
                {
                    if (CargoHold.CalculateOccupaiedVolume (Offer.ComodityId, AmountToBuy) <= _stationCargoHold.FreeSpace)
                    {
                        amountToBuy = AmountToBuy;//?
                    }
                    else
                    {
                        amountToBuy = CargoHold.CalculateAmountToFillVolume (Offer.ComodityId, _stationCargoHold.FreeSpace);//?
                    }
                }
                if (amountToBuy > 0)
                {
                    int Payment = amountToBuy * currentOffer.Price < _stationOwner.Balance ? amountToBuy * currentOffer.Price : _stationOwner.Balance;
                    _stationOwner.Balance -= Payment;
                    extacted = Seller.Sell (currentOffer, ref Payment, amountToBuy);
                    int extractedAmout = extacted [Offer.ComodityId];
                    _stationOwner.Balance += Payment;
                    _stationCargoHold.Store (Offer.ComodityId, ref extractedAmout);
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

        public Transform GetTransform()
        {
            return _objTransform;
        }
        #endregion

        #region Constructors
        public StationBase()
        {
            _objTransform = new Transform ();
            _stationCargoHold = new CargoHold ();
            _buyOffers = new List<TradeOffer> ();
            _sellOffers = new List<TradeOffer> ();
            _stationCargoHold.Log += _stationCargoHold_Log;
        }

        protected void _stationCargoHold_Log(object sender, LogEventArgs e)
        {
            LogAgregator.Log (new LogEventArgs (_title + " - " + e.Message, e.EventTime));
        }
        #endregion
    }
    public class PlantStation : StationBase, IProduction
    {
        #region Fields
        //[XmlElement ("Production")]
        Production _production;
        Dictionary<int, int> _productionBalance;
        #endregion

        #region Properties
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
        #endregion

        #region Constructors
        public PlantStation()
        {
            _production = new Production ();
            _productionBalance = new Dictionary<int, int> ();
            _production.Log += _production_Log;
            updateDelegate += _production.Update;
            updateDelegate += ProvideMaterials;
            Production.ProductionCargoHold.Capacity += 500;
        }
        #endregion

        #region Methods
        public override void Update(double TimeStep)
        {
            lock (this)
            {
                //updateDelegate.Invoke (TimeStep);
                _production.Update (TimeStep);
                ExtractProduction ();
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
            int totalPositiveBalanceUnits = 0;
            foreach (int key in _productionBalance.Keys)
            {
                if (_productionBalance [key] < 0)
                    totalNegativeBalanceUnits -= _productionBalance [key];
                else
                    totalPositiveBalanceUnits += _productionBalance [key];
            }
            foreach (int key in _productionBalance.Keys)
            {
                if (!_stationCargoHold.StoredComodities.ContainsKey (key))
                    _stationCargoHold.StoredComodities.Add (key, 0);
                if (_productionBalance [key] >= 0)
                {
                    int AllowableAmount = CargoHold.CalculateAmountToFillVolume (key, (int) ((double) (_productionBalance [key]) / totalPositiveBalanceUnits * 0.5 * StationCargoHold.FreeSpace));
                    TradeOffer offer = SellOffers.Find (x => x.ComodityId == key);
                    if (offer != null)
                    {
                        offer.Amount = StationCargoHold.StoredComodities [key];
                        offer.Price = AuxMath.CalculatePriceLinear (Relations.IdToComodity [key].MinCost, Relations.IdToComodity [key].BaseCost, Relations.IdToComodity [key].MaxCost, offer.Amount, AllowableAmount);
                    }
                    else
                    {
                        SellOffers.Add (new TradeOffer (key, StationCargoHold.StoredComodities [key], AuxMath.CalculatePriceLinear (Relations.IdToComodity [key].MinCost, Relations.IdToComodity [key].BaseCost, Relations.IdToComodity [key].MaxCost, StationCargoHold.StoredComodities [key], AllowableAmount), TradeOfferType.Sell, this));
                        GlobalData.sellOffers.Add (SellOffers.Last ());
                    }
                }
                else
                {
                    int AllowableAmount = CargoHold.CalculateAmountToFillVolume (key, -(int) ((double) (_productionBalance [key]) / totalNegativeBalanceUnits * 0.5 * StationCargoHold.FreeSpace));
                    TradeOffer offer = BuyOffers.Find (x => x.ComodityId == key);
                    if (offer != null)
                    {
                        offer.Amount = AllowableAmount - _stationCargoHold.StoredComodities [key];
                        offer.Price = AuxMath.CalculatePriceLinear (Relations.IdToComodity [key].MinCost, Relations.IdToComodity [key].BaseCost, Relations.IdToComodity [key].MaxCost, _stationCargoHold.StoredComodities [key], AllowableAmount);
                    }
                    else
                    {
                        BuyOffers.Add (new TradeOffer (key, AllowableAmount - _stationCargoHold.StoredComodities [key], AuxMath.CalculatePriceLinear (Relations.IdToComodity [key].MinCost, Relations.IdToComodity [key].BaseCost, Relations.IdToComodity [key].MaxCost, _stationCargoHold.StoredComodities [key], AllowableAmount), TradeOfferType.Buy, this));
                        GlobalData.buyOffers.Add (BuyOffers.Last ());
                    }
                }
            }
        }

        public override Dictionary<int, int> GetBuyOffers()
        {
            //return TradeOffers.BuyOffers;
            throw new NotImplementedException ();
        }

        public override Dictionary<int, int> GetSellOffers()
        {
            //return TradeOffers.SellOffers;
            throw new NotImplementedException ();
        }
        #endregion

        #region Handlers
        private void _production_Log(object sender, LogEventArgs e)
        {
            LogAgregator.Log (new LogEventArgs (_title + " - " + e.Message, e.EventTime));
        }
        #endregion
    }
}
