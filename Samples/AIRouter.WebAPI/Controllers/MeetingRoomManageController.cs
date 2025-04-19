using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace AIRouter.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MeetingRoomManageController : ControllerBase
{
    private static List<MeetingRoom> _meetingRooms = new List<MeetingRoom>
    {
        new MeetingRoom
        {
            Name = "会议室A",
            Capacity = 10,
            ReserveRecords = new List<MeetingRoomReserveRecord>
            {
                new MeetingRoomReserveRecord
                {
                    MeetingRoomName = "会议室A",
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2)
                }
            }
        },
        new MeetingRoom
        {
            Name = "会议室B",
            Capacity = 20,
            ReserveRecords = new List<MeetingRoomReserveRecord>
            {
                new MeetingRoomReserveRecord
                {
                    MeetingRoomName = "会议室B",
                    StartTime = DateTime.Now.AddHours(3),
                    EndTime = DateTime.Now.AddHours(4)
                }
            }
        }
    };

    public MeetingRoomManageController() { }

    /// <summary>
    /// 获取所有会议室信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public List<MeetingRoom> List()
    {
        return _meetingRooms;
    }

    /// <summary>
    /// 预定会议室
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public Response<MeetingRoom> Reserve([FromBody] ReserveMeetingRoomRequest request)
    {
        var meetingRoom = _meetingRooms.FirstOrDefault(m => m.Capacity >= request.Capacity);
        if (meetingRoom == null)
        {
            return new Response<MeetingRoom>
            {
                Success = false,
                Message = $"没有容量为{request.Capacity}人的会议室"
            };
        }
        // 检查时间冲突
        foreach (var existingRecord in meetingRoom.ReserveRecords)
        {
            if (
                (
                    request.StartTime >= existingRecord.StartTime
                    && request.StartTime < existingRecord.EndTime
                )
                || (
                    request.EndTime > existingRecord.StartTime
                    && request.EndTime <= existingRecord.EndTime
                )
            )
            {
                return new Response<MeetingRoom> { Success = false, Message = "这个时间点的会议室被其他人预定了" }; // 时间冲突
            }
        }
        meetingRoom.ReserveRecords.Add(
            new MeetingRoomReserveRecord
            {
                MeetingRoomName = meetingRoom.Name,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            }
        );
        return new Response<MeetingRoom>
        {
            Success = true,
            Message = "预定成功",
            Data = meetingRoom
        };
    }
}

[Description("会议室基本信息")]
public class MeetingRoom
{
    [Description("会议室名称")]
    public required string Name { get; set; }

    [Description("会议室容量，能坐多少人")]
    public int Capacity { get; set; }

    [Description("预定记录")]
    public List<MeetingRoomReserveRecord> ReserveRecords { get; set; } = [];
}

[Description("会议室预定记录")]
public class MeetingRoomReserveRecord
{
    [Description("会议室名称")]
    public required string MeetingRoomName { get; set; }

    [Description("预定的起始时间")]
    public DateTime StartTime { get; set; }

    [Description("预定的结束时间")]
    public DateTime EndTime { get; set; }
}

[Description("预定会议室请求参数")]
public class ReserveMeetingRoomRequest
{
    [Description("会议室容量，能坐多少人")]
    public int Capacity { get; set; }

    [Description("预定的起始时间")]
    public DateTime StartTime { get; set; }

    [Description("预定的结束时间")]
    public DateTime EndTime { get; set; }
}

public class Response<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
