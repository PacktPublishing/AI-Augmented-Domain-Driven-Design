using Muflone.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrewUp.Shared.DomainIds;

public sealed class AvailabilityId(string value) : DomainId(value);
