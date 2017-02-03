using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
    public abstract class Order
    {
        [JsonProperty]
        protected bool _inProgress;
        [JsonProperty]
        protected Queue<Order> _subOrders;
        public bool InProgress
        {
            get
            {
                return _inProgress;
            }

            set
            {
                _inProgress = value;
                if (_inProgress)
                    BuildSubOrdersQueue ();
            }
        }

        public abstract bool Act();

        public abstract void BuildSubOrdersQueue();

        public Order()
        {
            try
            {
                _inProgress = false;
                _subOrders = new Queue<Order> ();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
    public class TradeOrder : Order
    {
        [JsonProperty]
        int _amountToTrade;
        [JsonProperty]
        ShipBase _actor;
        [JsonProperty]
        TradeOffer _tradeOffer;
        [JsonProperty]
        ITrader _objTrader;
        [JsonProperty]
        readonly double successDistance = 0.5;

        public override bool Act()
        {
            try
            {
                if (Vector3.CalculateLength (_objTrader.GetTarget ().GetPosition () - _actor.GetPosition ()) > successDistance)
                    BuildSubOrdersQueue ();
                if (_subOrders.Count == 0)
                {
                    //TODO: Add trade processing logic.
                    switch (_tradeOffer.OfferType)
                    {
                        case TradeOfferType.Sell:
                            _actor.Buy (_tradeOffer, _tradeOffer.Offerer, _amountToTrade);
                            break;
                        case TradeOfferType.Buy:
                            int payment = 0;
                            _tradeOffer.Offerer.Buy (_tradeOffer, _actor, _amountToTrade);
                            _actor.ShipOwner.Balance += payment;
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                else
                {
                    if (_subOrders.Peek ().Act ())
                    {
                        _subOrders.Dequeue ();
                        if (_subOrders.Count > 0)
                            _subOrders.Peek ().InProgress = true;
                        else
                            return false;
                    }
                    return false;
                }

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public override void BuildSubOrdersQueue()
        {
            try
            {
                _subOrders.Clear ();
                if (Vector3.CalculateLength (_objTrader.GetTarget ().GetPosition () - _actor.GetPosition ()) > successDistance)
                {
                    _subOrders.Enqueue (new MoveOrder (_actor, _objTrader.GetTarget ()));
                    //_subOrders.Enqueue (new TradeOrder (_actor, _tradeOffer));
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public TradeOrder(ShipBase actor, TradeOffer tradeOffer, int amountToTrade)
        {
            try
            {
                _actor = actor;
                _tradeOffer = tradeOffer;
                _objTrader = tradeOffer.Offerer;
                _amountToTrade = amountToTrade;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
    public class MoveOrder : Order
    {
        IMovable _actor;
        ITargetable _obj;
        readonly double successDistance = 0.5;

        public MoveOrder(IMovable actor, ITargetable obj)
        {
            try
            {
                _actor = actor;
                _obj = obj;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public override bool Act()
        {
            try
            {
                _actor.SetDestination (_obj.GetTransform ());
                if (Vector3.CalculateLength (_obj.GetPosition () - _actor.GetPosition ()) > successDistance)
                {
                    if (~_actor.GetVelocity () != _actor.GetDirection ())
                        _actor.SetDirection ();
                    _actor.Move (GlobalData.TimeStep);
                    return false;
                }
                else
                    return true;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public override void BuildSubOrdersQueue()
        {
            // TODO: Add logic for trans-sector movement.
            //throw new NotImplementedException ();
        }
    }

    public class TradeRunOrder : Order, IComparable<TradeRunOrder>, IEquatable<TradeRunOrder>
    {
        [JsonProperty]
        int _probableProfit;
        [JsonProperty]
        ShipBase _actor;
        [JsonProperty]
        TradeOffer _sellOffer;
        [JsonProperty]
        TradeOffer _buyOffer;
        int _amountToTrade;

        public int ProbableProfit
        {
            get
            {
                return _probableProfit;
            }

            set
            {
                _probableProfit = value;
            }
        }

        public override bool Act()
        {
            try
            {
                if (_subOrders.Count == 0)
                {
                    return true;
                }
                else
                {
                    if (_subOrders.Peek ().Act ())
                    {
                        _subOrders.Dequeue ();
                        if (_subOrders.Count > 0)
                            _subOrders.Peek ().InProgress = true;
                        else
                            return false;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public override void BuildSubOrdersQueue()
        {
            _subOrders.Enqueue (new TradeOrder (_actor, _sellOffer, _amountToTrade));
            _subOrders.Enqueue (new TradeOrder (_actor, _buyOffer, _amountToTrade));
        }

        public TradeRunOrder(int ProbableProfit, int AmountToTrade, ShipBase Actor, TradeOffer SellOffer, TradeOffer BuyOffer)
        {
            try
            {
                _probableProfit = ProbableProfit;
                _amountToTrade = AmountToTrade;
                _actor = Actor;
                _sellOffer = SellOffer;
                _buyOffer = BuyOffer;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public TradeRunOrder(TradeRunOrder tradeRunOrder)
        {
            try
            {
                _probableProfit = tradeRunOrder._probableProfit;
                _amountToTrade = tradeRunOrder._amountToTrade;
                _actor = tradeRunOrder._actor;
                _sellOffer = tradeRunOrder._sellOffer;
                _buyOffer = tradeRunOrder._buyOffer;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int CompareTo(TradeRunOrder other)
        {
            if (other == null)
                return 1;

            else
                return this._probableProfit.CompareTo (other._probableProfit);
        }

        public bool Equals(TradeRunOrder other)
        {
            if (other == null)
                return false;
            return (this._probableProfit.Equals (other._probableProfit));
        }

    }
}
