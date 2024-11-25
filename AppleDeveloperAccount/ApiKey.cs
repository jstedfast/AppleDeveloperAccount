// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Security.Cryptography;

namespace AppleDeveloperAccount
{
    public class ApiKey
    {
        public string IssuerId { get; set; }

        public string KeyId { get; set; }

        public ECDsa PrivateKey { get; set; }
    }
}
