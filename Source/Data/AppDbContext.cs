using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VMS.Interface;
using VMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.EFCore.Extensions;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace VMS.Data
{
    public partial class AppDbContext: DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IHttpContextAccessor _contextAccessor;
                
        public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher, IHttpContextAccessor contextAccessor) : base(options)
        {
            _dispatcher = dispatcher;
            _contextAccessor = contextAccessor;
        }
       
    }

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var dbContext = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(), null, null);
            return;
            //dbContext.SaveChanges();
            dbContext.Dispose();
        }
    }
}
