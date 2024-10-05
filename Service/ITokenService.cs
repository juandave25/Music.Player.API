using Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ITokenService
    {
        string GenerateToken(string userId, JwtConfig config);
    }
}
