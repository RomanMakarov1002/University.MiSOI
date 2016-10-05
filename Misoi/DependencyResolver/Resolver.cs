using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Misoi;
using Misoi.Infrastructure;

namespace DependencyResolver
{
    public static class Resolver
    {
        public static void ConfigurateResolver(this IKernel kernel)
        {
            kernel.Bind<IImageRepository>().To<ImageRepository>();
        }
    }
}
