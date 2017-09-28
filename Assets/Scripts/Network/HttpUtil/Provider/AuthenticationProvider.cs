namespace HttpUtil.Provider
{
    public interface AuthenticationProvider
    {
        Header GetAuthHeader();
    }
}
