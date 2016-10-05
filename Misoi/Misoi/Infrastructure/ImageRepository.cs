using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Misoi.Models;

namespace Misoi.Infrastructure
{
    public class ImageRepository : IImageRepository
    {
        private readonly EntityModel context;

        public ImageRepository(EntityModel entityModelContext)
        {
            this.context = entityModelContext;
        }

        public void Add(LoadedImage entity)
        {
            context.Images.Add(entity);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<LoadedImage> GetAll()
        {
            return context.Images.ToList();
        }

        public LoadedImage GetById(int id)
        {
            return context.Images.FirstOrDefault(x => x.Id == id);
        }

        public void Update(LoadedImage entity)
        {
            var contextImg = context.Images.FirstOrDefault(x => x.Id == entity.Id);
            if (contextImg != null)
            {
                contextImg.ImagePath = entity.ImagePath;
                contextImg.ImagePhysicalPath = entity.ImagePhysicalPath;
                contextImg.Negative = entity.Negative;
                contextImg.Format = entity.Format;
                contextImg.Height = entity.Height;
                contextImg.Width = entity.Width;
                contextImg.Name = entity.Name;
            }
            context.SaveChanges();

        }
    }
}