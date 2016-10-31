using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;

public class u_iapManager : IStoreListener {
    
    private IStoreController controller;
    private IExtensionProvider extensions;

    public u_iapManager()
    {
        Debug.Log("---IAP MANAGER STARTED---");

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("removeAds", ProductType.NonConsumable, new IDs
        {
            {"ac_gp_noads", GooglePlay.Name},
            {"removeAds", MacAppStore.Name}
        });

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("---IAP INITIALIZED---");
        this.controller = controller;
        this.extensions = extensions;
        appManager.IAPInitialized = true;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
    }
}
