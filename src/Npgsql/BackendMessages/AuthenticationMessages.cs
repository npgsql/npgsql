using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;

namespace Npgsql.BackendMessages;

abstract class AuthenticationRequestMessage : IBackendMessage
{
    public BackendMessageCode Code => BackendMessageCode.AuthenticationRequest;
    internal abstract AuthenticationRequestType AuthRequestType { get; }
}

sealed class AuthenticationOkMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.Ok;

    internal static readonly AuthenticationOkMessage Instance = new();
    AuthenticationOkMessage() { }
}

sealed class AuthenticationCleartextPasswordMessage  : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.CleartextPassword;

    internal static readonly AuthenticationCleartextPasswordMessage Instance = new();
    AuthenticationCleartextPasswordMessage() { }
}

sealed class AuthenticationMD5PasswordMessage  : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.MD5Password;

    internal byte[] Salt { get; }

    internal static AuthenticationMD5PasswordMessage Load(NpgsqlReadBuffer buf)
    {
        var salt = new byte[4];
        buf.ReadBytes(salt, 0, 4);
        return new AuthenticationMD5PasswordMessage(salt);
    }

    AuthenticationMD5PasswordMessage(byte[] salt)
        => Salt = salt;
}

sealed class AuthenticationGSSMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.GSS;

    internal static readonly AuthenticationGSSMessage Instance = new();
    AuthenticationGSSMessage() { }
}

sealed class AuthenticationGSSContinueMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.GSSContinue;

    internal byte[] AuthenticationData { get; }

    internal static AuthenticationGSSContinueMessage Load(NpgsqlReadBuffer buf, int len)
    {
        len -= 4;   // The AuthRequestType code
        var authenticationData = new byte[len];
        buf.ReadBytes(authenticationData, 0, len);
        return new AuthenticationGSSContinueMessage(authenticationData);
    }

    AuthenticationGSSContinueMessage(byte[] authenticationData)
        => AuthenticationData = authenticationData;
}

sealed class AuthenticationSSPIMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.SSPI;

    internal static readonly AuthenticationSSPIMessage Instance = new();
    AuthenticationSSPIMessage() { }
}

#region SASL

sealed class AuthenticationSASLMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.SASL;
    internal List<string> Mechanisms { get; } = [];

    internal AuthenticationSASLMessage(NpgsqlReadBuffer buf)
    {
        while (buf.Buffer[buf.ReadPosition] != 0)
            Mechanisms.Add(buf.ReadNullTerminatedString());
        buf.ReadByte();
        if (Mechanisms.Count == 0)
            throw new NpgsqlException("Received AuthenticationSASL message with 0 mechanisms!");
    }
}

sealed class AuthenticationSASLContinueMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.SASLContinue;
    internal byte[] Payload { get; }

    internal AuthenticationSASLContinueMessage(NpgsqlReadBuffer buf, int len)
    {
        Payload = new byte[len];
        buf.ReadBytes(Payload, 0, len);
    }
}

sealed class AuthenticationSCRAMServerFirstMessage
{
    internal string Nonce { get; }
    internal string Salt { get; }
    internal int Iteration { get; }

    internal static AuthenticationSCRAMServerFirstMessage Load(byte[] bytes, ILogger connectionLogger)
    {
        var data = NpgsqlWriteBuffer.UTF8Encoding.GetString(bytes);
        string? nonce = null, salt = null;
        var iteration = -1;

        foreach (var part in data.Split(','))
        {
            if (part.StartsWith("r=", StringComparison.Ordinal))
                nonce = part.Substring(2);
            else if (part.StartsWith("s=", StringComparison.Ordinal))
                salt = part.Substring(2);
            else if (part.StartsWith("i=", StringComparison.Ordinal))
                iteration = int.Parse(part.Substring(2));
            else
                connectionLogger.LogDebug("Unknown part in SCRAM server-first message:" + part);
        }

        if (nonce == null)
            throw new NpgsqlException("Server nonce not received in SCRAM server-first message");
        if (salt == null)
            throw new NpgsqlException("Server salt not received in SCRAM server-first message");
        if (iteration == -1)
            throw new NpgsqlException("Server iterations not received in SCRAM server-first message");

        return new AuthenticationSCRAMServerFirstMessage(nonce, salt, iteration);
    }

    AuthenticationSCRAMServerFirstMessage(string nonce, string salt, int iteration)
    {
        Nonce = nonce;
        Salt = salt;
        Iteration = iteration;
    }
}

sealed class AuthenticationSASLFinalMessage : AuthenticationRequestMessage
{
    internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.SASLFinal;
    internal byte[] Payload { get; }

    internal AuthenticationSASLFinalMessage(NpgsqlReadBuffer buf, int len)
    {
        Payload = new byte[len];
        buf.ReadBytes(Payload, 0, len);
    }
}

sealed class AuthenticationSCRAMServerFinalMessage
{
    internal string ServerSignature { get; }

    internal static AuthenticationSCRAMServerFinalMessage Load(byte[] bytes, ILogger connectionLogger)
    {
        var data = NpgsqlWriteBuffer.UTF8Encoding.GetString(bytes);
        string? serverSignature = null;

        foreach (var part in data.Split(','))
        {
            if (part.StartsWith("v=", StringComparison.Ordinal))
                serverSignature = part.Substring(2);
            else
                connectionLogger.LogDebug("Unknown part in SCRAM server-first message:" + part);
        }

        if (serverSignature == null)
            throw new NpgsqlException("Server signature not received in SCRAM server-final message");

        return new AuthenticationSCRAMServerFinalMessage(serverSignature);
    }

    internal AuthenticationSCRAMServerFinalMessage(string serverSignature)
        => ServerSignature = serverSignature;
}

#endregion SASL

enum AuthenticationRequestType
{
    Ok = 0,
    CleartextPassword = 3,
    MD5Password = 5,
    GSS = 7,
    GSSContinue = 8,
    SSPI = 9,
    SASL = 10,
    SASLContinue = 11,
    SASLFinal = 12
}
