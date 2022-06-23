using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Lab3Web.DataBase;
using Lab3Web.Requests;

namespace Lab3Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static readonly Random _random = new();
        private readonly ApplicationContext _context;

        public AuthController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("ValidateSession")]
        public void ValidateSession()
        {
            if (Request.Headers.ContainsKey("auth-token"))
                if (_context.Users.FirstOrDefault(o => o.AuthToken == Request.Headers["auth-token"]) != null)
                    Response.StatusCode = 200;   
            else
                Response.StatusCode = 400;
        }

        [HttpPost("CredentialsAuth")]
        public void CredentialsAuth()
        {
            string rawJsonBody;
            using (StreamReader requestStream = new(Request.Body))
            {
                rawJsonBody = requestStream.ReadToEnd();
            }

            LoginRequest? loginRequest;
            try
            {
                loginRequest = JsonSerializer.Deserialize<LoginRequest>(rawJsonBody);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return;
            }

            DataBase.Sheets.User? user;
            if (loginRequest != null &&
                loginRequest.Login != null &&
                loginRequest.Password != null &&
                (user = _context.Users.FirstOrDefault(o => o.UserName == loginRequest.Login)) != null &&
                user.Password == loginRequest.Password)
            {
                string newAuthToken = GenerateAuthToken();
                user.AuthToken = newAuthToken;
                _context.Users.Update(user);
                _context.SaveChanges();
                Response.Headers.Add("auth-token", newAuthToken);
                Response.StatusCode = 200;
            }
            else
                Response.StatusCode = 404;
        }

        public static string GenerateAuthToken(int length = 20)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}