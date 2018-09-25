using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiApplication.Common.Attribute;
using ApiApplication.IRepository;
using ApiApplication.IService;
using ApiApplication.Models.Models;
using ApiApplication.Repository;
using ApiApplication.Service.Base;
using AutoMapper;

namespace ApiApplication.Service
{
    public class AdvertisementService : BaseService<Advertisement>, IAdvertisementService
    {
        IAdvertisementRepository _dal;
        IMapper _mapper;

        public AdvertisementService(IAdvertisementRepository dal, IMapper mapper)
        {
            this._dal = dal;
            base.baseDal = dal;
            this._mapper = mapper;
        }

        public int Sum(int i, int j)
        {
            return _dal.Sum(i, j);
        }

        //[Caching(AbsoluteExpiration = 10)]//增加特性
        public async Task<List<Advertisement>> GetAdvertisements()
        {
            var list = await _dal.Query(a => true);

            //AdvertisementViewModels models = IMapper.Map<AdvertisementViewModel>(list);

            return list;
        }
    }
}