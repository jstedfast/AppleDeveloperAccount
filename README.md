# AppleDeveloperAccount

This tool is meant to be used to diagnose [Unable to add Apple account - Provide a properly configured and signed bearer token, and make sure that it has not expired](https://developercommunity.visualstudio.com/t/Unable-to-add-Apple-account---Provide-a-/10795534)

Before you begin, you will need to:

* Go to [https://appstoreconnect.apple.com](https://appstoreconnect.apple.com) and login.
* Click on the **Users and Access** icon.
* Click on the **Integrations** tab.
* Find your key in the list and make sure you have downloaded the private key file.

Once complete, you can run the diagnostics program by hitting **F5** or by going to the **Debug** menu and selecting **Start Debugging**.

The diagnostics program will ask for the following information that can all be found on the **Integrations** tab on your AppStoreConnect web portal:

* Issuer ID
* Key ID
* The file path to the downloaded key file (it will have a `.p8` file extension)

After providing the 3 pieces of information above, it will attempt to authenticate with Apple's AppStoreConnect API
to determine which account type the provided API Key is meant for as well as which settings the Visual Studio
developers will need to use in order to fix the bug that is causing some users to experience the error about an
improperly signed bearer token.
