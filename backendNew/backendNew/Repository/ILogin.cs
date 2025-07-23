using backendNew.DataAccessLayer;
using backendNew.Dtos;
using backendNew.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface ILogin
{
    Task<string?> AuthenticateAsync(LoginDTO loginDto);
}