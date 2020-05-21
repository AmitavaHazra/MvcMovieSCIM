﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.SCIM
{
    using System.Collections.Generic;
    using System.Net.Http;

    public sealed class EventRequest : SystemForCrossDomainIdentityManagementRequest<IEventToken>
    {
        public EventRequest(
            HttpRequestMessage request,
            IEventToken payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}