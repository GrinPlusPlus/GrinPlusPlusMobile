﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GrinPlusPlus
{
    public class TorControlClient : IDisposable
    {
        private const string SuccessResponse = "250 OK";
        private const string ClosingConnectionResponse = "250 closing connection";
        private const int BufferSize = 4096;

        private TcpClient _tcpClient;
        private StreamReader _reader;
        private StreamWriter _writer;

        public async Task ConnectAsync(string hostname, int port)
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);
            var networkStream = _tcpClient.GetStream();
            _reader = new StreamReader(networkStream, Encoding.ASCII, false, BufferSize, true);
            _writer = new StreamWriter(networkStream, Encoding.ASCII, BufferSize, true);
        }

        public async Task AuthenticateAsync(string password)
        {
            var command = password != null ? $"AUTHENTICATE \"{password}\"" : "AUTHENTICATE";
            await SendCommandAsync(command, SuccessResponse).ConfigureAwait(false);
        }

        public async Task QuitAsync()
        {
            await SendCommandAsync("QUIT", ClosingConnectionResponse).ConfigureAwait(false);
        }

        public async Task CleanCircuitsAsync()
        {
            await SignalAsync("NEWNYM").ConfigureAwait(false);
        }

        private async Task SignalAsync(string signal)
        {
            await SendCommandAsync($"SIGNAL {signal}", SuccessResponse).ConfigureAwait(false);
        }

        public async Task DeleteONION(string key)
        {
            var command = $"DEL_ONION {key}";

            await SendCommandAsync(command, SuccessResponse).ConfigureAwait(false);
        }

        public async Task AddONION(string key, int inPort, int outPort)
        {
            var command = $"ADD_ONION ED25519-V3:{key} Flags=DiscardPK,Detach Port={inPort},{outPort}";

            await SendCommandAsync(command, SuccessResponse).ConfigureAwait(false);
        }

        private async Task<string> SendCommandAsync(string command, string expectedResponse)
        {
            if (_tcpClient == null)
            {
                throw new Exception("The Tor control client has not connected.");
            }

            await _writer.WriteLineAsync(command).ConfigureAwait(false);
            await _writer.FlushAsync().ConfigureAwait(false);

            var response = await _reader.ReadLineAsync().ConfigureAwait(false);
            if (response != expectedResponse)
            {
                throw new Exception($"The command to authenticate failed with error: {response}");
            }

            return response;
        }

        public void Dispose()
        {
            _tcpClient?.Close();
            _reader?.Dispose();
            _writer?.Dispose();
        }

        public void Close()
        {
            Dispose();
        }

        private class GetInfoValue
        {
            public GetInfoValue(string keyword, string value)
            {
                Keyword = keyword;
                Value = value;
            }

            public string Keyword { get; }
            public string Value { get; }
        }
    }
}
