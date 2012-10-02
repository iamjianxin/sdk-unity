/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System.Collections;

namespace Roar.Components
{

/**
 * \brief IInventory is an interface for viewing and manipulating a user's inventory.
 *
 * One can perform the following functions on an inventory item:
 * - activate
 * - deactivate
 * - sell
 * - use
 * - remove
 *
 * An inventory is composed of a list of inventory items.
 *
 * An inventory item contains properties to describe its use, along with specific
 * flags to indicate status such as equipped, consumable and collect tick/window.
 *
 *
 * An inventory item can be accessed through its Hashtable interface:
 *
 * \code
 *  - id                     [uint]
 *  - ikey                   [string]
 *  - label                  [string]
 *  - description            [string]
 *  - count                  [int]
 *  - stats                  [array]
 *     - ikey                [string]
 *     - value               [int]
 *  - custom_attributes      [array]
 *     - attribute_ikey      [string]
 *     - value               [int]
 *  - tags                   [array of strings]
 *  - type                   [string]
 *  - consumable             [bool]
 *  - equipped               [bool]
 *  - sellable               [bool]
 * \endcode
 *
 * @note The inventory view and manipulation functions of this interface can only be called after the inventory has been fetched from the server via a call to #fetch.
 * @note once #fetch has received and processed the inventory from the server, the #hasDataFromServer
 * property will return true and calls to this interface will be functional.
 **/
public interface IInventory
{
  /**
   * Fetch inventory information from the server.
   *
   * On success:
   * - invokes callback with parameter *Hastable data* containing the data for the inventory.
   * - fires the RoarManager#inventoryReadyEvent
   * - sets #hasDataFromServer to true
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
   */
  void fetch( Roar.Callback callback );

  /**
   * Check whether any inventory data has been obtained from the server.
   *
   * @returns true if #fetch has completed execution.
   */
  bool hasDataFromServer { get; }

  /**
   * Get a list of all the inventory items for the authenticated user.
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns A list of Hashtables for each inventory item.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   **/
  ArrayList list();

  /**
   * Get a list of all the inventory items for the authenticated user.
   *
   * On success:
   * - invokes callback with parameter *data* containing the list of Hashtable inventory items
   *
   * On failure:
   * - returns an empty list
   *
   * @param callback the callback function to be passed this function's result.
   *
   * @returns A list of Hashtables for each inventory item.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return an empty array.
   **/
  ArrayList list(Roar.Callback callback);

  /**
   * Activates an item in the user's inventory.
   *
   * On success:
   * - fires the RoarManager.goodActivatedEvent
   * - invokes callback passing *data* parameter a Hashtable containing the:
   *  - "id" : the id of the inventory item instance
   *  - "ikey" : the id of the inventory item type
   *  - "label" : the label of the inventory item
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param id the key that uniquely identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
   *
   **/
  void activate( string id, Roar.Callback callback ) ;

  /**
   * Deactivates an item in the user's inventory.
   *
   * On success:
   * - fires the RoarManager.goodDeactivatedEvent
   * - invokes callback passing *data* parameter a Hashtable containing the:
   *  - "id" : the id of the inventory item instance
   *  - "ikey" : the id of the inventory item type
   *  - "label" : the label of the inventory item
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param id the key that uniquely identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
   *
   **/
  void deactivate( string id, Roar.Callback callback );

  /**
   * Checks if the user's inventory contains at least one of a given item.
   *
   * @param ikey the key that identifies an inventory item.
   * @returns true if one or more instances of a given inventory item belong to the user.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return false.
   **/
  bool has( string ikey );

  /**
   * Checks if the user's inventory contains *num* units of a given item.
   *
   * @param ikey the key that identifies an inventory item.
   * @param num the quantity of an inventory item to check for.
   * @param callback the callback function to be passed this function's result.
   * @returns true if one or more instances of a given inventory item belong to the user.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return false.
   **/
  bool has( string ikey, int num, Roar.Callback callback );

  /**
   * @param ikey the key that identifies an inventory item.
   * @returns the quantity of a given inventory item held by the user.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return 0.
   **/
  int quantity( string ikey );

  /**
   * @param ikey the key that identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   * @returns the quantity of a given inventory item held by the user.
   *
   * @note This does _not_ make a server call. It requires the inventory to
   *       have already been fetched via a call to #fetch. If this function
   *       is called prior to the successful completion of a #fetch call,
   *       it will return 0.
   **/
  int quantity( string ikey, Roar.Callback callback );

  /**
   * Sells an item in the user's inventory.
   *
   * On success:
   * - fires the RoarManager.goodSoldEvent
   * - invokes callback passing *data* parameter a Hashtable containing the:
   *  - "id" : the id of the inventory item instance
   *  - "ikey" : the id of the inventory item type
   *  - "label" : the label of the inventory item
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param id the key that uniquely identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
   *
   **/
  void sell( string id, Roar.Callback callback );

  /**
   * Consumes/uses an item in the user's inventory.
   *
   * On success:
   * - fires the RoarManager.goodUsedEvent
   * - invokes callback passing *data* parameter a Hashtable containing the:
   *  - "id" : the id of the inventory item instance
   *  - "ikey" : the id of the inventory item type
   *  - "label" : the label of the inventory item
   *
   * On failure:
   * - invokes callback with error code and error message
   *
   * @param id the key that uniquely identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
   **/
  void use( string id, Roar.Callback callback );

  /**
   * The remove function for now is simply an *alias* to #sell
   **/
  void remove( string id, Roar.Callback callback );

  /**
   * Returns the inventory item for a given key.
   *
   * @param id the key that uniquely identifies an inventory item.
   *
   * @returns the inventory item Hashtable associated with the *id*
   *          or null if the inventory item does not exist in the data store.
   */
  Hashtable getGood( string id );

  /**
   * Returns the inventory item for a given key.
   *
   * On success:
   * - invokes callback with parameter *data* containing the Hashtable inventory item
   *
   * On failure:
   * - invokes callback with parameter *data* equaling null if inventory item does not exist.
   *
   * @param id the key that uniquely identifies an inventory item.
   * @param callback the callback function to be passed this function's result.
   *
   * @returns the inventory item Hashtable associated with the *id*
   *          or null if the inventory item does not exist in the data store.
   */
  Hashtable getGood( string id, Roar.Callback callback );
}

}