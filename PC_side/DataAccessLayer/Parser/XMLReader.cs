using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataAccessLayer.Parser
{
    public class XMLReader
    {
        
        public static void loadHistoriques(String FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/" + XMLTags.ListeConversation + "/" + XMLTags.Conversation );
                        
            foreach (XmlNode node in nodes)
            {


                EntityLayer.Contact cont = new EntityLayer.Contact(node.SelectSingleNode(XMLTags.Contact).Attributes[XMLTags.ContactNom].Value,
                                                                    node.SelectSingleNode(XMLTags.Contact).Attributes[XMLTags.ContactNum].Value);
                DalManager.Instance.AddConversation(cont);

                EntityLayer.Conversation conv = DalManager.Instance.getConversationsFromContact(cont.Num);
                
                foreach (XmlNode smsNode in node.SelectSingleNode(XMLTags.ListeSMS).ChildNodes)
                {
                    DalManager.Instance.AddMessageToConv(new EntityLayer.SMS(
                            Convert.ToInt16(smsNode.Attributes[XMLTags.SMSId].Value),
                            smsNode.Attributes[XMLTags.SMSBody].Value,
                            Convert.ToDateTime(smsNode.Attributes[XMLTags.SMSDate].Value),
                            smsNode.Attributes[XMLTags.SMSReceiverNum].Value.Equals("Moi")? null : conv.Receiver,
                            Convert.ToBoolean(smsNode.Attributes[XMLTags.SMSRecieved].Value),
                            Convert.ToBoolean(smsNode.Attributes[XMLTags.SMSNotified].Value)
                        ),conv);
                }
            }            
        }

        public static void loadContacts(String FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/" + XMLTags.ListeContact + "/" + XMLTags.Contact);

            foreach (XmlNode node in nodes)
            {


                EntityLayer.Contact cont = new EntityLayer.Contact(node.Attributes[XMLTags.ContactNom].Value,
                                                                    node.Attributes[XMLTags.ContactNum].Value);
                DalManager.Instance.addContact(cont);
                
            }
        }
    }
}
