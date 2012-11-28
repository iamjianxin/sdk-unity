using System;

public class MockRoar : DefaultRoar, IRoar
{
  public RequestSender api;

  new public void Awake ()
  {
    config = new Roar.implementation.Config ();
    Logger logger = new Logger ();

    api = new MockRequestSender (config, this, logger);
    Roar.implementation.DataStore data_store = new Roar.implementation.DataStore (api, logger);
    webAPI = new global::WebAPI (api);
    user = new Roar.implementation.Components.User (webAPI.user, data_store, logger);
    properties = new Roar.implementation.Components.Properties (data_store);
    inventory = new Roar.implementation.Components.Inventory (webAPI.items, data_store, logger);
    shop = new Roar.implementation.Components.Shop (webAPI.shop, data_store, logger);
    actions = new Roar.implementation.Components.Actions (webAPI.tasks, data_store);

    urbanAirship = new Roar.implementation.Adapters.UrbanAirship (webAPI);

    // Apply public settings
    // TODO: Not sure what this should be now.
    // Config.game = gameKey;
  }
}

