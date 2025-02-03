using IsTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemi.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                //yetkili olduğu birim adını yazdırma 
                int birimID = Convert.ToInt32(Session["PersonelBirimId"]);
                //birimlere ulaştık b dedik b içerisinde deki birimId ile sessiondan gelen birimID eşitse getir b ddik select ile
                var birim = (from b in entity.Birimler where b.birimId == birimID select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

          
        }
        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller
                                  where p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  select p).ToList();
                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #region
        //iş ataması yapıalcak üç tane parametre girilecek bunu form collection ile de ypabiliriz beginform kullandığımız için
        //ya da
        //parametre olarak viewinde ki name adları ile aynı olmak zorunda
        //[HttpPost]
        //public ActionResult Ata(string isBaslik, string isAciklama, int selectPer)
        //{

        //    string baslikIs = isBaslik;
        //    string aciklamaIs = isAciklama;
        //    int secilenKisiId = selectPer;

        //    Isler yeniIs = new Isler();
        //    yeniIs.isBaslik = baslikIs;
        //    yeniIs.isAciklama = aciklamaIs;
        //    yeniIs.isPersonelId = secilenKisiId;
        //    yeniIs.iletilenTarih = DateTime.Now;
        //    yeniIs.isDurumId = 1;

        //    entity.Isler.Add(yeniIs);
        //    entity.SaveChanges();
        //    return View();
        //}
        #endregion

        #region 
        //[HttpPost]
        //public ActionResult Ata(string isBaslik, string isAciklama, int selectPer)
        //{
        //    Isler yeniIs = new Isler();
        //    yeniIs.isBaslik = isBaslik;
        //    yeniIs.isAciklama = isAciklama;
        //    yeniIs.isPersonelId = selectPer;
        //    yeniIs.iletilenTarih = DateTime.Now;
        //    yeniIs.isDurumId = 1;
        //    // Parametreler otomatik alınır

        //    entity.Isler.Add(yeniIs);
        //    entity.SaveChanges();
        //    return RedirectToAction("Takip", "Yonetici");
        //}
        #endregion



        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            //namelere göre aldık form
            //string userName = Session["PersonelAdSoyad"].ToString();
    

            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);

            Isler yeniIs = new Isler();
            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;

            entity.Isler.Add(yeniIs);
            entity.SaveChanges();

            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from i in entity.Personeller
                                  where i.personelBirimId == birimId && i.personelYetkiTurId == 2
                                  select i).ToList();
                ViewBag.personeller = calisanlar;

                //view tarafında birim adını yazdırmak için 
                var birim = (from i in entity.Birimler
                             where i.birimId == birimId 
                             select i).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from i in entity.Personeller 
                                   where i.personelId == selectPer
                                   select i).FirstOrDefault();
            #region
            //takip actionundan listele actionuna geçeceğim çünkü bu takip actionunda personel seçtik başka bir 
            //actionda seçilmiş olan bu personelin işlerini listeleyeceğiz 
            //takip metodunda yakaladığımız secilenPersonel 'in bu actionmetodunda görüntülenebilir olması gerek
            //ViewBag: metoddan vieve veri gönderiyor ;tek aşamalı
            //burda takip metodundan listele metoduna veri göndermek istiyorum bunun için tempdata
            //TempData: iki aşamada verimizin saklanabilmesini sağlıyor 
            #endregion
            TempData["secilen"] = secilenPersonel;
            return RedirectToAction("Listele", "Yonetici");
        }

        [HttpGet]
        public ActionResult Listele()
        {
            #region
            //takip httpost metodu çalıştığında listele action'u tetiklenecek , Tempdata içerisinde seçilen 
            //personel bilgileri yer alacak
            //tempdata kullanarak işlerin listesini barındırması lazım viewine göndermesi lazım 
            //bu bir get metdou olduğu için erişilebilir bunun için yetki kontrolü yapmamız lazım
            #endregion

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                #region
                //TempData'ya erişmemiz lazım
                //TempData personeller tipinde , o yüzden personeller tipinden nesne üretip
                //personellere dönüştürdü tempdata verisini

                #endregion
                Personeller secilenPersonel = (Personeller)TempData["secilen"];
                //tempdata içerisindeki veri personeller tipinde ki seçilen personele aktarıldı 

                try
                {
                    var isler = (from i in entity.Isler
                                 where i.isPersonelId == secilenPersonel.personelId
                                 select i).ToList().OrderByDescending(i => i.iletilenTarih);
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                    return View();
                }
                catch (Exception)
                {

                    return RedirectToAction("Takip", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public ActionResult Calisan()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Calisan(Personeller personel)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                Personeller eklenecekPer = new Personeller();
                eklenecekPer.personelAdSoyad = personel.personelAdSoyad;
                eklenecekPer.personelKullaniciAd = personel.personelKullaniciAd;
                eklenecekPer.personelParola = personel.personelParola;
                eklenecekPer.personelYetkiTurId = 2;
                eklenecekPer.personelBirimId = birimId;

                entity.Personeller.Add(eklenecekPer);
                entity.SaveChanges();
                return Json(new { success = true, Message = "Veri başarıyla eklendi.", redirectUrl = Url.Action("Index", "Yonetici") });
            }
            
            else {
                return Json(new { isValid = false, Message = "Veri Eklenemedi" });
            }
        }
    }
}