using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApiApplication.IRepository.Base;
using ApiApplication.Models.Models;

namespace ApiApplication.IRepository
{
    public interface IAdvertisementRepository : IBaseRepository<Advertisement>
    {
        int Sum(int i, int j);
    }
}