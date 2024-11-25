// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Security.Cryptography;

using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;

namespace AppleDeveloperAccount
{
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

		public static bool TryGetPrivateKey (string text, out ECDsa? ecdsa)
		{
			using (var textReader = new StringReader (text)) {
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
	}
}

