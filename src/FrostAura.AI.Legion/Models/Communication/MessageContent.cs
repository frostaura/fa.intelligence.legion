﻿using FrostAura.AI.Legion.Enums.Communication;

namespace FrostAura.AI.Legion.Models.Communication;

/// <summary>
/// The content of a message in the Legion system.
/// </summary>
public class MessageContent
{
	/// <summary>
	/// The type of the content.
	/// </summary>
	public ContentType ContentType { get; set; }
	/// <summary>
	/// Content body of the message.
	/// </summary>
	public string Content { get; set; }
}
