﻿using IsTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemi.Controllers
{
    public class IsDurum
    {
        public string isBaslik { get; set; }

        public string isAciklama { get; set; }

        public DateTime? iletilenTarih { get; set; }

        public DateTime? yapilanTarih { get; set; }

        public string durumAd { get; set; }
        public string isYorum { get; set; }

    }
    public class CalisanController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Calisan
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 2)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from p in entity.Birimler where p.birimId == birimId select p).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.Isler 
                             where i.isPersonelId == personelId && i.isOkunma == false
                             orderby i.iletilenTarih descending 
                             select i).ToList();
                ViewBag.isler = isler;
                    
                return View();
            }
            else
            {
               return  RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index( int isId)
        {
            var tekIs=(from i in entity.Isler where i.isId==isId select i).FirstOrDefault();
            tekIs.isOkunma = true;
            entity.SaveChanges();
            return RedirectToAction("Index", "Calisan");
        }
        public ActionResult Yap()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from p in entity.Isler
                           where p.isPersonelId == personelId && p.isDurumId == 1
                           select p).ToList().OrderByDescending(p => p.iletilenTarih);
                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]

       public ActionResult Yap(int isId , string isYorum)
        {
            var tekIs = (from p in entity.Isler where p.isId == isId select p).FirstOrDefault();
            if (isYorum=="") isYorum = "Kullanıcı Yorum Yapmadı";
            tekIs.yapilanTarih = DateTime.Now;
            tekIs.isDurumId = 2;
            tekIs.isYorum = isYorum;
            entity.SaveChanges();
            return RedirectToAction("Index","Calisan");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.Isler
                             join d in entity.Durumlar on i.isDurumId equals d.durumId
                             where i.isPersonelId == personelId
                             select i).ToList().OrderByDescending(i => i.iletilenTarih);
                IsDurumModel model = new IsDurumModel();
                List<IsDurum> list = new List<IsDurum>();
                foreach(var i in isler)
                {
                    IsDurum isDurum = new IsDurum();
                    isDurum.isBaslik = i.isBaslik;
                    isDurum.isAciklama = i.isAciklama;
                    isDurum.iletilenTarih = i.iletilenTarih;
                    isDurum.yapilanTarih = i.yapilanTarih;
                    isDurum.durumAd = i.Durumlar.durumAd;
                    isDurum.isYorum = i.isYorum;
                    list.Add(isDurum);

                }
                model.isDurumlar = list;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}