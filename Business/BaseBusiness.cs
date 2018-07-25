using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.DataAccess;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : IBaseData<T>, new()
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;

        protected D Data => new D();

        private UserBusiness _userBusiness;
        private PasswordRecoveryBusiness _passwordRecoveryBusiness;
        private AdvisorBusiness _advisorBusiness;
        private AssetBusiness _assetBusiness;
        private AssetValueBusiness _assetValueBusiness;
        private FollowBusiness _followBusiness;
        private ExchangeApiAccessBusiness _exchangeApiAccessBusiness;
        private RequestToBeAdvisorBusiness _requestToBeAdvisorBusiness;

        protected BaseBusiness(ILoggerFactory loggerFactory, Cache cache)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        public IEnumerable<T> ListAll()
        {
            return Data.SelectAll();
        }

        public void Insert(T obj)
        {
            Data.Insert(obj);
        }

        public void Update(T obj)
        {
            Data.Update(obj);
        }

        public void Delete(T obj)
        {
            Data.Delete(obj);
        }

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(LoggerFactory, MemoryCache);
                return _userBusiness;
            }
        }

        protected PasswordRecoveryBusiness PasswordRecoveryBusiness
        {
            get
            {
                if (_passwordRecoveryBusiness == null)
                    _passwordRecoveryBusiness = new PasswordRecoveryBusiness(LoggerFactory, MemoryCache);
                return _passwordRecoveryBusiness;
            }
        }

        protected AdvisorBusiness AdvisorBusiness
        {
            get
            {
                if (_advisorBusiness == null)
                    _advisorBusiness = new AdvisorBusiness(LoggerFactory, MemoryCache);
                return _advisorBusiness;
            }
        }

        protected FollowBusiness FollowBusiness
        {
            get
            {
                if (_followBusiness == null)
                    _followBusiness = new FollowBusiness(LoggerFactory, MemoryCache);
                return _followBusiness;
            }
        }
        
        protected AssetBusiness AssetBusiness
        {
            get
            {
                if (_assetBusiness == null)
                    _assetBusiness = new AssetBusiness(LoggerFactory, MemoryCache);
                return _assetBusiness;
            }
        }

        protected AssetValueBusiness AssetValueBusiness
        {
            get
            {
                if (_assetValueBusiness == null)
                    _assetValueBusiness = new AssetValueBusiness(LoggerFactory, MemoryCache);
                return _assetValueBusiness;
            }
        }

        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness
        {
            get
            {
                if (_exchangeApiAccessBusiness == null)
                    _exchangeApiAccessBusiness = new ExchangeApiAccessBusiness(LoggerFactory, MemoryCache);
                return _exchangeApiAccessBusiness;
            }
        }

        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness
        {
            get
            {
                if (_requestToBeAdvisorBusiness == null)
                    _requestToBeAdvisorBusiness = new RequestToBeAdvisorBusiness(LoggerFactory, MemoryCache);
                return _requestToBeAdvisorBusiness;
            }
        }
    }
}
