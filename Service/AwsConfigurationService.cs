using Amazon.SecurityToken.Model;
using Amazon.SecurityToken;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AwsConfigurationService : IAwsConfigurationService
    {
        private readonly IConfiguration _configuration;
        private string _resolvedBucketName;
        private string _accountId;

        public AwsConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetBucketNameAsync()
        {
            if (!string.IsNullOrEmpty(_resolvedBucketName))
            {
                return _resolvedBucketName;
            }

            var bucketName = _configuration["AWS:BucketName"];
            var environment = _configuration["Environment"];
            // TO DO: Figure out how to use this method to bucket naming avoiding uppercase exception for cloud formation stack
            var accountId = await GetAccountIdAsync(); 

            _resolvedBucketName = bucketName
                .Replace("{Environment}", environment);

            return _resolvedBucketName;
        }

        private async Task<string> GetAccountIdAsync()
        {
            if (!string.IsNullOrEmpty(_accountId))
            {
                return _accountId;
            }

            using var stsClient = new AmazonSecurityTokenServiceClient();
            var callerIdentity = await stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());
            _accountId = callerIdentity.Account;

            return _accountId;
        }

    }
}
