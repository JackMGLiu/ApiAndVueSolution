using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiApplication.IRepository.Base;
using ApiApplication.IService.Base;
using ApiApplication.Repository.Base;

namespace ApiApplication.Service.Base
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        //public IBaseRepository<TEntity> baseDal = new BaseRepository<TEntity>();
        public IBaseRepository<TEntity> baseDal;//通过在子类的构造函数中注入，这里是基类，不用构造函数

        public async Task<TEntity> QueryById(object objId)
        {
            return await baseDal.QueryById(objId);
        }

        public async Task<TEntity> QueryById(object objId, bool blnUseCache = false)
        {
            return await baseDal.QueryById(objId, blnUseCache);
        }

        public async Task<List<TEntity>> QueryByIdArray(object[] lstIds)
        {
            return await baseDal.QueryByIdArray(lstIds);
        }

        public async Task<int> Add(TEntity model)
        {
            return await baseDal.Add(model);
        }

        public async Task<bool> DeleteById(object id)
        {
            return await baseDal.DeleteById(id);
        }

        public async Task<bool> Delete(TEntity model)
        {
            return await baseDal.Delete(model);
        }

        public async Task<bool> DeleteByIdArray(object[] ids)
        {
            return await baseDal.DeleteByIdArray(ids);
        }

        public async Task<bool> Update(TEntity model)
        {
            return await baseDal.Update(model);
        }

        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await baseDal.Update(entity, strWhere);
        }

        public async Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "")
        {
            return await baseDal.Update(entity, lstColumns, lstIgnoreColumns, strWhere);
        }

        public async Task<List<TEntity>> Query()
        {
            return await baseDal.Query();
        }

        public async Task<List<TEntity>> Query(string strWhere)
        {
            return await baseDal.Query(strWhere);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await baseDal.Query(whereExpression);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await baseDal.Query(whereExpression, strOrderByFileds);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await baseDal.Query(whereExpression, orderByExpression, isAsc);
        }

        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFileds)
        {
            return await baseDal.Query(strWhere, strOrderByFileds);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await baseDal.Query(whereExpression, intTop, strOrderByFileds);
        }

        public async Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds)
        {
            return await baseDal.Query(strWhere, intTop, strOrderByFileds);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await baseDal.Query(
                whereExpression,
                intPageIndex,
                intPageSize,
                strOrderByFileds);
        }

        public async Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await baseDal.Query(
                strWhere,
                intPageIndex,
                intPageSize,
                strOrderByFileds);
        }

        public async Task<List<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 0, int intPageSize = 20, string strOrderByFileds = null)
        {
            return await baseDal.QueryPage(whereExpression,
                intPageIndex = 0, intPageSize, strOrderByFileds);
        }
    }
}