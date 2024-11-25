// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppleDeveloperAccount
{
	/// <summary>
	/// A base class for responses that include a list of paged items.
	/// </summary>
	class PagedItemsResponse<T> where T : class
	{
		/// <summary>
		/// The resource data.
		/// </summary>
		[JsonProperty ("data")]
		public List<T>? Data { get; set; }

		/// <summary>
		/// Navigational links that include the self-link.
		/// </summary>
		[JsonProperty ("links")]
		public PagedDocumentLinks? Links { get; set; }

		/// <summary>
		/// Paging information.
		/// </summary>
		[JsonProperty ("meta")]
		public PagingInformation? Meta { get; set; }

		/// <summary>
		/// An array of included items.
		/// </summary>
		[JsonProperty ("included")]
		public JArray? Included { get; set; }
	}
}
