using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Shop : IShop
	{
		protected ZWebAPI.ShopActions shopActions;
		protected DataStore dataStore;
		protected ILogger logger;

		public Shop (ZWebAPI.ShopActions shopActions, DataStore dataStore, ILogger logger)
		{
			this.shopActions = shopActions;
			this.dataStore = dataStore;
			this.logger = logger;

			RoarManager.shopReadyEvent += () => CacheFromShop ();
		}

		public void Fetch (Roar.Callback<IDictionary<string,DomainObjects.ShopEntry> > callback)
		{
			dataStore.shop.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.shop.HasDataFromServer; } }

		public void Buy (string shop_ikey, Roar.Callback<WebObjects.Shop.BuyResponse> callback)
		{
			ShopBuy (shop_ikey, callback);
		}

		public IList<DomainObjects.ShopEntry> List ()
		{
			return dataStore.shop.List();
		}


		// Returns the *object* associated with attribute `key`
		public DomainObjects.ShopEntry GetShopItem (string ikey)
		{
			return dataStore.shop.Get (ikey);
		}

		public void ShopBuy (string shop_ikey, Roar.Callback<WebObjects.Shop.BuyResponse> cb)
		{
			var shop_item = dataStore.shop.Get (shop_ikey);

			// Make the call if the item is in the shop
			if (shop_item == null) {
				logger.DebugLog ("[roar] -- Cannot find to purchase: " + shop_ikey);
				return;
			}
			logger.DebugLog ("trying to buy me a : " + shop_item.as_json() );

			WebObjects.Shop.BuyArguments args = new Roar.WebObjects.Shop.BuyArguments();
			args.shop_item_ikey = shop_item.ikey;

			shopActions.buy (args, new ShopBuyCallback (cb, this, shop_item.ikey));
		}

		protected class ShopBuyCallback : CBBase<WebObjects.Shop.BuyResponse>
		{
			//Shop shop;
			string ikey;

			public ShopBuyCallback ( Roar.Callback<WebObjects.Shop.BuyResponse> in_cb, Shop in_shop, string in_ikey) : base(in_cb)
			{
				//shop = in_shop;
				ikey = in_ikey;
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Shop.BuyResponse> info)
			{
				//TODO: This needs to be implemented!
				//IXMLNode result = info.data.GetNode ("roar>0>shop>0>buy>0");

				// Obtain the server id for the purchased item
				// NOTE: Assumes ONLY ONE item per "shopitem" entity
				//TODO : So this assumption needs to be fixed!

				//IXMLNode cost = result.GetNode ("costs>0>cost>0");
				//IXMLNode item = result.GetNode ("modifiers>0>modifier>0");


				//string id = item.GetAttribute ("item_id");

				//RoarManager.OnGoodBought (
				//	new RoarManager.PurchaseInfo (
				//		cost.GetAttribute ("ikey"), //currency_name
				//		int.Parse (cost.GetAttribute ("value")), // item_price
				//		ikey, // iitem_id
				//		1                                      //item_qty
				//));

				//Hashtable data = new Hashtable ();
				//data ["id"] = id;
				//data ["ikey"] = ikey;
			}
		}

		// Builds a list of items to fetch from Server by comparing
		// what's in the Shop list and what's currently in the cache
		public bool CacheFromShop ()
		{
			if (dataStore.shop.HasDataFromServer) {
				// Build sanitised ARRAY of ikeys from Shop.list()
				IList<DomainObjects.ShopEntry> l = dataStore.shop.List ();
				var ikeyList = new List<string> ();

				foreach (DomainObjects.ShopEntry v in l) {
					// TODO: This is a new-style fudge that should be undone.
					ikeyList.Add (v.ikey);
				}

				return dataStore.cache.AddToCache (ikeyList);
			} else
				return false;
		}



	}

}
