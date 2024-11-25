// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Net;

namespace AppleDeveloperAccount
{
	public class AppStoreConnectException : Exception
	{
		static string? GetErrorMessage (ErrorResponse response)
		{
			if (response.Errors?.Count > 0)
				return response.Errors[0].Detail;

			return "Unknown error.";
		}

		public AppStoreConnectException (HttpStatusCode statusCode, ErrorResponse response) : base (GetErrorMessage (response))
		{
			StatusCode = statusCode;
			Errors = response.Errors;
		}

		public HttpStatusCode StatusCode { get; set; }

		public List<ErrorResponseError> Errors { get; set; }
	}
}
