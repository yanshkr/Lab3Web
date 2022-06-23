namespace Lab3Web.DataBase.Sheets
{
    public class Alarm
    {
        public int UserId { get; set; }
        public int AlarmId { get; set; }
        public DateTime EndTime { get; set; }
        public State Status { get; set; }
        public enum State
        {
            Active,
            Inactive,
            Expired
        }
    }
}
