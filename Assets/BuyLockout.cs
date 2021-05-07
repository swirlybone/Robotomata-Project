using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyLockout : DestructibleWall
{
    [SerializeField] Job lockedOut;

    protected override void OnMouseDown()
    {
        SellMe();
    }

    private void Start()
    {
        
    }

    protected override void Sell(Notification notification)
    {
        if ((DestructibleWall)notification.Object == this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (lockedOut != null) NotificationCenter.PostNotification("Open", lockedOut);
    }
}
