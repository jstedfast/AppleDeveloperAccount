// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace AppleDeveloperAccount
{
	public enum Pkcs8Backend
	{
		BouncyCastle,
		Native
	}

	public static class Pkcs8Helper
	{
		static byte[] GetPaddedByteArray (byte[] bytes, int length)
		{
			if (bytes.Length >= length)
				return bytes;

			var padded = new byte[length];

			Buffer.BlockCopy (bytes, 0, padded, length - bytes.Length, bytes.Length);

			return padded;
		}

		static bool TryGetPrivateKeyWithBouncyCastle (string pemData, out ECDsa? ecdsa)
		{
			using (var textReader = new StringReader (pemData)) {
				var pemReader = new PemReader (textReader); // We can't use the equivalent .NET methods, as they require .NET Standard 2.1
				var item = pemReader.ReadObject ();

				if (item is ECPrivateKeyParameters keyParameters) {
					var q = keyParameters.Parameters.G.Multiply (keyParameters.D).Normalize ();
					var d = keyParameters.D.ToByteArrayUnsigned ();
					var x = q.XCoord.GetEncoded ();
					var y = q.YCoord.GetEncoded ();

					// Q.X, Q.Y must be the same length. If D is specified, it must be the same length as Q.X and Q.Y
					// if also specified for named curves or the same length as Order for explicit curves.
					var len = Math.Max (Math.Max (x.Length, y.Length), d.Length);

					ecdsa = ECDsa.Create (new ECParameters {
						Curve = ECCurve.CreateFromValue (keyParameters.PublicKeyParamSet.Id),
						D = GetPaddedByteArray (d, len),
						Q = {
							X = GetPaddedByteArray (x, len),
							Y = GetPaddedByteArray (y, len)
						}
					});

					return true;
				}

				ecdsa = null;

				return false;
			}
		}

		public static bool TryGetPrivateKey (Pkcs8Backend backend, string pemData, out ECDsa? ecdsa)
		{
			if (backend == Pkcs8Backend.BouncyCastle)
				return TryGetPrivateKeyWithBouncyCastle (pemData, out ecdsa);

			ecdsa = ECDsa.Create ();

			try {
				ecdsa.ImportFromPem (pemData);
				return true;
			} catch {
				ecdsa.Dispose ();
				return false;
			}
		}
	}
}

