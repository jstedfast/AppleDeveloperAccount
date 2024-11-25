// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppleDeveloperAccount
{
	/// <summary>
	/// The data structure that represents a Users resource.
	/// </summary>
	[DebuggerDisplay ("{Attributes.Username,nq}")]
	public class User
	{
		/// <summary>
		/// The resource's attributes.
		/// </summary>
		[JsonProperty ("attributes")]
		public UserAttributes? Attributes { get; set; }

		/// <summary>
		/// The opaque resource ID that uniquely identifies the resource.
		/// </summary>
		[JsonProperty ("id")]
		public string? Id { get; set; }

		/// <summary>
		/// Navigational links to related data and included resource types and IDs.
		/// </summary>
		[JsonProperty ("relationships")]
		public JToken? Relationships { get; set; }

		/// <summary>
		/// The resource type.
		/// </summary>
		/// <value>users</value>
		[JsonProperty ("type")]
		public string? Type { get; set; }

		/// <summary>
		/// Navigational links that include the self-link.
		/// </summary>
		[JsonProperty ("links")]
		public JToken? ResourceLinks { get; set; }

		public User ()
		{
			Type = "users";
		}
	}

	/// <summary>
	/// Attributes that describe a Users resource.
	/// </summary>
	[DebuggerDisplay ("{FirstName,nq} {LastName,nq} <{Username,nq}> Roles={RolesAsString,nq}")]
	public class UserAttributes
	{
		/// <summary>
		/// The user's first name.
		/// </summary>
		[JsonProperty ("firstName")]
		public string? FirstName { get; set; }

		/// <summary>
		/// The user's last name.
		/// </summary>
		[JsonProperty ("lastName")]
		public string? LastName { get; set; }

		/// <summary>
		/// Assigned user roles that determine the user's access to sections of App Store Connect and tasks they can perform.
		/// </summary>
		[JsonProperty ("roles")]
		public List<string>? Roles { get; set; }

		internal string RolesAsString {
			get { return Roles != null ? string.Join (", ", Roles) : "null"; }
		}

		/// <summary>
		/// A Boolean value that indicates the user's specified role allows access to the provisioning functionality on the Apple Developer website.
		/// </summary>
		[JsonProperty ("provisioningAllowed")]
		public bool ProvisioningAllowed { get; set; }

		/// <summary>
		/// A Boolean value that indicates whether a user has access to all apps available to the team.
		/// </summary>
		[JsonProperty ("allAppsVisible")]
		public bool AllAppsVisible { get; set; }

		/// <summary>
		/// The user's Apple ID.
		/// </summary>
		[JsonProperty ("username")]
		public string? Username { get; set; }
	}
}
