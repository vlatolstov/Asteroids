using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;

namespace _Project.Runtime.InAppPurchase
{
    public interface IIapService
    {
        event Action PurchasesChanged;
        IReadOnlyList<Product> Products { get; }
        UniTask Connect();
        void FetchProducts();
        void Purchase(string productId);
        bool RestoreTransactions();
    }
}
