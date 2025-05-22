using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnHotel.Core.AuthAggregate.Specifications;
public sealed class RefreshTokenByTokenSpec : Specification<RefreshToken>
{
  public RefreshTokenByTokenSpec(string token)
  {
    Query
        .Where(rt => rt.Token == token)
        .Include(rt => rt.User);
  }
}
