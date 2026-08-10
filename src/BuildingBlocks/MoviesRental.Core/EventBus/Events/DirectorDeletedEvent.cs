using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesRental.Core.EventBus.Events;

public record DirectorDeletedEvent(string Id);