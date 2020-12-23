// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("myIdentityResource1",new List<string>{ "myclaim1"})
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                //name & displayname
               new ApiScope("api1","My API"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId  = "console-client",

                    //no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("console-secret".Sha256())
                    },

                    AllowedScopes = {"api1"}
                },
                new Client // 注册OIDC客户端
                {
                    //because the folws in OIDC are always INTERACTIVE, we need to add some URLs to the configuration

                    ClientId = "mvc-client",
                    ClientSecrets = {new Secret("mvc-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    //主动发送用户信息  （客户端也可以主动获取）
                     //AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris  = {"https://localhost:5002/signin-oidc"},

                    PostLogoutRedirectUris = {"https://localhost:5002/signout-callback-oidc"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId, // oidc 必须
                        IdentityServerConstants.StandardScopes.Profile ,//oidc 必须
                        "myIdentityResource1"
                    }
                }
            };
    }
}