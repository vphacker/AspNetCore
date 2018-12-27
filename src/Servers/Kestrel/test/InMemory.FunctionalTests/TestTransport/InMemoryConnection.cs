// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.Tracing;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Adapter.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.InMemory.FunctionalTests.TestTransport
{
    public class InMemoryConnection : StreamBackedTestConnection
    {

        public InMemoryConnection(InMemoryTransportConnection transportConnection)
            : base(new RawStream(transportConnection.Output, transportConnection.Input))
        {
            TransportConnection = transportConnection;
        }

        public InMemoryTransportConnection TransportConnection { get; }

        public override void Reset()
        {
            TransportConnection.Input.Complete(new ConnectionResetException(string.Empty));
            TransportConnection.OnClosed();
        }

        public override void ShutdownSend()
        {
            TransportConnection.Input.Complete();
            TransportConnection.OnClosed();
        }

        public override void Dispose()
        {
            if (KestrelEventSource.Log.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                TransportConnection.Log.LogDebug("InMemoryConnection.Dispose() started");
            }

            TransportConnection.Input.Complete();
            TransportConnection.Output.Complete();
            TransportConnection.OnClosed();
            base.Dispose();

            if (KestrelEventSource.Log.IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                TransportConnection.Log.LogDebug("InMemoryConnection.Dispose() complete");
            }
        }
    }
}
