using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
    public interface IMovable
    {
        double GetSpeed();
        Vector3 GetVelocity();
        Vector3 GetDirection();
        void SetDirection();
        Vector3 GetPosition();
        Vector3 GetDestPosition();
        ITargetable GetDestination();
        void SetDestination(Transform DestTransform);
        void Move(double TimeStep);
    }
    public interface IUnmovable
    {
        Vector3 GetPosition();
    }
    public interface IProducible
    {
        double GetProductionCost();
        TimeSpan GetProductionTime();
        Dictionary<int, int> GetRequiredMaterials();
    }
    public interface IStorable
    {
        double GetVolume();
        double GetMass();
    }
    public interface ITargetable
    {
        Vector3 GetPosition();
        Transform GetTransform();
    }
    public interface IDockable
    {

    }
    public interface IDock
    {

    }
    public interface IProductionLine
    {
        void SetProducibleComodity(int ComodityTamplateId);

        bool CheckResourseSatisfaction();
    }
    public interface IProduction
    {
        void AddProductionLine(ProductionLine NewProductionLine);
        void AddProductionLine(int Priority,int ComodityTamplateId, int CargoHoldCapacity);
        void AddProductionLine(int Priority, int ComodityTamplateId, CargoHold StarterCargoHold);
        Dictionary<int, int> GetProductionBalance();
    }
    public interface IManageable
    {
        bool IsIdle();
        Order CurrentOrder();
    }
    public interface IUpdatable
    {
        void Update(double TimeStep);
    }
    public interface ITrader
    {
        void FormTradeOffers();
        Dictionary<int, int> Sell(TradeOffer Offer, ref int Payment, int AmountToSell);
        bool Buy(TradeOffer Offer, ITrader Seller, int AmountToBuy);
        Dictionary<int, int> GetBuyOffers();
        Dictionary<int, int> GetSellOffers();
        ITargetable GetTarget();
    }
}
