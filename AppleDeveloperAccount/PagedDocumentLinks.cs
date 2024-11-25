// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using Newtonsoft.Json;

namespace AppleDeveloperAccount
{
	/// <summary>
	/// Links related to the response document, including paging links.
	/// </summary>
	public class PagedDocumentLinks
	{
		/// <summary>
		/// The link to the first page of documents.
		/// </summary>
		[JsonProperty ("first")]
		public Uri? First { get; set; }

		/// <summary>
		/// The link to the next page of documents.
		/// </summary>
		[JsonProperty ("next")]
		public Uri? Next { get; set; }

		/// <summary>
		/// The link that produced the current document.
		/// </summary>
		[JsonProperty ("self")]
		public Uri? Self { get; set; }
	}
}
