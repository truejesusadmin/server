﻿using System;
using Microsoft.AspNetCore.Http;
using Bit.Core.IdentityServer;
using Xunit;
using NSubstitute;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Bit.Core.Test.IdentityServer
{
    public class TokenRetrievalTests
    {
        private Func<HttpRequest, string> Retrieve = TokenRetrieval.FromAuthorizationHeaderOrQueryString();

        [Fact]
        public void RetrieveToken_FromHeader_ReturnsToken()
        {
            // Arrange
            var headers = new HeaderDictionary
            {
                { "Authorization", "Bearer test_value" },
                { "X-Test-Header", "random_value" }
            };

            var request = Substitute.For<HttpRequest>();

            request.Headers.Returns(headers);

            // Act
            var token = Retrieve(request);

            // Assert
            Assert.Equal("test_value", token);
        }

        [Fact]
        public void RetrieveToken_FromQueryString_ReturnsToken()
        {
            // Arrange
            var queryString = new Dictionary<string, StringValues>
            {
                { "access_token", "test_value" },
                { "test-query", "random_value" }
            };

            var request = Substitute.For<HttpRequest>();
            request.Query.Returns(new QueryCollection(queryString));

            // Act
            var token = Retrieve(request);

            // Assert
            Assert.Equal("test_value", token);
        }

        [Fact]
        public void RetrieveToken_HasBoth_ReturnsHeaderToken()
        {
            // Arrange
            var queryString = new Dictionary<string, StringValues>
            {
                { "access_token", "query_string_token" },
                { "test-query", "random_value" }
            };

            var headers = new HeaderDictionary
            {
                { "Authorization", "Bearer header_token" },
                { "X-Test-Header", "random_value" }
            };

            var request = Substitute.For<HttpRequest>();
            request.Headers.Returns(headers);
            request.Query.Returns(new QueryCollection(queryString));

            // Act
            var token = Retrieve(request);

            // Assert
            Assert.Equal("header_token", token);
        }

        [Fact]
        public void RetrieveToken_NoToken_ReturnsNull()
        {
            // Arrange
            var request = Substitute.For<HttpRequest>();

            // Act
            var token = Retrieve(request);

            // Assert
            Assert.Null(token);
        }
    }
}
