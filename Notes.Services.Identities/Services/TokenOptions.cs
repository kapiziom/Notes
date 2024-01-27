namespace Notes.Services.Identities.Services;

public class TokenOptions
{
    public int AuthorizationCodeExpireTime { get; set; }
    public int AccessTokenExpireTime { get; set; }
    public int RefreshTokenExpireTime { get; set; }
}