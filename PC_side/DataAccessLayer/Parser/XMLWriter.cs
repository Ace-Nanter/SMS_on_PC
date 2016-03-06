﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataAccessLayer.Parser
{
    public class XMLWriter
    {
        public static void saveContacts(String filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteComment("This file is generated by the program.");
            writer.WriteStartElement(XMLTags.ListeContact);
            foreach (EntityLayer.Contact c in DalManager.Instance.getContacts())
            {
                writer.WriteStartElement(XMLTags.Contact);

                writer.WriteAttributeString(XMLTags.ContactNom,c.Nom);
                writer.WriteAttributeString(XMLTags.ContactNum, c.Num);

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }

        public static void saveConversations(String filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteComment("This file is generated by the program.");
            writer.WriteStartElement(XMLTags.ListeConversation);
            foreach (EntityLayer.Conversation c in DalManager.Instance.getConversations())
            {
                writer.WriteStartElement(XMLTags.Conversation);
                    writer.WriteStartElement(XMLTags.Contact);
                        writer.WriteAttributeString(XMLTags.ContactNom, c.Receiver.Nom);
                        writer.WriteAttributeString(XMLTags.ContactNum, c.Receiver.Num);
                    writer.WriteEndElement();
                    writer.WriteStartElement(XMLTags.ListeSMS);
                    foreach (EntityLayer.SMS sms in c.Messages) {
                        writer.WriteStartElement(XMLTags.SMS);
                            if(sms.Contact != null)
                            {
                                writer.WriteAttributeString(XMLTags.SMSReceiverNum, sms.Contact.Num);
                            }
                            else
                            {
                                writer.WriteAttributeString(XMLTags.SMSReceiverNum,"Moi");
                            }                            
                            writer.WriteAttributeString(XMLTags.SMSBody, sms.Body);
                            writer.WriteAttributeString(XMLTags.SMSDate, sms.Date.ToString());
                            writer.WriteAttributeString(XMLTags.SMSId, sms.ID.ToString());
                            writer.WriteAttributeString(XMLTags.SMSNotified, sms.Notified.ToString());
                            writer.WriteAttributeString(XMLTags.SMSRecieved, sms.Received.ToString());
                        writer.WriteEndElement();
                    }                        
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }


    }
}
