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
    public class LoginController : Controller
    {
        IsTakipDBEntities entity =  new IsTakipDBEntities();
        // GET: Default
        public ActionResult Index()
        {
            //ındex actionu gönderdiği için tekrar tekrar yazıp silmiyor o yüzden ilk başta boş tanımaladı
            ViewBag.mesaj = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string kullaniciAd ,string parola)
        {
            var personel = (from p in entity.Personeller
                            where p.personelKullaniciAd == kullaniciAd && p.personelParola == parola
                            select p).FirstOrDefault();
            if(personel != null)
            {
                Session["PersonelAdSoyad"] = personel.personelAdSoyad;
                Session["PersonelId"] = personel.personelId;
                Session["PersonelBirimId"] = personel.personelBirimId;
                Session["PersonelYetkiTurId"] = personel.personelYetkiTurId;

                switch (personel.personelYetkiTurId)
                {
                    case 1:
                        return RedirectToAction("Index", "Yonetici");

                    case 2:
                        return RedirectToAction("Index", "Calisan");

                    default:
                        return View();
                }
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı ve ya parola yanlış";
                return View();
            }
            
           
        }
       
    }
}