// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FuseCP.Providers;
using FuseCP.Providers.OS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FuseCP.Web.Services
{

    public class TunnelHandlerBase
    {
        public string Route => "Tunnel";

        public async Task<TunnelSocket> GetTunnel(string caller, string method, byte[] arguments)
        {
            var service = new TunnelService(caller);
            return await service.Service.GetTunnel(method, arguments);
        }

        public async Task Transmit(TunnelSocket listener, TunnelSocket destination)
        {
            try
            {
                await listener.ProvideUpgradeTunnelSocketAsync(destination);
                await listener.Transmit(destination);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                if (listener.IsConnected) await listener.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.StackTrace);
                if (destination.IsConnected) await destination.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.StackTrace);
                throw new IOException(ex.Message, ex);
            } finally
            {
                destination.Dispose();
            }
        }

        public async Task<byte[]> ReadArgumentsAsync(TunnelSocket listener) => (await listener.ReceiveData()).ToArray();
    }

    public class TunnelHandlerCore : TunnelHandlerBase, ITunnelHandler
    {
        static bool WebSocketsInitialized = false;

        public void Init(WebApplication app)
        {
            if (!WebSocketsInitialized)
            {
                app.UseWebSockets();
                WebSocketsInitialized = true;
            }
            app.Map(Route, HandleRequest);
        }

        public async Task HandleRequest(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                string caller, method;
                caller = context.Request.Query["caller"];
                method = context.Request.Query["method"];
                if (string.IsNullOrEmpty(caller) || string.IsNullOrEmpty(method))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    try
                    {
                        var tunnel = new TunnelSocket(webSocket);

                        var args = await ReadArgumentsAsync(tunnel);

                        var dest = await GetTunnel(caller, method, args);
                        if (dest != null)
                        {
                            await Transmit(tunnel, dest);
                        }
                        else
                        {
                                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Cannot get a tunnel", CancellationToken.None);
                        }
                    }
                    catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, $"{ex.Message}\n{ex.StackTrace}", CancellationToken.None);
                    }

                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}


