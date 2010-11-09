/******************************************************************************
 * The MIT/X11/Expat License
 * Copyright (c) 2010 Manish Sinha<mail@manishsinha.net>

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE. 
********************************************************************************/

using System;
using System.Collections.Generic;

namespace Zeitgeist.Datamodel
{
	/// <summary>
	/// The delegate for an event which is called by the Daemon when a matching event is inserted
	/// </summary>
	/// <remarks>
	/// Using Monitor a client can recieve events from the daemon when some event is inserted which
	/// matches the Event template provided. This delegeate defined the event for the same
	/// </remarks>
	public delegate void NotifyInsertHandler(TimeRange range, List<Event> events);
	
	/// <summary>
	/// The delegate for an event which is called by the Daemon when a matching event is deleted
	/// </summary>
	/// <remarks>
	/// Using Monitor a client can recieve events from the daemon when some event is deleted which
	/// matches the Event template provided. This delegeate defined the event for the same
	/// </remarks>
	public delegate void NotifyDeleteHandler(TimeRange range, List<UInt32> eventIds);
}

