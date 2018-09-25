using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiApplication.IRepository.Base;
using ApiApplication.Repository.sugar;
using SqlSugar;

namespace ApiApplication.Repository.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        public DbContext Context { get; set; }
        internal SqlSugarClient Db { get; private set; }
        internal SimpleClient<TEntity> EntityDb { get; private set; }

        public BaseRepository()
        {
            DbContext.Init(BaseDbConfig.ConnectionString);
            Context = DbContext.GetDbContext();
            Db = Context.Db;
            EntityDb = Context.GetEntityDB<TEntity>(Db);
        }

        public async Task<TEntity> QueryById(object objId)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().InSingle(objId));
        }

        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId, bool blnUseCache = false)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().WithCacheIF(blnUseCache).InSingle(objId));
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIdArray(object[] lstIds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().In(lstIds).ToList());
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="model">实体类对象</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity model)
        {
            var res = await Task.Run(() => Db.Insertable(model).ExecuteReturnBigIdentity());
            //返回的i是long类型,这里你可以根据你的业务需要进行处理
            return (int)res;
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            var res = await Task.Run(() => Db.Deleteable<TEntity>(id).ExecuteCommand());
            return res > 0;
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="model">实体类对象</param>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity model)
        {
            var res = await Task.Run(() => Db.Deleteable(model).ExecuteCommand());
            return res > 0;
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">实体主键数组</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdArray(object[] ids)
        {
            var res = await Task.Run(() => Db.Deleteable<TEntity>().In(ids).ExecuteCommand());
            return res > 0;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="model">实体类对象</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity model)
        {
            //这种方式会以主键为条件
            var res = await Task.Run(() => Db.Updateable(model).ExecuteCommand());
            return res > 0;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体类对象</param>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await Task.Run(() => Db.Updateable(entity).Where(strWhere).ExecuteCommand() > 0);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体类对象</param>
        /// <param name="lstColumns">更新列</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "")
        {
            IUpdateable<TEntity> up = await Task.Run(() => Db.Updateable(entity));
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                up = await Task.Run(() => up.IgnoreColumns(it => lstIgnoreColumns.Contains(it)));
            }
            if (lstColumns != null && lstColumns.Count > 0)
            {
                up = await Task.Run(() => up.UpdateColumns(it => lstColumns.Contains(it)));
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                up = await Task.Run(() => up.Where(strWhere));
            }
            return await Task.Run(() => up.ExecuteCommand()) > 0;
        }

        public async Task<bool> Update(string strSql, SugarParameter[] parameters = null)
        {
            return await Task.Run(() => Db.Ado.ExecuteCommand(strSql, parameters) > 0);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> Query()
        {
            return await Task.Run(() => EntityDb.GetList());
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(string strWhere)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Task.Run(() => EntityDb.GetList(whereExpression));
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToList());
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="orderByExpression">排序条件</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToList());
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
        }

        /// <summary>
        /// 查询前N条数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToList());
        }

        /// <summary>
        /// 查询前N条数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToList());
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageList(intPageIndex, intPageSize));
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageList(intPageIndex, intPageSize));
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 0, int intPageSize = 20, string strOrderByFileds = null)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageList(intPageIndex, intPageSize));
        }
    }
}