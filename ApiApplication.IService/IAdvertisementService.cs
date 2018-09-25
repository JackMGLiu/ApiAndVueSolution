using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiApplication.IService.Base;
using ApiApplication.Models.Models;

namespace ApiApplication.IService
{
    public interface IAdvertisementService : IBaseService<Advertisement>
    {
        int Sum(int i, int j);

        Task<List<Advertisement>> GetAdvertisements();
    }
}