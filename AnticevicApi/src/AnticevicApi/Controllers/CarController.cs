using AnticevicApi.Config;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.View.Car;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class CarController : BaseController
    {
        public CarController(IOptions<AppSettings> options) : base(options)
        {

        }

        #region Get

        [HttpGet]
        [Route("{valueId}/log/count")]
        public int GetLogCount(string valueId)
        {
            return CarHandler.GetLogCount(valueId);
        }

        [HttpGet]
        [Route("{valueId}/log/latest")]
        public CarLog GetLogLatest(string valueId)
        {
            return CarHandler.GetLatestLog(valueId);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("{valueId}/log")]
        public DateTime PutLog([FromBody] CarLogBinding binding, string valueId)
        {
            binding.CarValueId = valueId;
            return CarHandler.CreateLog(binding);
        }

        #endregion
    }
}
