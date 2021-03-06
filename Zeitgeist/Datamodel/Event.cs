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
using System.Collections;
using System.Linq;

namespace Zeitgeist.Datamodel
{
	/// <summary>
	/// Core data structure in the Zeitgeist framework. It is an optimized and convenient representation of an event.
	/// This class is designed so that you can pass it directly over DBus using the Python DBus bindings. 
	/// It will automagically be marshalled with the signature a(asaasay). 
	/// </summary>
	/// <remarks>
	/// This class does integer based lookups everywhere and can wrap any conformant data structure without the need for marshalling back and forth between DBus wire format. 
	/// These two properties makes it highly efficient and is recommended for use everywhere.
	/// </remarks>
	public class Event
	{ 
		/// <summary>
		/// A parameterless constructor
		/// </summary>
		public Event()
		{
			Subjects = new List<Subject>();
			Payload =new byte[]{};
			Actor = string.Empty;
			Origin = string.Empty;
			Timestamp = ZsUtils.Epoch;
			Interpretation = new NameUri();
			Manifestation = new NameUri();
		}
		
		/// <summary>
		/// Event id if the event has one
		/// </summary>
		public UInt64 Id
		{
			get;set;
		}
		
		/// <summary>
		/// Event timestamp defined as milliseconds since the Epoch.
		/// By default it is set to the moment of instance creation
		/// </summary>
		public DateTime Timestamp
		{
			get;set;
		}
		
		/// <summary>
		/// The application or entity responsible for emitting the event. 
		/// For applications the format of this field is base filename of the corresponding .desktop file with an app:// URI scheme. 
		/// For example /usr/share/applications/firefox.desktop is encoded as app://firefox.desktop
		/// </summary>
		public string Actor
		{
			get;set;
		}
		
		/// <summary>
		/// Origin of the Event
		/// </summary>
		public string Origin
		{
			get;set;
		}
		
		/// <summary>
		/// The Interpretation of the event
		/// </summary>
		public NameUri Interpretation
		{
			get;set;
		}
		
		/// <summary>
		/// The Manifestation of the event
		/// </summary>
		public NameUri Manifestation
		{
			get;set;
		}
		
		/// <summary>
		/// All the Subjects attached with the Event
		/// </summary>
		public List<Subject> Subjects
		{
			get;set;
		}
		
		/// <summary>
		/// Free form attachment for the event
		/// </summary>
		public byte[] Payload
		{
			get;set;
		}
				
		/// <summary>
		/// Get the RawEvent for this Event
		/// </summary>
		/// <returns>
		/// The RawEvent instance generated from the current Event <see cref="RawEvent"/>
		/// </returns>
		internal RawEvent GetRawEvent()
		{
			return RawEvent.FromEvent(this);
		}
	}
	
	/// <summary>
	/// The Raw form of Event. Get Event.FromRaw to create an Event from a RawEvent
	/// </summary>
	internal struct RawEvent 
	{
		/// <summary>
		/// The event Metadata
		/// </summary>
		public string[] metadata
		{
			get
			{
				return _metadata;
			}
			set
			{
				_metadata = value;
			}
		}
		
		/// <summary>
		/// The Subjects related to the Event
		/// </summary>
		public string[][] subjects
		{
			get
			{
				return _subjects;
			}
			set
			{
				_subjects = value;
			}
		}
		
		/// <summary>
		/// Free form attachment for the event
		/// </summary>
		public byte[] payload
		{
			get
			{
				return _payload;
			}
			set
			{
				_payload = value;
			}
		}
		
		/// <summary>
		/// A parameterized constructor for creating a RawEvent
		/// Even though it allows to create a RawEvent, please avoid it.
		/// Create an Event and then use Event.GetRawEvent()
		/// </summary>
		/// <param name="metadata">
		/// The metadata string array <see cref="T:System.String[]"/>
		/// </param>
		/// <param name="subjects">
		/// The subject of this RawEvent <see cref="T:System.String[][]"/>
		/// </param>
		/// <param name="payload">
		/// The payload associated with the RawEvent <see cref="T:System.Byte[]"/>
		/// </param>
		public RawEvent(string[] metadata, string[][] subjects, byte[] payload) : this()
		{
			this.metadata = metadata;
			this.subjects = subjects;
			this.payload = payload;
		}
		
		/// <summary>
		/// A static method which takes in a Event and converts it to a RawEvent
		/// </summary>
		/// <param name="ev">
		/// The instance of an Event <see cref="Event"/>
		/// </param>
		/// <returns>
		/// The instance of an RawEvent created <see cref="RawEvent"/>
		/// </returns>
		public static RawEvent FromEvent(Event ev)
		{
			RawEvent raw = new RawEvent();
			
			#region MetaData
			
			int metaDataLength = Enum.GetNames(typeof(EventMetadataPosition)).Length;
			List<string> metaDataList = new List<string>(metaDataLength);
			
			for(int i=0; i< metaDataList.Capacity; i++)
				metaDataList.Add(null);
			
			metaDataList[(int)EventMetadataPosition.Id] = (ev.Id == 0)? string.Empty: ev.Id.ToString();
			metaDataList[(int)EventMetadataPosition.Timestamp] = (ev.Id == 0)? string.Empty: ZsUtils.ToTimestamp(ev.Timestamp).ToString();
			
			metaDataList[(int)EventMetadataPosition.Actor] = ev.Actor;
			metaDataList[(int)EventMetadataPosition.Origin] = ev.Origin;
			metaDataList[(int)EventMetadataPosition.Interpretation] = ev.Interpretation.Uri;
			metaDataList[(int)EventMetadataPosition.Manifestation] = ev.Manifestation.Uri;
			
			raw.metadata = metaDataList.ToArray();
			
			#endregion
			
			#region Subject
			
			List<string[]> subList= new List<string[]>();
			foreach(Subject sub in ev.Subjects)
			{
				#region Individual Subject
				
				int subLength = Enum.GetNames(typeof(EventSubjectPosition)).Length;
				List<string> subCont = new List<string>(subLength);
				
				for(int i=0; i< subCont.Capacity; i++)
					subCont.Add(null);
				
				subCont[(int)EventSubjectPosition.Uri] = sub.Uri;
				subCont[(int)EventSubjectPosition.CurrentUri] = sub.CurrentUri;
				subCont[(int)EventSubjectPosition.Origin] = sub.Origin;
				subCont[(int)EventSubjectPosition.Mimetype] = sub.MimeType;
				subCont[(int)EventSubjectPosition.Text] = sub.Text;
				subCont[(int)EventSubjectPosition.Storage] = sub.Storage;
				subCont[(int)EventSubjectPosition.Interpretation] = sub.Interpretation.Uri;
				subCont[(int)EventSubjectPosition.Manifestation] = sub.Manifestation.Uri;
				
				subList.Add(subCont.ToArray());
				
				#endregion
			}
			
			raw.subjects = subList.ToArray();
			
			#endregion
			
			#region Payload
			
			raw.payload = ev.Payload;
			
			#endregion
			
			return raw;
		}
		
		/// <summary>
		/// Create a Event from a RawEvent
		/// </summary>
		/// <param name="raw">
		/// The instance of the RawEvent <see cref="RawEvent"/>
		/// </param>
		/// <returns>
		/// The instance of an Event <see cref="Event"/>
		/// </returns>
		public static Event FromRaw(RawEvent raw)
		{
			Event e = new Event();
			
			if(raw.metadata.Length != Enum.GetNames(typeof(EventMetadataPosition)).Length)
				return null;
			
			#region Metadata
			
			ulong id;
			UInt64.TryParse(raw.metadata[(int)EventMetadataPosition.Id], out id);
			e.Id = id;
			
			ulong timestamp;
			UInt64.TryParse(raw.metadata[(int)EventMetadataPosition.Timestamp], out timestamp);
			e.Timestamp = ZsUtils.ToDateTime(timestamp);
			
			e.Actor = raw.metadata[(int)EventMetadataPosition.Actor];
			e.Origin = raw.metadata[(int)EventMetadataPosition.Origin];
			
			string _interpretation = raw.metadata[(int)EventMetadataPosition.Interpretation];
			string _manifestation = raw.metadata[(int)EventMetadataPosition.Manifestation];
			e.Interpretation = Zeitgeist.Datamodel.Interpretation.Instance.Search(_interpretation);
			e.Manifestation = Zeitgeist.Datamodel.Manifestation.Instance.Search(_manifestation);
			
			#endregion
			
			#region Subjects
			
			e.Subjects= new List<Subject>();
			
			for(int i = 0; i < raw.subjects.Length; i ++)
			{
				Subject sub = new Subject();
				string[] subjArr = raw.subjects[i];
				
				sub.Uri = subjArr[(int)EventSubjectPosition.Uri];
				sub.Origin = subjArr[(int)EventSubjectPosition.Origin];
				sub.MimeType = subjArr[(int)EventSubjectPosition.Mimetype];
				sub.Text = subjArr[(int)EventSubjectPosition.Text];
				sub.Storage = subjArr[(int)EventSubjectPosition.Storage];
				sub.CurrentUri = subjArr[(int)EventSubjectPosition.CurrentUri];
				
				string sub_interpretation = subjArr[(int)EventSubjectPosition.Interpretation];
				string sub_manifestation = subjArr[(int)EventSubjectPosition.Manifestation];
				sub.Interpretation = Zeitgeist.Datamodel.Interpretation.Instance.Search(sub_interpretation);
				sub.Manifestation = Zeitgeist.Datamodel.Manifestation.Instance.Search(sub_manifestation);
				
				e.Subjects.Add(sub);
			}
			
			#endregion
			
			#region Payload
			
			e.Payload = raw.payload;
			
			#endregion
			
			return e;
		}
		
		#region RawEvent Private Fields
		
		public string[] _metadata;
		public string[][] _subjects;
		public byte[] _payload;
		
		#endregion
	}
	
	enum EventMetadataPosition
	{
		Id = 0,
		Timestamp = 1,
		Interpretation = 2,
		Manifestation = 3,
		Actor = 4,
		Origin = 5
	}
	
	enum EventSubjectPosition
	{
		Uri = 0,
		Interpretation = 1, 
		Manifestation = 2, 
		Origin = 3, 
		Mimetype = 4,
		Text = 5,
		Storage = 6,
		CurrentUri = 7
	}
		                            
}

