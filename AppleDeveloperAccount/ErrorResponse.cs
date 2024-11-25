// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppleDeveloperAccount
{
	/// <summary>
	/// Information with error details that an API returns in the response body whenever the API request is not successful.
	/// </summary>
	public class ErrorResponse
	{
		/// <summary>
		/// An array of one or more errors.
		/// </summary>
		[JsonProperty ("errors")]
		public List<ErrorResponseError>? Errors { get; set; }
	}

	/// <summary>
	/// The details about one error that is returned when an API request is not successful.
	/// </summary>
	[DebuggerDisplay ("{Detail,nq}")]
	public class ErrorResponseError
	{
		/// <summary>
		/// A machine-readable code indicating the type of error. The code is a hierarchical value with levels of specificity separated by the '.' character. This value is parseable for programmatic error handling in code.
		/// </summary>
		[JsonProperty ("code")]
		public string? Code { get; set; }

		/// <summary>
		/// The HTTP status code of the error. This status code usually matches the response's status code; however, if the request produces multiple errors, these two codes may differ.
		/// </summary>
		[JsonProperty ("status")]
		public string? Status { get; set; }

		/// <summary>
		/// The unique ID of a specific instance of an error, request, and response. Use this ID when providing feedback to or debugging issues with Apple.
		/// </summary>
		[JsonProperty ("id")]
		public string? Id { get; set; }

		/// <summary>
		/// A summary of the error. Do not use this field for programmatic error handling.
		/// </summary>
		[JsonProperty ("title")]
		public string? Title { get; set; }

		/// <summary>
		/// A detailed explanation of the error. Do not use this field for programmatic error handling.
		/// </summary>
		[JsonProperty ("detail")]
		public string? Detail { get; set; }

		/// <summary>
		/// One of two possible types of values: source.Parameter, provided when a query parameter produced the error, or source.JsonPointer, provided when a problem with the entity produced the error.
		/// </summary>
		[JsonProperty ("source")]
		public JToken? Source { get; set; }
	}
}
