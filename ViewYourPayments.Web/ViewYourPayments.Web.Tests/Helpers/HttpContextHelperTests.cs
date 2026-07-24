using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using ViewYourPayments.Web.Helpers;
using Xunit;

namespace ViewYourPayments.Web.Tests.Helpers
{
    [TestClass, TestCategory("Unit")]
    public class HttpContextHelperTests
    {
        private const string HttpHeaderKey = "the-HTTP-header-key";
        private const string HttpHeaderValue = "the-HTTP-header-value";

        private readonly Mock<HttpContext> _httpContext = new Mock<HttpContext>(MockBehavior.Strict);
        private readonly Mock<HttpResponse> _httpResponse = new Mock<HttpResponse>(MockBehavior.Strict);
        private readonly Mock<IHeaderDictionary> _headerDictionary = new Mock<IHeaderDictionary>(MockBehavior.Strict);

        #region AddOrReplaceResponseHeader

        [Fact]
        public void AddOrReplaceResponseHeader_WhenHttpContextIsNull_Throws()
        {
            // Act
            Action action = () => HttpContextHelper.AddOrReplaceResponseHeader(null, HttpHeaderKey, HttpHeaderValue);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("httpContext");
        }

        [Fact]
        public void AddOrReplaceResponseHeader_WhenKeyIsNull_Throws()
        {
            // Act
            Action action = () => HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, null, HttpHeaderValue);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("     ")]
        public void AddOrReplaceResponseHeader_WhenKeyIsEmptyOrWhiteSpace_Throws(string key)
        {
            // Act
            Action action = () => HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, key, HttpHeaderValue);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddOrReplaceResponseHeader_WhenValueIsNull_Throws()
        {
            // Act
            Action action = () => HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, HttpHeaderKey, null);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("value");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("     ")]
        public void AddOrReplaceResponseHeader_WhenValueIsEmptyOrWhiteSpace_Throws(string value)
        {
            // Act
            Action action = () => HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, HttpHeaderKey, value);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("value");
        }

        [Fact]
        public void AddOrReplaceResponseHeader_WhenKeyDoesNotExists_Adds()
        {
            // Arrange
            _httpContext
                .SetupGet(context => context.Response)
                .Returns(_httpResponse.Object);

            _httpResponse
                .SetupGet(response => response.Headers)
                .Returns(_headerDictionary.Object);

            _headerDictionary
                .Setup(dict => dict.ContainsKey(HttpHeaderKey))
                .Returns(false);

            _headerDictionary
                .Setup(dict => dict.Add(HttpHeaderKey, HttpHeaderValue));

            // Act
            HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, HttpHeaderKey, HttpHeaderValue);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void AddOrReplaceResponseHeader_WhenKeyExists_Replaces()
        {
            // Arrange
            _httpContext
                .SetupGet(context => context.Response)
                .Returns(_httpResponse.Object);

            _httpResponse
                .SetupGet(response => response.Headers)
                .Returns(_headerDictionary.Object);

            _headerDictionary
                .Setup(dict => dict.ContainsKey(HttpHeaderKey))
                .Returns(true);

            _headerDictionary
                .Setup(dict => dict.Remove(HttpHeaderKey))
                .Returns(true);

            _headerDictionary
                .Setup(dict => dict.Add(HttpHeaderKey, HttpHeaderValue));

            // Act
            HttpContextHelper.AddOrReplaceResponseHeader(_httpContext.Object, HttpHeaderKey, HttpHeaderValue);

            // Assert
            VerifyAllMocks();
        }

        #endregion


        #region RemoveResponseHeader

        [Fact]
        public void RemoveResponseHeader_WhenHttpContextIsNull_Throws()
        {
            // Act
            Action action = () => HttpContextHelper.RemoveResponseHeader(null, HttpHeaderKey);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("httpContext");
        }

        [Fact]
        public void RemoveResponseHeader_WhenKeyIsNull_Throws()
        {
            // Act
            Action action = () => HttpContextHelper.RemoveResponseHeader(_httpContext.Object, null);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("     ")]
        public void RemoveResponseHeader_WhenKeyIsEmptyOrWhiteSpace_Throws(string key)
        {
            // Act
            Action action = () => HttpContextHelper.RemoveResponseHeader(_httpContext.Object, key);

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("key");
        }

        [Fact]
        public void RemoveResponseHeader_WhenKeyDoesNotExist_Ignores()
        {
            // Arrange
            _httpContext
                .SetupGet(context => context.Response)
                .Returns(_httpResponse.Object);

            _httpResponse
                .SetupGet(response => response.Headers)
                .Returns(_headerDictionary.Object);

            _headerDictionary
                .Setup(dict => dict.ContainsKey(HttpHeaderKey))
                .Returns(false);

            // Act
            HttpContextHelper.RemoveResponseHeader(_httpContext.Object, HttpHeaderKey);

            // Assert
            VerifyAllMocks();
        }

        [Fact]
        public void RemoveResponseHeader_WhenKeyExists_Removes()
        {
            // Arrange
            _httpContext
                .SetupGet(context => context.Response)
                .Returns(_httpResponse.Object);

            _httpResponse
                .SetupGet(response => response.Headers)
                .Returns(_headerDictionary.Object);

            _headerDictionary
                .Setup(dict => dict.ContainsKey(HttpHeaderKey))
                .Returns(true);

            _headerDictionary
                .Setup(dict => dict.Remove(HttpHeaderKey))
                .Returns(true);

            // Act
            HttpContextHelper.RemoveResponseHeader(_httpContext.Object, HttpHeaderKey);

            // Assert
            VerifyAllMocks();
        }

        #endregion

        private void VerifyAllMocks()
            => Mock.VerifyAll(_httpContext, _httpResponse, _headerDictionary);
    }
}
