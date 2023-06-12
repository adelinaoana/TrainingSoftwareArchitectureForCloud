namespace DDDCqrsEs.Common.Identity
{
    public interface ISignInUserService
    {
        bool SignIn(string email, string password);
        void SignOut();
    }
}
