using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ISDOC
{
    class Program
    {
        static void Main(string[] args)
        {

            var xDoc = Isdoc.CreateXmlDocument();
            var invoice = xDoc.CreateInvoice();
            invoice.CreateInvoiceHeader("12345", "7B4C5BE0-288C-11D2-8E62-004095452B84", "2018-09-09", "2018-10-10", true, "ABC/134/23");
            invoice.AppendAccountingParties("1222", "146644", "Sunski", "Ramzová", "45", "Ramzová", "65830", "Česká republika", "CZ",
                "1345646", "Jan Šubert", "789345224", "subert@sunski.cz", "234", "222222", "Tomáš Novotný", "", "", "", "", "Česká republika", "CZ", "", "Tomáš Novotný", "", "");
            invoice.AddInvoiceLines();
            invoice.AddTaxTotal();
            invoice.LegalMonetaryTotal("75", "84", "0", "0", "6", "6", "0", "0", "0");


            
            xDoc.Save("isdocFile.isdoc");
        }
    }
}
