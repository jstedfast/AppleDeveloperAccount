// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace AppleDeveloperAccount
{
    internal class Program
    {
        static void Main (string[] args)
        {
            var apiKey = new ApiKey ();

            apiKey.IssuerId = GetValue ("Issuer ID: ");
            apiKey.KeyId = GetValue ("Key ID: ");

            var path = GetValue ("Private key file path: ", true);

            if (!Pkcs8Helper.TryGetPrivateKey (File.ReadAllText (path), out var privateKey)) {
                Console.WriteLine ("Invalid private key.");
                return;
            }

            apiKey.PrivateKey = privateKey!;

            Console.WriteLine ();
            foreach (AppleAccountType accountType in Enum.GetValues (typeof (AppleAccountType))) {
                Console.WriteLine ($"Testing {accountType} account...");
                try {
                    var client = new AppStoreConnectClient (accountType, apiKey) {
                        BackDateMinutes = 0,
                        ExpireAfterMinutes = 2
                    };
                    var users = client.GetUsersAsync (CancellationToken.None).GetAwaiter ().GetResult ();
                    Console.WriteLine ("Success!");
                    Console.WriteLine ();
                    Console.WriteLine ($"Users ({users.Count}):");
                    //foreach (var user in users)
                    //    Console.WriteLine($"* {user.Attributes?.FirstName} {user.Attributes?.LastName} <{user.Attributes?.Username}> Roles={user.Attributes?.RolesAsString}");
                    Console.WriteLine ();
                    Console.WriteLine ();
                    Console.WriteLine ($"When adding a new Apple Developer Account in Visual Studio, make sure to add an {accountType} account.");
                    Console.WriteLine ();
                    return;
                } catch (AppStoreConnectException asce) {
                    foreach (var error in asce.Errors) {
                        Console.WriteLine ($"* Error: {error.Title}");
                        Console.WriteLine ($"  Status: {error.Status}");
                        Console.WriteLine ($"  Code: {error.Code}");
                        Console.WriteLine ($"  Detail: {error.Detail}");
                    }
                } catch (Exception ex) {
                    Console.WriteLine ($"Failed: {ex.GetType ().Name}: {ex.Message}");
                }
                Console.WriteLine ();
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
