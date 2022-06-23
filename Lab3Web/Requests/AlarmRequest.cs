using Lab3Web.DataBase.Sheets;

namespace Lab3Web.Requests
{
    public class AlarmRequest
    {
        public string? AuthToken { get; set; }
        public DateTime EndTime { get; set; }
        public Alarm.State Status { get; set; }
    }
}
