using Microsoft.AspNetCore.Mvc;
using Lab3Web.DataBase;
using Lab3Web.DataBase.Sheets;
using Lab3Web.Requests;
using System.Text.Json;

namespace Lab3Web.Controllers
{
    [ApiController]
    [Route("api/alarm")]
    public class AlarmController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AlarmController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("GetAlarms")]
        public Alarm? GetAlarms()
        {
            if (Request.Headers.ContainsKey("auth-token"))
            {
                User? user;
                if ((user = _context.Users.FirstOrDefault(o => o.AuthToken == Request.Headers["auth-token"])) != null)
                {
                    Response.StatusCode = 200;
                    return _context.Alarms.FirstOrDefault(o => o.UserId == user.Id);
                }
                else
                    Response.StatusCode = 404;
             }
            else
                Response.StatusCode = 400;

            return null;
        }

        [HttpPost("AddAlarm")]
        public void AddAlarm()
        {
            string rawJsonBody;
            using (StreamReader requestStream = new(Request.Body))
            {
                rawJsonBody = requestStream.ReadToEnd();
            }

            AlarmRequest? alarmRequest;
            try
            {
                alarmRequest = JsonSerializer.Deserialize<AlarmRequest>(rawJsonBody);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return;
            }

            User? user;
            if (alarmRequest != null &&
                alarmRequest.AuthToken != null &&
                (user = _context.Users.FirstOrDefault(o => o.AuthToken == alarmRequest.AuthToken)) != null)
            {
                _context.Alarms.Add(new Alarm()
                {
                    UserId = user.Id,
                    EndTime = alarmRequest.EndTime,
                    Status = alarmRequest.Status
                });
                _context.SaveChanges();
                Response.StatusCode = 200;
            }
            else
                Response.StatusCode = 404;

        }
    }
}