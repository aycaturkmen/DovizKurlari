using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DovizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CurrenciesController:ControllerBase
    {

        XElement xelement = XElement.Load("https://www.tcmb.gov.tr/kurlar/today.xml"); //xelement tcmb'den çekilen tüm kur bilgilerini tutar

        [HttpGet]
        public string Get()
        {

            XmlDocument xmlDoc = new XmlDocument();
            string xmlTcmbData = xelement.ToString(); 
            xmlDoc.LoadXml(xmlTcmbData);
            string jsonTcmbData = JsonConvert.SerializeXmlNode(xmlDoc);
            return jsonTcmbData;

        }

        [HttpGet("{secilenDoviz}")]
        public string Get(string secilenDoviz)
        {

            IEnumerable<XElement> dovizCinsleri = xelement.Elements(); 
            string secilenDovizCinsi = "";
            //Tüm döviz cinsleri taranarak seçilen döviz cinsi bulunur. Döviz cinsi Türkçe veya İngilizce aratılabilir.
            foreach (var dovizCinsi in dovizCinsleri)
            {
                if (dovizCinsi.Element("Isim").Value.Equals(secilenDoviz) || dovizCinsi.Element("CurrencyName").Value.Equals(secilenDoviz) )
                {
                    secilenDovizCinsi = dovizCinsi.ToString();
                }              
            }
            return secilenDovizCinsi.ToString();
        }

        [HttpGet("{altLimit},{ustLimit}")]
        public string Get(decimal altLimit, decimal ustLimit)
        {

            IEnumerable<XElement> dovizCinsleri = xelement.Elements();
            string secilenAraliktakiler = "";

            //Döviz satış fiyatı seçilen aralıklar içerisinde olan döviz cinslerini bulur.
            foreach (var dovizCinsi in dovizCinsleri)
            {
                decimal satis=0;               
                bool e = dovizCinsi.Element("CurrencyName").ToString().Contains("SDR");
                if (dovizCinsi.Element("ForexSelling") != null && !e)
                {
                    satis = (decimal)dovizCinsi.Element("ForexSelling");
                }

                if (Decimal.Compare(satis, altLimit)>=0 && Decimal.Compare(satis, ustLimit) <= 0)
                {
                    secilenAraliktakiler = secilenAraliktakiler + dovizCinsi.ToString();
                }      
            }
            return secilenAraliktakiler.ToString();

        }
    }
}