﻿using ClassicUO.Game.Data;
using System;

namespace ClassicUO.Game.Managers
{
    public class EventSink
    {
        /// <summary>
        /// Invoked when the player is connected to a server
        /// </summary>
        public static event EventHandler<EventArgs> OnConnected;
        public static void InvokeOnConnected(object sender) => OnConnected?.Invoke(sender, EventArgs.Empty);

        /// <summary>
        /// Invoked when any message is received from the server after client processing
        /// </summary>
        public static event EventHandler<MessageEventArgs> MessageReceived;
        public static void InvokeMessageReceived(object sender, MessageEventArgs e) => MessageReceived?.Invoke(sender, e);

        /// <summary>
        /// Invoked when any message is received from the server *before* client processing
        /// </summary>
        public static event EventHandler<MessageEventArgs> RawMessageReceived;
        public static void InvokeRawMessageReceived(object sender, MessageEventArgs e) => RawMessageReceived?.Invoke(sender, e);

        /// <summary>
        /// Not currently used. May be removed later or put into use, not sure right now
        /// </summary>
        public static event EventHandler<MessageEventArgs> LocalizedMessageReceived;
        public static void InvokeLocalizedMessageReceived(object sender, MessageEventArgs e) => LocalizedMessageReceived?.Invoke(sender, e);

        /// <summary>
        /// Invoked anytime a message is added to the journal
        /// </summary>
        public static event EventHandler<JournalEntry> JournalEntryAdded;
        public static void InvokeJournalEntryAdded(object sender, JournalEntry e) => JournalEntryAdded?.Invoke(sender, e);

        /// <summary>
        /// Invoked anytime we receive object property list data (Tooltip text for items)
        /// </summary>
        public static event EventHandler<OPLEventArgs> OPLOnReceive;
        public static void InvokeOPLOnReceive(object sender, OPLEventArgs e) => OPLOnReceive?.Invoke(sender, e);

        /// <summary>
        /// Invoked when a buff is "added" to a player
        /// </summary>
        public static event EventHandler<BuffEventArgs> OnBuffAdded;
        public static void InvokeOnBuffAdded(object sender, BuffEventArgs e) => OnBuffAdded(sender, e);

        /// <summary>
        /// Invoked when a buff is "removed" to a player. (Called before removal)
        /// </summary>
        public static event EventHandler<BuffEventArgs> OnBuffRemoved;
        public static void InvokeOnBuffRemoved(object sender, BuffEventArgs e) => OnBuffRemoved(sender, e);
    }

    public class OPLEventArgs : EventArgs
    {
        public readonly uint Serial;
        public readonly string Name;
        public readonly string Data;

        public OPLEventArgs(uint serial, string name, string data)
        {
            Serial = serial;
            Name = name;
            Data = data;
        }
    }

    public class BuffEventArgs : EventArgs
    {
        public BuffEventArgs(BuffIcon buff)
        {
            Buff = buff;
        }

        public BuffIcon Buff { get; }
    }
}
