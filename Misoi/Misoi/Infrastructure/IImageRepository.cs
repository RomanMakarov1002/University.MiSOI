using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misoi.Models;

namespace Misoi.Infrastructure
{
    public interface IImageRepository : IRepository<LoadedImage>
    {
    }
}
