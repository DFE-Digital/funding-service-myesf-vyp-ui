using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Web.Controllers;
using Xunit;
using System.Collections.Generic;
using System;
using ViewYourPayments.Web.Shared;
using Newtonsoft.Json;

namespace ViewYourPayments.Web.Tests.Controllers
{
    public class BaseControllerTests
    {
        private readonly Mock<IOptions<AppSettings>> _appSettingsOptions;
        private readonly Mock<IUserAuthorisationService> _userAuthorisationService;
        private readonly Mock<IApplicationLogger> _applicationLogger;
        private readonly BaseController _baseController;
        private readonly AppSettings _appSetting;
        public BaseControllerTests()
        {
            _appSettingsOptions = new Mock<IOptions<AppSettings>>();
            _userAuthorisationService = new Mock<IUserAuthorisationService>();
            _applicationLogger = new Mock<IApplicationLogger>();
            _appSetting = new AppSettings { MyEsfUrl = "https://test.myesf.com" };
            _baseController = new BaseController(_applicationLogger.Object, _userAuthorisationService.Object);
        }
        protected IEnumerable<KeyValuePair<string,string>> GenerateStringEnumerator(string key, string value)
        {
            var values = new KeyValuePair <string,string>[] {
                  new KeyValuePair<string,string>(key,value),
                };
            return values;
        }
    }
}
