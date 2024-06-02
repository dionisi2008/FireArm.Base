namespace CSO
{
    public class ActionHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }

        public ActionHistory(string id, string userId, string action, DateTime time)
        {
            Id = id;
            UserId = userId;
            Action = action;
            Time = time;
        }
    }
}