using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ISDOC
{
    public static class Isdoc
    {
        public static XmlDocument CreateXmlDocument()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, root);

            return xDoc;
        }

        public static XmlElement CreateInvoice(this XmlDocument xDoc)
        {
            XmlAttribute version = xDoc.CreateAttribute("version");
            version.Value = "6.0.1";
            XmlElement invoice = xDoc.CreateElement("Invoice", "http://isdoc.cz/namespace/2013");
            invoice.Attributes.Append(version);
            xDoc.AppendChild(invoice);

            return invoice;
        }

        public static void CreateInvoiceHeader(this XmlElement invoice, string docNumber, string id, string issueDate, string taxPointDate,
            bool vatApplicable, string elPossibilityAgreementRef)
        {
            var xDoc = invoice.OwnerDocument;
            
            invoice.AppendChild(xDoc.AddElement("DocumentType", "1"));
            invoice.AppendChild(xDoc.AddElement("ID", docNumber));
            invoice.AppendChild(xDoc.AddElement("UUID", id));
            invoice.AppendChild(xDoc.AddElement("IssueDate", issueDate));
            invoice.AppendChild(xDoc.AddElement("TaxPointDate", taxPointDate));
            invoice.AppendChild(xDoc.AddElement("VATApplicable", vatApplicable? "true" : "false"));
            invoice.AppendChild(xDoc.AddElement("ElectronicPossibilityAgreementReference", elPossibilityAgreementRef));
            invoice.AppendChild(xDoc.AddElement("LocalCurrencyCode", "CZK"));
            invoice.AppendChild(xDoc.AddElement("CurrRate", "1"));
            invoice.AppendChild(xDoc.AddElement("RefCurrRate", "1"));
        }

        public static XmlElement CreateParty(this XmlDocument xDoc, string accountingPartyName, string userId, string id, string companyName,
            string street, string buildingNumber, string city, string postalZone, string country, string countryCode,
            string vatId, string name, string telephone, string mail)
        {
            var accountingParty = xDoc.AddElement(accountingPartyName);
            var party = xDoc.AddElement("Party");
            accountingParty.AppendChild(party);

            var partyIdentification = xDoc.AddElement("PartyIdentification");
            party.AppendChild(partyIdentification);
            partyIdentification.AppendChild(xDoc.AddElement("UserID", userId));
            partyIdentification.AppendChild(xDoc.AddElement("CatalogFirmIdentification", "0"));
            partyIdentification.AppendChild(xDoc.AddElement("ID", id));

            var partyName = xDoc.AddElement("PartyName");
            partyName.AppendChild(xDoc.AddElement("Name", companyName));
            party.AppendChild(partyName);

            var postalAddress = xDoc.AddElement("PostalAddress");
            party.AppendChild(postalAddress);
            postalAddress.AppendChild(xDoc.AddElement("StreetName", street));
            postalAddress.AppendChild(xDoc.AddElement("BuildingNumber", buildingNumber));
            postalAddress.AppendChild(xDoc.AddElement("CityName", city));
            postalAddress.AppendChild(xDoc.AddElement("PostalZone", postalZone));
            var _country = xDoc.AddElement("Country");
            _country.AppendChild(xDoc.AddElement("IdentificationCode", countryCode));
            _country.AppendChild(xDoc.AddElement("Name", country));
            postalAddress.AppendChild(_country);

            var partyTaxScheme = xDoc.AddElement("PartyTaxScheme");
            party.AppendChild(partyTaxScheme);
            partyTaxScheme.AppendChild(xDoc.AddElement("CompanyID", vatId));
            partyTaxScheme.AppendChild(xDoc.AddElement("TaxScheme", "VAT"));

            var contact = xDoc.AddElement("Contact");
            party.AppendChild(contact);
            contact.AppendChild(xDoc.AddElement("Name", name));
            contact.AppendChild(xDoc.AddElement("Telephone", telephone));
            contact.AppendChild(xDoc.AddElement("ElectronicMail", mail));

            return accountingParty;
        }

        public static void AppendAccountingParties(this XmlElement invoice, string sellerUserId, string sellerId, string sellerCompanyName,
            string sellerStreet, string sellerBuildingNumber, string sellerCity, string sellerPostalZone, string sellerCountry, string sellerCountryCode,
            string sellerVatId, string sellerName, string sellerTelephone, string sellerMail,
            string buyerUserId, string buyerId, string buyerCompanyName,
            string buyerStreet, string buyerBuildingNumber, string buyerCity, string buyerPostalZone, string buyerCountry, string buyerCountryCode,
            string buyerVatId, string buyerName, string buyerTelephone, string buyerMail)
        {
            var xDoc = invoice.OwnerDocument;

            invoice.AppendChild(xDoc.CreateParty("AccountingSupplierParty", sellerUserId, sellerId, sellerCompanyName,
            sellerStreet, sellerBuildingNumber, sellerCity, sellerPostalZone, sellerCountry, sellerCountryCode,
            sellerVatId, sellerName, sellerTelephone, sellerMail));

            invoice.AppendChild(xDoc.CreateParty("SellerSupplierParty", sellerUserId, sellerId, sellerCompanyName,
            sellerStreet, sellerBuildingNumber, sellerCity, sellerPostalZone, sellerCountry, sellerCountryCode,
            sellerVatId, sellerName, sellerTelephone, sellerMail));

            invoice.AppendChild(xDoc.CreateParty("AccountingCustomerParty", buyerUserId, buyerId, buyerCompanyName,
            buyerStreet, buyerBuildingNumber, buyerCity, buyerPostalZone, buyerCountry, buyerCountryCode,
            buyerVatId, buyerName, buyerTelephone, buyerMail));

            invoice.AppendChild(xDoc.CreateParty("BuyerCustomerParty", buyerUserId, buyerId, buyerCompanyName,
            buyerStreet, buyerBuildingNumber, buyerCity, buyerPostalZone, buyerCountry, buyerCountryCode,
            buyerVatId, buyerName, buyerTelephone, buyerMail));
        }
        
        public static void AddInvoiceLines(this XmlElement invoice)
        {
            var xDoc = invoice.OwnerDocument;
            var invoiceLines = xDoc.AddElement("InvoiceLines");
            invoice.AppendChild(invoiceLines);

            invoiceLines.AddInvoiceLine("12", "1", "120", "150", "30", "120", "150", "21", "Běžky Olpran");
            invoiceLines.AddInvoiceLine("11", "2", "240", "300", "60", "120", "150", "21", "Snowboard Head");
        }

        public static void AddInvoiceLine(this XmlElement invoiceLines, string id, string quantity, string amount, 
           string amountTaxInclusive, string taxAmount, string unitPrice, string unitPriceTaxInclusive, string taxPercent,
           string itemName)
        {
            var xDoc = invoiceLines.OwnerDocument;

            var invoiceLine = xDoc.AddElement("InvoiceLine");
            invoiceLine.AppendChild(xDoc.AddElement("ID", id));

            XmlAttribute unitCode = xDoc.CreateAttribute("unitCode");
            unitCode.Value = "Ks";
            XmlElement invoicedQuantity = xDoc.CreateElement("InvoicedQuantity", xDoc.DocumentElement.NamespaceURI);
            invoicedQuantity.Attributes.Append(unitCode);
            invoicedQuantity.InnerText = quantity;

            invoiceLine.AppendChild(invoicedQuantity);


            invoiceLine.AppendChild(xDoc.AddElement("LineExtensionAmount", amount));
            invoiceLine.AppendChild(xDoc.AddElement("LineExtensionAmountTaxInclusive", amount));
            invoiceLine.AppendChild(xDoc.AddElement("LineExtensionTaxAmount", taxAmount));
            invoiceLine.AppendChild(xDoc.AddElement("UnitPrice", unitPrice));
            invoiceLine.AppendChild(xDoc.AddElement("UnitPriceTaxInclusive", unitPriceTaxInclusive));

            var classifiedTaxCategory = xDoc.AddElement("ClassifiedTaxCategory");
            invoiceLine.AppendChild(classifiedTaxCategory);
            classifiedTaxCategory.AppendChild(xDoc.AddElement("Percent", taxPercent));
            classifiedTaxCategory.AppendChild(xDoc.AddElement("VATCalculationMethod", "0"));

            invoiceLine.AppendChild(xDoc.AddElement("Note"));
            var item = xDoc.AddElement("Item");
            item.AppendChild(xDoc.AddElement("Description", itemName));
            invoiceLine.AppendChild(item);


            invoiceLines.AppendChild(invoiceLine);
        }

        public static void AddTaxTotal(this XmlElement invoice)
        {
            var xDoc = invoice.OwnerDocument;
            var taxTotal = xDoc.AddElement("TaxTotal");

            taxTotal.AddTaxSubTotal("25", "2", "27", "0", "0", "0", "25", "27", "2","21");
            taxTotal.AddTaxSubTotal("50", "4", "54", "0", "0", "0", "50", "54", "4", "21");

            taxTotal.AppendChild(xDoc.AddElement("TaxAmount", "6"));
            invoice.AppendChild(taxTotal);
        }

        public static void AddTaxSubTotal(this XmlElement taxTotal, string taxableAmount, string taxAmount, string taxInclusiveAmount, string alreadyClaimedTaxableAmount,
            string alreadyClaimedTaxAmount, string alreadyClaimedTaxInclusiveAmount, string differenceTaxableAmount, string differenceTaxAmount, string differenceTaxInclusiveAmount,
            string taxPercent)
        {
            var xDoc = taxTotal.OwnerDocument;
            var taxSubTotal = xDoc.AddElement("TaxSubTotal");
            taxTotal.AppendChild(taxSubTotal);

            taxSubTotal.AppendChild(xDoc.AddElement("TaxableAmount", taxableAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("TaxAmount", taxAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("TaxInclusiveAmount", taxInclusiveAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("AlreadyClaimedTaxableAmount", alreadyClaimedTaxableAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("AlreadyClaimedTaxAmount", alreadyClaimedTaxAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("AlreadyClaimedTaxInclusiveAmount", alreadyClaimedTaxInclusiveAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("DifferenceTaxableAmount", differenceTaxableAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("DifferenceTaxAmount", differenceTaxAmount));
            taxSubTotal.AppendChild(xDoc.AddElement("DifferenceTaxInclusiveAmount", differenceTaxInclusiveAmount));
            var taxCategory = xDoc.AddElement("TaxCategory");
            taxCategory.AppendChild(xDoc.AddElement("Percent", taxPercent));
            taxSubTotal.AppendChild(taxCategory);
        }

        public static void LegalMonetaryTotal(this XmlElement invoice, string taxExclusiveAmount, string taxInclusiveAmount, string alreadyClaimedTaxExclusiveAmount,
            string alreadyClaimedTaxInclusiveAmount, string differenceTaxExclusiveAmount, string differenceTaxInclusiveAmount,
            string payableRoundingAmount, string paidDepositsAmount, string payableAmount)
        {
            var xDoc = invoice.OwnerDocument;
            var legalMonetaryTotal = xDoc.AddElement("LegalMonetaryTotal");
            invoice.AppendChild(legalMonetaryTotal);

            legalMonetaryTotal.AppendChild(xDoc.AddElement("TaxExclusiveAmount", taxExclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("TaxInclusiveAmount", taxInclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("AlreadyClaimedTaxExclusiveAmount", alreadyClaimedTaxExclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("AlreadyClaimedTaxInclusiveAmount", alreadyClaimedTaxInclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("DifferenceTaxExclusiveAmount", differenceTaxExclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("DifferenceTaxInclusiveAmount", differenceTaxInclusiveAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("PayableRoundingAmount", payableRoundingAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("PaidDepositsAmount", paidDepositsAmount));
            legalMonetaryTotal.AppendChild(xDoc.AddElement("PayableAmount", payableAmount));
        }

        public static XmlElement AddElement(this XmlDocument xDoc, string name, string value = null)
        {
            XmlElement el = xDoc.CreateElement(name, xDoc.DocumentElement.NamespaceURI);
            if(!string.IsNullOrEmpty(value))
                el.InnerText = value;
            return el;
        }

        public static XmlElement AddElement(this XmlDocument xDoc, string name, decimal value)
        {
            XmlElement el = xDoc.CreateElement(name, xDoc.DocumentElement.NamespaceURI);
                el.InnerText = value.ToString("N");
            return el;
        }

        public static XmlElement AddElement(this XmlDocument xDoc, string name, int value)
        {
            XmlElement el = xDoc.CreateElement(name, xDoc.DocumentElement.NamespaceURI);
            el.InnerText = value.ToString("N");
            return el;
        }
    }
}
