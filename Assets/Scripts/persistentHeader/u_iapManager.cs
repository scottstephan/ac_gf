using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;

public class u_iapManager : IStoreListener
{

    private IStoreController controller;
    private IExtensionProvider extensions;
    public string attempt_CategoryPurchaseName;
    public enum IAPTypes
    {
        noAds,
        categoryCredit
    }

    public enum androidIAPID
    {
        ac_gp_noads,
        ac_gp_categorycredit
    }

    public enum iOSIAPID
    {
        ac_ios_noads,
        ac_ios_categoryCredit
    }

    public u_iapManager()
    {
        Debug.Log("---IAP MANAGER STARTED---");

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(IAPTypes.noAds.ToString(), ProductType.NonConsumable, new IDs
        {
            {androidIAPID.ac_gp_noads.ToString(), GooglePlay.Name},
            {iOSIAPID.ac_ios_noads.ToString(), MacAppStore.Name}
        });

        builder.AddProduct(IAPTypes.categoryCredit.ToString(), ProductType.NonConsumable, new IDs
        {
            {androidIAPID.ac_gp_categorycredit.ToString(), GooglePlay.Name},
            {iOSIAPID.ac_ios_categoryCredit.ToString(), MacAppStore.Name}
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
        appManager.IAPInitialized = false;
        Debug.Log("IAP FAILURE: " + error);
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("PURCHASE SUCCESS: " + e.purchasedProduct.definition.id);
        
        switch (e.purchasedProduct.definition.id)
        {
            case "categoryCredit":
                Debug.Log("BUYING: " + attempt_CategoryPurchaseName);
                obj_playerIAPData.addCredit();
                obj_playerIAPData.removeCredit();
                u_acJsonUtility.instance.findAndUnlockCategory(attempt_CategoryPurchaseName);
                m_iapShopPanelManager.instance.refreshIAPStore();
                attempt_CategoryPurchaseName = null;
                break;
            case "noAds":
                break;
        }
        // actOnPurchaseSuccess(IAPTypes.categoryCredit);
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("PURCHASE FAILED: " + p.ToString());
    }

    public void BuyProductID(string productId, string categoryName)
    {
        Debug.Log("TRYING TO BUY: " + productId);
        attempt_CategoryPurchaseName = categoryName;
        // If Purchasing has been initialized ...
        if (appManager.IAPInitialized)
        {
            Product product = controller.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                controller.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                attempt_CategoryPurchaseName = null;
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            Debug.Log("BuyProductID FAIL. Not initialized.");
            attempt_CategoryPurchaseName = null;
        }
    }

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!appManager.IAPInitialized)
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = extensions.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

}

