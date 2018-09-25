using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApiApplication.IRepository;
using ApiApplication.Models.Models;
using ApiApplication.Repository.Base;
using ApiApplication.Repository.sugar;
using SqlSugar;

namespace ApiApplication.Repository
{
    public class AdvertisementRepository: BaseRepository<Advertisement>, IAdvertisementRepository
    {
        public int Sum(int i, int j)
        {
            return i + j;
        }
    }
}