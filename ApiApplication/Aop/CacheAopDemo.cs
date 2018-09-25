﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiApplication.Common.Attribute;
using Castle.DynamicProxy;

namespace ApiApplication.Aop
{
    public class CacheAopDemo : IInterceptor
    {
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private ICaching _cache;

        public CacheAopDemo(ICaching cache)
        {
            _cache = cache;
        }
        //public void Intercept(IInvocation invocation)
        //{
        //    //获取自定义缓存键
        //    var cacheKey = CustomCacheKey(invocation);
        //    //根据key获取相应的缓存值
        //    var cacheValue = _cache.Get(cacheKey);
        //    if (cacheValue != null)
        //    {
        //        //将当前获取到的缓存值，赋值给当前执行方法
        //        invocation.ReturnValue = cacheValue;
        //        return;
        //    }
        //    //去执行当前的方法
        //    invocation.Proceed();
        //    //存入缓存
        //    if (!string.IsNullOrWhiteSpace(cacheKey))
        //    {
        //        _cache.Set(cacheKey, invocation.ReturnValue);
        //    }
        //}

        /// <summary>
        /// Attribute版本
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            var qCachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;

            //如果需要验证
            if (qCachingAttribute != null)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                //根据key获取相应的缓存值
                var cacheValue = _cache.Get(cacheKey);
                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    invocation.ReturnValue = cacheValue;
                    return;
                }
                //去执行当前的方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }

        }

        /// <summary>
        /// 自定义缓存键
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个

            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key += $"{param}:";
            }

            return key.TrimEnd(':');
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string GetArgumentValue(object arg)
        {
            if (arg is int || arg is long || arg is string)
            {
                return arg.ToString();
            }

            if (arg is DateTime)
            {
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");
            }

            return "";
        }
    }
}
