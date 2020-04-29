using DirectoryService.Dtos;
using DirectoryService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;

namespace DirectoryService.Controllers
{
    [Authorize]
    public class AuthanticateController : ApiController
    {
        private readonly Model1Container context;

        public AuthanticateController() { this.context = new Model1Container(); }

        // GET api/Authanticate/Login?
        [AllowAnonymous]
        [HttpGet]
        [ActionName("Login")]
        public AuthorizedDto Login([Required]string email, [Required]string password)
        {
            if (!ModelState.IsValid)
                return null;

            try
            {

                Admin admin = (from c in context.Admin.AsNoTracking() where c.Email == email select c).FirstOrDefault();
                User user = (from c in context.User.AsNoTracking() where c.Email == email select c).FirstOrDefault();
                ChildUser childuser = (from c in context.ChildUser.AsNoTracking() where c.Email == email select c).FirstOrDefault();

                if (admin == null && user == null && childuser == null)
                    return null;

                string strLocalUrl = "http://localhost:50894";

                WebRequest webRequest = WebRequest.Create(strLocalUrl + "/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                byte[] byteBody = new ASCIIEncoding().GetBytes("grant_type=password&username=" + email + "&password=" + password);

                webRequest.ContentLength = byteBody.Length;

                webRequest.GetRequestStream().Write(byteBody, 0, byteBody.Length);
                webRequest.GetRequestStream().Close();

                WebResponse webResponse = webRequest.GetResponse();

                var serializer = new DataContractJsonSerializer(typeof(AuthTokenDto));
                AuthTokenDto authTokenDto = serializer.ReadObject(webResponse.GetResponseStream()) as AuthTokenDto;

                if (admin != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = admin.Admin_ID,
                        Name = admin.Name,
                        Email = admin.Email,
                        ImageUrl = admin.ImageUrl,
                        Role = "admin",
                        Token = authTokenDto.access_token,
                    };
                }
                else if (user != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = user.User_ID,
                        Name = user.Name,
                        Email = user.Email,
                        ImageUrl = user.ImageUrl,
                        Role = "user",
                        Token = authTokenDto.access_token,
                    };
                }
                else if (childuser != null)
                {
                    return new AuthorizedDto()
                    {
                        ID = childuser.ChildUser_ID,
                        ParentID = childuser.Parent_ID,
                        Name = childuser.Name,
                        Email = childuser.Email,
                        ImageUrl = childuser.ImageUrl,
                        Role = "childuser",
                        Token = authTokenDto.access_token,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // GET api/Authanticate/Register?
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Register")]
        
        public HttpResponseMessage Register([Required]string name, [Required]string surname, [Required]string email, [Required]string password)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Model State not valid!");

            try
            {
                Admin admin = (from c in context.Admin where c.Email == email select c).FirstOrDefault();
               // User user = (from c in context.User where c.Email == email select c).FirstOrDefault();

                if (admin != null)
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is already exist!");

                byte[] passwordHash;
                byte[] passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                if (passwordHash != null || passwordSalt != null)
                {
                    Admin newAdmin = new Admin()
                    {
                        Name = name,
                        Surname = surname,
                        Email = email,
                        

                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Status = true,
                        
                        CreatorIP = ":::",
                        CreatorRole = "admin",
                        CreationDate = DateTime.Now,

                    };
                    context.Admin.Add(newAdmin);
                    context.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message);
                   // return Newtonsoft.Json.JsonConvert.SerializeObject(new { status = "reject(IntServerError)" });

            }
            //return Newtonsoft.Json.JsonConvert.SerializeObject(new { status = "accepted" });

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}
