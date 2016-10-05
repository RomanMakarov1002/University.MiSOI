using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Misoi.Infrastructure;
using Misoi.Models;

namespace Misoi.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IImageRepository imageRepository;

        public HomeController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        public ActionResult Images()
        {
            var result = imageRepository.GetAll();
            return View("Images",result);
        }

        [HttpGet]
        [ActionName("AddPicture")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ActionName("AddPicture")]
        public ActionResult CreatePicture(HttpPostedFileBase baseImg)
        {
            var image = new LoadedImage();
            BitmapConverter.SaveFileToDisk(baseImg, Server.MapPath("~/"), image);
            if (TryUpdateModel(image, new FormValueProvider(this.ControllerContext)))
            {
                imageRepository.Add(image);
                return RedirectToAction("Images");
            }
            else
            {
                return RedirectToAction("AddPicture");
            }
        }

        public ActionResult Details(int id)
        {
            var img = imageRepository.GetById(id);
            if (img != null)
                return View(img);
            return RedirectToAction("Images");
        }

        [HttpPost]
        public JsonResult GetChart(int id)
        {
            var image = imageRepository.GetById(id);
            var chart = BitmapConverter.GetChart(BitmapConverter.GetImage(image.ImagePhysicalPath));
            return Json(chart);
        }

        [HttpPost]
        public JsonResult GetNegativeImg(int id)
        {
            var image = imageRepository.GetById(id);
            var negativeImg = BitmapConverter.MakeNegative(BitmapConverter.GetImage(image.ImagePhysicalPath), image, Server.MapPath("~/"));
            imageRepository.Update(image);
            return Json(negativeImg);
        }

        [HttpPost]
        public JsonResult GetFilteredImg(int id)
        {
            double[,] PrewitX = new double[3,3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
            double[,] PrewitY  = new double[3,3] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
            var image = imageRepository.GetById(id);
            var filteredImg = BitmapConverter.AddFilter(Server.MapPath("~/"), image,
                BitmapConverter.GetImage(image.ImagePhysicalPath), PrewitX, PrewitY);
            imageRepository.Update(image);
            return Json(filteredImg);
        }

        [HttpPost]
        public JsonResult GetChartParams(int id, string type)
        {
            var image = imageRepository.GetById(id);
            ImageChart chart = null;
            if (type == "negative")
                chart = BitmapConverter.GetChart(BitmapConverter.GetImage(Server.MapPath("~/") + image.Negative));
            if (type == "filtered")
                chart = BitmapConverter.GetChart(BitmapConverter.GetImage(Server.MapPath("~/") + image.ImageFiltered));

            return Json(chart);
        }
    }
}