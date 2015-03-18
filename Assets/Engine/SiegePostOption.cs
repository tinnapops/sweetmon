using UnityEngine;
using System.Collections;
using System;

namespace Sinoze.Engine
{
	public enum SiegePostOption 
	{
		None, 

		// The message lives until it's consumed
		// Warning : order of message receive is not guarantee
		SingleComsume,

		// After the message is sent to all recipients, server delete the message from its memory.
		SingleFrame,
		
		// After the message is sent to all recipients, server retain the message (for later peek)
		// the message will be deleted only when server Start(), Stop() or Reboot() is called.
		Persist,
	}
}