namespace Admin.NET.Application.CoalMine.PersonAttendance.Dtos;

public class PersonAttendanceInput
{
    public long MineId { get; set; }
    public long PersonInfoId { get; set; }
    public string CardId { get; set; }
    public string PersonName { get; set; }
    public string Department { get; set; }
    public string WorkType { get; set; }
    public DateTime? InTime { get; set; }
    public DateTime? OutTime { get; set; }
    public int? WorkDuration { get; set; }
    public int Status { get; set; }
    public DateTime AttendanceDate { get; set; }
}
