using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Lab3Web.DataBase;
using Lab3Web.Requests;

namespace Lab3Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RegistrationController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public void Register()
        {
            string rawJsonBody;
            using (StreamReader requestStream = new(Request.Body))
            {
                rawJsonBody = requestStream.ReadToEnd();
            }

            RegistrationRequest? registerRequest;
            try
            {
                registerRequest = JsonSerializer.Deserialize<RegistrationRequest>(rawJsonBody);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return;
            }

            if (registerRequest != null &&
                registerRequest.Login != null &&
                registerRequest.Password != null &&
                registerRequest.RepeatPassword != null &&
                registerRequest.Password == registerRequest.RepeatPassword &&
                !_context.Users.Any(o => o.UserName == registerRequest.Login))
            {
                DataBase.Sheets.User user = new()
                {
                    UserName = registerRequest.Login,
                    Password = registerRequest.Password,
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                Response.StatusCode = 200;
            }
            else
                Response.StatusCode = 400;
        }
    }
}
