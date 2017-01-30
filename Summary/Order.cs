using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
    public abstract class Order
    {
        protected bool _inProgress;
        protected IMovable _actor;
        protected ITargetable _obj;
        protected Queue<Order> _subOrders;
        public IMovable Actor
        {
            get
            {
                return _actor;
            }

            set
            {
                _actor = value;
            }
        }

        public ITargetable Obj
        {
            get
            {
                return _obj;
            }

            set
            {
                _obj = value;
            }
        }

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

        public Order(IMovable actor, ITargetable obj)
        {
            _subOrders = new Queue<Order> ();
        }
    }
    public class TradeOrder : Order
    {
        ITrader _actorTrader;
        ITrader _objTrader;
        readonly double successDistance = 0.5;

        public TradeOrder(IMovable actor, ITargetable obj, ITrader actorTrader, ITrader objTrader) : base (actor, obj)
        {
            _actorTrader = actorTrader;
            _objTrader = objTrader;

        }

        public override bool Act()
        {
            throw new NotImplementedException ();
        }

        public override void BuildSubOrdersQueue()
        {
            if (Vector3.CalculateLength (_obj.GetPosition () - _actor.GetPosition ()) > successDistance)
            {
                _subOrders.Enqueue (new MoveOrder (_actor, _obj));
                _subOrders.Enqueue (new TradeOrder (_actor, _obj,_actorTrader,_objTrader));
            }
        }
    }
    public class MoveOrder : Order
    {
        readonly double successDistance = 0.5;

        public MoveOrder(IMovable actor, ITargetable obj) : base (actor, obj)
        {
        }

        public override bool Act()
        {
            if (Vector3.CalculateLength (_obj.GetPosition () - _actor.GetPosition ()) > successDistance)
            {
                _actor.Move (GlobalData.TimeStep);
                return false;
            }
            else
                return true;
        }

        public override void BuildSubOrdersQueue()
        {
            throw new NotImplementedException ();
        }        
    }
}
