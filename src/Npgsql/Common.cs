#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Base class for all classes which represent a message sent by the PostgreSQL backend.
    /// </summary>
    internal interface IBackendMessage
    {
        BackendMessageCode Code { get; }
    }

    /// <summary>
    /// Base class for all classes which represent a message sent to the PostgreSQL backend.
    /// </summary>
    internal abstract class FrontendMessage {}

    /// <summary>
    /// Represents a simple frontend message which is typically small and fits well within
    /// the write buffer. The message is first queries for the number of bytes it requires,
    /// and then writes itself out.
    /// </summary>
    internal abstract class SimpleFrontendMessage : FrontendMessage
    {
        /// <summary>
        /// Returns the number of bytes needed to write this message.
        /// </summary>
        internal abstract int Length { get; }

        /// <summary>
        /// Writes the message contents into the buffer.
        /// </summary>
        internal abstract void Write(NpgsqlBuffer buf);
    }

    /// <summary>
    /// Represents an arbitrary-length message capable of flushing the buffer internally as it's
    /// writing itself out.
    /// </summary>
    internal abstract class ChunkingFrontendMessage : FrontendMessage
    {
        /// <param name="buf">the buffer into which to write the message.</param>
        /// <param name="directBuf">
        /// an option buffer that, if returned, will be written to the server directly, bypassing our
        /// NpgsqlBuffer. This is an optimization hack for bytea.
        /// </param>
        /// <returns>
        /// Whether there was enough space in the buffer to contain the entire message.
        /// If false, the buffer should be flushed and write should be called again.
        /// </returns>
        internal abstract bool Write(NpgsqlBuffer buf, ref DirectBuffer directBuf);
    }

    internal enum BackendMessageCode : byte
    {
        AuthenticationRequest = (byte)'R',
        BackendKeyData        = (byte)'K',
        BindComplete          = (byte)'2',
        CloseComplete         = (byte)'3',
        CompletedResponse     = (byte)'C',
        CopyData              = (byte)'d',
        CopyDone              = (byte)'c',
        CopyBothResponse      = (byte)'W',
        CopyInResponse        = (byte)'G',
        CopyOutResponse       = (byte)'H',
        DataRow               = (byte)'D',
        EmptyQueryResponse    = (byte)'I',
        ErrorResponse         = (byte)'E',
        FunctionCall          = (byte)'F',
        FunctionCallResponse  = (byte)'V',
        NoData                = (byte)'n',
        NoticeResponse        = (byte)'N',
        NotificationResponse  = (byte)'A',
        ParameterDescription  = (byte)'t',
        ParameterStatus       = (byte)'S',
        ParseComplete         = (byte)'1',
        PasswordPacket        = (byte)' ',
        PortalSuspended       = (byte)'s',
        ReadyForQuery         = (byte)'Z',
        RowDescription        = (byte)'T',
    }

    enum StatementOrPortal : byte
    {
        Statement = (byte)'S',
        Portal = (byte)'P'
    }

    /// <summary>
    /// Specifies the type of SQL statement, e.g. SELECT
    /// </summary>
    public enum StatementType
    {
#pragma warning disable 1591
        Select,
        Insert,
        Delete,
        Update,
        CreateTableAs,
        Move,
        Fetch,
        Copy,
        Other
#pragma warning restore 1591
    }

    /// <summary>
    /// The way how to order bytes.
    /// </summary>
    enum ByteOrder
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Most significant byte first (XDR)
        /// </summary>
        MSB = 0,
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Less significant byte first (NDR)
        /// </summary>
        LSB = 1
    }

    #region Component model attributes missing from CoreCLR

#if DNXCORE50 || DOTNET
    [AttributeUsage(AttributeTargets.Property)]
    class DisplayNameAttribute : Attribute
    {
        internal string DisplayName { get; private set; }

        internal DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    class CategoryAttribute : Attribute
    {
        internal CategoryAttribute(string category) {}
    }

    [AttributeUsage(AttributeTargets.Property)]
    class DescriptionAttribute : Attribute
    {
        internal DescriptionAttribute(string description) {}
    }

    [AttributeUsage(AttributeTargets.Property)]
    sealed class BrowsableAttribute : Attribute
    {
        public BrowsableAttribute(bool browsable) {}
    }

    [AttributeUsage(AttributeTargets.Property)]
    sealed class PasswordPropertyTextAttribute : Attribute
    {
        public PasswordPropertyTextAttribute(bool password) {}
    }

    enum RefreshProperties {
        All
    }

    [AttributeUsage(AttributeTargets.Property)]
    sealed class RefreshPropertiesAttribute : Attribute
    {
        public RefreshPropertiesAttribute(RefreshProperties refreshProperties) {}
    }
#endif

    #endregion
}
