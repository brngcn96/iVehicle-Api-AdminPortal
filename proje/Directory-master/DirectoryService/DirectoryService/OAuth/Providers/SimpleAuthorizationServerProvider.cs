using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;

using DirectoryService.Models;
using System.Linq;
using System.Data.Entity;
using System.Data;

namespace DirectoryService.OAuth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // OAuthAuthorizationServerProvider sınıfının client erişimine izin verebilmek için ilgili ValidateClientAuthentication metotunu override ediyoruz.
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        // OAuthAuthorizationServerProvider sınıfının kaynak erişimine izin verebilmek için ilgili GrantResourceOwnerCredentials metotunu override ediyoruz.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // User verify
            Model1Container _contextModel = new Model1Container();

            string email = context.UserName.ToString();

            Admin admin = (from c in _contextModel.Admin.AsNoTracking() where c.Email == email select c).FirstOrDefault();

            User user = (from c in _contextModel.User.AsNoTracking() where c.Email == email select c).FirstOrDefault();
            ChildUser childuser = (from c in _contextModel.ChildUser.AsNoTracking() where c.Email == email select c).FirstOrDefault();

            if (admin != null)
            {

                if (context.UserName == admin.Email && VerifyPasswordHash(context.Password.ToString(), admin.PasswordHash, admin.PasswordSalt))
                {

                    // CORS ayarlarını set ediyoruz.
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                    // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                    identity.AddClaim(new Claim("adminID", admin.Admin_ID.ToString()));
                    identity.AddClaim(new Claim("userName", context.UserName));
                    identity.AddClaim(new Claim("role", "admin"));

                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "The Username or Password is incorrect");
                }

            }
            else if (user != null)
            {

                if (context.UserName == user.Email && VerifyPasswordHash(context.Password.ToString(), user.PasswordHash, user.PasswordSalt))
                {

                    // CORS ayarlarını set ediyoruz.
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                    // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                    identity.AddClaim(new Claim("userID", user.User_ID.ToString()));
                    identity.AddClaim(new Claim("userName", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));

                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "The Username or Password is incorrect");
                }
            }
            else if (childuser != null)
            {

                if (context.UserName == childuser.Email && VerifyPasswordHash(context.Password.ToString(), childuser.PasswordHash, childuser.PasswordSalt))
                {
                    // CORS ayarlarını set ediyoruz.
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                    // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                    identity.AddClaim(new Claim("ParentID", childuser.Parent_ID.ToString()));
                    identity.AddClaim(new Claim("userName", context.UserName));
                    identity.AddClaim(new Claim("role", "childuser"));
                    identity.AddClaim(new Claim("childuserID", childuser.ChildUser_ID.ToString()));

                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "The Username or Password is incorrect");
                }
            }
            else { context.SetError("invalid_grant", "The Username or Password is incorrect"); }
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}