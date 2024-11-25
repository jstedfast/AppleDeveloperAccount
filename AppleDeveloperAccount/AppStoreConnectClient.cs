// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;

using Microsoft.IdentityModel.Tokens;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AppleDeveloperAccount
{
    public class AppStoreConnectClient
    {
        const string BaseEnterpriseProgramApiEndpointUrl = "https://api.enterprise.developer.apple.com";
        const string BaseAppStoreConnectApiEndpointUrl = "https://api.appstoreconnect.apple.com";
        const string EnterpriseProgramApiAudience = "apple-developer-enterprise-v1";
        const string AppStoreConnectApiAudience = "appstoreconnect-v1";
        const string UsersPath = "/v1/users";

        static readonly HttpClient SharedClient = new HttpClient ();

        readonly JwtSecurityTokenHandler handler;
        readonly SecurityKey securityKey;
        readonly bool disposeClient;
        readonly HttpClient client;
        readonly string baseUrl;
        readonly string aud;

        public AppStoreConnectClient (AppleAccountType accountType, ApiKey apiKey) : this (accountType, apiKey, SharedClient, false)
        {
        }

        public AppStoreConnectClient (AppleAccountType accountType, ApiKey apiKey, HttpClient client, bool disposeClient)
        {
            handler = new JwtSecurityTokenHandler ();
            securityKey = new ECDsaSecurityKey (apiKey.PrivateKey);
            this.disposeClient = disposeClient;
            this.client = client;
            AccountType = accountType;
            ApiKey = apiKey;

            if (accountType == AppleAccountType.Enterprise) {
                baseUrl = BaseEnterpriseProgramApiEndpointUrl;
                aud = EnterpriseProgramApiAudience;
            } else {
                baseUrl = BaseAppStoreConnectApiEndpointUrl;
                aud = AppStoreConnectApiAudience;
            }

            BackDateMinutes = 1;
            ExpireAfterMinutes = 2;
        }

        public AppleAccountType AccountType {
            get; private set;
        }

        public ApiKey ApiKey {
            get; private set;
        }

        public int BackDateMinutes {
            get; set;
        }

        public int ExpireAfterMinutes {
            get; set;
        }

        JwtHeader CreateJwtHeader ()
        {
            var credentials = new SigningCredentials (securityKey, SecurityAlgorithms.EcdsaSha256);
            var header = new JwtHeader (credentials);
            header.Add ("kid", ApiKey.KeyId);
            return header;
        }

        JwtPayload CreateJwtPayload (HttpMethod method, string pathAndQuery)
        {
            var iat = EpochTime.GetIntDate (DateTime.UtcNow);

            var payload = new JwtPayload ();
            payload.Add (JwtRegisteredClaimNames.Iss, ApiKey.IssuerId);
            payload.Add (JwtRegisteredClaimNames.Iat, iat - (BackDateMinutes * 60)); // backdate the iat by a few minutes to allow for clock skew
            payload.Add (JwtRegisteredClaimNames.Exp, iat + (ExpireAfterMinutes * 60)); // expiration time
            payload.Add (JwtRegisteredClaimNames.Aud, aud);

            if (method.Equals (HttpMethod.Get)) {
                var scopes = new string[] { $"GET {pathAndQuery}" };
                payload.Add ("scope", scopes);
            }

            return payload;
        }

        HttpRequestMessage CreateHttpRequestMessage (HttpMethod method, string pathAndQuery)
        {
            var header = CreateJwtHeader ();
            var payload = CreateJwtPayload (method, pathAndQuery);
            var securityToken = new JwtSecurityToken (header, payload);
            var token = handler.WriteToken (securityToken);

            var request = new HttpRequestMessage (method, baseUrl + pathAndQuery);

            request.Headers.Authorization = new AuthenticationHeaderValue ("Bearer", token);

            return request;
        }

        async Task<T> GetAsync<T> (string pathAndQuery, CancellationToken cancellationToken)
        {
            using (var request = CreateHttpRequestMessage (HttpMethod.Get, pathAndQuery)) {
                using (var response = await client.SendAsync (request, cancellationToken).ConfigureAwait (false)) {
                    var content = await response.Content.ReadAsStringAsync (cancellationToken).ConfigureAwait (false);

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<T> (content ?? string.Empty);

                    var result = JsonConvert.DeserializeObject<ErrorResponse> (content);
                    throw new AppStoreConnectException (response.StatusCode, result);
                }
            }
        }

        async Task<List<T>> GetItemsAsync<T>(string pathAndQuery, CancellationToken cancellationToken) where T : class
        {
            var response = await GetAsync<PagedItemsResponse<T>> (pathAndQuery, cancellationToken).ConfigureAwait (false);
            var items = response.Data ?? new List<T> ();

            while (response.Links?.Next != null) {
                response = await GetAsync<PagedItemsResponse<T>> (response.Links.Next.PathAndQuery, cancellationToken).ConfigureAwait (false);

                if (response.Data != null)
                    items.AddRange (response.Data);
            }

            return items;
        }

        public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            return GetItemsAsync<User> (UsersPath, cancellationToken);
        }

        public void Dispose ()
        {
            if (disposeClient)
                client.Dispose ();
        }
    }
}
