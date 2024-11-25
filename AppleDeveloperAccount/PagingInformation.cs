// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using Newtonsoft.Json;

namespace AppleDeveloperAccount
{
	/// <summary>
	/// Paging information for data responses.
	/// </summary>
	public class PagingInformation
	{
		/// <summary>
		/// The paging information details.
		/// </summary>
		[JsonProperty ("paging")]
		public PagingInformationPaging? Paging { get; set; }
	}

	/// <summary>
	/// Paging details such as the total number of resources and the per-page limit.
	/// </summary>
	public class PagingInformationPaging
	{
		/// <summary>
		/// The total number of resources matching your request.
		/// </summary>
		[JsonProperty ("total", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int? Total { get; set; }

		/// <summary>
		/// The maximum number of resources to return per page, from 0 to 200.
		/// </summary>
		[JsonProperty ("limit")]
		public int? Limit { get; set; }
	}
}
