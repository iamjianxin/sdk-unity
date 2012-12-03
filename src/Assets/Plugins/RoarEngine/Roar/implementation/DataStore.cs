using DC = Roar.implementation.DataConversion;
using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;


public interface IDomToCache<DT,CT>
{
	Dictionary<string,CT> convert(DT d);
}

public class UserViewToProperty : IDomToCache<Roar.WebObjects.User.ViewResponse, Property>
{
	public Dictionary<string,Property> convert(Roar.WebObjects.User.ViewResponse d)
	{
		Dictionary<string,Property> retval = new Dictionary<string, Property>();
		//TODO: Duplication between PlayerAttribute class and Property class is not usefull
		//      we probably only need one.
		foreach( Roar.WebObjects.User.PlayerAttribute p in d.attributes)
		{
			Property pp = new Property();
			pp.label = p.label;
			pp.value = p.value;
			//TODO: Copy the data across.
			retval[p.ikey] = pp;
		}
		//TODO: Implement this
		return retval;
	}
}

public class ItemsListToItem : IDomToCache<Roar.WebObjects.Items.ListResponse,Roar.DomainObjects.InventoryItem>
{
	public Dictionary<string, Roar.DomainObjects.InventoryItem> convert( Roar.WebObjects.Items.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.InventoryItem> retval = new Dictionary<string, Roar.DomainObjects.InventoryItem>();
		//TODO: Implement this
		return retval;
	}
}

public class ShopListToShopEntry : IDomToCache<Roar.WebObjects.Shop.ListResponse,Roar.DomainObjects.ShopEntry>
{
	public Dictionary<string, Roar.DomainObjects.ShopEntry> convert( Roar.WebObjects.Shop.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.ShopEntry> retval = new Dictionary<string, Roar.DomainObjects.ShopEntry>();
		//TODO: Implement this
		return retval;
	}
}

public class LeaderboardListToLeaderboard : IDomToCache<Roar.WebObjects.Leaderboards.ListResponse,Roar.DomainObjects.Leaderboard>
{
	public Dictionary<string, Roar.DomainObjects.Leaderboard> convert( Roar.WebObjects.Leaderboards.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.Leaderboard> retval = new Dictionary<string, Roar.DomainObjects.Leaderboard>();
		//TODO: Implement this
		return retval;
	}
}

public class FriendsListToFriend : IDomToCache<Roar.WebObjects.Friends.ListResponse,Roar.DomainObjects.Friend>
{
	public Dictionary<string, Roar.DomainObjects.Friend> convert( Roar.WebObjects.Friends.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.Friend> retval = new Dictionary<string, Roar.DomainObjects.Friend>();
		//TODO: Implement this
		return retval;
	}
}

public class ItemsViewToItemPrototype: IDomToCache<Roar.WebObjects.Items.ViewResponse,Roar.DomainObjects.ItemPrototype>
{
	public Dictionary<string, Roar.DomainObjects.ItemPrototype> convert( Roar.WebObjects.Items.ViewResponse d)
	{
		Dictionary<string,Roar.DomainObjects.ItemPrototype> retval = new Dictionary<string, Roar.DomainObjects.ItemPrototype>();
		//TODO: Implement this
		return retval;
	}
}

public class FooToFoo : IDomToCache<Foo,Foo>
{
	public Dictionary<string, Foo> convert( Foo d)
	{
		throw new System.NotImplementedException();
	}
}


public interface IDomGetter<DT>
{
	void get( ZWebAPI.Callback<DT> cb );
	

}

	
	
public class GenericGetter<T,A> : IDomGetter<T> where A:new()
{
	public delegate void GetFn( A args, ZWebAPI.Callback<T> cb );
	
	GetFn getfn;
	
	public GenericGetter( GetFn getfn ) 
	{
		this.getfn = getfn;
	}
	
	
	public void get( ZWebAPI.Callback<T> cb )
	{
		A args = new A();
		getfn(args,cb);
	}
}

public class UserViewGetter : GenericGetter<Roar.WebObjects.User.ViewResponse, Roar.WebObjects.User.ViewArguments>
{
	public UserViewGetter( IWebAPI api ) : base( api.user.view ) {}
}

public class ItemsListGetter : GenericGetter<Roar.WebObjects.Items.ListResponse, Roar.WebObjects.Items.ListArguments>
{
	public ItemsListGetter( IWebAPI api ) :base( api.items.list ) {}
}

public class ShopListGetter : GenericGetter<Roar.WebObjects.Shop.ListResponse, Roar.WebObjects.Shop.ListArguments>
{
	public ShopListGetter( IWebAPI api ) :base( api.shop.list ) {}
}

public class LeaderboardListGetter : GenericGetter<Roar.WebObjects.Leaderboards.ListResponse, Roar.WebObjects.Leaderboards.ListArguments>
{
	public LeaderboardListGetter( IWebAPI api ) : base( api.leaderboards.list ) {}
}

public class FriendsListGetter : GenericGetter<Roar.WebObjects.Friends.ListResponse, Roar.WebObjects.Friends.ListArguments>
{
	public FriendsListGetter( IWebAPI api ) : base( api.friends.list ) {}
}

public class ItemsViewGetter : GenericGetter<Roar.WebObjects.Items.ViewResponse, Roar.WebObjects.Items.ViewArguments>
{
	public ItemsViewGetter( IWebAPI api ) : base( api.items.view ) {}
}

public class FooGetter : IDomGetter<Foo>
{
	public FooGetter( IWebAPI api ) 
	{
		this.api = api;
	}
	
	protected IWebAPI api;
	public void get( ZWebAPI.Callback<Foo> cb )
	{
		throw new System.NotImplementedException();
	}
}

public class Property
{
//TODO: Fix up the types in here!
	public string value;
	public string type;
	public string max;
	public string min;
	public string regen_every;
	public string label;
	
}


namespace Roar.implementation
{
	public interface IDataStore
	{
		void Clear (bool x);
		
		IDataModel<Property,Roar.WebObjects.User.ViewResponse> properties { get; }
		IDataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory { get; }
		IDataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop { get; }
		IDataModel<Foo,Foo> actions { get; }
		IDataModel<Foo,Foo> gifts { get; }
		IDataModel<Foo,Foo> achievements { get; }
		IDataModel<DomainObjects.Leaderboard,WebObjects.Leaderboards.ListResponse> leaderboards { get; }
		IDataModel<Foo,Foo> ranking { get; }
		IDataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends { get; }
		IDataModel<Foo,Foo> appStore { get; }
		IItemCache cache { get; }
	}


	public class DataStore : IDataStore
	{
		public DataStore (IWebAPI webapi, ILogger logger)
		{
			//This should convert from the response type to key-value pairs
			
			properties_ = new DataModel<Property,Roar.WebObjects.User.ViewResponse> ("properties", new UserViewGetter(webapi), new UserViewToProperty(), logger);
			inventory_ = new DataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> ("inventory", new ItemsListGetter(webapi), new ItemsListToItem(), logger);
			shop_ = new DataModel<DomainObjects.ShopEntry,WebObjects.Shop.ListResponse>("shop", new ShopListGetter(webapi), new ShopListToShopEntry(), logger);
			actions_ = new DataModel<Foo,Foo> ("tasks", new FooGetter(webapi), new FooToFoo(), logger);
			gifts_ = new DataModel<Foo,Foo> ("gifts", new FooGetter(webapi), new FooToFoo(), logger);
			achievements_ = new DataModel<Foo,Foo> ("achievements", new FooGetter(webapi), new FooToFoo(), logger);
			leaderboards_ = new DataModel<DomainObjects.Leaderboard,WebObjects.Leaderboards.ListResponse>( "leaderboards", new LeaderboardListGetter(webapi), new LeaderboardListToLeaderboard(), logger);
			ranking_ = new DataModel<Foo,Foo> ("ranking", new FooGetter(webapi), new FooToFoo(), logger);
			friends_ = new DataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> ("friends",  new FriendsListGetter(webapi), new FriendsListToFriend(), logger);
			friendInvites_ = new DataModel<DomainObjects.FriendInvite> ("friends", "friends/list_invites", "friend_invite", null, new DC.XmlToFriendInvite (), api, logger);
			friendInviteInfo_ = new DataModel<DomainObjects.FriendInviteInfo> ("friends", "friends/info", null, null, new DC.XmlToFriendInviteInfo (), api, logger);
			cache_ = new ItemCache ("cache", new ItemsViewGetter(webapi), new ItemsViewToItemPrototype(), logger);
			appStore_ = new DataModel<Foo,Foo> ("appstore", new FooGetter(webapi), new FooToFoo(), logger);
		}

		public void Clear (bool x)
		{
			properties.Clear (x);
			inventory.Clear (x);
			shop.Clear (x);
			actions.Clear (x);
			gifts.Clear (x);
			achievements.Clear (x);
			leaderboards.Clear (x);
			ranking.Clear (x);
			friends.Clear (x);
			cache.Clear (x);
			appStore.Clear (x);
		}
		
		

		public IDataModel<Property,Roar.WebObjects.User.ViewResponse> properties { get { return properties_; } }
		public IDataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory { get { return inventory_; } }
		public IDataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop { get { return shop_; } }
		public IDataModel<Foo,Foo> actions { get { return actions_; } }
		public IDataModel<Foo,Foo> gifts { get { return gifts_; } }
		public IDataModel<Foo,Foo> achievements { get { return achievements_; } }
		public IDataModel<DomainObjects.Leaderboard,WebObjects.Leaderboards.ListResponse> leaderboards { get { return leaderboards_; } }
		public IDataModel<Foo,Foo> ranking { get { return ranking_; } }
		public IDataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends { get { return friends_; } }
		public DataModel<DomainObjects.FriendInvite,WebObjects.Friends.Foo> friendInvites { get { return friendInvites_; } }
		public DataModel<DomainObjects.FriendInviteInfo,WebObject.Friends.Foo> friendInviteInfo { get { return friendInviteInfo_; } }
		public IDataModel<Foo,Foo> appStore { get { return appStore_; } }
		public IItemCache cache { get { return cache_; } }
		
		public DataModel<Property,Roar.WebObjects.User.ViewResponse> properties_;
		public DataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory_;
		public DataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop_;
		public DataModel<Foo,Foo> actions_;
		public DataModel<Foo,Foo> gifts_;
		public DataModel<Foo,Foo> achievements_;
		public DataModel<DomainObjects.Leaderboard,WebObjects.Leaderboards.ListResponse> leaderboards_;
		public DataModel<Foo,Foo> ranking_;
		public DataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends_;
		public DataModel<DomainObjects.FriendInvite,WebObjects.Friends.Foo> friendInvites_;
		public DataModel<DomainObjects.FriendInviteInfo,WebObjects.Friends.Foo> friendInviteInfo_;
		public DataModel<Foo,Foo> appStore_;
		public ItemCache cache_;
	}

}
