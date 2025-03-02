using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services;

internal class AuthServices : IAuthServices
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UserRegistrationDTO> _userValidator;

    public AuthServices(
        IOptions<JwtSettings> jwtSettings,
        IUserRepository userRepository,
        IValidator<UserRegistrationDTO> userValidator,
        IMapper mapper)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _userValidator = userValidator;
        _mapper = mapper;
    }

    public async Task Registration(UserRegistrationDTO user, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        var validationResult = await _userValidator.ValidateAsync(user, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        await _userRepository.Add(_mapper.Map<User>(user), cancellationToken);
    }

    public async Task<TokenDTO> Login(LoginDTO loginDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        var user = await _userRepository.Login(loginDto.Email, loginDto.Password, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var accessToken = await GenerateAccessToken(user, cancellationToken);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.Expire);
        
        await _userRepository.Update(user, cancellationToken);

        return new TokenDTO
        {
            AccessToken = "Bearer " + accessToken,
            RefreshToken = refreshToken,
            ExpirationAccess = DateTime.UtcNow.AddMinutes(_jwtSettings.Expire)
        };
    }
    
    private async Task<string> GenerateAccessToken(User user, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        var roles = await _userRepository.GetRoles(user, cancellationToken);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Issuer);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Expire),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Issuer)),
            ValidateLifetime = false, 
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            token.Replace("Bearer ", ""), 
            tokenValidationParameters, 
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
    
    public async Task<TokenDTO> RefreshToken(string tokenDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        if (string.IsNullOrWhiteSpace(tokenDto))
            throw new ArgumentException(null, nameof(tokenDto));
        
        var principal = GetPrincipalFromExpiredToken(tokenDto);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userRepository.GetById(int.Parse(userId!), cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Refresh token has expired.");
        }

        var newAccessToken = await GenerateAccessToken(user, cancellationToken);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.Expire);
        await _userRepository.Update(user, cancellationToken);

        return new TokenDTO
        {
            AccessToken = "Bearer " + newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}