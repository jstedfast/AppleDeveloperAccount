// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace AppleDeveloperAccount
{
    internal class Program
    {
        static void Main (string[] args)
        {
            var successful = new List<(Pkcs8Backend, AppleAccountType, int, int)> ();
            var logFile = Path.GetFullPath ("apple-developer.log");
            var apiKey = new ApiKey ();

            apiKey.IssuerId = GetValue ("Issuer ID: ");
            apiKey.KeyId = GetValue ("Key ID: ");

            var path = GetValue ("Private key file path: ", true);
            var pemData = File.ReadAllText (path);

            Console.WriteLine ();
            Console.WriteLine ($"Logging diagnostics to {logFile}...");
            Console.WriteLine ();

            using (var logger = new StreamWriter ("apple-developer.log")) {
                foreach (Pkcs8Backend backend in Enum.GetValues (typeof (Pkcs8Backend))) {
                    logger.WriteLine ($"Attempting to use the {backend} backend for loading PEM private keys...");

                    if (!Pkcs8Helper.TryGetPrivateKey (backend, pemData, out var privateKey)) {
                        logger.WriteLine ($"Error: Failed to load PEM key data using the {backend} backend.");
                        continue;
                    }

                    apiKey.PrivateKey = privateKey!;

                    foreach (AppleAccountType accountType in Enum.GetValues (typeof (AppleAccountType))) {
                        logger.WriteLine ($"Testing {accountType} account + {backend} backend...");

                        var client = new AppStoreConnectClient (accountType, apiKey) {
                            BackDateMinutes = 0,
                            ExpireAfterMinutes = 2
                        };

                        if (CanAuthenticate (client, logger)) {
                            successful.Add ((backend, accountType, client.BackDateMinutes, client.ExpireAfterMinutes));
                        } else {
                            client.BackDateMinutes = 10;
                            client.ExpireAfterMinutes = 20;

                            if (CanAuthenticate (client, logger))
                                successful.Add ((backend, accountType, client.BackDateMinutes, client.ExpireAfterMinutes));
                        }

                        logger.WriteLine ();
                    }
                }

                // Print results to both the console -and- to the diagnostics log file
                Console.WriteLine ();
                logger.WriteLine ();

                if (successful.Count > 0) {
                    Console.WriteLine ("Successful configurations:");
                    logger.WriteLine ("Successful configurations:");
                    foreach (var (backend, accountType, backdate, expiresAfter) in successful) {
                        Console.WriteLine ($"* {accountType} account using the {backend} backend.");
                        Console.WriteLine ($"\tBackDateMinutes = {backdate}; ExpiresAfterMinutes = {expiresAfter}");
                        logger.WriteLine ($"* {accountType} account using the {backend} backend.");
                        logger.WriteLine ($"\tBackDateMinutes = {backdate}; ExpiresAfterMinutes = {expiresAfter}");
                    }
                } else {
                    Console.WriteLine ("No successful configurations.");
                    logger.WriteLine ("No successful configurations.");
                }
            }
        }

        static bool CanAuthenticate (AppStoreConnectClient client, StreamWriter logger)
        {
            try {
                var users = client.GetUsersAsync (CancellationToken.None).GetAwaiter ().GetResult ();
                logger.WriteLine ("\tSucessfully authenticated.");
                logger.WriteLine ($"\tAccount has {users.Count} active users.");
                return true;
            } catch (AppStoreConnectException asce) {
                logger.WriteLine ("\tFailed to authenticate.");
                foreach (var error in asce.Errors) {
                    logger.WriteLine ($"\t* Error: {error.Title}");
                    logger.WriteLine ($"\t  Status: {error.Status}");
                    logger.WriteLine ($"\t  Code: {error.Code}");
                    logger.WriteLine ($"\t  Detail: {error.Detail}");
                }
            } catch (Exception ex) {
                logger.WriteLine ($"\tFailed: {ex.GetType ().Name}: {ex.Message}");
            }

            return false;
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
