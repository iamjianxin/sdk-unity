using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roar.implementation.DataConversion
{
	/**
	 * The attribute conversion code in this namespace uses Native.Extract
	 * to convert string representations of booleans to native bools as
	 * it's easier to work with booleans than string representations of booleans.
	 **/
	public class Native
	{
		public static object Extract (string v)
		{
			if (v == "true") {
				return true;
			} else if (v == "false") {
				return false;
			} else {
				return v;
			}
		}
	}

	public interface IXmlToHashtable
	{
		Hashtable BuildHashtable (IXMLNode n);

		string GetKey (IXMLNode n);
	};

	public interface ICRMParser
	{
		ArrayList ParseCostList (IXMLNode n);
		ArrayList ParseModifierList (IXMLNode n);
		ArrayList ParseRequirementList (IXMLNode n);
		ArrayList ParsePropertiesList (IXMLNode n);
		ArrayList ParseChildrenForAttribute (IXMLNode n, string attribute);
	}

	public class CRMParser : ICRMParser
	{
		protected Hashtable AttributesAsHash (IXMLNode n)
		{
			Hashtable h = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				h [kv.Key] = Native.Extract (kv.Value);
			}
			return h;
		}

		public ArrayList ParseModifierList (IXMLNode n)
		{
			ArrayList modifier_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				modifier_list.Add (hh);
			}
			return modifier_list;
		}

		public ArrayList ParseCostList (IXMLNode n)
		{
			ArrayList cost_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				cost_list.Add (hh);
			}
			return cost_list;
		}

		public ArrayList ParseRequirementList (IXMLNode n)
		{
			ArrayList req_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				req_list.Add (hh);
			}
			return req_list;
		}

		public ArrayList ParsePropertiesList (IXMLNode n)
		{
			ArrayList prop_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				prop_list.Add (AttributesAsHash (nn));
			}
			return prop_list;
		}

		// for each child node that has a matching attribute, adds the attribute
		// to the list then returns the list
		public ArrayList ParseChildrenForAttribute (IXMLNode n, string attribute)
		{
			ArrayList list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				string attributeValue = nn.GetAttribute (attribute);
				if (attributeValue != null) {
					list.Add (attributeValue);
				}
			}
			return list;
		}

	}

	public class XmlToTaskHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToTaskHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			retval ["ikey"] = n.GetAttribute ("ikey");

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "location":
					break;
				case "mastery_level":
					break;
				case "rewards":
					retval ["rewards"] = CrmParser_.ParseModifierList (nn);
					break;
				case "costs":
					retval ["costs"] = CrmParser_.ParseCostList (nn);
					break;
				case "requires":
					retval ["requires"] = CrmParser_.ParseRequirementList (nn);
					break;
				case "tags":
					retval ["tags"] = CrmParser_.ParseChildrenForAttribute (nn, "value");
					break;
				case "properties":
					retval ["properties"] = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}

			return retval;
		}

	}

	public class XmlToInventoryItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToInventoryItemHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public virtual string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("id");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "price":
					retval ["price"] = CrmParser_.ParseModifierList (nn);
					break;
				case "tags":
					retval ["tags"] = CrmParser_.ParseChildrenForAttribute (nn, "value");
					break;
				case "properties":
					retval ["properties"] = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}

			return retval;
		}

	}

	public class XmlToShopItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToShopItemHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				//We move the ikey to shop_ikey
				if (kv.Key == "ikey") {
					retval ["shop_ikey"] = kv.Value;
				} else {
					retval [kv.Key] = Native.Extract (kv.Value);
				}
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "costs":
					retval ["costs"] = CrmParser_.ParseCostList (nn);
					break;
				case "modifiers":
					//We require and expect that there be only one modifier and that modifier is a grant_item
					//were we to support the more general case we'd do this:
					//    retval["modifiers"] = CrmParser_.ParseModifierList( nn );
					IXMLNode grantItemNode = nn.GetFirstChild ("grant_item");
					if (grantItemNode != null) {
						retval ["ikey"] = grantItemNode.GetAttribute ("ikey");
					}
					break;
				case "tags":
					retval ["tags"] = CrmParser_.ParseChildrenForAttribute (nn, "value");
					break;
				case "properties":
					retval ["properties"] = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}

			return retval;
		}

	}

	public class XmlToPropertyHashtable : IXmlToHashtable
	{
		public XmlToPropertyHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			string key = n.GetAttribute ("ikey");
			if (key != null)
				return key;
			return n.GetAttribute ("name");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XMLToItemHashtable : XmlToInventoryItemHashtable
	{
		public override string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}
	}

	public class XmlToAchievementHashtable : IXmlToHashtable
	{

		public XmlToAchievementHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XmlToLeaderboardsHashtable : IXmlToHashtable
	{

		public XmlToLeaderboardsHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XmlToRankingHashtable : IXmlToHashtable
	{

		public XmlToRankingHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			Hashtable properties = new Hashtable ();
			ArrayList entries = new ArrayList();
			foreach (KeyValuePair<string,string> kv in n.Attributes)
			{
				//Debug.Log (string.Format ("BuildHashtable: {0} => {1}", kv.Key, kv.Value));
				properties [kv.Key] = Native.Extract (kv.Value);
			}
			foreach (IXMLNode child in n.Children)
			{
				Hashtable entry = new Hashtable();
				foreach (KeyValuePair<string,string> kv in child.Attributes)
				{
					//Debug.Log (string.Format ("BuildHashtable: {0} => {1}", kv.Key, kv.Value));
					entry[kv.Key] = Native.Extract(kv.Value);

					// any custom data? need the player_name if it's there
					foreach (IXMLNode subChild in child.Children)
					{
						if (subChild.Name == "custom")
						{
							foreach (IXMLNode propertyNode in subChild.Children)
							{
								string propertyKey = string.Empty, propertyValue = string.Empty;
								foreach (KeyValuePair<string,string> kvp in propertyNode.Attributes)
								{
									if (kvp.Key == "ikey")
										propertyKey = kvp.Value;
									else if (kvp.Key == "value")
										propertyValue = kvp.Value;
								}
								if (propertyKey.Length > 0)
									entry[propertyKey] = propertyValue;
							}
						}
					}
				}
				entries.Add(entry);
			}
			retval.Add("properties", properties);
			retval.Add("entries", entries);
			return retval;
		}
	}

	public class XmlToAppstoreItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToAppstoreItemHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("product_identifier");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "modifiers":
					retval ["modifiers"] = CrmParser_.ParseModifierList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}
			return retval;
		}
	}

	public class XmlToGiftHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToGiftHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("id");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "costs":
					retval ["costs"] = CrmParser_.ParseCostList (nn);
					break;
				case "requirements":
					retval ["requirements"] = CrmParser_.ParseRequirementList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}
			return retval;
		}
	}
}
