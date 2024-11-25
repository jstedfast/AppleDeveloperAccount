// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Security.Cryptography;

namespace AppleDeveloperAccount
{
    internal class Program
    {
        static void Main (string[] args)
        {
            var successful = new List<(Pkcs8Backend, AppleAccountType)> ();
            var apiKey = new ApiKey ();

            apiKey.IssuerId = GetValue ("Issuer ID: ");
            apiKey.KeyId = GetValue ("Key ID: ");

            var path = GetValue ("Private key file path: ", true);
            var pemData = File.ReadAllText (path);

            Console.WriteLine ();
            foreach (Pkcs8Backend backend in Enum.GetValues (typeof (Pkcs8Backend))) {
                //Console.WriteLine ($"Attempting to use the {backend} backend for loading PEM private keys...");

                if (!Pkcs8Helper.TryGetPrivateKey (backend, pemData, out var privateKey)) {
                    //Console.WriteLine ("Invalid private key.");
                    continue;
                }

                apiKey.PrivateKey = privateKey!;

                Console.WriteLine ();
                foreach (AppleAccountType accountType in Enum.GetValues (typeof (AppleAccountType))) {
                    Console.WriteLine ($"Testing {accountType} account + {backend} backend...");
                    try {
                        var client = new AppStoreConnectClient (accountType, apiKey) {
                            BackDateMinutes = 0,
                            ExpireAfterMinutes = 2
                        };
                        var users = client.GetUsersAsync (CancellationToken.None).GetAwaiter ().GetResult ();
                        //Console.WriteLine ("Success!");
                        //Console.WriteLine ();
                        //Console.WriteLine ($"Users ({users.Count}):");
                        //foreach (var user in users)
                        //    Console.WriteLine($"* {user.Attributes?.FirstName} {user.Attributes?.LastName} <{user.Attributes?.Username}> Roles={user.Attributes?.RolesAsString}");
                        //Console.WriteLine ();
                        //Console.WriteLine ();
                        //Console.WriteLine ($"When adding a new Apple Developer Account in Visual Studio, make sure to add an {accountType} account.");
                        //Console.WriteLine ();
                        successful.Add ((backend, accountType));
                    } catch (AppStoreConnectException asce) {
                        //foreach (var error in asce.Errors) {
                        //    Console.WriteLine ($"* Error: {error.Title}");
                        //    Console.WriteLine ($"  Status: {error.Status}");
                        //    Console.WriteLine ($"  Code: {error.Code}");
                        //    Console.WriteLine ($"  Detail: {error.Detail}");
                        //}
                    } catch (Exception ex) {
                        //Console.WriteLine ($"Failed: {ex.GetType().Name}: {ex.Message}");
                    }
                    //Console.WriteLine ();
                }
            }

            Console.WriteLine ();
            if (successful.Count > 0) {
                Console.WriteLine ("Succesasful configurations:");
                foreach (var (backend, accountType) in successful) {
                    Console.WriteLine ($"* {accountType} account using the {backend} backend.");
                }
            } else {
                Console.WriteLine ("No successful configurations.");
            }
        }

        static string? Unquote (string? text)
        {
            if (text == null || text.Length < 2 || text[0] != '"' || text[text.Length - 1] != '"')
                return null;

            return text.Substring (1, text.Length - 2);
        }

        static string GetValue (string prompt, bool isFilePath = false)
        {
            string? value;

            do {
                Console.Write (prompt);
                value = Console.ReadLine ();

                // Unquote paths because the user may have used "Copy as path" in Windows Explorer
                if (isFilePath)
                    value = Unquote (value);

                if (string.IsNullOrWhiteSpace (value)) {
                    Console.WriteLine ("Value cannot be empty.");
                } else if (isFilePath && !File.Exists (value)) {
                    Console.WriteLine ("File does not exist.");
                } else {
                    return value;
                }
            } while (true);
        }
    }
}
