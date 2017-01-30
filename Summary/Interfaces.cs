using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{
    interface IMovableObject
    {
        double GetSpeed();
        Vector3 GetVelocity();
        Vector3 GetDirection();
        void SetDirection();
        Vector3 GetPosition();
        Vector3 GetDestPosition();
        Object GetDestination();
        void SetDestination(Transform DestTransform);
        void Move(double TimeStep);
    }
    interface IUnmovableObject
    {
        Vector3 GetPosition();
    }
    interface IProducibleObject
    {
        double GetProductionCost();
        TimeSpan GetProductionTime();
        Dictionary<int, int> GetRequiredMaterials();
    }
    interface IStorableObject
    {
        double GetVolume();
        double GetMass();
    }
    interface IProduction
    {
        void SetProducibleComodity(int ComodityTamplateId);

        bool CheckResourseSatisfaction();
    }
    interface IManagable
    {
        bool IsIdle();
        string CurrentOrder();
    }
    interface IUpdatable
    {
        void Update(double TimeStep);
    }
}
