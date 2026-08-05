using MoviesRental.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesRental.Application.Contracts;

public interface IDirectorsWriteRepository : IWriteRepository<Director>
{
    Task<Director> GetDirectorWithMoviesAsync(Guid Id);
}
